using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusHR.Application.Common.Interfaces.SelfService;
using VenusHR.Core.SelfService;
using WorkFlow_EF;


namespace VenusHR.Infrastructure.Presistence.SelfService
{
    public class RequestActionService : IRequestActionService
    {
        private readonly ApplicationDBContext _context;
        private GeneralOutputClass<object> Result;


        public RequestActionService(ApplicationDBContext context)
        {
                _context = context;
        }
        public object GetUserActionNotificationCount(int SSEmployeeID)
        {
            Result = new GeneralOutputClass<object>();

            Result.ResultObject=_context.SS_RequestActions.Where(c=>c.Seen != true)
                .Count(R => R.Ss_EmployeeId == SSEmployeeID);
            Result.ErrorCode = 1;

            return Result;
        }

        public object GetAllPendingRequests(int SSEmployeeID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_RequestActions in _context.SS_RequestActions

                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_RequestActions.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestTypes in _context.SS_RequestTypes
                                          on SS_RequestActions.FormCode equals SS_RequestTypes.RequestCode
                                          join SS_VacationRequest in _context.SS_VacationRequest
                                          on SS_RequestActions.RequestSerial equals SS_VacationRequest.ID
                                          join SS_UserActions in _context.SS_UserActions
                                          on SS_RequestActions.ActionId equals SS_UserActions.Id into _U
                                          from x in _U.DefaultIfEmpty()
                                          where (SS_RequestActions.Ss_EmployeeId == SSEmployeeID && SS_RequestActions.Seen != true)
                                          select new
                                          {
                                              RequestType = SS_RequestTypes.RequestArbName,
                                              FormCode = SS_RequestTypes.RequestCode,
                                              RequestDate = SS_VacationRequest.RequestDate,
                                              Requestid = SS_RequestActions.RequestSerial,
                                              RequestCode = SS_VacationRequest.Code,
                                              ConfigID = SS_RequestActions.ConfigId,
                                              EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName,
                                              ActionName = x.ActionAraName
                                          };
                }
                else
                {
                    Result.ResultObject = from SS_RequestActions in _context.SS_RequestActions

                                          join Hrs_Employees in _context.Hrs_Employees
                                              on SS_RequestActions.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestTypes in _context.SS_RequestTypes
                                          on SS_RequestActions.FormCode equals SS_RequestTypes.RequestCode
                                          join SS_VacationRequest in _context.SS_VacationRequest
                                          on SS_RequestActions.RequestSerial equals SS_VacationRequest.ID
                                          join SS_UserActions in _context.SS_UserActions
                                          on SS_RequestActions.ActionId equals SS_UserActions.Id into _U
                                          from x in _U.DefaultIfEmpty()
                                          where (SS_RequestActions.Ss_EmployeeId == SSEmployeeID && SS_RequestActions.Seen != true)
                                          select new { RequestType = SS_RequestTypes.RequestEngName, FormCode = SS_RequestTypes.RequestCode, RequestDate = SS_VacationRequest.RequestDate, RequestCode = SS_RequestActions.ActionSerial, ConfigID = SS_RequestActions.ConfigId, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName, ActionName = x.ActionEngName };
                }
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }
            return Result;
        }

    }
}
