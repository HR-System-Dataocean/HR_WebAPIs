using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IBankAccountUpdateService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_BankAccountUpdate request);
        object Update(SS_BankAccountUpdate request);
        object Delete(int id);
    }
}
