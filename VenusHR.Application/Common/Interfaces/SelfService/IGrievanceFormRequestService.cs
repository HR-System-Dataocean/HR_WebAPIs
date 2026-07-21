using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IGrievanceFormRequestService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_GrievanceFormRequest request);
        object Update(SS_GrievanceFormRequest request);
        object Delete(int id);
    }
}
