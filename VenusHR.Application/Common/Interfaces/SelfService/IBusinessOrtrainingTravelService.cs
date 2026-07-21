using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IBusinessOrtrainingTravelService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_BusinessOrtrainingTravel request);
        object Update(SS_BusinessOrtrainingTravel request);
        object Delete(int id);
    }
}
