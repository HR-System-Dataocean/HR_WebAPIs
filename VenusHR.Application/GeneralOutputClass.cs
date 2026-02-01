using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace WorkFlow_EF
{
    public class GeneralOutputClass<T>
    {
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }
        public T ResultObject { get; set; }
    }
}
