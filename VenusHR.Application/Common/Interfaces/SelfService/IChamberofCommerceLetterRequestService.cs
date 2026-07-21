using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IChamberofCommerceLetterRequestService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_ChamberofCommerceLetterRequest request);
        object Update(SS_ChamberofCommerceLetterRequest request);
        object Delete(int id);
    }
}
