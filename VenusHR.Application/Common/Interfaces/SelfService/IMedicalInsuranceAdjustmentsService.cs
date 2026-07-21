using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IMedicalInsuranceAdjustmentsService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_MedicalInsuranceAdjustments request);
        object Update(SS_MedicalInsuranceAdjustments request);
        object Delete(int id);
    }
}
