using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IDaycareSupportReaquestService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_DaycareSupportReaquest request);
        object Update(SS_DaycareSupportReaquest request);
        object Delete(int id);
    }
}
