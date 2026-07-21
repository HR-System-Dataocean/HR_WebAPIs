using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IOtherLegalDocumentUpdatesService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_OtherLegalDocumentUpdates request);
        object Update(SS_OtherLegalDocumentUpdates request);
        object Delete(int id);
    }
}
