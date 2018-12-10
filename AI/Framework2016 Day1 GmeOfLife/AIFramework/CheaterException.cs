using AIFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIFramework
{
    class CheaterException : ApplicationException
    {

        public CheaterException(Agent a, string reason)
            : base(String.Format("{0} is a cheater (Reason : {1})",a.GetType().Name,reason))
        {

        }

    }
}
