
using CurePlease.Model;
using CurePlease.Model.Config;
using CurePlease.Model.Constants;
using CurePlease.Utilities;
using EliteMMO.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CurePlease.Engine
{
    public class DebuffEngine
    {                
        private EliteAPI PL { get; set; }
        private EliteAPI Monitored { get; set; }

        public Dictionary<string, IEnumerable<short>> ActiveDebuffs = new Dictionary<string, IEnumerable<short>>();

        private IEnumerable<string> SpecifiedPartyMembers = new List<string>();

        public DebuffEngine(EliteAPI pl, EliteAPI mon)
        {
            PL = pl;
            Monitored = mon;           
        }

        public EngineAction Run(DebuffConfig Config, Dictionary<string, IEnumerable<short>> buffs)
        {
            var wakeSleepSpell = Data.WakeSleepSpells[Config.WakeSleepSpell];

            // PL Specific debuff removal
            if (PL.Player.Status != 33 && Config.PLDebuffEnabled)
            {
                var debuffIds = PL.Player.Buffs.Where(id => Data.DebuffPriorities.Keys.Cast<short>().Contains(id));
                var debuffPriorityList = debuffIds.Cast<StatusEffect>().OrderBy(status => Array.IndexOf(Data.DebuffPriorities.Keys.ToArray(), status));

                if (debuffPriorityList.Any())
                {
                    // Get the highest priority debuff we have the right spell off cooldown for.
                    var targetDebuff = debuffPriorityList.FirstOrDefault(status => Config.DebuffEnabled.ContainsKey(status) && Config.DebuffEnabled[status] && PL.SpellAvailable(Data.DebuffPriorities[status]));

                    if ((short)targetDebuff > 0)
                    {
                        return new EngineAction
                        {
                            Target = Target.Me,
                            Spell = Data.DebuffPriorities[targetDebuff]
                        };
                    }
                }
            }

            // Monitored specific debuff removal
            if (Config.MonitoredDebuffEnabled && (PL.Entity.GetEntity((int)Monitored.Party.GetPartyMember(0).TargetIndex).Distance < 21) && (Monitored.Player.HP > 0) && PL.Player.Status != 33)
            {
                var debuffIds = Monitored.Player.Buffs.Where(id => Data.DebuffPriorities.Keys.Cast<short>().Contains(id));
                var debuffPriorityList = debuffIds.Cast<StatusEffect>().OrderBy(status => Array.IndexOf(Data.DebuffPriorities.Keys.ToArray(), status));

                if (debuffPriorityList.Any())
                {
                    // Get the highest priority debuff we have the right spell off cooldown for.
                    var targetDebuff = debuffPriorityList.FirstOrDefault(status => Config.DebuffEnabled[status] && PL.SpellAvailable(Data.DebuffPriorities[status]));

                    if ((short)targetDebuff > 0)
                    {
                        // Don't try and curaga outside our party.
                        if ((targetDebuff == StatusEffect.Sleep || targetDebuff == StatusEffect.Sleep2) && !PL.SamePartyAs(Monitored))
                        {
                            return new EngineAction
                            {
                                Target = Monitored.Player.Name,
                                Spell = Spells.Cure
                            };
                        }

                        if (Data.DebuffPriorities[targetDebuff] != Spells.Erase || PL.SamePartyAs(Monitored))
                        {
                            return new EngineAction
                            {
                                Target = Monitored.Player.Name,
                                Spell = Data.DebuffPriorities[targetDebuff]
                            };
                        }
                    }
                }
            }

            // PARTY DEBUFF REMOVAL
            //lock (ActiveDebuffs)
            //{             
            // First remove the highest priority debuff.
                var priorityMember = Monitored.GetHighestPriorityDebuff(buffs);
                var name = priorityMember?.Name;

                if (Config.PartyDebuffEnabled && priorityMember != null)
                {
                    if ((!Config.OnlySpecificMembers || SpecifiedPartyMembers.Contains(name)) && buffs.ContainsKey(name) && buffs[name].Any())
                    {
                        var debuffs = buffs[name].Where(buff => Data.DebuffPriorities.Keys.Cast<short>().Contains(buff));

                        // Filter out non-debuffs, and convert to short IDs. Then calculate the priority order.
                        var debuffPriorityList = debuffs.Cast<StatusEffect>().OrderBy(status => Array.IndexOf(Data.DebuffPriorities.Keys.ToArray(), status));
              
                        // Get the highest priority debuff we have the right spell off cooldown for.
                        var targetDebuff = debuffPriorityList.FirstOrDefault(status => Config.DebuffEnabled[status] && PL.SpellAvailable(Data.DebuffPriorities[status]));

                        if ((short)targetDebuff > 0)
                        {
                            // Don't try and curaga outside our party.
                            if (!priorityMember.InParty(1) && (targetDebuff == StatusEffect.Sleep || targetDebuff == StatusEffect.Sleep2))
                            {
                                return new EngineAction
                                {
                                    Target = name,
                                    Spell = Spells.Cure
                                };
                            }

                            return new EngineAction
                            {
                                Target = name,
                                Spell = Data.DebuffPriorities[targetDebuff]
                            };
                        }
                    }
                }
            //}

            return null;
        }

        public void UpdateDebuffs(string memberName, IEnumerable<short> debuffs)
        {
            lock (ActiveDebuffs)
            {
                if (debuffs.Any())
                {
                    ActiveDebuffs[memberName] = debuffs;
                }
                else if (ActiveDebuffs.ContainsKey(memberName))
                {
                    ActiveDebuffs.Remove(memberName);
                }
            }
        }

        public void ToggleSpecifiedMember(string memberName)
        {
            if(SpecifiedPartyMembers.Contains(memberName))
            {
                SpecifiedPartyMembers = SpecifiedPartyMembers.Where(name => name != memberName);
            }
            else
            {
                SpecifiedPartyMembers.Append(memberName);
            }
        }

        public bool MemberSpecified(string memberName)
        {
            return SpecifiedPartyMembers.Contains(memberName);
        }      
    }
}
