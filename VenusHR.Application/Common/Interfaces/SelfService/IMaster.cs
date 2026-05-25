using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusHR.Core.SelfService;


namespace VenusHR.Application.Common.Interfaces.SelfService
{
    public  interface IMaster
    {
        object GetAllRequestTypes();
        object GetAllEmployees(int Lang);
        object GetEmployeeByID(int employeeId,int Lang);
        object GetUserNotificationCount(string employeeId);
        object GetAllPendingRequests(int EmployeeID ,int Lang);
        object SaveRequestAction(SS_RequestAction RequestAction);
        Object GetAllVacationsTypes(int Lang);
        Object GetEndOfServiceAllResignationReason(int Lang);
        Object GetEndOfServiceAllExperienceRate(int Lang);
        object GetEmployeeMonthlyTransactions(int employeeId, int month, int year, int lang, bool hideNotPaid = true);
        object GetEmployeeDependants(int employeeId, int lang);
        object GetEmployeeVacationBalance(int employeeId, int lang, int? vacationTypeId = null, DateTime? balanceDate = null, DateTime? vacationEndDate = null, int? vacationId = null);
        object GetEmployeeHealthInsurance(int employeeId, int lang);

    }
}
