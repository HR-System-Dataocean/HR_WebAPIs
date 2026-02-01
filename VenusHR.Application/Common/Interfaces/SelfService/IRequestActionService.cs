using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IRequestActionService
    {
        object GetAllPendingRequests(int SSEmployeeID, int Lang);
        object GetUserActionNotificationCount(int SSEmployeeID);

    }
}
