using CurePlease.Model;
using CurePlease.Model.Enums;
using EliteMMO.API;
using System;
using System.Collections.Generic;
using System.Linq;
using static EliteMMO.API.EliteAPI;

namespace CurePlease.Utilities
{
    public static class EliteAPIExtensions
    {
        public static bool SamePartyAs(this EliteAPI api, EliteAPI other)
        {
            int relativePartyIndex = api.GetPartyRelativeTo(other);

            return relativePartyIndex == 1;
        }

        public static int GetPartyRelativeTo(this EliteAPI api, EliteAPI other)
        {
            // FIRST CHECK THAT BOTH THE PL AND MONITORED PLAYER ARE IN THE SAME PT/ALLIANCE
            List<PartyMember> otherParty = other.Party.GetPartyMembers();

            if (otherParty.Any(member => member.Name == api.Player.Name))
            {
                int plParty = otherParty.FirstOrDefault(p => p.Name == api.Player.Name).MemberNumber;

                if (plParty <= 5)
                {
                    return 1;
                }
                else if (plParty <= 11 && plParty >= 6)
                {
                    return 2;
                }
                else if (plParty <= 17 && plParty >= 12)
                {
                    return 3;
                }
            }

            return 0;
        }

        public static void UseJobAbility(this EliteAPI api, string ability)
        {
            api.ThirdParty.SendString("/ja \"" + ability + "\" <me>");
        }

        public static void CastSpell(this EliteAPI api, string spell, string target)
        {
            api.ThirdParty.SendString("/ma \"" + spell + "\" " + target);
        }

        public static void ExecuteAction(this EliteAPI api, EngineAction action)
        {

            if (!string.IsNullOrEmpty(action.JobAbility))
            {
                api.UseJobAbility(action.JobAbility);
            }

            if (!string.IsNullOrEmpty(action.Spell))
            {
                api.CastSpell(action.Spell, action.Target);
            }
        }

        public static bool EntityWithin(this EliteAPI api, int distance, uint targetIndex)
        {
            var entity = api.Entity.GetEntity((int)targetIndex);

            return entity.Distance < distance;
        }
        public static bool CanCastOn(this EliteAPI api, PartyMember member)
        {
            // If they're in range, and alive, should be able to cast.
            // Since we include distance = 0, should work when target is the PL itself.
            //var entity = api.Entity.GetEntity((int)member.TargetIndex);

            return api.EntityWithin(21, member.TargetIndex) && member.CurrentHP > 0;         
        }
        public static bool HasMPFor(this EliteAPI api, string spell)
        {
            return api.Player.MP >= Data.SpellCosts[spell];
        }

        public static bool HasStatus(this EliteAPI api, StatusEffect effect)
        {
            return api.HasStatus((short)effect);
        }

        public static bool HasStatus(this EliteAPI api, short effectId)
        {
            return api.Player.Buffs.Any(buff => buff == effectId);
        }

        public static bool SpellAvailable(this EliteAPI api, string spell)
        {
            // IF YOU HAVE OMERTA THEN BLOCK MAGIC CASTING
            if (api.HasStatus(StatusEffect.No_Magic_Casting))
            {
                return false;
            }

            var apiSpell = api.Resources.GetSpell(spell, 0);

            var mainLevelReq = apiSpell.LevelRequired[api.Player.MainJob];
            var subLevelReq = apiSpell.LevelRequired[api.Player.SubJob];

            // First check for definite no.
            // If our main either can't learn it, or isn't high enough, and similar for our sub.
            if((mainLevelReq == -1 || mainLevelReq > api.Player.MainJobLevel) && (subLevelReq == -1 || subLevelReq > api.Player.SubJobLevel))
            {
                return false;
            }

            // Then check for special case where we need job points
            if(mainLevelReq > 99)
            {
                var jobPointsTuple = Data.JobPointSpells[spell];
                if (api.Player.MainJob != (byte)jobPointsTuple.Item1 || api.Player.GetJobPoints(api.Player.MainJob).SpentJobPoints < jobPointsTuple.Item2)
                {
                    return false;
                }
            }
           
            // If we get here, we qualify for the spell. So make sure we own it, and that it's off cooldown.
            return api.Player.HasSpell(apiSpell.ID) && (api.Recast.GetSpellRecast(apiSpell.Index) == 0);
        }

        public static int GetAbilityRecast(this EliteAPI api, string ability)
        {
            var apiAbility = api.Resources.GetAbility(ability, 0);
            var recastIndex = api.Recast.GetAbilityIds().IndexOf(apiAbility.TimerID);

            return (recastIndex > -1) ? api.Recast.GetAbilityRecast(recastIndex) : 0;
        }

        public static bool AbilityAvailable(this EliteAPI api, string ability)
        {
            // IF YOU HAVE INPAIRMENT/AMNESIA THEN BLOCK JOB ABILITY CASTING
            if (api.HasStatus(StatusEffect.No_Job_Abilities) || api.HasStatus(StatusEffect.Amnesia))
            {
                return false;
            }

            var apiAbility = api.Resources.GetAbility(ability, 0);

            //int recast = api.Recast.GetAbilityRecast(apiAbility);
            return api.Player.HasAbility(apiAbility.ID) && api.GetAbilityRecast(ability) == 0;
        }

        public static uint HPLoss( this PartyMember member)
        {
            return member.CurrentHP * 100 / member.CurrentHPP - member.CurrentHP;
        }

        public static int CurrentSCHCharges(this EliteAPI api)
        {
            if (api != null)
            {
                int MainJob = api.Player.MainJob;
                int SubJob = api.Player.SubJob;

                if (MainJob == (int)Job.SCH || SubJob == (int)Job.SCH)
                {
                    if (api.HasStatus(StatusEffect.Light_Arts) || api.HasStatus(StatusEffect.Addendum_White))
                    {
                        // Stragem charge recast = ability ID 231?
                        int currentRecastTimer = api.Recast.GetAbilityRecast(231);

                        int SpentPoints = api.Player.GetJobPoints((int)Job.SCH).SpentJobPoints;

                        int MainLevel = api.Player.MainJobLevel;
                        int SubLevel = api.Player.SubJobLevel;

                        int baseTimer = 240;
                        int baseCharges = 1;

                        // Generate the correct timer between charges depending on level / Job Points
                        if (MainJob == (int)Job.SCH)
                        {
                            if (SpentPoints >= 550)
                            {
                                baseTimer = 33;
                                baseCharges = 5;
                            }
                            else if (MainLevel >= 90)
                            {
                                baseTimer = 48;
                                baseCharges = 5;
                            }
                            else if (MainLevel >= 70 && MainLevel < 90)
                            {
                                baseTimer = 60;
                                baseCharges = 4;
                            }
                            else if (MainLevel >= 50 && MainLevel < 70)
                            {
                                baseTimer = 80;
                                baseCharges = 3;
                            }
                            else if (MainLevel >= 30 && MainLevel < 50)
                            {
                                baseTimer = 120;
                                baseCharges = 2;
                            }
                            else if (MainLevel >= 10 && MainLevel < 30)
                            {
                                baseTimer = 240;
                                baseCharges = 1;
                            }
                        }
                        else if (SubJob == (int)Job.SCH)
                        {
                            if (SubLevel >= 30 && SubLevel < 50)
                            {
                                baseTimer = 120;
                                baseCharges = 2;
                            }
                        }

                        // Now knowing what the time between charges is lets calculate how many
                        // charges are available

                        if (currentRecastTimer == 0)
                        {
                            return baseCharges;
                        }
                        else
                        {
                            int t = currentRecastTimer / 60;

                            int stratsUsed = t / baseTimer;

                            int currentCharges = (int)Math.Ceiling((decimal)baseCharges - stratsUsed);

                            return (baseTimer == 120) ? currentCharges-- : currentCharges;
                        }
                    }
                }
            }

            return -1;
        }

        public static IEnumerable<int> PartyNeedsAoeCure(this EliteAPI api, int countThreshold, int cureThreshold)
        {
            List<int> partiesResult = new List<int>();

            // Full alliance list of who's active and below the threshold.
            var activeMembers = api.GetActivePartyMembers().Where(pm => pm.CurrentHPP <= cureThreshold);

            // Figure out which parties specifically qualify.
            if (activeMembers.Where(pm => pm.InParty(1)).Count() >= countThreshold)
            {
                partiesResult.Add(1);
            }
            if (activeMembers.Where(pm => pm.InParty(2)).Count() >= countThreshold)
            {
                partiesResult.Add(2);
            }
            if (activeMembers.Where(pm => pm.InParty(3)).Count() >= countThreshold)
            {
                partiesResult.Add(3);
            }

            return partiesResult.OrderByDescending(partyNumber => api.AverageHpLossForParty(partyNumber));
        }

        public static IEnumerable<PartyMember> GetActivePartyMembers(this EliteAPI api)
        {
            return api.Party.GetPartyMembers().Where(pm => pm.Active > 0 && pm.CurrentHP > 0).OrderBy(pm => pm.CurrentHPP);
        }

        public static bool InParty(this PartyMember member, int partyNumber)
        {
            switch (partyNumber)
            {
                case 1:
                    return member.MemberNumber <= 5;
                case 2:
                    return member.MemberNumber > 5 && member.MemberNumber <= 11;
                case 3:
                    return member.MemberNumber > 11;
            }

            return false;
        }

        public static uint AverageHpLossForParty(this EliteAPI api, int partyNumber)
        {
            IEnumerable<PartyMember> members = api.GetActivePartyMembers().Where(pm => pm.InParty(partyNumber));        

            if(members != null && members.Any())
            {
                return (uint)(members.Sum(pm => pm.HPLoss()) / members.Count());
            }

            return 0;
        }

        // TODO: Not working? No debuffs being cured
        public static PartyMember GetHighestPriorityDebuff(this EliteAPI api, Dictionary<string, IEnumerable<short>> debuffs)
        {
            var members = api.GetActivePartyMembers().Where(pm => api.CanCastOn(pm));

            int lowestIndex = int.MaxValue;
            PartyMember priorityMember = null;

            foreach(PartyMember pm in members)
            {
                if(!debuffs.ContainsKey(pm.Name))
                {
                    continue;
                }

                // We get the debuffs and order them by priority, filtering for statuses we have the right spell off cooldown.
                var debuffIds = debuffs[pm.Name].Where(id => Data.DebuffPriorities.Keys.Cast<short>().Contains(id));
                
                if(debuffIds.Any())
                {
                    var pmPriorities = debuffIds.Cast<StatusEffect>().OrderBy(status => Array.IndexOf(Data.DebuffPriorities.Keys.ToArray(), status)).Where(status => api.SpellAvailable(Data.DebuffPriorities[status]));

                    if (pmPriorities.Any())
                    {
                        var priority = Array.IndexOf(Data.DebuffPriorities.Keys.ToArray(), pmPriorities.FirstOrDefault());
                        if (priority < lowestIndex)
                        {
                            lowestIndex = priority;
                            priorityMember = pm;
                        }
                    }
                }            
            }

            return priorityMember;
        }

        public static int ShadowsRemaining(this EliteAPI api)
        {
            if (api.HasStatus(StatusEffect.Utsusemi_4_Shadows_Left))
            {
                return 4;
            }
            else if (api.HasStatus(StatusEffect.Utsusemi_3_Shadows_Left))
            {
                return 3;
            }
            else if (api.HasStatus(StatusEffect.Utsusemi_2_Shadows_Left))
            {
                return 2;
            }
            else if (api.HasStatus(StatusEffect.Utsusemi_1_Shadow_Left))
            {
                return 1;
            }

            return 0;
        }

        public static int GetInventoryItemCount(this EliteAPI api, ushort itemid)
        {
            int count = 0;
            for (int x = 0; x <= 80; x++)
            {
                InventoryItem item = api.Inventory.GetContainerItem(0, x);
                if (item != null && item.Id == itemid)
                {
                    count += (int)item.Count;
                }
            }

            return count;
        }

        public static int GetTempItemCount(this EliteAPI api, ushort itemid)
        {
            int count = 0;
            for (int x = 0; x <= 80; x++)
            {
                InventoryItem item = api.Inventory.GetContainerItem(3, x);
                if (item != null && item.Id == itemid)
                {
                    count += (int)item.Count;
                }
            }

            return count;
        }

        public static ushort GetItemId(this EliteAPI api, string name)
        {
            IItem item = api.Resources.GetItem(name, 0);
            return item != null ? (ushort)item.ItemID : (ushort)0;
        }
    }
}
