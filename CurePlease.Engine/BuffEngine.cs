
using CurePlease.Model;
using CurePlease.Model.Config;
using CurePlease.Model.Constants;
using CurePlease.Utilities;
using EliteMMO.API;
using System;
using System.Collections.Generic;
using System.Linq;
using static EliteMMO.API.EliteAPI;

namespace CurePlease.Engine
{
    // TODO:
    // - Figure out tiers.
    // - Prevent overlap with haste/flurry/storms.
    // - Don't consider buffs we can't cast? Or just prevent that before we get here?
    public class BuffEngine
    {
        // Auto Spells:
        // Haste, Haste II, Phalanx II, Regen, Shell, Protect, Sandstorm, Rainstorm, Windstorm, Firestorm, Hailstorm, Thunderstorm, Voidstorm, Aurorastorm, Refresh, Adloquium
             
        private EliteAPI PL { get; set; }
        private EliteAPI Monitored { get; set; }

        public Dictionary<string, IEnumerable<short>> ActiveBuffs = new Dictionary<string, IEnumerable<short>>();

        // TODO: Should this just be the exact spell we want to cast instead of the buff id?
        // Would probably be cleaner.
        // Phalanx II, Regen, Refresh, Storms
        private Dictionary<string, IList<string>> AutoBuffs = new Dictionary<string, IList<string>>();

        // This tracks members auto-buffs outside party 1. It maps the spell we want to cast, to the next time we're supposed to cast
        // it base on our config.
        // Haste/Flurry/Protect/Shell/Adloquium
        private Dictionary<string, IList<Tuple<string, DateTime>>> TimedBuffs = new Dictionary<string, IList<Tuple<string, DateTime>>>();

        public BuffEngine(EliteAPI pl, EliteAPI mon)
        {
            PL = pl;
            Monitored = mon;                  
        }

        // TODO: We don't get alliance members buffs client-side. So party 2 and 3 need to be handled with timers.
        public EngineAction Run(BuffConfig config, Dictionary<string, IEnumerable<short>> buffs)
        {

            // Want to find party members where they have an autobuff configured but it isn't in their list of buffs.
            foreach (PartyMember ptMember in Monitored.GetActivePartyMembers())
            {
                // Check buffs we re-cast based on reported statuses from addon.
                // Make sure there's at least 1 auto-buff for the player.
                if(AutoBuffs.ContainsKey(ptMember.Name) && AutoBuffs[ptMember.Name].Any())
                {
                    // First check if they're ActiveBuffs are empty, and if so return first buff to cast.
                    if(!buffs.ContainsKey(ptMember.Name) || !buffs[ptMember.Name].Any())
                    {
                        return new EngineAction()
                        {
                            Spell = AutoBuffs[ptMember.Name].First(),
                            Target = ptMember.Name
                        };
                           
                    }
                    else
                    {
                        var missingBuffSpell = AutoBuffs[ptMember.Name].FirstOrDefault(buff => !buffs[ptMember.Name].Contains(Data.SpellEffects[buff]));
                        if(!string.IsNullOrEmpty(missingBuffSpell))
                        {
                            return new EngineAction()
                            {
                                Spell = missingBuffSpell,
                                Target = ptMember.Name
                            };
                        }
                    }                         
                }
                
                // Check buffs we re-cast based on timers.
                if(TimedBuffs.ContainsKey(ptMember.Name) && TimedBuffs[ptMember.Name].Any())
                {
                    foreach(Tuple<string, DateTime> spellRecast in TimedBuffs[ptMember.Name])
                    {
                        decimal recastDelay;
                        var spell = spellRecast.Item1;
                        if(spell == Spells.Phalanx_II)
                        {
                            recastDelay = config.PhalanxRecast;
                        }
                        else if(spell.Contains(Spells.Regen))
                        {
                            recastDelay = config.RegenRecast;
                            spell = Data.RegenTiers[config.RegenTier];
                        }
                        else if(spell.Contains(Spells.Refresh))
                        {
                            recastDelay = config.RefreshRecast;
                            spell = Data.RefreshTiers[config.RefreshTier];
                        }
                        else if(spell.Contains(Spells.Haste) || spell.Contains(Spells.Flurry))
                        {
                            recastDelay = config.HasteFlurryRecast;
                        }
                        else if(spell.Contains(Spells.Shell))
                        {
                            recastDelay = config.ShellRecast;
                            spell = Data.ShellTiers[config.ShellTier];
                        }
                        else if(spell.Contains(Spells.Protect))
                        {
                            recastDelay = config.ProtectRecast;
                            spell = Data.ProtectTiers[config.ProtectTier];
                        }
                        else if(spell == Spells.Adloquium)
                        {
                            recastDelay = config.AdloquiumRecast;
                        }
                        else
                        {
                            recastDelay = config.StormRecast;
                        }

                        var targetTime = spellRecast.Item2.AddMinutes(decimal.ToDouble(recastDelay));

                        if(DateTime.Now > targetTime)
                        {
                            return new EngineAction()
                            {
                                Spell = spell,
                                Target = ptMember.Name
                            };
                        }
                    }
                }
            }     

            return null;
        }

        public void ToggleTimedBuff(string memberName, string spellName)
        {
            if (!TimedBuffs.ContainsKey(memberName))
            {
                TimedBuffs.Add(memberName, new List<Tuple<string, DateTime>>());
            }

            if (TimedBuffs[memberName].Any(tuple => tuple.Item1 == spellName))
            {
                // If we already had the buff enabled, remove it.
                TimedBuffs[memberName] = TimedBuffs[memberName].Where(tuple => tuple.Item1 != spellName).ToList();
            }
            else
            {   
                // Small hack to add things way in the past so we always cast them right away when first added.
                TimedBuffs[memberName].Add(Tuple.Create(spellName, DateTime.Now.AddMinutes(-100)));
            }
        }

        public bool BuffEnabled(string memberName, string spellName)
        {
            return AutoBuffs.ContainsKey(memberName) && AutoBuffs[memberName].Any(spell => spell == spellName);
        }

        // Since we can only have one socket for the addon, we let the main form
        // explicitly manage our buff list instead of doing it internally.
        public void UpdateBuffs(string memberName, IEnumerable<short> buffs)
        {
            lock(ActiveBuffs)
            {
                if(buffs.Any())
                {
                    ActiveBuffs[memberName] = buffs;
                }
                else if(ActiveBuffs.ContainsKey(memberName))
                {
                    ActiveBuffs.Remove(memberName);
                }
            }
        }

        public void UpdateTimer(string memberName, string spell)
        {
            if(TimedBuffs.ContainsKey(memberName))
            {
                // TODO: Figure out better way to handle regen/refresh/storm tiers.
                var spellKey = spell;
                
                if(Data.RegenTiers.Contains(spell))
                {
                    spellKey = Spells.Regen;
                }
                else if(Data.RefreshTiers.Contains(spell))
                {
                    spellKey = Spells.Refresh;
                }
                else if(Data.ShellTiers.Contains(spell)) 
                {
                    spellKey = Spells.Shell;
                }
                else if(Data.ProtectTiers.Contains(spell))
                {
                    spellKey = Spells.Protect;
                }

                TimedBuffs[memberName] = TimedBuffs[memberName].Where(timer => timer.Item1 != spellKey).ToList();
                TimedBuffs[memberName].Add(Tuple.Create(spellKey,DateTime.Now));
            }
        }

    }
}
