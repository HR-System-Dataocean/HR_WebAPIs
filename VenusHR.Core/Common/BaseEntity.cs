using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.Commom
{
    public abstract  class BaseEntity<T>
    {
        public virtual T ID { get; set; }
    }
}
