using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIFramework.Entities;

namespace AIFramework.Actions
{

    public class Attack : MarshalByRefObject, IAction
    {
        public Agent Defender { get; set; }

        public Attack( Agent defender)
        {

            Defender = defender;
        }
    }
}
