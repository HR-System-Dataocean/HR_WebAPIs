using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IAnnualTicketRelatedRequestsService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_AnnualTicketRelatedRequests request);
        object Update(SS_AnnualTicketRelatedRequests request);
        object Delete(int id);
    }
}
