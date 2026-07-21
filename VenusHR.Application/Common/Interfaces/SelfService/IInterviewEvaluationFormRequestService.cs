using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IInterviewEvaluationFormRequestService
    {
        object GetAll(int Lang);
        object GetById(int id, int Lang);
        object GetByEmployeeId(int employeeId, int Lang);
        object Create(SS_InterviewEvaluationFormRequest request);
        object Update(SS_InterviewEvaluationFormRequest request);
        object Delete(int id);
    }
}
