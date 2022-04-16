using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurePlease.Model
{
    public class EngineActionQueue
    {
        private List<EngineAction> actions = new List<EngineAction>();

        public void Push(EngineAction action)
        {
            actions.Add(action);
        }
    }
}
