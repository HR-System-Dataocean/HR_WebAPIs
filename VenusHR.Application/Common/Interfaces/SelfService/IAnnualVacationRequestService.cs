using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusHR.Core.SelfService;

namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public interface IAnnualVacationRequestService
    {
        object GetAnnualVacationRequestByID(int RequestSerial,int Lang);
        object GetAnnualVacationRequestByEmpID(int EmpID,int Lang);
        object GetRequestStages(int RequestSerial, string FormCode,int Lang);
        object GetAnnualVacsBalancesByEMP(int EmpID, DateTime ToDate);
        object SaveAnnualVacationRequest( SS_VacationRequest Request);
        object SaveAnnulavacationMedicalRequest( SS_VacationRequest Request);
        object DeleteWfannualVacationRequests(int id);
        object SaveSelfServiceRequest(object requestData, string requestType);
         object GetSelfServiceRequestDetails(string requestType, int ReauestID, int Lang,int ConfigID);
         object GetMySelfServiceRequestDetails(string requestType, int ReauestID, int Lang);
         object GeatServicePeriod(int EmployeeID,DateTime EndServiceDate);
         object GetEmployeeRequests(int EmployeeID,int Lang);
 

    }

}
