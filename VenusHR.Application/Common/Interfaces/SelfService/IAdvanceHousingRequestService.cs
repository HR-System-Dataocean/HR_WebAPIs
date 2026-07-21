using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IAdvanceHousingRequestService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_AdvanceHousingRequest request);
        object Update(SS_AdvanceHousingRequest request);
        object Delete(int id);
    }
}
