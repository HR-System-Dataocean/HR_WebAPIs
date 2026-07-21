using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IContactInformationUpdateService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_ContactInformationUpdate request);
        object Update(SS_ContactInformationUpdate request);
        object Delete(int id);
    }
}
