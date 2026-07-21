using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IDependentsInformationUpdateService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_DependentsInformationUpdate request);
        object Update(SS_DependentsInformationUpdate request);
        object Delete(int id);
    }
}
