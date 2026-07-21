using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IChangeWorkHoursRequestService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_ChangeWorkHoursRequest request);
        object Update(SS_ChangeWorkHoursRequest request);
        object Delete(int id);
    }
}
