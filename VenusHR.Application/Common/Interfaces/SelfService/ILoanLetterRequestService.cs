using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface ILoanLetterRequestService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_LoanLetterRequest request);
        object Update(SS_LoanLetterRequest request);
        object Delete(int id);
    }
}
