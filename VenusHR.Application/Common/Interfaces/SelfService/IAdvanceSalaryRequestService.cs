using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IAdvanceSalaryRequestService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_AdvanceSalaryRequest request);
        object Update(SS_AdvanceSalaryRequest request);
        object Delete(int id);
    }
}
