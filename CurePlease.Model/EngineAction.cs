﻿
namespace CurePlease.Model
{
    // TODO: Update this to handle multiple JA's at once
    // if an engine decision calls for it.
    public class EngineAction
    {
        public int Priority { get; set; }
        public string Spell { get; set; }
        public string Target { get; set; }
        public string JobAbility { get; set; }
        public string Item { get; set; }
        public string CustomAction { get; set; }
        public string Message { 
            get {
                if(!string.IsNullOrEmpty(Item))
                {
                    return $"Using {Item}";
                }

                if(!string.IsNullOrEmpty(CustomAction))
                {
                    return "Using custom action";
                }

                if (!string.IsNullOrEmpty(JobAbility))
                {
                    if(!string.IsNullOrEmpty(Spell))
                    {
                        return $"Using {JobAbility} + Casting {Spell} on {Target}";
                    }

                    return $"Using {JobAbility} on {Target}";
                }

                return $"Casting {Spell} on {Target}";
            } 
        }
        public string Error { get; set; }
    }
}