using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IEducationFeesCompensationApplicationService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_EducationFeesCompensationApplication request);
        object Update(SS_EducationFeesCompensationApplication request);
        object Delete(int id);
    }
}
