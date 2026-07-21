using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IOtherLetterRequestService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_OtherLetterRequest request);
        object Update(SS_OtherLetterRequest request);
        object Delete(int id);
    }
}
