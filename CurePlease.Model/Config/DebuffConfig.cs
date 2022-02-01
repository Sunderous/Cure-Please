
using EliteMMO.API;
using System.Collections.Generic;

namespace CurePlease.Model.Config
{
    public class DebuffConfig
    {
        public string AddonPort { get; set; }
        public string AddonIP { get; set; }
        public int WakeSleepSpell { get; set; }
        public bool PLDebuffEnabled { get; set; }
        public bool MonitoredDebuffEnabled { get; set; }
        public bool PartyDebuffEnabled { get; set; }
        public bool OnlySpecificMembers { get; set; }
        public Dictionary<short, bool> DebuffEnabled;
        public bool PrioritizeOverLowerCures { get; set; }
    }
}
