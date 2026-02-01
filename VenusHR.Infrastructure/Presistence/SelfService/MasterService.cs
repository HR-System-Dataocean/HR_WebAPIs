 using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VenusHR.Application.Common;
using VenusHR.Application.Common.Interfaces.SelfService;
using VenusHR.Core.Login;
using VenusHR.Core.Master;
using VenusHR.Core.SelfService;
using WorkFlow_EF;

namespace VenusHR.Infrastructure.Presistence.SelfService
{
    public class MasterService : IMaster
    {
        private readonly ApplicationDBContext _context;
        private GeneralOutputClass <object> Result;
        public MasterService(ApplicationDBContext context)
        {
                            _context = context;

        }

        public object GaetTequestDetails(string FormCOde, int ReauestSerial, int Lang)
        {
            throw new NotImplementedException();
        }

        public object GetAllEmployees(int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang==1)
                {
                    Result.ResultObject = _context.Hrs_Employees.Select(E => new { E.id, E.Code, ArabName = E.ArbName + " " + E.FatherArbName +""+E.FamilyArbName,E.JoinDate }).ToList();
                }
                else
                {
                    Result.ResultObject = _context.Hrs_Employees.Select(E => new { E.id, E.Code, EngName = E.EngName + " " + E.FamilyEngName + "" + E.FamilyEngName, E.JoinDate }).ToList();

                }

                Result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }


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
                                           join SS_UserActions in _context.SS_UserActions
                                          on SS_RequestActions.ActionId equals SS_UserActions.Id into _U
                                          from x in _U.DefaultIfEmpty()
                                          where (SS_RequestActions.Ss_EmployeeId == SSEmployeeID && SS_RequestActions.Seen != true)
                                          select new
                                          {
                                              RequestID=SS_RequestActions.ID,
                                              employeeId = Hrs_Employees.id,
                                              RequestType = SS_RequestTypes.RequestArbName,
                                              FormCode = SS_RequestTypes.RequestCode,
                                               RequestCode = SS_RequestActions.RequestSerial,
                                              ConfigID = SS_RequestActions.ConfigId,
                                              EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName,
                                           };
                    
                }
                else
                {
                    Result.ResultObject = from SS_RequestActions in _context.SS_RequestActions

                                          join Hrs_Employees in _context.Hrs_Employees
                                              on SS_RequestActions.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestTypes in _context.SS_RequestTypes
                                          on SS_RequestActions.FormCode equals SS_RequestTypes.RequestCode
                                           join SS_UserActions in _context.SS_UserActions
                                          on SS_RequestActions.ActionId equals SS_UserActions.Id into _U
                                          from x in _U.DefaultIfEmpty()
                                          where (SS_RequestActions.Ss_EmployeeId == SSEmployeeID && SS_RequestActions.Seen != true)
                                          select new {
                                              RequestID = SS_RequestActions.ID,
                                              employeeId = Hrs_Employees.id,
                                              RequestType = SS_RequestTypes.RequestArbName,
                                              FormCode = SS_RequestTypes.RequestCode,
                                              RequestCode = SS_RequestActions.RequestSerial,
                                              ConfigID = SS_RequestActions.ConfigId,
                                              EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
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


        public object GetAllRequestTypes()
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                Result.ResultObject = _context.SS_RequestTypes.Select(H => new { H.RequestCode, H.RequestArbName, H.RequestEngName,H.NoOfTimes,H.TimesPeriodPerMonth,H.AutoSerialAttach,H.RequiredAttach });
                Result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }
            return Result;
        }

        public object GetAllVacationsTypes(int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                Result.ResultObject = _context.hrs_VacationsTypes.Where(ACC => ACC.IsAnnual != true)
              .ToList();
                Result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }
            return Result;
        }

        public object GetEmployeeByID(int employeeId,int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from Hrs_Employees in _context.Hrs_Employees
                                          join sys_Departments in _context.sys_Departments
                                          on Hrs_Employees.DepartmentId equals sys_Departments.ID
                                          join hrs_Contracts in _context.hrs_Contracts
                                          on Hrs_Employees.id equals hrs_Contracts.EmployeeID
                                          join hrs_Positions in _context.hrs_Positions
                                          on hrs_Contracts.PositionID equals hrs_Positions.Id
                                          where (Hrs_Employees.id == employeeId)
                                          select new { Position = hrs_Positions.ArbName, Department = sys_Departments.ArbName, PhoneNo = Hrs_Employees.Phone, Email = Hrs_Employees.WorkE_Mail, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };


                }
                else
                {
                    Result.ResultObject = from Hrs_Employees in _context.Hrs_Employees
                                          join sys_Departments in _context.sys_Departments
                                          on Hrs_Employees.DepartmentId equals sys_Departments.ID
                                          join hrs_Contracts in _context.hrs_Contracts
                                          on Hrs_Employees.id equals hrs_Contracts.EmployeeID
                                          join hrs_Positions in _context.hrs_Positions
                                          on hrs_Contracts.PositionID equals hrs_Positions.Id
                                          where (Hrs_Employees.id == employeeId)
                                          select new { Position = hrs_Positions.EngName, Department = sys_Departments.EngName, PhoneNo = Hrs_Employees.Phone, Email = Hrs_Employees.WorkE_Mail, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }
                Result.ErrorCode = 1;
 
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }


            return Result;
        }

        public object GetEndOfServiceAllExperienceRate(int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = _context.SS_ExperienceRate
               .Select(C => new { C.ID, C.ArbName })
                    .ToList();


                    Result.ErrorCode = 1;
                }
                else
                {
                    Result.ResultObject = _context.SS_ExperienceRate
               .Select(C => new { C.ID, C.EngName })
                    .ToList();


                    Result.ErrorCode = 1;
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

        public object GetEndOfServiceAllResignationReason(int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = _context.SS_ResignationReason
               .Select(C => new { C.ID, C.ArbName })
                    .ToList();


                    Result.ErrorCode = 1;
                }
                else
                {
                    Result.ResultObject = _context.SS_ResignationReason
               .Select(C => new { C.ID, C.EngName })
                    .ToList();


                    Result.ErrorCode = 1;
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

        public object GetUserNotificationCount(string EmployeeId)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                Result.ResultObject = _context.SS_RequestActions.Where(ACC => ACC.Seen != true)
              .Count(R => R.Ss_EmployeeId == int.Parse(EmployeeId));
                Result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }
            return Result;
        }
        public object SaveRequestAction(SS_RequestAction RequestAction)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
              
                    int newstatus = RequestAction.ActionId??0;
                 
                
                var existingRecord = _context.SS_RequestActions
                   .FirstOrDefault(r => r.RequestSerial == RequestAction.RequestSerial
                                     && r.EmployeeId == RequestAction.EmployeeId
                                     && r.Ss_EmployeeId == RequestAction.Ss_EmployeeId
                                     && r.ID == RequestAction.ID);


                if (existingRecord != null)
                {
                     existingRecord.Seen = true; 
                    existingRecord.ActionId = RequestAction.ActionId;  
                    existingRecord.ActionDate = DateTime.Now;
                    existingRecord.ActionRemarks = RequestAction.ActionRemarks;
                    existingRecord.ConfirmedNoOfdays = RequestAction.ConfirmedNoOfdays;
                  
                    existingRecord.IsHidden = RequestAction.IsHidden;

                     _context.SS_RequestActions.Update(existingRecord);

            

                _context.SaveChanges();
                int CurrentRank = _context.SS_Configuration.Where(C => C.FormCode == RequestAction.FormCode && C.ID == RequestAction.ConfigId).Select(R => R.Rank).FirstOrDefault();
                int NextRank = 0;
                  NextRank =_context.SS_Configuration.Where(C => C.Rank > CurrentRank && C.FormCode == RequestAction.FormCode).Select(R => R.Rank).FirstOrDefault();
                if (NextRank>0 && RequestAction.ActionId!=4 && RequestAction.ActionId!=2)
                {
                    UpdateRequestStatus(RequestAction.RequestSerial, RequestAction.FormCode, 4);
                  

                }
                else if (NextRank > 0 && RequestAction.ActionId == 2)
                {
                        UpdateRequestStatus(RequestAction.RequestSerial, RequestAction.FormCode, 2);
                        var ApplyForAll = _context.SS_Configuration.Where(C => C.FormCode == RequestAction.FormCode && C.ID == RequestAction.ConfigId).Select(X => X.ApplyForAll).FirstOrDefault();
                        if (ApplyForAll == true)
                        {
                            var allRelatedRecords = _context.SS_RequestActions
                           .Where(ra => ra.RequestSerial == RequestAction.RequestSerial
                                     && ra.FormCode == RequestAction.FormCode
                                     && ra.ConfigId == RequestAction.ConfigId
                                     && ra.Ss_EmployeeId != RequestAction.Ss_EmployeeId)
                           .ToList();
                            if (allRelatedRecords != null)
                            {
                                foreach (var record in allRelatedRecords)
                                {
                                    record.Seen = true;
                                    record.IsHidden = true;

                                }

                                _context.SaveChanges();
                            }

                        }
                    }
                    else
                {
                    if (NextRank == 0)
                    {
                        if (RequestAction.ActionId == 1)
                        {
                            UpdateRequestStatus(RequestAction.RequestSerial, RequestAction.FormCode, 1);

                        }
                        if (RequestAction.ActionId == 2)
                        {
                            UpdateRequestStatus(RequestAction.RequestSerial, RequestAction.FormCode, 2);
                                 var ApplyForAll = _context.SS_Configuration.Where(C => C.FormCode == RequestAction.FormCode && C.ID == RequestAction.ConfigId).Select(X => X.ApplyForAll).FirstOrDefault();
                                if (ApplyForAll == true)
                                {
                                    var allRelatedRecords = _context.SS_RequestActions
                                   .Where(ra => ra.RequestSerial == RequestAction.RequestSerial
                                             && ra.FormCode == RequestAction.FormCode
                                             && ra.ConfigId == RequestAction.ConfigId
                                             && ra.Ss_EmployeeId != RequestAction.Ss_EmployeeId)
                                   .ToList();
                                    if (allRelatedRecords != null)
                                    {
                                        foreach (var record in allRelatedRecords)
                                        {
                                            record.Seen = true;
                                            record.IsHidden = true;

                                        }

                                        _context.SaveChanges();
                                    }

                                }

                            }
                       
                    }
                   
                 
                }
                if (RequestAction.ActionId == 1)
                {
                    SaveRequestActionNextLevel(RequestAction.ConfigId, RequestAction.FormCode, RequestAction.EmployeeId, RequestAction.RequestSerial,RequestAction.Ss_EmployeeId);
                }

                Result.ErrorMessage = "Transaction Done Successfully";
                Result.ErrorCode = 1;
                }
                else
                {

                    if (RequestAction.ActionId == 4)
                    {
                        //need to check can be canceled in this level or not
                        //1- Get Last ConfigID For This Requst
                        var LastActionRecord = _context.SS_RequestActions
    .Where(S => S.FormCode == RequestAction.FormCode && S.RequestSerial == RequestAction.RequestSerial)
    .OrderByDescending(S => S.ID)  
    .FirstOrDefault();
                        if (LastActionRecord != null)
                        {
                            int ConfigID = LastActionRecord.ConfigId;
                            if (ConfigID > 0)
                            {
                                bool Canbecanceled = _context.SS_Configuration.Where(s => s.ID == ConfigID).Select(R => R.CanBeCanceledInThisLevel).FirstOrDefault();
                                if (Canbecanceled)
                                {
                                    var newAction = new SS_RequestAction
                                    {
                                        RequestSerial = RequestAction.RequestSerial,
                                        Ss_EmployeeId = RequestAction.EmployeeId,
                                        FormCode = RequestAction.FormCode,
                                        ConfigId = 0,
                                        EmployeeId = RequestAction.EmployeeId,
                                        Seen = true,
                                        ActionId = RequestAction.ActionId,
                                        ActionRemarks = RequestAction.ActionRemarks,
                                        ActionDate = RequestAction.ActionDate,
                                    };
                                    _context.SS_RequestActions.Add(newAction);

                                    _context.SaveChanges();
                                    newAction.ActionSerial = newAction.ID;
                                    _context.SaveChanges();
                                    // Update كل الـ Records المرتبطة بنفس الـ RequestSerial
                                    var relatedRecords = _context.SS_RequestActions
                                        .Where(r => r.RequestSerial == RequestAction.RequestSerial && r.FormCode == RequestAction.FormCode)
                                        .ToList();

                                    foreach (var record in relatedRecords)
                                    {
                                        record.Seen = true;

                                    }

                                    _context.SaveChanges();

                                    UpdateRequestStatus(RequestAction.RequestSerial, RequestAction.FormCode, 5);
                                    Result.ErrorMessage = "Request Canceled Successfully";
                                    Result.ErrorCode = 1;

                                }
                                else
                                {
                                    Result.ErrorMessage = "Sorry...Request Can't be canceled in this stage";
                                    Result.ErrorCode = 0;
                                }
                              
                            }
                            }
                        }

             

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" خطأ في SaveRequestAction: {ex.Message}");
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }
            return Result;
        }
 
     
        public object SaveRequestActionNextLevel(int ConfigID,string FormCode, int EmpID, int RequestSerial,int SSEmployeeID)
        {
            Result = new GeneralOutputClass<object>();
            var recipients = new List<SysUser>();

            var CurrentRank = _context.SS_Configuration.Where(C => C.FormCode == FormCode && C.ID == ConfigID).Select(R=>R.Rank).FirstOrDefault();
            var ApplyForAll= _context.SS_Configuration.Where(C => C.FormCode == FormCode && C.ID == ConfigID).Select(X=>X.ApplyForAll).FirstOrDefault();
            if (ApplyForAll==true)
            {
                var allRelatedRecords = _context.SS_RequestActions
               .Where(ra => ra.RequestSerial == RequestSerial
                         && ra.FormCode == FormCode
                         && ra.ConfigId == ConfigID
                         &&ra.Ss_EmployeeId!= SSEmployeeID)
               .ToList();
                if (allRelatedRecords !=null)
                {
                    foreach (var record in allRelatedRecords)
                    {
                        record.Seen = true;
                        record.IsHidden = true;

                    }

                    _context.SaveChanges();
                }
                
            }
            var Configuration = _context.SS_Configuration.Where(C => C.FormCode == FormCode  &&  C.Rank == (CurrentRank + 1)).FirstOrDefault();
            if (Configuration!=null)
            {
                int UserTypeID = int.Parse(Configuration.UserTypeID);
            if (UserTypeID == 1)
            {
                int DitrectManager = (int)_context.Hrs_Employees.Where(E => E.id == EmpID).Select(E => E.ManagerId).FirstOrDefault();
                if (DitrectManager > 0)
                {
                        //_context.SS_RequestActions.Add(new SS_RequestAction { RequestSerial = RequestSerial, Ss_EmployeeId = DitrectManager, FormCode = FormCode, ConfigId = Configuration.ID, EmployeeId = EmpID, Seen = false });

                        var newAction = new SS_RequestAction
                        {
                            RequestSerial = RequestSerial,
                            Ss_EmployeeId = DitrectManager,
                            FormCode = FormCode,
                            ConfigId = Configuration.ID,
                            EmployeeId = EmpID,
                            Seen = false
                        };
                        _context.SS_RequestActions.Add(newAction);
                        _context.SaveChanges();
                        newAction.ActionSerial = newAction.ID;
                        _context.SaveChanges();

                    }



                    var managerToken = _context.Sys_Users
            .Where(u => u.RelEmployee == DitrectManager && u.DeviceToken != null)
            .Select(u => new SysUser
            {
                DeviceToken = u.DeviceToken,
                EngName = u.EngName,
                ArbName = u.ArbName,
                Code = u.Code,
            })
            .FirstOrDefault();

                    if (managerToken != null)
                        recipients.Add(managerToken);


                }
            //Position
            else if (UserTypeID == 2 && Configuration.PositionID != null)
            {
                
                    int ssEmpID = 0;

                    var EmployssinPosition = _context.hrs_Contracts.Where(E => E.PositionID == int.Parse(Configuration.PositionID) && E.CancelDate == null && (E.EndDate > DateTime.Now || E.EndDate == null)).Select(C => new { C.EmployeeID }).ToList();
                    foreach (var Position in EmployssinPosition)
                    {
                        ssEmpID = Position.EmployeeID;
                        var newAction = new SS_RequestAction
                        {
                            RequestSerial = RequestSerial,
                            Ss_EmployeeId = Position.EmployeeID,
                            FormCode = FormCode,
                            ConfigId = Configuration.ID,
                            EmployeeId = EmpID,
                            Seen = false
                        };

                        _context.SS_RequestActions.Add(newAction);
                        _context.SaveChanges();

                        newAction.ActionSerial = newAction.ID;
                        _context.SaveChanges();
                    }
                    if (ssEmpID > 0)
                    {
                        var employeeToken = _context.Sys_Users
    .Where(u => u.RelEmployee == ssEmpID && u.DeviceToken != null)
    .Select(u => new SysUser
    {
        DeviceToken = u.DeviceToken,
        EngName = u.EngName,
        ArbName = u.ArbName,
        Code = u.Code
    })
    .FirstOrDefault();

                        if (employeeToken != null)
                            recipients.Add(employeeToken);
                    }


                }
            //Employee
            else if (UserTypeID == 3 && Configuration.EmployeeID != null)
            {

                    var newAction = new SS_RequestAction
                    {
                        RequestSerial = RequestSerial,
                        Ss_EmployeeId = int.Parse(Configuration.EmployeeID),
                        FormCode = FormCode,
                        ConfigId = Configuration.ID,
                        EmployeeId = EmpID,
                        Seen = false
                    };

                    _context.SS_RequestActions.Add(newAction);
                    _context.SaveChanges();

                    newAction.ActionSerial = newAction.ID;
                    _context.SaveChanges();
                    var employeeToken = _context.Sys_Users
    .Where(u => u.RelEmployee == int.Parse(Configuration.EmployeeID) && u.DeviceToken != null)
    .Select(u => new SysUser
    {
        DeviceToken = u.DeviceToken,
        EngName = u.EngName,
        ArbName = u.ArbName,
        Code = u.Code
    })
    .FirstOrDefault();

                    if (employeeToken != null)
                        recipients.Add(employeeToken);

                }
             }

            Result.ErrorCode = 1;
            Result.ErrorMessage = "Success";
            Result.ResultObject = new
            {
                Recipients = recipients,
                RequestSerial = RequestSerial
            };


            return Result;
        }

        public object UpdateRequestStatus(int requestId, string formCode, int newStatus )
        {
            Result = new GeneralOutputClass<object>();

            try
            {
                 if (requestId <= 0 || string.IsNullOrEmpty(formCode))
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Invalid input parameters";
                    return Result;
                }

                 switch (formCode.ToUpper())
                {
                    case "SS_0011": // Annual Vacation-Admin
                    case "SS_0013": // Other Vacation Request
                        return UpdateVacationRequestStatus(requestId, newStatus);

                    case "SS_0014": // Execuse Request
                        return UpdateExecuseRequestStatus(requestId, newStatus);

                    case "SS_0015": // End Of Service Request
                        return UpdateEndOfServiceStatus(requestId, newStatus);

                    case "SS_00191": // Exit & Re-entry Request
                        return UpdateExitReentryStatus(requestId, newStatus);

                    case "SS_00193": // Bank Letter Request
                        return UpdateBankLetterStatus(requestId, newStatus);

                    case "SS_00194": // Other Letter Request
                        return UpdateOtherLetterStatus(requestId, newStatus);

                    case "SS_00195": // Training Request
                        return UpdateTrainingRequestStatus(requestId, newStatus);

                    case "SS_00196": // Grievance Form Request
                        return UpdateGrievanceStatus(requestId, newStatus);

                    case "SS_00198": // Assault Escalation Form Request
                        return UpdateAssaultEscalationStatus(requestId, newStatus);

                    case "SS_001911": // Daycare Support Request
                        return UpdateDaycareSupportStatus(requestId, newStatus);

                    case "SS_001912": // Education Support Request
                        return UpdateEducationSupportStatus(requestId, newStatus);

                    case "SS_001913": // Advance Housing Request
                        return UpdateAdvanceHousingStatus(requestId, newStatus);

                    case "SS_001914": // Advance Salary Request
                        return UpdateAdvanceSalaryStatus(requestId, newStatus);

                    case "SS_001915": // Chamber of Commerce Letter Request
                        return UpdateChamberCommerceStatus(requestId, newStatus);

                    case "SS_001916": // SCFHS Letter Request
                        return UpdateSCFHSLetterStatus(requestId, newStatus);

                    case "SS_001917": // Pay Slip letter Request
                        return UpdatePaySlipStatus(requestId, newStatus);

                    case "SS_001919": // Overtime Request
                        return UpdateOvertimeStatus(requestId, newStatus);

                    case "SS_001920": // Education Fees Compensation Application Request
                        return UpdateEducationFeesStatus(requestId, newStatus);

                    case "SS_001921": // Bank Account Data Update Request
                        return UpdateBankAccountStatus(requestId, newStatus);

                    case "SS_001922": // Contact Information Update Request
                        return UpdateContactInfoStatus(requestId, newStatus);

                    case "SS_001923": // Dependents Information Update Request
                        return UpdateDependentsInfoStatus(requestId, newStatus);

                    case "SS_001924": // Medical Insurance Adjustments Request
                        return UpdateMedicalInsuranceStatus(requestId, newStatus);

                    case "SS_001925": // Other Legal Document Updates Request
                        return UpdateLegalDocumentsStatus(requestId, newStatus);

                    case "SS_001926": // Employee File Update Request
                        return UpdateEmployeeFileStatus(requestId, newStatus);

                    case "SS_001928": // Annual Ticket Related Request
                        return UpdateAnnualTicketStatus(requestId, newStatus);

                    default:
                        Result.ErrorCode = 0;
                        Result.ErrorMessage = $"Unknown FormCode: {formCode}";
                        return Result;
                }
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating request status: {ex.Message}";
                return Result;
            }
        }

         private object UpdateVacationRequestStatus(int requestId, int newStatus)
        {
            try
            {
 
                var request = _context.SS_VacationRequest.FirstOrDefault(r => r.ID ==  requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;  
                    Result.ErrorMessage = "Request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating vacation request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateExecuseRequestStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_ExecuseRequest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Execuse request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Execuse request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating execuse request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateEndOfServiceStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_EndOfServiceRequest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "End of service request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "End of service request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating end of service request: {ex.Message}";
                return Result;
            }
        }

         private object UpdateExitReentryStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_ExitEntryRequest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Exit & Re-entry request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Exit & Re-entry request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating exit & re-entry request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateBankLetterStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_LoanLetterRequest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Bank letter request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Bank letter request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating bank letter request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateOtherLetterStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_OtherLetterRequest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Other letter request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Other letter request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating other letter request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateTrainingRequestStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_TrainingRequest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Training request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Training request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating training request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateGrievanceStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_GrievanceFormRequest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Grievance request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Grievance request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating grievance request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateAssaultEscalationStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_AssaultEscalationFormRequest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Assault escalation request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Assault escalation request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating assault escalation request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateDaycareSupportStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_DaycareSupportReaquest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Daycare support request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Daycare support request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating daycare support request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateEducationSupportStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_EducationSupportRequest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Education support request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Education support request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating education support request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateAdvanceHousingStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_AdvanceHousingRequest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Advance housing request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Advance housing request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating advance housing request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateAdvanceSalaryStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_AdvanceSalaryRequest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Advance salary request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Advance salary request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating advance salary request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateChamberCommerceStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_ChamberofCommerceLetterRequest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Chamber of commerce request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Chamber of commerce request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating chamber of commerce request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateSCFHSLetterStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_ScfhsletterRequest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "SCFHS letter request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "SCFHS letter request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating SCFHS letter request: {ex.Message}";
                return Result;
            }
        }

        private object UpdatePaySlipStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_PaySlipRequest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Pay slip request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Pay slip request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating pay slip request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateOvertimeStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_OvertimeRequest.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Overtime request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Overtime request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating overtime request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateEducationFeesStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_EducationFeesCompensationApplication.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Education fees request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Education fees request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating education fees request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateBankAccountStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_BankAccountUpdate.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Bank account update request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Bank account update request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating bank account update request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateContactInfoStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_ContactInformationUpdate.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Contact information update request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Contact information update request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating contact information update request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateDependentsInfoStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_DependentsInformationUpdate.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Dependents information update request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Dependents information update request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating dependents information update request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateMedicalInsuranceStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_MedicalInsuranceAdjustments.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Medical insurance request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Medical insurance request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating medical insurance request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateLegalDocumentsStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_OtherLegalDocumentUpdates.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Legal documents request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Legal documents request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating legal documents request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateEmployeeFileStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_EmployeeFileUpdate.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Employee file update request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Employee file update request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating employee file update request: {ex.Message}";
                return Result;
            }
        }

        private object UpdateAnnualTicketStatus(int requestId, int newStatus)
        {
            try
            {
                var request = _context.SS_AnnualTicketRelatedRequests.FirstOrDefault(r => r.Id == requestId);
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Annual ticket request not found";
                    return Result;
                }

                request.RequestStautsTypeId = newStatus;
                _context.SaveChanges();

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Annual ticket request status updated successfully";
                Result.ResultObject = new { RequestId = requestId, NewStatus = newStatus };
                return Result;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error updating annual ticket request: {ex.Message}";
                return Result;
            }
        }





    }
}
