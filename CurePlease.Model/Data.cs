using CurePlease.Model.Constants;
using CurePlease.Model.Enums;
using EliteMMO.API;
using System;
using System.Collections.Generic;

namespace CurePlease.Model
{
    public static class Data
    {
        public static Dictionary<string, int> SpellCosts = new Dictionary<string, int> {
            { Spells.Cure, 8 } ,
            { Spells.Cure_II, 24 },
            { Spells.Cure_III, 46 },
            { Spells.Cure_IV, 88 },
            { Spells.Cure_V, 125 },
            { Spells.Cure_VI, 227 },
            { Spells.Curaga, 60 },
            { Spells.Curaga_II, 120 },
            { Spells.Curaga_III, 180 },
            { Spells.Curaga_IV, 260 },
            { Spells.Curaga_V, 380 },
            { Spells.Regen, 15 },
            { Spells.Regen_II, 36 },
            { Spells.Regen_III, 64 },
            { Spells.Regen_IV, 82 },
            { Spells.Regen_V, 100 },
            { Spells.Reraise, 150 },
            { Spells.Reraise_II, 150 },
            { Spells.Reraise_III, 150 },
            { Spells.Reraise_IV, 150 },
        };

        public static string[] CureTiers = { Spells.Cure, Spells.Cure_II, Spells.Cure_III, Spells.Cure_IV, Spells.Cure_V, Spells.Cure_VI };

        public static string[] CuragaTiers = { Spells.Curaga, Spells.Curaga_II, Spells.Curaga_III, Spells.Curaga_IV, Spells.Curaga_V };

        public static string[] ProtectTiers = { Spells.Protect, Spells.Protect_II, Spells.Protect_III, Spells.Protect_IV, Spells.Protect_V };
        public static string[] ShellTiers = { Spells.Shell, Spells.Shell_II, Spells.Shell_III, Spells.Shell_IV, Spells.Shell_V };
       
        public static string[] ProtectraTiers = { Spells.Protectra, Spells.Protectra_II, Spells.Protectra_III, Spells.Protectra_IV, Spells.Protectra_V };
        public static string[] ShellraTiers = { Spells.Shellra, Spells.Shellra_II, Spells.Shellra_III, Spells.Shellra_IV, Spells.Shellra_V };

        public static string[] ReraiseTiers = { Spells.Reraise, Spells.Reraise_II, Spells.Reraise_III, Spells.Reraise_IV };

        public static string[] RegenTiers = { Spells.Regen, Spells.Regen_II, Spells.Regen_III, Spells.Regen_IV, Spells.Regen_V };
        public static string[] RefreshTiers = { Spells.Refresh, Spells.Refresh_II, Spells.Refresh_III };

        public static string[] HasteTiers = { Spells.Haste, Spells.Haste_II };
        public static string[] TemperTiers = { Spells.Temper, Spells.Temper_II };

        public static string[] SpikesSpells = { Spells.Blaze_Spikes, Spells.Ice_Spikes, Spells.Shock_Spikes };

        public static string[] GainBoostSpells = {
            Spells.Gain_STR,Spells.Gain_DEX, Spells.Gain_VIT, Spells.Gain_AGI, Spells.Gain_INT, Spells.Gain_MND, Spells.Gain_CHR,
            Spells.Boost_STR, Spells.Boost_DEX, Spells.Boost_VIT, Spells.Boost_AGI, Spells.Boost_INT, Spells.Boost_MND, Spells.Boost_CHR
        };

        public static string[] StormTiers =
        {
            Spells.Firestorm, Spells.Hailstorm, Spells.Windstorm, Spells.Sandstorm, Spells.Thunderstorm, Spells.Rainstorm, Spells.Aurorastorm, Spells.Voidstorm,
            Spells.Firestorm_II, Spells.Hailstorm_II, Spells.Windstorm_II, Spells.Sandstorm_II, Spells.Thunderstorm_II, Spells.Rainstorm_II, Spells.Aurorastorm_II, Spells.Voidstorm_II
        };

        public static string[] BarSpells = { Spells.Barfire, Spells.Barstone, Spells.Barwater, Spells.Baraero, Spells.Barblizzard, Spells.Barthunder };

        public static string[] AoeBarSpells = { Spells.Barfira, Spells.Barstonra, Spells.Barwatera, Spells.Baraera, Spells.Barblizzara, Spells.Barthundra };

        public static string[] BarStatus = { Spells.Baramnesia, Spells.Barvirus, Spells.Barparalyze, Spells.Barsilence, Spells.Barpetrify, Spells.Barpoison, Spells.Barblind, Spells.Barsleep };

        public static string[] AoeBarStatus = { Spells.Baramnesra, Spells.Barvira, Spells.Barparalyzra, Spells.Barsilencera, Spells.Barpetra, Spells.Barpoisonra, Spells.Barblindra, Spells.Barsleepra };

        public static string[] Enspells = { 
            Spells.Enfire, Spells.Enstone, Spells.Enwater, Spells.Enaero, Spells.Enblizzard, Spells.Enthunder,
            Spells.Enfire_II, Spells.Enstone_II, Spells.Enwater_II, Spells.Enaero_II, Spells.Enblizzard_II, Spells.Enthunder_II,
        };

        // This is tricky because the EliteAPI is closed source, so we're stuck working around it.
        // In this case they don't have the newer status effects yet, so we need to still treat them
        // as shorts, for the situations where there isn't an enum value for it.
        public static Dictionary<string, short> SpellEffects = new Dictionary<string, short>
        {
            { Spells.Gain_STR, (short)StatusEffect.STR_Boost2 },
            { Spells.Gain_DEX, (short)StatusEffect.DEX_Boost2 },
            { Spells.Gain_VIT, (short)StatusEffect.VIT_Boost2 },
            { Spells.Gain_AGI, (short)StatusEffect.AGI_Boost2 },
            { Spells.Gain_INT, (short)StatusEffect.INT_Boost2 },
            { Spells.Gain_MND, (short)StatusEffect.MND_Boost2 },
            { Spells.Gain_CHR, (short)StatusEffect.CHR_Boost2 },
            { Spells.Boost_STR, (short)StatusEffect.STR_Boost2 },
            { Spells.Boost_DEX, (short)StatusEffect.DEX_Boost2 },
            { Spells.Boost_VIT, (short)StatusEffect.VIT_Boost2 },
            { Spells.Boost_AGI, (short)StatusEffect.AGI_Boost2 },
            { Spells.Boost_INT, (short)StatusEffect.INT_Boost2 },
            { Spells.Boost_MND, (short)StatusEffect.MND_Boost2 },
            { Spells.Boost_CHR, (short)StatusEffect.CHR_Boost2 },
            { Spells.Firestorm, (short)StatusEffect.Firestorm },
            { Spells.Hailstorm, (short)StatusEffect.Hailstorm },
            { Spells.Windstorm, (short)StatusEffect.Windstorm },
            { Spells.Sandstorm, (short)StatusEffect.Sandstorm },
            { Spells.Thunderstorm, (short)StatusEffect.Thunderstorm },
            { Spells.Rainstorm, (short)StatusEffect.Rainstorm },
            { Spells.Aurorastorm, (short)StatusEffect.Aurorastorm },
            { Spells.Voidstorm, (short)StatusEffect.Voidstorm },
            { Spells.Firestorm_II, 589 },
            { Spells.Hailstorm_II, 590},
            { Spells.Windstorm_II, 591 },
            { Spells.Sandstorm_II, 592 },
            { Spells.Thunderstorm_II, 593 },
            { Spells.Rainstorm_II, 594 },
            { Spells.Aurorastorm_II, 595 },
            { Spells.Voidstorm_II, 596 },
            { Spells.Barfire, (short)StatusEffect.Barfire },
            { Spells.Barfira, (short)StatusEffect.Barfire },
            { Spells.Barstone, (short)StatusEffect.Barstone },
            { Spells.Barstonra, (short)StatusEffect.Barstone },
            { Spells.Barwater, (short)StatusEffect.Barwater },
            { Spells.Barwatera, (short)StatusEffect.Barwater },
            { Spells.Baraero, (short)StatusEffect.Baraero },
            { Spells.Baraera, (short)StatusEffect.Baraero },
            { Spells.Barblizzard, (short)StatusEffect.Barblizzard },
            { Spells.Barblizzara, (short)StatusEffect.Barblizzard },
            { Spells.Barthunder, (short)StatusEffect.Barthunder },
            { Spells.Barthundra, (short)StatusEffect.Barthunder },
            { Spells.Baramnesia, (short)StatusEffect.Baramnesia },
            { Spells.Baramnesra, (short)StatusEffect.Baramnesia },
            { Spells.Barvirus, (short)StatusEffect.Barvirus },
            { Spells.Barvira, (short)StatusEffect.Barvirus },
            { Spells.Barparalyze, (short)StatusEffect.Barparalyze },
            { Spells.Barparalyzra, (short)StatusEffect.Barparalyze },
            { Spells.Barsilence, (short)StatusEffect.Barsilence },
            { Spells.Barsilencera, (short)StatusEffect.Barsilence },
            { Spells.Barpetrify, (short)StatusEffect.Barpetrify },
            { Spells.Barpetra, (short)StatusEffect.Barpetrify },
            { Spells.Barpoison, (short)StatusEffect.Barpoison },
            { Spells.Barpoisonra, (short)StatusEffect.Barpoison },
            { Spells.Barblind, (short)StatusEffect.Barblind },
            { Spells.Barblindra, (short)StatusEffect.Barblind },
            { Spells.Barsleep, (short)StatusEffect.Barsleep },
            { Spells.Barsleepra, (short)StatusEffect.Barsleep },
            { Spells.Enfire, (short)StatusEffect.Enfire },
            { Spells.Enstone, (short)StatusEffect.Enstone },
            { Spells.Enwater, (short)StatusEffect.Enwater },
            { Spells.Enaero, (short)StatusEffect.Enaero },
            { Spells.Enblizzard, (short)StatusEffect.Enblizzard },
            { Spells.Enthunder, (short)StatusEffect.Enthunder },
            { Spells.Enfire_II, (short)StatusEffect.Enfire_2 },
            { Spells.Enstone_II, (short)StatusEffect.Enstone_2 },
            { Spells.Enwater_II, (short)StatusEffect.Enwater_2 },
            { Spells.Enaero_II, (short)StatusEffect.Enaero_2 },
            { Spells.Enblizzard_II, (short)StatusEffect.Enblizzard_2 },
            { Spells.Enthunder_II, (short)StatusEffect.Enthunder_2 },
            { Spells.Haste, (short)StatusEffect.Haste },
            { Spells.Haste_II, (short)StatusEffect.Haste },
            { Spells.Flurry, (short)StatusEffect.Flurry },
            { Spells.Flurry_II, (short)StatusEffect.Flurry },
            { Spells.Adloquium, (short)StatusEffect.Regain },
            { Spells.Protect, (short)StatusEffect.Protect },
            { Spells.Protect_II, (short)StatusEffect.Protect },
            { Spells.Protect_III, (short)StatusEffect.Protect },
            { Spells.Protect_IV, (short)StatusEffect.Protect },
            { Spells.Protect_V, (short)StatusEffect.Protect },
            { Spells.Shell, (short)StatusEffect.Shell },
            { Spells.Shell_II, (short)StatusEffect.Shell },
            { Spells.Shell_III, (short)StatusEffect.Shell },
            { Spells.Shell_IV, (short)StatusEffect.Shell },
            { Spells.Shell_V, (short)StatusEffect.Shell },
            { Spells.Regen, (short)StatusEffect.Regen },
            { Spells.Regen_II, (short)StatusEffect.Regen },
            { Spells.Regen_III, (short)StatusEffect.Regen },
            { Spells.Regen_IV, (short)StatusEffect.Regen },
            { Spells.Regen_V, (short)StatusEffect.Regen },
            { Spells.Refresh, (short)StatusEffect.Refresh },
            { Spells.Refresh_II, (short)StatusEffect.Refresh },
            { Spells.Refresh_III, (short)StatusEffect.Refresh },
            { Spells.Phalanx, (short)StatusEffect.Phalanx },
            { Spells.Phalanx_II, (short)StatusEffect.Phalanx },
            { Spells.Blaze_Spikes, (short)StatusEffect.Blaze_Spikes },
            { Spells.Ice_Spikes, (short)StatusEffect.Ice_Spikes },
            { Spells.Shock_Spikes, (short)StatusEffect.Shock_Spikes },

        };

        public static StatusEffect[] GainBoostEffects =
        {
            StatusEffect.STR_Boost2, StatusEffect.DEX_Boost2, StatusEffect.VIT_Boost2, StatusEffect.AGI_Boost2, StatusEffect.INT_Boost2, StatusEffect.MND_Boost2, StatusEffect.CHR_Boost2
        };

        // We use this dictionary to both prioritize certain debuffs above others, and map which spells are
        // used to cure each debuff.
        // TODO: Currently this is hardcoded to only auto-sleep charmed members on bard, will have to make this configurable
        // in a generic way later.
        public static Dictionary<short, string> DebuffPriorities = new Dictionary<short, string>
        {
            { (short)StatusEffect.Charm1, Spells.Sleep },
            { (short)StatusEffect.Charm2, Spells.Sleep },
            { (short)StatusEffect.Doom, Spells.Cursna },
            { (short)StatusEffect.Sleep, Spells.Curaga },
            { (short)StatusEffect.Sleep2, Spells.Curaga },
            { (short)StatusEffect.Petrification, Spells.Stona },
            { (short)StatusEffect.Silence, Spells.Silena },
            { (short)StatusEffect.Bind, Spells.Erase },
            { (short)StatusEffect.Weight, Spells.Erase },
            { (short)StatusEffect.Paralysis, Spells.Paralyna },
            { 566, Spells.Paralyna },
            { (short)StatusEffect.Amnesia, Spells.Esuna },
            { (short)StatusEffect.Slow, Spells.Erase },
            { (short)StatusEffect.Blindness, Spells.Blindna },
            { (short)StatusEffect.Poison, Spells.Poisona }, 
            { (short)StatusEffect.Attack_Down, Spells.Erase }, 
            { (short)StatusEffect.Curse, Spells.Cursna }, 
            { (short)StatusEffect.Curse2, Spells.Cursna }, 
            { (short)StatusEffect.Addle, Spells.Erase }, 
            { (short)StatusEffect.Bane, Spells.Cursna }, 
            { (short)StatusEffect.Plague, Spells.Viruna },
            { (short)StatusEffect.Disease, Spells.Viruna }, 
            { (short)StatusEffect.Burn, Spells.Erase },
            { (short)StatusEffect.Frost, Spells.Erase },
            { (short)StatusEffect.Choke, Spells.Erase },
            { (short)StatusEffect.Rasp, Spells.Erase }, 
            { (short)StatusEffect.Shock, Spells.Erase }, 
            { (short)StatusEffect.Drown, Spells.Erase },
            { (short)StatusEffect.Dia, Spells.Erase },
            { (short)StatusEffect.Bio, Spells.Erase },
            { (short)StatusEffect.STR_Down, Spells.Erase },
            { (short)StatusEffect.DEX_Down, Spells.Erase },
            { (short)StatusEffect.VIT_Down, Spells.Erase }, 
            { (short)StatusEffect.AGI_Down, Spells.Erase }, 
            { (short)StatusEffect.INT_Down, Spells.Erase }, 
            { (short)StatusEffect.MND_Down, Spells.Erase }, 
            { (short)StatusEffect.CHR_Down, Spells.Erase }, 
            { (short)StatusEffect.Max_HP_Down, Spells.Erase }, 
            { (short)StatusEffect.Max_MP_Down, Spells.Erase }, 
            { (short)StatusEffect.Accuracy_Down, Spells.Erase },
            { (short)StatusEffect.Evasion_Down, Spells.Erase },
            { (short)StatusEffect.Defense_Down, Spells.Erase }, 
            { (short)StatusEffect.Flash, Spells.Erase }, 
            { (short)StatusEffect.Magic_Acc_Down, Spells.Erase }, 
            { (short)StatusEffect.Magic_Atk_Down, Spells.Erase }, 
            { (short)StatusEffect.Helix, Spells.Erase },
            { (short)StatusEffect.Max_TP_Down, Spells.Erase },
            { (short)StatusEffect.Requiem, Spells.Erase },
            { (short)StatusEffect.Elegy, Spells.Erase }, 
            { (short)StatusEffect.Threnody, Spells.Erase }
        };

        public static Dictionary<string, Tuple<Job, int>> JobPointSpells = new Dictionary<string, Tuple<Job, int>>
        {
            { Spells.Temper_II, Tuple.Create(Job.RDM, 1200) },
            { Spells.Refresh_III, Tuple.Create(Job.RDM, 1200) },
            { Spells.Distract_III, Tuple.Create(Job.RDM, 550) },
            { Spells.Frazzle_III, Tuple.Create(Job.RDM, 550) },
            { Spells.Firestorm_II, Tuple.Create(Job.SCH, 100) },
            { Spells.Hailstorm_II, Tuple.Create(Job.SCH, 100) },
            { Spells.Windstorm_II, Tuple.Create(Job.SCH, 100) },
            { Spells.Sandstorm_II, Tuple.Create(Job.SCH, 100) },
            { Spells.Thunderstorm_II, Tuple.Create(Job.SCH, 100) },
            { Spells.Rainstorm_II, Tuple.Create(Job.SCH, 100) },
            { Spells.Aurorastorm_II, Tuple.Create(Job.SCH, 100) },
            { Spells.Voidstorm_II, Tuple.Create(Job.SCH, 100) },
            { Spells.Reraise_IV, Tuple.Create(Job.WHM, 100) },
            { Spells.FullCure, Tuple.Create(Job.WHM, 1200) },
        };

        public static string[] WakeSleepSpells = { Spells.Cure, Spells.Cura, Spells.Curaga };

    }
}
