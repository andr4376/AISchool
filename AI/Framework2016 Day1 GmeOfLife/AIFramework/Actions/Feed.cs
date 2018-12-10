using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIFramework.Entities;

namespace AIFramework.Actions
{
    public class Feed : MarshalByRefObject,IAction
    {

       public Plant FoodSource { get; set; }

        public Feed(Plant foodSource)
        {
            FoodSource = foodSource;
        }

    }
}
