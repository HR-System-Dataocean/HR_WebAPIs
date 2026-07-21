using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IPhysiciansPrivilegingFormRequestService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_PhysiciansPrivilegingFormRequest request);
        object Update(SS_PhysiciansPrivilegingFormRequest request);
        object Delete(int id);
    }
}
