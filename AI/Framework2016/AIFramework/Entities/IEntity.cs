using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIFramework.Entities
{
     public interface IEntity
    {
         AIVector Position { get;}
         string EntityType {get;}

         Guid Id
         {
             get;
         }


    }
}
