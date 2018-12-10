using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIFramework.Actions
{
    public class Move : MarshalByRefObject,IAction
    {
        public AIVector Direction{get;set;}

        public Move(AIVector direction)
        {
            Direction = direction;
        }
    }
}
