using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusHR.Core.Login;


namespace VenusHR.Application.Common.Interfaces.Login
{
    public interface ILoginServices
    {
        object Login(string username, string password,int Lang, string deviceToken);
    }
}
