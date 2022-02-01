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
    public class PLEngine
    {
        private EliteAPI PL { get; set; }
        private EliteAPI Monitored { get; set; }

        private bool LowMP = false;

        private int lastKnownEstablisherTarget = 0;

        private IEnumerable<short> localBuffs;

        public PLEngine(EliteAPI pl, EliteAPI mon)
        {
            PL = pl;
            Monitored = mon;
        }

        public EngineAction Run(PLConfig Config, IEnumerable<short> playerBuffs)
        {
            localBuffs = playerBuffs ?? new List<short>();

            // FIRST IF YOU ARE SILENCED OR DOOMED ATTEMPT REMOVAL NOW
            if (HasStatus(StatusEffect.Silence) && Config.PLSilenceItemEnabled)
            {
                var plSilenceItem = Items.SilenceRemoval[Config.PLSilenceItem];

                // Check to make sure we have echo drops
                if (PL.GetInventoryItemCount(PL.GetItemId(plSilenceItem)) > 0 || PL.GetTempItemCount(PL.GetItemId(plSilenceItem)) > 0)
                {
                    return new EngineAction
                    {
                        Item = plSilenceItem
                    };
                }

            }
            else if (HasStatus(StatusEffect.Doom) && Config.PLDoomItemEnabled)
            {
                var plDoomItem = Items.DoomRemoval[Config.PLDoomItem];

                // Check to make sure we have holy water
                if (PL.GetInventoryItemCount(PL.GetItemId(plDoomItem)) > 0 || PL.GetTempItemCount(PL.GetItemId(plDoomItem)) > 0)
                {
                    return new EngineAction
                    {
                        Item = plDoomItem
                    };
                }
            }

            else if (Config.DivineSeal && PL.Player.MPP <= 11 && PL.AbilityAvailable(Ability.DivineSeal) && !HasStatus(StatusEffect.Weakness))
            {
                return new EngineAction
                {
                    Target = Target.Me,
                    JobAbility = Ability.DivineSeal
                };
            }
            else if (Config.Convert && (PL.Player.MP <= Config.ConvertMP) && PL.AbilityAvailable(Ability.Convert) && !HasStatus(StatusEffect.Weakness))
            {
                return new EngineAction
                {
                    Target = Target.Me,
                    JobAbility = Ability.Convert
                };
            }

            if (PL.Player.MP <= Config.MinCastingMP && PL.Player.MP != 0)
            {
                if (Config.LowMPEnabled && !LowMP && !Config.HealLowMPEnabled)
                {
                    PL.ThirdParty.SendString("/tell " + Monitored.Player.Name + " MP is low!");
                    
                }

                LowMP = true;
            }
            if (PL.Player.MP > Config.MinCastingMP && PL.Player.MP != 0)
            {
                if (Config.LowMPEnabled && LowMP && !Config.HealLowMPEnabled)
                {
                    PL.ThirdParty.SendString("/tell " + Monitored.Player.Name + " MP OK!");
                    
                }

                LowMP = false;
            }

            if (Config.HealLowMPEnabled && PL.Player.MP <= Config.HealMPThreshold && PL.Player.Status == 0)
            {
                if (Config.LowMPEnabled && !LowMP)
                {
                    PL.ThirdParty.SendString("/tell " + Monitored.Player.Name + " MP is seriously low, /healing.");
                }
                
                LowMP = true;

                return new EngineAction
                {
                    CustomAction = "/heal"
                };
            }
            else if (Config.StandMPEnabled && PL.Player.MPP >= Config.StandMPThreshold && PL.Player.Status == 33)
            {
                if (Config.LowMPEnabled && !LowMP)
                {
                    PL.ThirdParty.SendString("/tell " + Monitored.Player.Name + " MP has recovered.");               
                }

                LowMP = false;

                return new EngineAction
                {
                    CustomAction = "/heal"
                };
            }

            if (Config.AfflatusSolaceEnabled && (!HasStatus(StatusEffect.Afflatus_Solace)) && PL.AbilityAvailable(Ability.AfflatusSolace))
            {
                return new EngineAction { JobAbility = Ability.AfflatusSolace };
            }
            
            if (Config.AfflatusMiseryEnabled && (!HasStatus(StatusEffect.Afflatus_Misery)) && PL.AbilityAvailable(Ability.AfflatusMisery))
            {
                return new EngineAction { JobAbility = Ability.AfflatusMisery };
            }                          
            
            if (Config.Composure && (!HasStatus(StatusEffect.Composure)) && PL.AbilityAvailable(Ability.Composure))
            {
                return new EngineAction
                {
                    JobAbility = Ability.Composure
                };
            }
            
            if (Config.LightArts && (!HasStatus(StatusEffect.Light_Arts)) && (!HasStatus(StatusEffect.Addendum_White)) && PL.AbilityAvailable(Ability.LightArts))
            {
                return new EngineAction
                {
                    JobAbility = Ability.LightArts
                };
            }
            
            if (Config.AddendumWhite && (!HasStatus(StatusEffect.Addendum_White)) && HasStatus(StatusEffect.Light_Arts) && PL.CurrentSCHCharges() > 0)
            {
                return new EngineAction
                {
                    JobAbility = Ability.AddendumWhite
                };
            }

            if (Config.DarkArts && (!HasStatus(StatusEffect.Dark_Arts)) && (!HasStatus(StatusEffect.Addendum_Black)) && PL.AbilityAvailable(Ability.DarkArts))
            {
                return new EngineAction
                {
                    JobAbility = Ability.DarkArts
                };
            }

            if (Config.AddendumBlack && HasStatus(StatusEffect.Dark_Arts) && (!HasStatus(StatusEffect.Addendum_Black)) && PL.CurrentSCHCharges() > 0)
            {
                return new EngineAction
                {
                    JobAbility = Ability.AddendumBlack
                };
            }
            if (Config.SublimationEnabled && (!HasStatus(StatusEffect.Sublimation_Activated)) && (!HasStatus(StatusEffect.Sublimation_Complete)) && (!HasStatus(StatusEffect.Refresh)) && PL.AbilityAvailable(Ability.Sublimation))
            {
                return new EngineAction { JobAbility = Ability.Sublimation };
            }

            if (Config.SublimationEnabled && ((PL.Player.MPMax - PL.Player.MP) > Config.SublimationMPLossThreshold) && HasStatus(StatusEffect.Sublimation_Complete) && PL.AbilityAvailable(Ability.Sublimation))
            {
                return new EngineAction { JobAbility = Ability.Sublimation };
            }

            if (Config.DivineCaressEnabled && Config.DebuffsEnabled && PL.AbilityAvailable(Ability.DivineCaress))
            {
                return new EngineAction { JobAbility = Ability.DivineCaress };
            }
            
            if (Config.DevotionEnabled && PL.AbilityAvailable(Ability.Devotion) && PL.Player.HPP > 80 && (!Config.DevotionWhenEngaged || (Monitored.Player.Status == 1)))
            {
                int plParty = PL.GetPartyRelativeTo(Monitored);

                // Get all active members who are in the PLs party.
                IEnumerable<PartyMember> cParty = Monitored.GetActivePartyMembers().Where(member => member.InParty(plParty) && member.Name != PL.Player.Name);

                // If we're set to only devotion a specific target, filter the list for that target.
                if (Config.DevotionSpecifiedTarget)
                {
                    cParty = cParty.Where(member => member.Name == Config.DevotionTargetName);
                }

                // Get the first member who's within range, and has enough missing MP to meet our config criteria.
                var devotionTarget = cParty.FirstOrDefault(member => member.CurrentMP <= Config.DevotionMPThreshold && PL.EntityWithin(10, member.TargetIndex));

                if (devotionTarget != default)
                {
                    return new EngineAction
                    {
                        Target = devotionTarget.Name,
                        JobAbility = Ability.Devotion
                    };
                }
            }
            
            if (Config.ShellraEnabled && (!HasStatus(StatusEffect.Shell)))
            {
                var shellraSpell = Data.ShellraTiers[Config.ShellraLevel - 1];
                if (PL.SpellAvailable(shellraSpell))
                {
                    return new EngineAction
                    {
                        Spell = shellraSpell
                    };
                }
            }
            
            if (Config.ProtectraEnabled && (!HasStatus(StatusEffect.Protect)))
            {
                var protectraSpell = Data.ProtectraTiers[Config.ProtectraLevel - 1];
                if (PL.SpellAvailable(protectraSpell))
                {
                    return new EngineAction { Spell = protectraSpell };
                }
            }
            
            if (Config.BarElementEnabled && !HasStatus((StatusEffect)Data.SpellEffects[Config.BarElementSpell]) && PL.SpellAvailable(Config.BarElementSpell))
            {
                // TODO: Make this work properly, so it can figure out if there's 2 charges and return 
                // an action that says: Do Accession + Perpetuance + Barspell.
                var result = new EngineAction
                {
                    Spell = Config.BarElementSpell
                };

                if (Config.AccessionEnabled && Config.BarElementAccession && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Accession) && !Config.AOEBarElementEnabled && !HasStatus(StatusEffect.Accession))
                {
                    result.JobAbility = Ability.Accession;
                }
                else if (Config.PerpetuanceEnabled && Config.BarElemenetPerpetuance && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Perpetuance) && !HasStatus(StatusEffect.Perpetuance))
                {
                    result.JobAbility = Ability.Perpetuance;
                }

                return result;
            }
            
            if (Config.BarStatusEnabled && !HasStatus(Data.SpellEffects[Config.BarStatusSpell]) && PL.SpellAvailable(Config.BarStatusSpell))
            {
                var result = new EngineAction
                {
                    Spell = Config.BarStatusSpell
                };

                if (Config.AccessionEnabled && Config.BarStatusAccession && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Accession) && !Config.AOEBarStatusEnabled && !HasStatus(StatusEffect.Accession))
                {
                    result.JobAbility = Ability.Accession;
                }
                else if (Config.PerpetuanceEnabled  && Config.BarStatusPerpetuance && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Perpetuance) && !HasStatus(StatusEffect.Perpetuance))
                {
                    result.JobAbility = Ability.Perpetuance;
                }

                return result;
            }
            
            if (Config.GainBoostSpellEnabled && !HasStatus(Data.SpellEffects[Config.GainBoostSpell]) && PL.SpellAvailable(Config.GainBoostSpell))
            {
                return new EngineAction { Spell = Config.GainBoostSpell };
            }
            
            if (Config.StormSpellEnabled && !HasStatus(Data.SpellEffects[Config.StormSpell]) && PL.SpellAvailable(Config.StormSpell))
            {
                var result = new EngineAction { Spell = Config.StormSpell };
                if (Config.AccessionEnabled && Config.StormspellAccession && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Accession) && !HasStatus(StatusEffect.Accession))
                {
                    result.JobAbility = Ability.Accession;
                }
                else if (Config.PerpetuanceEnabled && Config.StormspellPerpetuance && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Perpetuance) && !HasStatus(StatusEffect.Perpetuance))
                {
                    result.JobAbility = Ability.Perpetuance;
                }

                return result;
            }
            
            if (Config.ProtectEnabled && (!HasStatus(StatusEffect.Protect)))
            {
                var result = new EngineAction { Spell = Config.ProtectSpell };

                if (Config.AccessionEnabled&& Config.AccessionProtectShell && PL.Party.GetPartyMembers().Count() > 2 && PL.AbilityAvailable(Ability.Accession) && PL.CurrentSCHCharges() > 0)
                {
                    if (!HasStatus(StatusEffect.Accession))
                    {
                        result.JobAbility = Ability.Accession;
                    }
                }

                return result;
            }
            
            if (Config.ShellEnabled && (!HasStatus(StatusEffect.Shell)))
            {
                var result = new EngineAction { Spell = Config.ShellSpell };

                if (Config.AccessionEnabled && Config.AccessionProtectShell && PL.Party.GetPartyMembers().Count() > 2 && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Accession))
                {
                    if (!HasStatus(StatusEffect.Accession))
                    {
                        result.JobAbility = Ability.Accession;
                    }
                }

                return result;
            }
            
            if (Config.ReraiseEnabled && (!HasStatus(StatusEffect.Reraise)))
            {
                var result = new EngineAction { Spell = Config.ReraiseSpell };

                if(Config.EnlightenmentReraise && !HasStatus(StatusEffect.Addendum_White) && PL.AbilityAvailable(Ability.Enlightenment))
                {
                    result.JobAbility = Ability.Enlightenment;
                }

                return result;                
            }
            
            if (Config.UtsusemiEnabled && PL.ShadowsRemaining() < 2)
            {
                if(PL.GetInventoryItemCount(PL.GetItemId(Items.Shihei)) > 0)
                {
                    if (PL.SpellAvailable(Spells.Utsusemi_Ni) )
                    {
                        return new EngineAction { Spell = Spells.Utsusemi_Ni };
                    }
                    else if (PL.SpellAvailable(Spells.Utsusemi_Ichi) && (PL.ShadowsRemaining() == 0))
                    {
                        return new EngineAction { Spell = Spells.Utsusemi_Ichi };
                    }
                }             
            }
            
            if (Config.BlinkEnabled && (!HasStatus(StatusEffect.Blink)) && PL.SpellAvailable(Spells.Blink))
            {
                var result = new EngineAction { Spell = Spells.Blink };

                if (Config.AccessionEnabled && Config.BlinkAccession && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Accession) && !HasStatus(StatusEffect.Accession))
                {
                    result.JobAbility = Ability.Accession;
                }
                else if (Config.PerpetuanceEnabled && Config.BlinkPerpetuance && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Perpetuance) && !HasStatus(StatusEffect.Perpetuance))
                {
                    result.JobAbility = Ability.Perpetuance;
                }

                return result;
            }
            
            if (Config.PhalanxEnabled && (!HasStatus(StatusEffect.Phalanx)) && PL.SpellAvailable(Spells.Phalanx))
            {
                var result = new EngineAction { Spell = Spells.Phalanx };

                if (Config.AccessionEnabled && Config.PhalanxAccession && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Accession) && !HasStatus(StatusEffect.Accession))
                {
                    result.JobAbility = Ability.Accession;
                }
                else if (Config.PerpetuanceEnabled && Config.PhalanxPerpetuance && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Perpetuance) && !HasStatus(StatusEffect.Perpetuance))
                {
                    result.JobAbility = Ability.Perpetuance;
                }

                return result;
            }
            
            if (Config.RefreshEnabled && (!HasStatus(StatusEffect.Refresh)))
            {
                var result = new EngineAction { Spell = Config.RefreshSpell };

                if (Config.AccessionEnabled && Config.RefreshAccession && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Accession) && !HasStatus(StatusEffect.Accession))
                {
                    result.JobAbility = Ability.Accession;
                }
                else if (Config.PerpetuanceEnabled && Config.RefreshPerpetuance && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Perpetuance) && !HasStatus(StatusEffect.Perpetuance))
                {
                    result.JobAbility = Ability.Perpetuance;
                }

                return result;              
            }
            
            if (Config.RegenEnabled && (!HasStatus(StatusEffect.Regen)))
            {
                var result = new EngineAction { Spell = Config.RegenSpell };

                if (Config.AccessionEnabled && Config.RegenAccession && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Accession) && !HasStatus(StatusEffect.Accession))
                {
                    result.JobAbility = Ability.Accession;
                }
                else if (Config.PerpetuanceEnabled && Config.RegenPerpetuance && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Perpetuance) && !HasStatus(StatusEffect.Perpetuance))
                {
                    result.JobAbility = Ability.Perpetuance;
                }

                return result;
            }
            
            if (Config.AdloquiumEnabled && (!HasStatus(StatusEffect.Regain)) && PL.SpellAvailable(Spells.Adloquium))
            {
                var result = new EngineAction { Spell = Spells.Adloquium };

                if (Config.AccessionEnabled && Config.AdloquiumAccession && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Accession) && !HasStatus(StatusEffect.Accession))
                {
                    result.JobAbility = Ability.Accession;
                }
                else if (Config.PerpetuanceEnabled && Config.AdloquiumPerpetuance && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Perpetuance) && !HasStatus(StatusEffect.Perpetuance))
                {
                    result.JobAbility = Ability.Perpetuance;
                }

                return result;
            }
            
            if (Config.StoneskinEnabled && (!HasStatus(StatusEffect.Stoneskin)) && PL.SpellAvailable(Spells.Stoneskin))
            {
                var result = new EngineAction { Spell = Spells.Stoneskin };

                if (Config.AccessionEnabled && Config.StoneskinAccession && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Accession) && !HasStatus(StatusEffect.Accession))
                {
                    result.JobAbility = Ability.Accession;
                }
                else if (Config.PerpetuanceEnabled && Config.StoneskinPerpetuance && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Perpetuance) && !HasStatus(StatusEffect.Perpetuance))
                {
                    result.JobAbility = Ability.Perpetuance;
                }

                return result;
            }
            
            if (Config.AquaveilEnabled && (!HasStatus(StatusEffect.Aquaveil)) && PL.SpellAvailable(Spells.Aquaveil))
            {
                var result = new EngineAction { Spell = Spells.Aquaveil };

                if (Config.AccessionEnabled && Config.AquaveilAccession && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Accession) && !HasStatus(StatusEffect.Accession))
                {
                    result.JobAbility = Ability.Accession;
                }
                else if (Config.PerpetuanceEnabled && Config.AquaveilPerpetuance && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Perpetuance) && !HasStatus(StatusEffect.Perpetuance))
                {
                    result.JobAbility = Ability.Perpetuance;
                }

                return result;
            }
            
            if (Config.KlimaformEnabled && !HasStatus(StatusEffect.Klimaform))
            {
                return new EngineAction { Spell = Spells.Klimaform };
            }
            
            if (Config.TemperEnabled && (!HasStatus(StatusEffect.Multi_Strikes)))
            {
                return new EngineAction { Spell = Config.TemperSpell };
            }
            
            if (Config.HasteEnabled && (!HasStatus(StatusEffect.Haste)))
            {
                return new EngineAction { Spell = Config.HasteSpell };
            }
            
            if (Config.SpikesEnabled && !HasStatus(Data.SpellEffects[Config.SpikesSpell]))
            {
                return new EngineAction { Spell = Config.SpikesSpell };
            }
            
            if (Config.EnSpellEnabled && !HasStatus(Data.SpellEffects[Config.EnSpell]) && PL.SpellAvailable(Config.EnSpell))
            {
                var result = new EngineAction { Spell = Config.EnSpell };

                // Don't want to try and accession/perpetuance tier II.
                if(!Config.EnSpell.Contains("II"))
                {
                    if (Config.AccessionEnabled && Config.EnspellAccession && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Accession) && !HasStatus(StatusEffect.Accession))
                    {
                        result.JobAbility = Ability.Accession;
                    }
                    else if (Config.PerpetuanceEnabled && Config.EnspellPerpetuance && PL.CurrentSCHCharges() > 0 && PL.AbilityAvailable(Ability.Perpetuance) && !HasStatus(StatusEffect.Perpetuance))
                    {
                        result.JobAbility = Ability.Perpetuance;
                    }
                }

                return result;
            }
            
            if (Config.AuspiceEnabled && (!HasStatus(StatusEffect.Auspice)) && PL.SpellAvailable(Spells.Auspice))
            {
                return new EngineAction { Spell = Spells.Auspice };
            }

            // TODO: Rethink this whole logic.
            // Probably doesn't work at all right now, and need to figure out a better way
            // to find entities than iterating 2048 things...           
            if (Config.AutoTargetEnabled && PL.SpellAvailable(Config.AutoTargetSpell))
            {            
                if (Config.PartyBasedHateSpell)
                {
                    // PARTY BASED HATE SPELL
                    int enemyID = CheckEngagedStatus_Hate(Config);

                    if (enemyID != 0 && enemyID != lastKnownEstablisherTarget)
                    {
                        lastKnownEstablisherTarget = enemyID;

                        return new EngineAction
                        {
                            Target = Config.AutoTargetTarget,
                            Spell = Config.AutoTargetSpell
                        };
                    }
                }
                else
                {
                    // ENEMY BASED TARGET
                    int enemyID = CheckEngagedStatus_Hate(Config);

                    if (enemyID != 0 && enemyID != lastKnownEstablisherTarget)
                    {
                        PL.Target.SetTarget(enemyID);

                        lastKnownEstablisherTarget = enemyID;

                        return new EngineAction
                        {
                            Target = "<t>",
                            Spell = Config.AutoTargetSpell
                        };
                     
                        //if (ConfigForm.config.DisableTargettingCancel == false)
                        //{
                        //    await Task.Delay(TimeSpan.FromSeconds((double)ConfigForm.config.TargetRemoval_Delay));
                        //    PL.Target.SetTarget(0);
                        //}
                    }
                }
            }
           
            return null;
        }

        private bool HasStatus(StatusEffect status)
        {
            return localBuffs.Contains((short)status);
        }

        private bool HasStatus(short status)
        {
            return localBuffs.Contains(status);
        }

        private int CheckEngagedStatus_Hate(PLConfig Config)
        {
            if (Config.AssistSpecifiedTarget == true && !string.IsNullOrEmpty(Config.AutoTargetTarget))
            {
                for (int x = 0; x < 2048; x++)
                {
                    XiEntity z = PL.Entity.GetEntity(x);

                    if (!string.IsNullOrEmpty(z.Name) && z.Name.ToLower() == Config.AutoTargetTarget.ToLower())
                    {
                        if (z.Status == 1)
                        {
                            return z.TargetingIndex;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
                return 0;
            }
            else
            {
                if (Monitored.Player.Status == 1)
                {
                    TargetInfo target = Monitored.Target.GetTargetInfo();
                    XiEntity entity = Monitored.Entity.GetEntity(Convert.ToInt32(target.TargetIndex));
                    return Convert.ToInt32(entity.TargetID);

                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
