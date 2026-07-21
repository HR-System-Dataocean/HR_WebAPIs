using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IEmployeeFileUpdateService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_EmployeeFileUpdate request);
        object Update(SS_EmployeeFileUpdate request);
        object Delete(int id);
    }
}
