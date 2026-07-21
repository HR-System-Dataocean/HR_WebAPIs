using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IExitEntryRequestService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_ExitEntryRequest request);
        object Update(SS_ExitEntryRequest request);
        object Delete(int id);
    }
}
