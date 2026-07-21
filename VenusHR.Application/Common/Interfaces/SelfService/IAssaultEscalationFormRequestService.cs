using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IAssaultEscalationFormRequestService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_AssaultEscalationFormRequest request);
        object Update(SS_AssaultEscalationFormRequest request);
        object Delete(int id);
    }
}
