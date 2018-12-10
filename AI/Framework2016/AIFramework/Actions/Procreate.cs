using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIFramework.Entities;

namespace AIFramework.Actions
{
    public class Procreate : MarshalByRefObject,IAction
    {
        public Agent Mate { get; set; }

        public Procreate(Agent mate)
        {
            Mate = mate;
        }

    }
}
