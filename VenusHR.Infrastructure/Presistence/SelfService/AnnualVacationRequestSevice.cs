using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using VenusHR.Application.Common.Interfaces.SelfService;
using VenusHR.Core.Login;
using VenusHR.Core.Master;
using VenusHR.Core.SelfService;
using WorkFlow_EF;

namespace VenusHR.Infrastructure.Presistence.SelfService
{
    public class AnnualVacationRequestSevice : IAnnualVacationRequestService
    {
        private readonly ApplicationDBContext _context;
        private GeneralOutputClass<object> Result;
        public AnnualVacationRequestSevice(ApplicationDBContext context)
        {
            _context = context;

        }

     
        public object DeleteWfannualVacationRequests(int id)
        {
            throw new NotImplementedException();
        }

       

        public object GetAnnualVacationRequestByEmpID(int EmpID,int Lang)
        { 
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_VacationRequest in _context.SS_VacationRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_VacationRequest.AlternativeUser equals Hrs_Employees.id

                                          where (SS_VacationRequest.EmployeeID == EmpID)
                                          select new { SS_VacationRequest.EmployeeID, RequestSerial = SS_VacationRequest.Code, SS_VacationRequest.RequestDate, SS_VacationRequest.StartDate, SS_VacationRequest.EndDate, SS_VacationRequest.NoOfDays, SS_VacationRequest.TotalBalance, AlternativeUserID = SS_VacationRequest.AlternativeUser, AlternativeUser = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_VacationRequest in _context.SS_VacationRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_VacationRequest.AlternativeUser equals Hrs_Employees.id
                                          where (SS_VacationRequest.EmployeeID == EmpID)
                                          select new { SS_VacationRequest.EmployeeID, RequestSerial = SS_VacationRequest.Code, SS_VacationRequest.RequestDate, SS_VacationRequest.StartDate, SS_VacationRequest.EndDate, SS_VacationRequest.NoOfDays, SS_VacationRequest.TotalBalance, AlternativeUserID = SS_VacationRequest.AlternativeUser, AlternativeUser = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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

        public object GetAnnualVacationRequestByID(int RequestSerial,int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang==1)
                {
                  Result.ResultObject = from SS_VacationRequest in _context.SS_VacationRequest
                                        join Hrs_Employees in _context.Hrs_Employees
                                        on SS_VacationRequest.AlternativeUser equals Hrs_Employees.id
                                        where (SS_VacationRequest.ID == RequestSerial)
                                        select new { SS_VacationRequest.EmployeeID, Requestid = SS_VacationRequest.ID, RequestSerial = SS_VacationRequest.Code, SS_VacationRequest.RequestDate, SS_VacationRequest.StartDate, SS_VacationRequest.EndDate, SS_VacationRequest.NoOfDays, SS_VacationRequest.TotalBalance, AlternativeUserID =SS_VacationRequest.AlternativeUser, AlternativeUser = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_VacationRequest in _context.SS_VacationRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_VacationRequest.AlternativeUser equals Hrs_Employees.id
                                          where (SS_VacationRequest.ID == RequestSerial)
                                          select new { SS_VacationRequest.EmployeeID, Requestid = SS_VacationRequest.ID, RequestSerial = SS_VacationRequest.Code, SS_VacationRequest.RequestDate, SS_VacationRequest.StartDate, SS_VacationRequest.EndDate, SS_VacationRequest.NoOfDays, SS_VacationRequest.TotalBalance, AlternativeUserID = SS_VacationRequest.AlternativeUser, AlternativeUser = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }
                
                Result.ErrorMessage = "";
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

        public object GetRequestStages(int RequestSerial,string  FormCode,int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_RequestActions in _context.SS_RequestActions

                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_RequestActions.Ss_EmployeeId equals Hrs_Employees.id
                                          join SS_UserActions in _context.SS_UserActions
                                          on SS_RequestActions.ActionId equals SS_UserActions.Id into _U
                                          from x in _U.DefaultIfEmpty()
                                          where (SS_RequestActions.RequestSerial == RequestSerial && SS_RequestActions.FormCode == FormCode && (SS_RequestActions.IsHidden == null || SS_RequestActions.IsHidden == false))
                                          orderby SS_RequestActions.ID descending
                                          select new { Requestid = SS_RequestActions.ActionSerial,FormCode=SS_RequestActions.FormCode ,ConfigID = SS_RequestActions.ConfigId, ActionDate = SS_RequestActions.ActionDate, ActionRemarks = SS_RequestActions.ActionRemarks, ConfirmedNoOfdays = SS_RequestActions.ConfirmedNoOfdays, ActionBy = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName, ActionName = x.ActionAraName };

                }
                else
                {
                    Result.ResultObject = from SS_RequestActions in _context.SS_RequestActions

                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_RequestActions.Ss_EmployeeId equals Hrs_Employees.id
                                          join SS_UserActions in _context.SS_UserActions
                                          on SS_RequestActions.ActionId equals SS_UserActions.Id into _U
                                          from x in _U.DefaultIfEmpty()
                                          where (SS_RequestActions.RequestSerial == RequestSerial && SS_RequestActions.FormCode == FormCode && (SS_RequestActions.IsHidden == null || SS_RequestActions.IsHidden == false))
                                          orderby SS_RequestActions.ID descending
                                          select new { Requestid = SS_RequestActions.ActionSerial, FormCode = SS_RequestActions.FormCode, ConfigID = SS_RequestActions.ConfigId, ActionDate = SS_RequestActions.ActionDate, ActionRemarks = SS_RequestActions.ActionRemarks, ConfirmedNoOfdays = SS_RequestActions.ConfirmedNoOfdays, ActionBy = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName, ActionName = x.ActionEngName};
                }

                Result.ErrorMessage = "";
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

        public object GetAnnualVacsBalancesByEMP(int EmpID, DateTime ToDate)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                var Empcontract = _context.hrs_Contracts
       .Where(C => C.CancelDate == null && C.EmployeeID == EmpID && (C.EndDate == null || C.EndDate > DateTime.Now))
       .Select(C => new
       {
           C.ID,
           C.EmployeeID,
           C.EndDate,
           C.EmployeeClassID,
        })
       .SingleOrDefault();

                int EmpClassID = 0;
                bool IsAdvanceBalance=false;
                if (Empcontract!=null )
                {
                    EmpClassID= Empcontract.EmployeeClassID;

                  var EmpClassDetails=_context.hrs_EmployeesClasses.Where(C=>C.ID==EmpClassID)
                        .Select(E=>new
                        {
                            E.ID,
                            E.AdvanceBalance,

                        }
                      )
                        .SingleOrDefault();
                    if (EmpClassDetails != null)
                    {
                        IsAdvanceBalance =  EmpClassDetails.AdvanceBalance.Value;
                    }

                    if (IsAdvanceBalance)
                    {
                        decimal RemainingBalance = _context.hrs_VacationsBalance.Where(B => B.EmployeeID == EmpID && B.Year == DateTime.Now.Year && B.ExpireDate != null && B.ExpireDate >= DateTime.Now).Select(B => B.Remaining).Sum()??0;
                        Result.ResultObject = Math.Round(RemainingBalance, 2);

                    }
                    else
                    {


                //Opening Balance
                DateTime VactionBalanceCalculationFrom;
                double RemainingDays;
                double OldBalance;
                DateTime JoinDate = (DateTime)_context.Hrs_Employees.Where(E => E.id == EmpID).Select(E=>E.JoinDate).FirstOrDefault();
                VactionBalanceCalculationFrom =Convert.ToDateTime( JoinDate);

                var maxOpeningBalanceDate = _context.hrs_EmployeeVacationOpenBalance.Where(B => B.EmployeeID == EmpID).Max(O=>O.GBalanceDate);
                if (maxOpeningBalanceDate > VactionBalanceCalculationFrom)
                {
                    VactionBalanceCalculationFrom = maxOpeningBalanceDate;

                }
                double OpeningBalance =Convert.ToDouble( _context.hrs_EmployeeVacationOpenBalance
                    .Where(B => B.GBalanceDate == maxOpeningBalanceDate && B.EmployeeID == EmpID).Select(O => O.Days ).FirstOrDefault());
                //MaxReturnDate
                var MaxReturnDate = _context.Hrs_EmployeesVacations
                    .Where(B => B.CancelDate == null && B.VacationTypeId==1 && B.EmployeeId==EmpID).Max(v => v.ActualEndDate);
                
                
                OldBalance = Convert.ToDouble( OpeningBalance);
             
                if (MaxReturnDate> VactionBalanceCalculationFrom)
                {
                    VactionBalanceCalculationFrom =Convert.ToDateTime( MaxReturnDate);
                    RemainingDays =Convert.ToDouble( _context.Hrs_EmployeesVacations.Where(V => V.ActualEndDate == MaxReturnDate && V.EmployeeId==EmpID).Select(V => V.RemainingDays).FirstOrDefault());
                    OldBalance = Convert.ToDouble(RemainingDays);
                }

                //ALLWORKING DAYS

                double AllWorkingDays = (ToDate - JoinDate).TotalDays;
                double LastWorkingDays= (ToDate - VactionBalanceCalculationFrom).TotalDays;

                int ContractID = _context.hrs_Contracts.Where(C => (C.EndDate ==null || C.EndDate >DateTime.Now)&&   C.CancelDate == null && C.EmployeeID == EmpID).Select(C => C.ID).DefaultIfEmpty()
                    .Max(); 
                double DurationDays =(double) _context.hrs_ContractsVacations.Where(M => M.ContractID == ContractID && AllWorkingDays >M.FromMonth && AllWorkingDays <M.ToMonth).Select(M => M.DurationDays).FirstOrDefault();
                int RequiredWorkingMonths= (int)_context.hrs_ContractsVacations.Where(M => M.ContractID == ContractID && AllWorkingDays > M.FromMonth && AllWorkingDays < M.ToMonth).Select(M => M.RequiredWorkingMonths).FirstOrDefault();
                double DaysFactor = DurationDays / RequiredWorkingMonths;
                double NewBalance = LastWorkingDays * DaysFactor;
                double DueDays = NewBalance + OldBalance;
                Result.ResultObject = Math.Round(DueDays,2);
                Result.ErrorCode = 1;

                    }

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

        public object SaveSelfServiceRequest(object request, string requestType)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                 string jsonString = JsonSerializer.Serialize(request);
                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return requestType switch
                {
                     "SS_0011" or "SS_0012" or "SS_0013" or "SS_0018"
                        => SaveVacationRequest(JsonSerializer.Deserialize<SS_VacationRequest>(jsonString, jsonOptions), requestType),

                     "SS_0014" => SaveExcuseRequest(JsonSerializer.Deserialize<SS_ExecuseRequest>(jsonString, jsonOptions), requestType),
                    "SS_0015" or "SS_0019" => SaveEndOfServiceRequest(JsonSerializer.Deserialize<SS_EndOfServiceRequest>(jsonString, jsonOptions), requestType),
                    "SS_00193" => SaveLoanLetterRequest(JsonSerializer.Deserialize<SS_LoanLetterRequest>(jsonString, jsonOptions), requestType),

                     "SS_00191" => SaveExitReentryRequest(JsonSerializer.Deserialize<SS_ExitEntryRequest>(jsonString, jsonOptions), requestType),
                    "SS_00192" => SaveVisaRequest(JsonSerializer.Deserialize<SS_VisaRequest>(jsonString, jsonOptions), requestType),
                    "SS_001928" => SaveAnnualTicketRequest(JsonSerializer.Deserialize<SS_AnnualTicketRelatedRequests>(jsonString, jsonOptions), requestType),

                     "SS_00194" => SaveOtherLetterRequest(JsonSerializer.Deserialize<SS_OtherLetterRequest>(jsonString, jsonOptions), requestType),
                    "SS_001915" => SaveChamberCommerceRequest(JsonSerializer.Deserialize<SS_ChamberofCommerceLetterRequest>(jsonString, jsonOptions), requestType),
                    "SS_001916" => SaveSCFHSLetterRequest(JsonSerializer.Deserialize<SS_ScfhsletterRequest>(jsonString, jsonOptions), requestType),
                    "SS_001917" => SavePayslipLetterRequest(JsonSerializer.Deserialize<SS_PaySlipRequest>(jsonString, jsonOptions), requestType),
                    "SS_001918" => SaveOccurrenceVarianceRequest(JsonSerializer.Deserialize<SS_OccurrenceVarianceReportingLetter>(jsonString, jsonOptions), requestType),

                     "SS_00195" => SaveTrainingRequest(JsonSerializer.Deserialize<SS_TrainingRequest>(jsonString, jsonOptions), requestType),
                    "SS_001911" => SaveDaycareSupportRequest(JsonSerializer.Deserialize<SS_DaycareSupportReaquest>(jsonString, jsonOptions), requestType),
                    "SS_001912" => SaveEducationSupportRequest(JsonSerializer.Deserialize<SS_EducationSupportRequest>(jsonString, jsonOptions), requestType),
                    "SS_001913" => SaveAdvanceHousingRequest(JsonSerializer.Deserialize<SS_AdvanceHousingRequest>(jsonString, jsonOptions), requestType),
                    "SS_001914" => SaveAdvanceSalaryRequest(JsonSerializer.Deserialize<SS_AdvanceSalaryRequest>(jsonString, jsonOptions), requestType),
                    "SS_001919" => SaveOvertimeRequest(JsonSerializer.Deserialize<SS_OvertimeRequest>(jsonString, jsonOptions), requestType),
                    "SS_001920" => SaveEducationFeesRequest(JsonSerializer.Deserialize<SS_EducationFeesCompensationApplication>(jsonString, jsonOptions), requestType),

                     "SS_00196" => SaveGrievanceRequest(JsonSerializer.Deserialize<SS_GrievanceFormRequest>(jsonString, jsonOptions), requestType),
                    "SS_00197" => SaveInterviewEvaluationRequest(JsonSerializer.Deserialize<SS_InterviewEvaluationFormRequest>(jsonString, jsonOptions), requestType),
                    "SS_00198" => SaveAssaultEscalationRequest(JsonSerializer.Deserialize<SS_AssaultEscalationFormRequest>(jsonString, jsonOptions), requestType),
                    "SS_001910" => SavePhysiciansPrivilegingRequest(JsonSerializer.Deserialize<SS_PhysiciansPrivilegingFormRequest>(jsonString, jsonOptions), requestType),

                     "SS_001921" => SaveBankAccountUpdateRequest(JsonSerializer.Deserialize<SS_BankAccountUpdate>(jsonString, jsonOptions), requestType),
                    "SS_001922" => SaveSS_ContactInformationUpdateRequest(JsonSerializer.Deserialize<SS_ContactInformationUpdate>(jsonString, jsonOptions), requestType),
                    "SS_001923" => SaveDependentsUpdateRequest(JsonSerializer.Deserialize<SS_DependentsInformationUpdate>(jsonString, jsonOptions), requestType),
                    "SS_001924" => SaveMedicalInsuranceUpdateRequest(JsonSerializer.Deserialize<SS_MedicalInsuranceAdjustments>(jsonString, jsonOptions), requestType),
                    "SS_001925" => SaveLegalDocumentsUpdateRequest(JsonSerializer.Deserialize<SS_OtherLegalDocumentUpdates>(jsonString, jsonOptions), requestType),
                    "SS_001926" => SaveEmployeeFileUpdateRequest(JsonSerializer.Deserialize<SS_EmployeeFileUpdate>(jsonString, jsonOptions), requestType),

                    _ => throw new Exception("Unknown Request Type")
                };
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        public object GetSelfServiceRequestDetails(string requestType, int ReauestID, int Lang,int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
             

                return requestType.ToUpper() switch
                {
                    "SS_0011" => GetAnnualVacationAdminDetails(ReauestID,  Lang, ConfigID),
                    "SS_0013" => GetOtherVacationDetails(ReauestID, Lang, ConfigID),
                    "SS_0014" => GetExcuseRequestDetails(ReauestID, Lang, ConfigID),
                    "SS_0015" => GetEndOfServiceDetails(ReauestID, Lang, ConfigID),
                    "SS_00191" => GetExitReentryDetails(ReauestID, Lang, ConfigID),
                    "SS_001928" => GetAnnualTicketDetails(ReauestID, Lang, ConfigID),
                    "SS_00193" => GetBankLetterDetails(ReauestID, Lang, ConfigID),
                    "SS_00194" => GetOtherLetterDetails(ReauestID, Lang, ConfigID),
                    "SS_001915" => GetChamberCommerceDetails(ReauestID, Lang, ConfigID),
                    "SS_001916" => GetSCFHSLetterDetails(ReauestID, Lang, ConfigID),
                    "SS_001917" => GetPayslipLetterDetails(ReauestID, Lang, ConfigID),
                    "SS_00195" => GetTrainingRequestDetails(ReauestID, Lang, ConfigID),
                    "SS_001911" => GetDaycareSupportDetails(ReauestID, Lang, ConfigID),
                    "SS_001912" => GetEducationSupportDetails(ReauestID, Lang, ConfigID),
                    "SS_001913" => GetAdvanceHousingDetails(ReauestID, Lang, ConfigID),
                    "SS_001914" => GetAdvanceSalaryDetails(ReauestID, Lang, ConfigID),
                    "SS_001919" => GetOvertimeRequestDetails(ReauestID, Lang, ConfigID),
                    "SS_001920" => GetEducationFeesDetails(ReauestID, Lang, ConfigID),
                    "SS_00196" => GetGrievanceFormDetails(ReauestID, Lang, ConfigID),
                    "SS_00198" => GetAssaultEscalationDetails(ReauestID, Lang, ConfigID),
                    "SS_001921" => GetBankAccountUpdateDetails(ReauestID, Lang, ConfigID),
                    "SS_001922" => GetContactInfoUpdateDetails(ReauestID, Lang, ConfigID),
                    "SS_001923" => GetDependentsInfoUpdateDetails(ReauestID, Lang, ConfigID),
                    "SS_001924" => GetMedicalInsuranceUpdateDetails(ReauestID, Lang, ConfigID),
                    "SS_001925" => GetLegalDocumentsUpdateDetails(ReauestID, Lang, ConfigID),
                    "SS_001926" => GetEmployeeFileUpdateDetails(ReauestID, Lang, ConfigID),

                 };
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
         private object GetAnnualVacationAdminDetails( int ReauestID, int Lang,int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_VacationRequest in _context.SS_VacationRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_VacationRequest.EmployeeID equals Hrs_Employees.id
                                         join SS_RequestActions in _context.SS_RequestActions
                                         on SS_VacationRequest.ID equals SS_RequestActions.RequestSerial
                                          where (SS_VacationRequest.ID == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID=SS_RequestActions.ConfigId,SS_VacationRequest.EmployeeID, RequestSerial= SS_VacationRequest.ID, RequestCode = SS_VacationRequest.Code, Formcode = SS_VacationRequest.VacationType, RequestDate = SS_VacationRequest.RequestDate.ToString("yyyy-MM-dd"), StartDate= SS_VacationRequest.StartDate.ToString("yyyy-MM-dd"), EndDate=SS_VacationRequest.EndDate.ToString("yyyy-MM-dd"), SS_VacationRequest.NoOfDays, SS_VacationRequest.TotalBalance, ContactNo=SS_VacationRequest.ContactNo, Remarks= SS_VacationRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_VacationRequest in _context.SS_VacationRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_VacationRequest.EmployeeID equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                       on SS_VacationRequest.ID equals SS_RequestActions.RequestSerial
                                          where (SS_VacationRequest.ID == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_VacationRequest.EmployeeID, RequestSerial = SS_VacationRequest.ID, RequestCode = SS_VacationRequest.Code, Formcode=SS_VacationRequest.VacationType, RequestDate= SS_VacationRequest.RequestDate.ToString("yyyy-MM-dd"), StartDate=SS_VacationRequest.StartDate.ToString("yyyy-MM-dd"), EndDate= SS_VacationRequest.EndDate.ToString("yyyy-MM-dd"), SS_VacationRequest.NoOfDays, SS_VacationRequest.TotalBalance, ContactNo = SS_VacationRequest.ContactNo, Remarks = SS_VacationRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
           
        private object GetOtherVacationDetails(int ReauestID, int Lang,int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_VacationRequest in _context.SS_VacationRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_VacationRequest.EmployeeID equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_VacationRequest.ID equals SS_RequestActions.RequestSerial
                                          join hrs_VacationsTypes in _context.hrs_VacationsTypes
                                          on SS_VacationRequest.VacationTypeID equals hrs_VacationsTypes.Id
                                          where (SS_VacationRequest.ID == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_VacationRequest.EmployeeID, RequestSerial = SS_VacationRequest.ID,
                                              RequestCode = SS_VacationRequest.Code,
                                              RequestDate = SS_VacationRequest.RequestDate.ToString("yyyy-MM-dd"),
                                              VacationType= hrs_VacationsTypes.ArbName, 
                                              StartDate = SS_VacationRequest.StartDate.ToString("yyyy-MM-dd"), 
                                              EndDate = SS_VacationRequest.EndDate.ToString("yyyy-MM-dd"), 
                                              SS_VacationRequest.NoOfDays, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_VacationRequest in _context.SS_VacationRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_VacationRequest.EmployeeID equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_VacationRequest.ID equals SS_RequestActions.RequestSerial
                                          join hrs_VacationsTypes in _context.hrs_VacationsTypes
                                          on SS_VacationRequest.VacationTypeID equals hrs_VacationsTypes.Id
                                          where (SS_VacationRequest.ID == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_VacationRequest.EmployeeID, RequestSerial = SS_VacationRequest.ID, RequestCode = SS_VacationRequest.Code, RequestDate = SS_VacationRequest.RequestDate.ToString("yyyy-MM-dd"), VacationType = hrs_VacationsTypes.EngName, StartDate = SS_VacationRequest.StartDate.ToString("yyyy-MM-dd"), EndDate = SS_VacationRequest.EndDate.ToString("yyyy-MM-dd"), SS_VacationRequest.NoOfDays,Reamarks= SS_VacationRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetExcuseRequestDetails(int RequestID, int Lang,int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_ExecuseRequest in _context.SS_ExecuseRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ExecuseRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_ExecuseRequest.Id equals SS_RequestActions.RequestSerial
                                          where (SS_ExecuseRequest.Id == RequestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_ExecuseRequest.EmployeeId,
                                            RequestSerial = SS_ExecuseRequest.Id, RequestCode = SS_ExecuseRequest.Code,
                                            RequestDate = SS_ExecuseRequest.RequestDate.ToString("yyyy-MM-dd"),
                                            SS_ExecuseRequest.ExecuseType,
                                            SS_ExecuseRequest.ExecuseReason,
                                            ExecuseDate= SS_ExecuseRequest.ExecuseDate.ToString("yyyy-MM-dd"),
                                            SS_ExecuseRequest.ExecuseTime,SS_ExecuseRequest.ExecuseShift,
                                            SS_ExecuseRequest.ExecuseRemarks,
                                            EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName
                                          };
                }
                else
                {
                    Result.ResultObject = from SS_ExecuseRequest in _context.SS_ExecuseRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ExecuseRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_ExecuseRequest.Id equals SS_RequestActions.RequestSerial
                                          where (SS_ExecuseRequest.Id == RequestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new
                                          {
                                              ConfigID = SS_RequestActions.ConfigId,
                                              SS_ExecuseRequest.EmployeeId,
                                              RequestSerial = SS_ExecuseRequest.Id,
                                              RequestCode = SS_ExecuseRequest.Code,
                                              RequestDate = SS_ExecuseRequest.RequestDate.ToString("yyyy-MM-dd"),
                                              SS_ExecuseRequest.ExecuseType,
                                              SS_ExecuseRequest.ExecuseReason,
                                              ExecuseDate = SS_ExecuseRequest.ExecuseDate.ToString("yyyy-MM-dd"),
                                              SS_ExecuseRequest.ExecuseTime,
                                              SS_ExecuseRequest.ExecuseShift,
                                              SS_ExecuseRequest.ExecuseRemarks,
                                              EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName
                                          };
                }

                Result.ErrorMessage = "";
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
        private object GetEndOfServiceDetails(int RequestID, int Lang,int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from eos in _context.SS_EndOfServiceRequest
                                          join emp in _context.Hrs_Employees
                                          on eos.EmployeeId equals emp.id
                                          join action in _context.SS_RequestActions
                                          on eos.Id equals action.RequestSerial
                                          from reason in _context.SS_ResignationReason
                                              .Where(r => r.Code == eos.ResignationReasonCode)
                                              .DefaultIfEmpty()  
                                          where (eos.Id == RequestID && action.ConfigId == ConfigID)
                                          select new
                                          {
                                              ConfigID = action.ConfigId,
                                              eos.EmployeeId,
                                              eos.ServiceYears,
                                              eos.SerciveMonths,
                                              eos.ServiceDays,
                                              RequestSerial = eos.Id,
                                              RequestCode = eos.Code,
                                              RequestDate = eos.RequestDate.ToString("yyyy-MM-dd"),
                                              EosDate = eos.Eosdate.ToString("yyyy-MM-dd"),
                                              eos.Eosremarks,
                                              RsignationReason = reason.ArbName ?? string.Empty,  
                                              EmployeeName = emp.ArbName + " " + emp.FatherArbName + " " + emp.GrandArbName + " " + emp.FamilyArbName
                                          };
                }
                else
                {
                    Result.ResultObject = from eos in _context.SS_EndOfServiceRequest
                                          join emp in _context.Hrs_Employees
                                          on eos.EmployeeId equals emp.id
                                          join action in _context.SS_RequestActions
                                          on eos.Id equals action.RequestSerial
                                          from reason in _context.SS_ResignationReason
                                              .Where(r => r.Code == eos.ResignationReasonCode)
                                              .DefaultIfEmpty()  
                                          where (eos.Id == RequestID && action.ConfigId == ConfigID)
                                          select new
                                          {
                                              ConfigID = action.ConfigId,
                                              eos.EmployeeId,
                                              eos.ServiceYears,
                                              eos.SerciveMonths,
                                              eos.ServiceDays,
                                              RequestSerial = eos.Id,
                                              RequestCode = eos.Code,
                                              RequestDate = eos.RequestDate.ToString("yyyy-MM-dd"),
                                              EosDate = eos.Eosdate.ToString("yyyy-MM-dd"),
                                              eos.Eosremarks,
                                              RsignationReason = reason.EngName ?? string.Empty,  
                                              EmployeeName = emp.EngName + " " + emp.FatherEngName + " " + emp.GrandEngName + " " + emp.FamilyEngName
                                          };
                }
                Result.ErrorMessage = "";
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
       private object GetExitReentryDetails(int RequestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_ExitEntryRequest in _context.SS_ExitEntryRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ExitEntryRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_ExitEntryRequest.Id equals SS_RequestActions.RequestSerial
                                          where (SS_ExitEntryRequest.Id == RequestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_ExitEntryRequest.EmployeeId, RequestSerial = SS_ExitEntryRequest.Id, RequestCode = SS_ExitEntryRequest.Code, RequestDate= SS_ExitEntryRequest.RequestDate.ToString("yyyy-MM-dd"), ExitDate= SS_ExitEntryRequest.ExitDate.ToString("yyyy-MM-dd"), ReturnDate= SS_ExitEntryRequest.EntryDate.ToString("yyyy-MM-dd"), SS_ExitEntryRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_ExitEntryRequest in _context.SS_ExitEntryRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ExitEntryRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_ExitEntryRequest.Id equals SS_RequestActions.RequestSerial
                                          where (SS_ExitEntryRequest.Id == RequestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_ExitEntryRequest.EmployeeId, RequestSerial = SS_ExitEntryRequest.Id, RequestCode = SS_ExitEntryRequest.Code, RequestDate= SS_ExitEntryRequest.RequestDate.ToString("yyyy-MM-dd"), ExitDate= SS_ExitEntryRequest.ExitDate.ToString("yyyy-MM-dd"), ReturnDate = SS_ExitEntryRequest.EntryDate.ToString("yyyy-MM-dd"), SS_ExitEntryRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetAnnualTicketDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_AnnualTicketRelatedRequests in _context.SS_AnnualTicketRelatedRequests
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AnnualTicketRelatedRequests.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_AnnualTicketRelatedRequests.Id equals SS_RequestActions.RequestSerial



                                          where (SS_AnnualTicketRelatedRequests.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_AnnualTicketRelatedRequests.EmployeeId, RequestSerial = SS_AnnualTicketRelatedRequests.Id, RequestCode = SS_AnnualTicketRelatedRequests.Code, RequestDate=SS_AnnualTicketRelatedRequests.RequestDate.ToString("yyyy-MM-dd"), TicketDate= SS_AnnualTicketRelatedRequests.TicketDate.ToString("yyyy-MM-dd"), SS_AnnualTicketRelatedRequests.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_AnnualTicketRelatedRequests in _context.SS_AnnualTicketRelatedRequests
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AnnualTicketRelatedRequests.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_AnnualTicketRelatedRequests.Id equals SS_RequestActions.RequestSerial



                                          where (SS_AnnualTicketRelatedRequests.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_AnnualTicketRelatedRequests.EmployeeId, RequestSerial = SS_AnnualTicketRelatedRequests.Id, RequestCode = SS_AnnualTicketRelatedRequests.Code, RequestDate = SS_AnnualTicketRelatedRequests.RequestDate.ToString("yyyy-MM-dd"), TicketDate = SS_AnnualTicketRelatedRequests.TicketDate.ToString("yyyy-MM-dd"), SS_AnnualTicketRelatedRequests.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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

        private object GetBankLetterDetails(int ReauestID,int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_LoanLetterRequest in _context.SS_LoanLetterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_LoanLetterRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_LoanLetterRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_LoanLetterRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_LoanLetterRequest.EmployeeId, RequestSerial = SS_LoanLetterRequest.Id, RequestCode = SS_LoanLetterRequest.Code, RequestDate= SS_LoanLetterRequest.RequestDate.ToString("yyyy-MM-dd"),  SS_LoanLetterRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_LoanLetterRequest in _context.SS_LoanLetterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_LoanLetterRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_LoanLetterRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_LoanLetterRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_LoanLetterRequest.EmployeeId, RequestSerial = SS_LoanLetterRequest.Id, RequestCode = SS_LoanLetterRequest.Code, RequestDate = SS_LoanLetterRequest.RequestDate.ToString("yyyy-MM-dd"), SS_LoanLetterRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetOtherLetterDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_OtherLetterRequest in _context.SS_OtherLetterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_OtherLetterRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_OtherLetterRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_OtherLetterRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_OtherLetterRequest.EmployeeId, RequestSerial = SS_OtherLetterRequest.Id, RequestCode = SS_OtherLetterRequest.Code, RequestDate= SS_OtherLetterRequest.RequestDate.ToString("yyyy-MM-dd"), SS_OtherLetterRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_OtherLetterRequest in _context.SS_OtherLetterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_OtherLetterRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_OtherLetterRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_OtherLetterRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_OtherLetterRequest.EmployeeId, RequestSerial = SS_OtherLetterRequest.Id, RequestCode = SS_OtherLetterRequest.Code, RequestDate = SS_OtherLetterRequest.RequestDate.ToString("yyyy-MM-dd"), SS_OtherLetterRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetChamberCommerceDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_ChamberofCommerceLetterRequest in _context.SS_ChamberofCommerceLetterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ChamberofCommerceLetterRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_ChamberofCommerceLetterRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_ChamberofCommerceLetterRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_ChamberofCommerceLetterRequest.EmployeeId, RequestSerial = SS_ChamberofCommerceLetterRequest.Id, RequestCode = SS_ChamberofCommerceLetterRequest.Code, RequestDate= SS_ChamberofCommerceLetterRequest.RequestDate.ToString("yyyy-MM-dd"), SS_ChamberofCommerceLetterRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_ChamberofCommerceLetterRequest in _context.SS_ChamberofCommerceLetterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ChamberofCommerceLetterRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_ChamberofCommerceLetterRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_ChamberofCommerceLetterRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_ChamberofCommerceLetterRequest.EmployeeId, RequestSerial = SS_ChamberofCommerceLetterRequest.Id, RequestCode = SS_ChamberofCommerceLetterRequest.Code, RequestDate = SS_ChamberofCommerceLetterRequest.RequestDate.ToString("yyyy-MM-dd"), SS_ChamberofCommerceLetterRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetSCFHSLetterDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_SCFHSLetterRequest in _context.SS_ScfhsletterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_SCFHSLetterRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_SCFHSLetterRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_SCFHSLetterRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_SCFHSLetterRequest.EmployeeId, RequestSerial = SS_SCFHSLetterRequest.Id, RequestCode = SS_SCFHSLetterRequest.Code, RequestDate=SS_SCFHSLetterRequest.RequestDate.ToString("yyyy-MM-dd"), SS_SCFHSLetterRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_SCFHSLetterRequest in _context.SS_ScfhsletterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_SCFHSLetterRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_SCFHSLetterRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_SCFHSLetterRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_SCFHSLetterRequest.EmployeeId, RequestSerial = SS_SCFHSLetterRequest.Id, RequestCode = SS_SCFHSLetterRequest.Code, RequestDate = SS_SCFHSLetterRequest.RequestDate.ToString("yyyy-MM-dd"), SS_SCFHSLetterRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetPayslipLetterDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_PaySlipRequest in _context.SS_PaySlipRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_PaySlipRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_PaySlipRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_PaySlipRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_PaySlipRequest.EmployeeId, RequestSerial = SS_PaySlipRequest.Id, RequestCode = SS_PaySlipRequest.Code, RequestDate= SS_PaySlipRequest.RequestDate.ToString("yyyy-MM-dd"), SS_PaySlipRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_PaySlipRequest in _context.SS_PaySlipRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_PaySlipRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_PaySlipRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_PaySlipRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_PaySlipRequest.EmployeeId, RequestSerial = SS_PaySlipRequest.Id, RequestCode = SS_PaySlipRequest.Code, RequestDate= SS_PaySlipRequest.RequestDate.ToString("yyyy-MM-dd"), SS_PaySlipRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
         private object GetTrainingRequestDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_TrainingRequest in _context.SS_TrainingRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_TrainingRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_TrainingRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_TrainingRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_TrainingRequest.EmployeeId, RequestSerial = SS_TrainingRequest.Id, RequestCode = SS_TrainingRequest.Code, RequestDate= SS_TrainingRequest.RequestDate.ToString("yyyy-MM-dd"), SS_TrainingRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_TrainingRequest in _context.SS_TrainingRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_TrainingRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_TrainingRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_TrainingRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_TrainingRequest.EmployeeId, RequestSerial = SS_TrainingRequest.Id, RequestCode = SS_TrainingRequest.Code, RequestDate=SS_TrainingRequest.RequestDate.ToString("yyyy-MM-dd"), SS_TrainingRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetDaycareSupportDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_DaycareSupportReaquest in _context.SS_DaycareSupportReaquest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_DaycareSupportReaquest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_DaycareSupportReaquest.Id equals SS_RequestActions.RequestSerial
                                          where (SS_DaycareSupportReaquest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_DaycareSupportReaquest.EmployeeId, RequestSerial = SS_DaycareSupportReaquest.Id, RequestCode = SS_DaycareSupportReaquest.Code, RequestDate= SS_DaycareSupportReaquest.RequestDate.ToString("yyyy-MM-dd"), SS_DaycareSupportReaquest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_DaycareSupportReaquest in _context.SS_DaycareSupportReaquest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_DaycareSupportReaquest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_DaycareSupportReaquest.Id equals SS_RequestActions.RequestSerial
                                          where (SS_DaycareSupportReaquest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_DaycareSupportReaquest.EmployeeId, RequestSerial = SS_DaycareSupportReaquest.Id, RequestCode = SS_DaycareSupportReaquest.Code, RequestDate= SS_DaycareSupportReaquest.RequestDate.ToString("yyyy-MM-dd"), SS_DaycareSupportReaquest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetEducationSupportDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_EducationSupportRequest in _context.SS_EducationSupportRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_EducationSupportRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_EducationSupportRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_EducationSupportRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_EducationSupportRequest.EmployeeId, RequestSerial = SS_EducationSupportRequest.Id, RequestCode = SS_EducationSupportRequest.Code, RequestDate=SS_EducationSupportRequest.RequestDate.ToString("yyyy-MM-dd"), SS_EducationSupportRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_EducationSupportRequest in _context.SS_EducationSupportRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_EducationSupportRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_EducationSupportRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_EducationSupportRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_EducationSupportRequest.EmployeeId, RequestSerial = SS_EducationSupportRequest.Id, RequestCode = SS_EducationSupportRequest.Code, RequestDate=SS_EducationSupportRequest.RequestDate.ToString("yyyy-MM-dd"), SS_EducationSupportRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetAdvanceHousingDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_AdvanceHousingRequest in _context.SS_AdvanceHousingRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AdvanceHousingRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_AdvanceHousingRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_AdvanceHousingRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_AdvanceHousingRequest.EmployeeId, RequestSerial = SS_AdvanceHousingRequest.Id, RequestCode = SS_AdvanceHousingRequest.Code, RequestDate= SS_AdvanceHousingRequest.RequestDate.ToString("yyyy-MM-dd"), SS_AdvanceHousingRequest.Remarks, InstallmentDate= SS_AdvanceHousingRequest.InstallmentDate.ToString("yyyy-MM-dd"), EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_AdvanceHousingRequest in _context.SS_AdvanceHousingRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AdvanceHousingRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_AdvanceHousingRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_AdvanceHousingRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_AdvanceHousingRequest.EmployeeId, RequestSerial = SS_AdvanceHousingRequest.Id, RequestCode = SS_AdvanceHousingRequest.Code, RequestDate=SS_AdvanceHousingRequest.RequestDate.ToString("yyyy-MM-dd"), SS_AdvanceHousingRequest.Remarks, InstallmentDate= SS_AdvanceHousingRequest.InstallmentDate.ToString("yyyy-MM-dd"), EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetAdvanceSalaryDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_AdvanceSalaryRequest in _context.SS_AdvanceSalaryRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AdvanceSalaryRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_AdvanceSalaryRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_AdvanceSalaryRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_AdvanceSalaryRequest.EmployeeId, RequestSerial = SS_AdvanceSalaryRequest.Id, RequestCode = SS_AdvanceSalaryRequest.Code, RequestDate= SS_AdvanceSalaryRequest.RequestDate.ToString("yyyy-MM-dd"), SS_AdvanceSalaryRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_AdvanceSalaryRequest in _context.SS_AdvanceSalaryRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AdvanceSalaryRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_AdvanceSalaryRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_AdvanceSalaryRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_AdvanceSalaryRequest.EmployeeId, RequestSerial = SS_AdvanceSalaryRequest.Id, RequestCode = SS_AdvanceSalaryRequest.Code, RequestDate = SS_AdvanceSalaryRequest.RequestDate.ToString("yyyy-MM-dd"), SS_AdvanceSalaryRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetOvertimeRequestDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_OvertimeRequest in _context.SS_OvertimeRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_OvertimeRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_OvertimeRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_OvertimeRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_OvertimeRequest.EmployeeId, RequestSerial = SS_OvertimeRequest.Id, RequestCode = SS_OvertimeRequest.Code, RequestDate=SS_OvertimeRequest.RequestDate.ToString("yyyy-MM-dd"), SS_OvertimeRequest.Remarks, OvertimeDate=SS_OvertimeRequest.OvertimeDate.ToString("yyyy-MM-dd"), SS_OvertimeRequest.HoursCount, SS_OvertimeRequest.MinutsCount, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_OvertimeRequest in _context.SS_OvertimeRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_OvertimeRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_OvertimeRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_OvertimeRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_OvertimeRequest.EmployeeId, RequestSerial = SS_OvertimeRequest.Id, RequestCode = SS_OvertimeRequest.Code, RequestDate = SS_OvertimeRequest.RequestDate.ToString("yyyy-MM-dd"), SS_OvertimeRequest.Remarks, OvertimeDate = SS_OvertimeRequest.OvertimeDate.ToString("yyyy-MM-dd"), SS_OvertimeRequest.HoursCount, SS_OvertimeRequest.MinutsCount, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetEducationFeesDetails(int ReauestID, int Lang, int ConfigID)
        {
                Result = new GeneralOutputClass<object>();
                try
                {
                    if (Lang == 1)
                    {
                        Result.ResultObject = from SS_EducationFeesCompensationApplication in _context.SS_EducationFeesCompensationApplication
                                              join Hrs_Employees in _context.Hrs_Employees
                                              on SS_EducationFeesCompensationApplication.EmployeeId equals Hrs_Employees.id
                                              join SS_RequestActions in _context.SS_RequestActions
                                              on SS_EducationFeesCompensationApplication.Id equals SS_RequestActions.RequestSerial



                                              where (SS_EducationFeesCompensationApplication.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                              select new { ConfigID = SS_RequestActions.ConfigId, SS_EducationFeesCompensationApplication.EmployeeId, RequestSerial = SS_EducationFeesCompensationApplication.Id, RequestCode = SS_EducationFeesCompensationApplication.Code, RequestDate= SS_EducationFeesCompensationApplication.RequestDate.ToString("yyyy-MM-dd"), SS_EducationFeesCompensationApplication.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                    }
                    else
                    {
                    Result.ResultObject = from SS_EducationFeesCompensationApplication in _context.SS_EducationFeesCompensationApplication
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_EducationFeesCompensationApplication.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_EducationFeesCompensationApplication.Id equals SS_RequestActions.RequestSerial



                                          where (SS_EducationFeesCompensationApplication.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_EducationFeesCompensationApplication.EmployeeId, RequestSerial = SS_EducationFeesCompensationApplication.Id, RequestCode = SS_EducationFeesCompensationApplication.Code, RequestDate = SS_EducationFeesCompensationApplication.RequestDate.ToString("yyyy-MM-dd"), SS_EducationFeesCompensationApplication.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                    }

                    Result.ErrorMessage = "";
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
        private object GetGrievanceFormDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_GrievanceFormRequest in _context.SS_GrievanceFormRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_GrievanceFormRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_GrievanceFormRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_GrievanceFormRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_GrievanceFormRequest.EmployeeId, RequestSerial = SS_GrievanceFormRequest.Id, RequestCode = SS_GrievanceFormRequest.Code, RequestDate= SS_GrievanceFormRequest.RequestDate.ToString("yyyy-MM-dd"), SS_GrievanceFormRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_GrievanceFormRequest in _context.SS_GrievanceFormRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_GrievanceFormRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_GrievanceFormRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_GrievanceFormRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_GrievanceFormRequest.EmployeeId, RequestSerial = SS_GrievanceFormRequest.Id, RequestCode = SS_GrievanceFormRequest.Code, RequestDate=SS_GrievanceFormRequest.RequestDate.ToString("yyyy-MM-dd"), SS_GrievanceFormRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetAssaultEscalationDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_AssaultEscalationFormRequest in _context.SS_AssaultEscalationFormRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AssaultEscalationFormRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_AssaultEscalationFormRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_AssaultEscalationFormRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_AssaultEscalationFormRequest.EmployeeId, RequestSerial = SS_AssaultEscalationFormRequest.Id, RequestCode = SS_AssaultEscalationFormRequest.Code, RequestDate=SS_AssaultEscalationFormRequest.RequestDate.ToString("yyyy-MM-dd"), SS_AssaultEscalationFormRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_AssaultEscalationFormRequest in _context.SS_AssaultEscalationFormRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AssaultEscalationFormRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_AssaultEscalationFormRequest.Id equals SS_RequestActions.RequestSerial



                                          where (SS_AssaultEscalationFormRequest.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_AssaultEscalationFormRequest.EmployeeId, RequestSerial = SS_AssaultEscalationFormRequest.Id, RequestCode = SS_AssaultEscalationFormRequest.Code, RequestDate=SS_AssaultEscalationFormRequest.RequestDate.ToString("yyyy-MM-dd"), SS_AssaultEscalationFormRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetBankAccountUpdateDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_BankAccountUpdate in _context.SS_BankAccountUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_BankAccountUpdate.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_BankAccountUpdate.Id equals SS_RequestActions.RequestSerial
                                          join sys_Banks in _context.sys_Banks
                                          on SS_BankAccountUpdate.BankId equals sys_Banks.ID



                                          where (SS_BankAccountUpdate.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, BankName = sys_Banks.ArbName, SS_BankAccountUpdate.BankAccountNumber, SS_BankAccountUpdate.EmployeeId, RequestSerial = SS_BankAccountUpdate.Id, RequestCode = SS_BankAccountUpdate.Code, RequestDate=SS_BankAccountUpdate.RequestDate.ToString("yyyy-MM-dd"), SS_BankAccountUpdate.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_BankAccountUpdate in _context.SS_BankAccountUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_BankAccountUpdate.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_BankAccountUpdate.Id equals SS_RequestActions.RequestSerial
                                          join sys_Banks in _context.sys_Banks
                                          on SS_BankAccountUpdate.BankId equals sys_Banks.ID



                                          where (SS_BankAccountUpdate.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, BankName = sys_Banks.EngName, SS_BankAccountUpdate.BankAccountNumber, SS_BankAccountUpdate.EmployeeId, RequestSerial = SS_BankAccountUpdate.Id, RequestCode = SS_BankAccountUpdate.Code, RequestDate=SS_BankAccountUpdate.RequestDate.ToString("yyyy-MM-dd"), SS_BankAccountUpdate.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };


                    Result.ErrorMessage = "";
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
        private object GetContactInfoUpdateDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_ContactInformationUpdate in _context.SS_ContactInformationUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ContactInformationUpdate.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_ContactInformationUpdate.Id equals SS_RequestActions.RequestSerial



                                          where (SS_ContactInformationUpdate.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_ContactInformationUpdate.EmployeeId, RequestSerial = SS_ContactInformationUpdate.Id, RequestCode = SS_ContactInformationUpdate.Code, RequestDate=SS_ContactInformationUpdate.RequestDate.ToString("yyyy-MM-dd"), SS_ContactInformationUpdate.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_ContactInformationUpdate in _context.SS_ContactInformationUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ContactInformationUpdate.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_ContactInformationUpdate.Id equals SS_RequestActions.RequestSerial



                                          where (SS_ContactInformationUpdate.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_ContactInformationUpdate.EmployeeId, RequestSerial = SS_ContactInformationUpdate.Id, RequestCode = SS_ContactInformationUpdate.Code, RequestDate=SS_ContactInformationUpdate.RequestDate.ToString("yyyy-MM-dd"), SS_ContactInformationUpdate.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetDependentsInfoUpdateDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_DependentsInformationUpdate in _context.SS_DependentsInformationUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_DependentsInformationUpdate.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_DependentsInformationUpdate.Id equals SS_RequestActions.RequestSerial



                                          where (SS_DependentsInformationUpdate.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_DependentsInformationUpdate.EmployeeId, RequestSerial = SS_DependentsInformationUpdate.Id, RequestCode = SS_DependentsInformationUpdate.Code, RequestDate= SS_DependentsInformationUpdate.RequestDate.ToString("yyyy-MM-dd"), SS_DependentsInformationUpdate.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_DependentsInformationUpdate in _context.SS_DependentsInformationUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_DependentsInformationUpdate.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_DependentsInformationUpdate.Id equals SS_RequestActions.RequestSerial



                                          where (SS_DependentsInformationUpdate.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_DependentsInformationUpdate.EmployeeId, RequestSerial = SS_DependentsInformationUpdate.Id, RequestCode = SS_DependentsInformationUpdate.Code, RequestDate = SS_DependentsInformationUpdate.RequestDate.ToString("yyyy-MM-dd"), SS_DependentsInformationUpdate.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMedicalInsuranceUpdateDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_MedicalInsuranceAdjustments in _context.SS_MedicalInsuranceAdjustments
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_MedicalInsuranceAdjustments.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_MedicalInsuranceAdjustments.Id equals SS_RequestActions.RequestSerial



                                          where (SS_MedicalInsuranceAdjustments.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_MedicalInsuranceAdjustments.EmployeeId, RequestSerial = SS_MedicalInsuranceAdjustments.Id, RequestCode = SS_MedicalInsuranceAdjustments.Code, RequestDate=SS_MedicalInsuranceAdjustments.RequestDate.ToString("yyyy-MM-dd"), SS_MedicalInsuranceAdjustments.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_MedicalInsuranceAdjustments in _context.SS_MedicalInsuranceAdjustments
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_MedicalInsuranceAdjustments.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_MedicalInsuranceAdjustments.Id equals SS_RequestActions.RequestSerial



                                          where (SS_MedicalInsuranceAdjustments.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_MedicalInsuranceAdjustments.EmployeeId, RequestSerial = SS_MedicalInsuranceAdjustments.Id, RequestCode = SS_MedicalInsuranceAdjustments.Code, RequestDate= SS_MedicalInsuranceAdjustments.RequestDate.ToString("yyyy-MM-dd"), SS_MedicalInsuranceAdjustments.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetLegalDocumentsUpdateDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_OtherLegalDocumentUpdates in _context.SS_OtherLegalDocumentUpdates
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_OtherLegalDocumentUpdates.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_OtherLegalDocumentUpdates.Id equals SS_RequestActions.RequestSerial



                                          where (SS_OtherLegalDocumentUpdates.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_OtherLegalDocumentUpdates.EmployeeId, RequestSerial = SS_OtherLegalDocumentUpdates.Id, RequestCode = SS_OtherLegalDocumentUpdates.Code, SS_OtherLegalDocumentUpdates.RequestDate, SS_OtherLegalDocumentUpdates.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_OtherLegalDocumentUpdates in _context.SS_OtherLegalDocumentUpdates
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_OtherLegalDocumentUpdates.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_OtherLegalDocumentUpdates.Id equals SS_RequestActions.RequestSerial



                                          where (SS_OtherLegalDocumentUpdates.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_OtherLegalDocumentUpdates.EmployeeId, RequestSerial = SS_OtherLegalDocumentUpdates.Id, RequestCode = SS_OtherLegalDocumentUpdates.Code, SS_OtherLegalDocumentUpdates.RequestDate, SS_OtherLegalDocumentUpdates.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetEmployeeFileUpdateDetails(int ReauestID, int Lang, int ConfigID)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_EmployeeFileUpdate in _context.SS_EmployeeFileUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_EmployeeFileUpdate.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_EmployeeFileUpdate.Id equals SS_RequestActions.RequestSerial



                                          where (SS_EmployeeFileUpdate.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_EmployeeFileUpdate.EmployeeId, RequestSerial = SS_EmployeeFileUpdate.Id, RequestCode = SS_EmployeeFileUpdate.Code, RequestDate=SS_EmployeeFileUpdate.RequestDate.ToString("yyyy-MM-dd"), SS_EmployeeFileUpdate.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_EmployeeFileUpdate in _context.SS_EmployeeFileUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_EmployeeFileUpdate.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_EmployeeFileUpdate.Id equals SS_RequestActions.RequestSerial



                                          where (SS_EmployeeFileUpdate.Id == ReauestID && SS_RequestActions.ConfigId == ConfigID)
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_EmployeeFileUpdate.EmployeeId, RequestSerial = SS_EmployeeFileUpdate.Id, RequestCode = SS_EmployeeFileUpdate.Code, RequestDate = SS_EmployeeFileUpdate.RequestDate.ToString("yyyy-MM-dd"), SS_EmployeeFileUpdate.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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


        #region GetMySelfServiceRequestDetails
        public object GetMySelfServiceRequestDetails(string requestType, int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {


                return requestType.ToUpper() switch
                {
                    "SS_0011" => GetMyAnnualVacationAdminDetails(ReauestID, Lang),
                    "SS_0013" => GetMyOtherVacationDetails(ReauestID, Lang),
                    "SS_0014" => GetMyExcuseRequestDetails(ReauestID, Lang),
                    "SS_0015" => GetMyEndOfServiceDetails(ReauestID, Lang),
                    "SS_00191" => GetMyExitReentryDetails(ReauestID, Lang),
                    "SS_001928" => GetMyAnnualTicketDetails(ReauestID, Lang),
                    "SS_00193" => GetMyBankLetterDetails(ReauestID, Lang),
                    "SS_00194" => GetMyOtherLetterDetails(ReauestID, Lang),
                    "SS_001915" => GetMyChamberCommerceDetails(ReauestID, Lang),
                    "SS_001916" => GetMySCFHSLetterDetails(ReauestID, Lang),
                    "SS_001917" => GetMyPayslipLetterDetails(ReauestID, Lang),
                    "SS_00195" => GetMyTrainingRequestDetails(ReauestID, Lang),
                    "SS_001911" => GetMyDaycareSupportDetails(ReauestID, Lang),
                    "SS_001912" => GetMyEducationSupportDetails(ReauestID, Lang),
                    "SS_001913" => GetMyAdvanceHousingDetails(ReauestID, Lang),
                    "SS_001914" => GetMyAdvanceSalaryDetails(ReauestID, Lang),
                    "SS_001919" => GetMyOvertimeRequestDetails(ReauestID, Lang),
                    "SS_001920" => GetMyEducationFeesDetails(ReauestID, Lang),
                    "SS_00196" => GetMyGrievanceFormDetails(ReauestID, Lang),
                    "SS_00198" => GetMyAssaultEscalationDetails(ReauestID, Lang),
                    "SS_001921" => GetMyBankAccountUpdateDetails(ReauestID, Lang),
                    "SS_001922" => GetMyContactInfoUpdateDetails(ReauestID, Lang),
                    "SS_001923" => GetMyDependentsInfoUpdateDetails(ReauestID, Lang),
                    "SS_001924" => GetMyMedicalInsuranceUpdateDetails(ReauestID, Lang),
                    "SS_001925" => GetMyLegalDocumentsUpdateDetails(ReauestID, Lang),
                    "SS_001926" => GetMyEmployeeFileUpdateDetails(ReauestID, Lang),

                };
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        private object GetMyAnnualVacationAdminDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_VacationRequest in _context.SS_VacationRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_VacationRequest.EmployeeID equals Hrs_Employees.id
                                        
                                          where (SS_VacationRequest.ID == ReauestID )
                                          select new {  SS_VacationRequest.EmployeeID, RequestSerial = SS_VacationRequest.ID, RequestCode = SS_VacationRequest.Code, Formcode = SS_VacationRequest.VacationType, 
                                              RequestDate= SS_VacationRequest.RequestDate.ToString("yyyy-MM-dd"),
                                              StartDate= SS_VacationRequest.StartDate.ToString("yyyy-MM-dd"),
                                              EndDate= SS_VacationRequest.EndDate.ToString("yyyy-MM-dd"), 
                                              SS_VacationRequest.NoOfDays, SS_VacationRequest.TotalBalance, ContactNo = SS_VacationRequest.ContactNo, Remarks = SS_VacationRequest.Remarks,EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_VacationRequest in _context.SS_VacationRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_VacationRequest.EmployeeID equals Hrs_Employees.id
                                   
                                          where (SS_VacationRequest.ID == ReauestID )
                                          select new {  SS_VacationRequest.EmployeeID, RequestSerial = SS_VacationRequest.ID, RequestCode = SS_VacationRequest.Code, Formcode = SS_VacationRequest.VacationType,
                                              RequestDate = SS_VacationRequest.RequestDate.ToString("yyyy-MM-dd"),
                                              StartDate = SS_VacationRequest.StartDate.ToString("yyyy-MM-dd"),
                                              EndDate = SS_VacationRequest.EndDate.ToString("yyyy-MM-dd"),
                                              SS_VacationRequest.NoOfDays, SS_VacationRequest.TotalBalance, ContactNo= SS_VacationRequest.ContactNo, Remarks= SS_VacationRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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

        private object GetMyOtherVacationDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_VacationRequest in _context.SS_VacationRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_VacationRequest.EmployeeID equals Hrs_Employees.id
                                       
                                          join hrs_VacationsTypes in _context.hrs_VacationsTypes
                                          on SS_VacationRequest.VacationTypeID equals hrs_VacationsTypes.Id
                                          where (SS_VacationRequest.ID == ReauestID  )
                                          select new {   SS_VacationRequest.EmployeeID, RequestSerial = SS_VacationRequest.ID, RequestCode = SS_VacationRequest.Code,
                                              RequestDate = SS_VacationRequest.RequestDate.ToString("yyyy-MM-dd"),
                                              VacationType = hrs_VacationsTypes.ArbName,
                                              StartDate = SS_VacationRequest.StartDate.ToString("yyyy-MM-dd"),
                                              EndDate = SS_VacationRequest.EndDate.ToString("yyyy-MM-dd"),
                                              SS_VacationRequest.NoOfDays,
                                              Remarks= SS_VacationRequest.Remarks,
                                              EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_VacationRequest in _context.SS_VacationRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_VacationRequest.EmployeeID equals Hrs_Employees.id
                                        
                                          join hrs_VacationsTypes in _context.hrs_VacationsTypes
                                          on SS_VacationRequest.VacationTypeID equals hrs_VacationsTypes.Id
                                          where (SS_VacationRequest.ID == ReauestID )
                                          select new {  SS_VacationRequest.EmployeeID, RequestSerial = SS_VacationRequest.ID, RequestCode = SS_VacationRequest.Code,
                                              RequestDate = SS_VacationRequest.RequestDate.ToString("yyyy-MM-dd"),
                                              VacationType = hrs_VacationsTypes.EngName,
                                              StartDate = SS_VacationRequest.StartDate.ToString("yyyy-MM-dd"),
                                              EndDate = SS_VacationRequest.EndDate.ToString("yyyy-MM-dd"),
                                              Remarks = SS_VacationRequest.Remarks,
                                              SS_VacationRequest.NoOfDays, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyExcuseRequestDetails(int RequestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_ExecuseRequest in _context.SS_ExecuseRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ExecuseRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_ExecuseRequest.Id == RequestID )
                                          select new { SS_ExecuseRequest.EmployeeId, RequestSerial = SS_ExecuseRequest.Id, RequestCode = SS_ExecuseRequest.Code,
                                          RequestDate = SS_ExecuseRequest.RequestDate.ToString("yyyy-MM-dd"),
                                              SS_ExecuseRequest.ExecuseType, SS_ExecuseRequest.ExecuseReason,
                                              ExecuseDate=SS_ExecuseRequest.ExecuseDate.ToString("yyyy-MM-dd"),
                                              SS_ExecuseRequest.ExecuseTime, SS_ExecuseRequest.ExecuseShift, SS_ExecuseRequest.ExecuseRemarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_ExecuseRequest in _context.SS_ExecuseRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ExecuseRequest.EmployeeId equals Hrs_Employees.id
                                          join SS_RequestActions in _context.SS_RequestActions
                                          on SS_ExecuseRequest.Id equals SS_RequestActions.RequestSerial
                                          where (SS_ExecuseRequest.Id == RequestID )
                                          select new { ConfigID = SS_RequestActions.ConfigId, SS_ExecuseRequest.EmployeeId, RequestSerial = SS_ExecuseRequest.Id, RequestCode = SS_ExecuseRequest.Code,
                                              RequestDate = SS_ExecuseRequest.RequestDate.ToString("yyyy-MM-dd"),
                                              SS_ExecuseRequest.ExecuseType, SS_ExecuseRequest.ExecuseReason,
                                              ExecuseDate = SS_ExecuseRequest.ExecuseDate.ToString("yyyy-MM-dd"),
                                              SS_ExecuseRequest.ExecuseTime, SS_ExecuseRequest.ExecuseShift, SS_ExecuseRequest.ExecuseRemarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyEndOfServiceDetails(int RequestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from eosRequest in _context.SS_EndOfServiceRequest
                                          join employee in _context.Hrs_Employees
                                          on eosRequest.EmployeeId equals employee.id
                                          from resignationReason in _context.SS_ResignationReason
                                              .Where(rr => rr.Code == eosRequest.ResignationReasonCode)
                                              .DefaultIfEmpty()  
                                          where eosRequest.Id == RequestID
                                          select new
                                          {
                                              eosRequest.EmployeeId,
                                              eosRequest.ServiceYears,
                                              eosRequest.SerciveMonths,
                                              eosRequest.ServiceDays,
                                              RequestSerial = eosRequest.Id,
                                              RequestCode = eosRequest.Code,
                                              RequestDate = eosRequest.RequestDate.ToString("yyyy-MM-dd"),
                                              Eosdate = eosRequest.Eosdate.ToString("yyyy-MM-dd"),
                                              eosRequest.Eosremarks,
                                              RsignationReason = resignationReason.ArbName ?? string.Empty, 
                                              EmployeeName = employee.ArbName + " " +
                                                           employee.FatherArbName + " " +
                                                           employee.GrandArbName + " " +
                                                           employee.FamilyArbName
                                          };
                }
                else
                {
                    Result.ResultObject = from eosRequest in _context.SS_EndOfServiceRequest
                                          join employee in _context.Hrs_Employees
                                          on eosRequest.EmployeeId equals employee.id
                                          from resignationReason in _context.SS_ResignationReason
                                              .Where(rr => rr.Code == eosRequest.ResignationReasonCode)
                                              .DefaultIfEmpty()  
                                          where eosRequest.Id == RequestID
                                          select new
                                          {
                                              eosRequest.EmployeeId,
                                              eosRequest.ServiceYears,
                                              eosRequest.SerciveMonths,
                                              eosRequest.ServiceDays,
                                              RequestSerial = eosRequest.Id,
                                              RequestCode = eosRequest.Code,
                                              RequestDate = eosRequest.RequestDate.ToString("yyyy-MM-dd"),
                                              Eosdate = eosRequest.Eosdate.ToString("yyyy-MM-dd"),
                                              eosRequest.Eosremarks,
                                              RsignationReason = resignationReason.ArbName ?? string.Empty,  
                                              EmployeeName = employee.EngName + " " +
                                                           employee.FatherEngName + " " +
                                                           employee.GrandEngName + " " +
                                                           employee.FatherEngName
                                          };

            
                }

                Result.ErrorMessage = "";
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
        private object GetMyExitReentryDetails(int RequestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_ExitEntryRequest in _context.SS_ExitEntryRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ExitEntryRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_ExitEntryRequest.Id == RequestID )
                                          select new { SS_ExitEntryRequest.EmployeeId, RequestSerial = SS_ExitEntryRequest.Id, RequestCode = SS_ExitEntryRequest.Code, SS_ExitEntryRequest.RequestDate, SS_ExitEntryRequest.ExitDate, ReturnDate = SS_ExitEntryRequest.EntryDate, SS_ExitEntryRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_ExitEntryRequest in _context.SS_ExitEntryRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ExitEntryRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_ExitEntryRequest.Id == RequestID)
                                          select new {SS_ExitEntryRequest.EmployeeId, RequestSerial = SS_ExitEntryRequest.Id, RequestCode = SS_ExitEntryRequest.Code, SS_ExitEntryRequest.RequestDate, SS_ExitEntryRequest.ExitDate, ReturnDate = SS_ExitEntryRequest.EntryDate, SS_ExitEntryRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyAnnualTicketDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_AnnualTicketRelatedRequests in _context.SS_AnnualTicketRelatedRequests
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AnnualTicketRelatedRequests.EmployeeId equals Hrs_Employees.id
                                     



                                          where (SS_AnnualTicketRelatedRequests.Id == ReauestID )
                                          select new { SS_AnnualTicketRelatedRequests.EmployeeId, RequestSerial = SS_AnnualTicketRelatedRequests.Id, RequestCode = SS_AnnualTicketRelatedRequests.Code, SS_AnnualTicketRelatedRequests.RequestDate, SS_AnnualTicketRelatedRequests.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_AnnualTicketRelatedRequests in _context.SS_AnnualTicketRelatedRequests
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AnnualTicketRelatedRequests.EmployeeId equals Hrs_Employees.id
                                          where (SS_AnnualTicketRelatedRequests.Id == ReauestID)
                                          select new {  SS_AnnualTicketRelatedRequests.EmployeeId, RequestSerial = SS_AnnualTicketRelatedRequests.Id, RequestCode = SS_AnnualTicketRelatedRequests.Code, SS_AnnualTicketRelatedRequests.RequestDate, SS_AnnualTicketRelatedRequests.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyBankLetterDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_LoanLetterRequest in _context.SS_LoanLetterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_LoanLetterRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_LoanLetterRequest.Id == ReauestID )
                                          select new {  SS_LoanLetterRequest.EmployeeId, RequestSerial = SS_LoanLetterRequest.Id, RequestCode = SS_LoanLetterRequest.Code, SS_LoanLetterRequest.RequestDate, SS_LoanLetterRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_LoanLetterRequest in _context.SS_LoanLetterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_LoanLetterRequest.EmployeeId equals Hrs_Employees.id
                                       



                                          where (SS_LoanLetterRequest.Id == ReauestID)
                                          select new {  SS_LoanLetterRequest.EmployeeId, RequestSerial = SS_LoanLetterRequest.Id, RequestCode = SS_LoanLetterRequest.Code, SS_LoanLetterRequest.RequestDate, SS_LoanLetterRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyOtherLetterDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_OtherLetterRequest in _context.SS_OtherLetterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_OtherLetterRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_OtherLetterRequest.Id == ReauestID )
                                          select new {  SS_OtherLetterRequest.EmployeeId, RequestSerial = SS_OtherLetterRequest.Id, RequestCode = SS_OtherLetterRequest.Code, SS_OtherLetterRequest.RequestDate, SS_OtherLetterRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_OtherLetterRequest in _context.SS_OtherLetterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_OtherLetterRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_OtherLetterRequest.Id == ReauestID )
                                          select new {  SS_OtherLetterRequest.EmployeeId, RequestSerial = SS_OtherLetterRequest.Id, RequestCode = SS_OtherLetterRequest.Code, SS_OtherLetterRequest.RequestDate, SS_OtherLetterRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyChamberCommerceDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_ChamberofCommerceLetterRequest in _context.SS_ChamberofCommerceLetterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ChamberofCommerceLetterRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_ChamberofCommerceLetterRequest.Id == ReauestID )
                                          select new { SS_ChamberofCommerceLetterRequest.EmployeeId, RequestSerial = SS_ChamberofCommerceLetterRequest.Id, RequestCode = SS_ChamberofCommerceLetterRequest.Code, SS_ChamberofCommerceLetterRequest.RequestDate, SS_ChamberofCommerceLetterRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_ChamberofCommerceLetterRequest in _context.SS_ChamberofCommerceLetterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ChamberofCommerceLetterRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_ChamberofCommerceLetterRequest.Id == ReauestID )
                                          select new {  SS_ChamberofCommerceLetterRequest.EmployeeId, RequestSerial = SS_ChamberofCommerceLetterRequest.Id, RequestCode = SS_ChamberofCommerceLetterRequest.Code, SS_ChamberofCommerceLetterRequest.RequestDate, SS_ChamberofCommerceLetterRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMySCFHSLetterDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_SCFHSLetterRequest in _context.SS_ScfhsletterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_SCFHSLetterRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_SCFHSLetterRequest.Id == ReauestID )
                                          select new {  SS_SCFHSLetterRequest.EmployeeId, RequestSerial = SS_SCFHSLetterRequest.Id, RequestCode = SS_SCFHSLetterRequest.Code, SS_SCFHSLetterRequest.RequestDate, SS_SCFHSLetterRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_SCFHSLetterRequest in _context.SS_ScfhsletterRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_SCFHSLetterRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_SCFHSLetterRequest.Id == ReauestID )
                                          select new { SS_SCFHSLetterRequest.EmployeeId, RequestSerial = SS_SCFHSLetterRequest.Id, RequestCode = SS_SCFHSLetterRequest.Code, SS_SCFHSLetterRequest.RequestDate, SS_SCFHSLetterRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyPayslipLetterDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_PaySlipRequest in _context.SS_PaySlipRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_PaySlipRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_PaySlipRequest.Id == ReauestID )
                                          select new {  SS_PaySlipRequest.EmployeeId, RequestSerial = SS_PaySlipRequest.Id, RequestCode = SS_PaySlipRequest.Code, SS_PaySlipRequest.RequestDate, SS_PaySlipRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_PaySlipRequest in _context.SS_PaySlipRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_PaySlipRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_PaySlipRequest.Id == ReauestID )
                                          select new { SS_PaySlipRequest.EmployeeId, RequestSerial = SS_PaySlipRequest.Id, RequestCode = SS_PaySlipRequest.Code, SS_PaySlipRequest.RequestDate, SS_PaySlipRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyTrainingRequestDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_TrainingRequest in _context.SS_TrainingRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_TrainingRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_TrainingRequest.Id == ReauestID )
                                          select new {  SS_TrainingRequest.EmployeeId, RequestSerial = SS_TrainingRequest.Id, RequestCode = SS_TrainingRequest.Code, SS_TrainingRequest.RequestDate, SS_TrainingRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_TrainingRequest in _context.SS_TrainingRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_TrainingRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_TrainingRequest.Id == ReauestID )
                                          select new {  SS_TrainingRequest.EmployeeId, RequestSerial = SS_TrainingRequest.Id, RequestCode = SS_TrainingRequest.Code, SS_TrainingRequest.RequestDate, SS_TrainingRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyDaycareSupportDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_DaycareSupportReaquest in _context.SS_DaycareSupportReaquest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_DaycareSupportReaquest.EmployeeId equals Hrs_Employees.id
                                          where (SS_DaycareSupportReaquest.Id == ReauestID )
                                          select new {  SS_DaycareSupportReaquest.EmployeeId, RequestSerial = SS_DaycareSupportReaquest.Id, RequestCode = SS_DaycareSupportReaquest.Code, SS_DaycareSupportReaquest.RequestDate, SS_DaycareSupportReaquest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_DaycareSupportReaquest in _context.SS_DaycareSupportReaquest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_DaycareSupportReaquest.EmployeeId equals Hrs_Employees.id
                                          where (SS_DaycareSupportReaquest.Id == ReauestID )
                                          select new {  SS_DaycareSupportReaquest.EmployeeId, RequestSerial = SS_DaycareSupportReaquest.Id, RequestCode = SS_DaycareSupportReaquest.Code, SS_DaycareSupportReaquest.RequestDate, SS_DaycareSupportReaquest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyEducationSupportDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_EducationSupportRequest in _context.SS_EducationSupportRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_EducationSupportRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_EducationSupportRequest.Id == ReauestID )
                                          select new {  SS_EducationSupportRequest.EmployeeId, RequestSerial = SS_EducationSupportRequest.Id, RequestCode = SS_EducationSupportRequest.Code, SS_EducationSupportRequest.RequestDate, SS_EducationSupportRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_EducationSupportRequest in _context.SS_EducationSupportRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_EducationSupportRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_EducationSupportRequest.Id == ReauestID )
                                          select new {  SS_EducationSupportRequest.EmployeeId, RequestSerial = SS_EducationSupportRequest.Id, RequestCode = SS_EducationSupportRequest.Code, SS_EducationSupportRequest.RequestDate, SS_EducationSupportRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyAdvanceHousingDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_AdvanceHousingRequest in _context.SS_AdvanceHousingRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AdvanceHousingRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_AdvanceHousingRequest.Id == ReauestID )
                                          select new { SS_AdvanceHousingRequest.EmployeeId, RequestSerial = SS_AdvanceHousingRequest.Id, RequestCode = SS_AdvanceHousingRequest.Code, SS_AdvanceHousingRequest.RequestDate, SS_AdvanceHousingRequest.Remarks, SS_AdvanceHousingRequest.InstallmentDate, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_AdvanceHousingRequest in _context.SS_AdvanceHousingRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AdvanceHousingRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_AdvanceHousingRequest.Id == ReauestID )
                                          select new {  SS_AdvanceHousingRequest.EmployeeId, RequestSerial = SS_AdvanceHousingRequest.Id, RequestCode = SS_AdvanceHousingRequest.Code, SS_AdvanceHousingRequest.RequestDate, SS_AdvanceHousingRequest.Remarks, SS_AdvanceHousingRequest.InstallmentDate, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyAdvanceSalaryDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_AdvanceSalaryRequest in _context.SS_AdvanceSalaryRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AdvanceSalaryRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_AdvanceSalaryRequest.Id == ReauestID )
                                          select new {  SS_AdvanceSalaryRequest.EmployeeId, RequestSerial = SS_AdvanceSalaryRequest.Id, RequestCode = SS_AdvanceSalaryRequest.Code, SS_AdvanceSalaryRequest.RequestDate, SS_AdvanceSalaryRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_AdvanceSalaryRequest in _context.SS_AdvanceSalaryRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AdvanceSalaryRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_AdvanceSalaryRequest.Id == ReauestID)
                                          select new {  SS_AdvanceSalaryRequest.EmployeeId, RequestSerial = SS_AdvanceSalaryRequest.Id, RequestCode = SS_AdvanceSalaryRequest.Code, SS_AdvanceSalaryRequest.RequestDate, SS_AdvanceSalaryRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyOvertimeRequestDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_OvertimeRequest in _context.SS_OvertimeRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_OvertimeRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_OvertimeRequest.Id == ReauestID)
                                          select new {  SS_OvertimeRequest.EmployeeId, RequestSerial = SS_OvertimeRequest.Id, RequestCode = SS_OvertimeRequest.Code, SS_OvertimeRequest.RequestDate, SS_OvertimeRequest.Remarks, SS_OvertimeRequest.OvertimeDate, SS_OvertimeRequest.HoursCount, SS_OvertimeRequest.MinutsCount, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_OvertimeRequest in _context.SS_OvertimeRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_OvertimeRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_OvertimeRequest.Id == ReauestID )
                                          select new {  SS_OvertimeRequest.EmployeeId, RequestSerial = SS_OvertimeRequest.Id, RequestCode = SS_OvertimeRequest.Code, SS_OvertimeRequest.RequestDate, SS_OvertimeRequest.Remarks, SS_OvertimeRequest.OvertimeDate, SS_OvertimeRequest.HoursCount, SS_OvertimeRequest.MinutsCount, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyEducationFeesDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_EducationFeesCompensationApplication in _context.SS_EducationFeesCompensationApplication
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_EducationFeesCompensationApplication.EmployeeId equals Hrs_Employees.id
                                          where (SS_EducationFeesCompensationApplication.Id == ReauestID )
                                          select new { SS_EducationFeesCompensationApplication.EmployeeId, RequestSerial = SS_EducationFeesCompensationApplication.Id, RequestCode = SS_EducationFeesCompensationApplication.Code, SS_EducationFeesCompensationApplication.RequestDate, SS_EducationFeesCompensationApplication.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_EducationFeesCompensationApplication in _context.SS_EducationFeesCompensationApplication
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_EducationFeesCompensationApplication.EmployeeId equals Hrs_Employees.id
                                          where (SS_EducationFeesCompensationApplication.Id == ReauestID)
                                          select new { SS_EducationFeesCompensationApplication.EmployeeId, RequestSerial = SS_EducationFeesCompensationApplication.Id, RequestCode = SS_EducationFeesCompensationApplication.Code, SS_EducationFeesCompensationApplication.RequestDate, SS_EducationFeesCompensationApplication.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyGrievanceFormDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_GrievanceFormRequest in _context.SS_GrievanceFormRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_GrievanceFormRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_GrievanceFormRequest.Id == ReauestID)
                                          select new {  SS_GrievanceFormRequest.EmployeeId, RequestSerial = SS_GrievanceFormRequest.Id, RequestCode = SS_GrievanceFormRequest.Code, SS_GrievanceFormRequest.RequestDate, SS_GrievanceFormRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_GrievanceFormRequest in _context.SS_GrievanceFormRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_GrievanceFormRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_GrievanceFormRequest.Id == ReauestID )
                                          select new {  SS_GrievanceFormRequest.EmployeeId, RequestSerial = SS_GrievanceFormRequest.Id, RequestCode = SS_GrievanceFormRequest.Code, SS_GrievanceFormRequest.RequestDate, SS_GrievanceFormRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyAssaultEscalationDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_AssaultEscalationFormRequest in _context.SS_AssaultEscalationFormRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AssaultEscalationFormRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_AssaultEscalationFormRequest.Id == ReauestID )
                                          select new {  SS_AssaultEscalationFormRequest.EmployeeId, RequestSerial = SS_AssaultEscalationFormRequest.Id, RequestCode = SS_AssaultEscalationFormRequest.Code, SS_AssaultEscalationFormRequest.RequestDate, SS_AssaultEscalationFormRequest.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_AssaultEscalationFormRequest in _context.SS_AssaultEscalationFormRequest
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_AssaultEscalationFormRequest.EmployeeId equals Hrs_Employees.id
                                          where (SS_AssaultEscalationFormRequest.Id == ReauestID)
                                          select new {  SS_AssaultEscalationFormRequest.EmployeeId, RequestSerial = SS_AssaultEscalationFormRequest.Id, RequestCode = SS_AssaultEscalationFormRequest.Code, SS_AssaultEscalationFormRequest.RequestDate, SS_AssaultEscalationFormRequest.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyBankAccountUpdateDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_BankAccountUpdate in _context.SS_BankAccountUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_BankAccountUpdate.EmployeeId equals Hrs_Employees.id
                                          join sys_Banks in _context.sys_Banks
                                          on SS_BankAccountUpdate.BankId equals sys_Banks.ID
                                          where (SS_BankAccountUpdate.Id == ReauestID)
                                          select new {  BankName = sys_Banks.ArbName, SS_BankAccountUpdate.BankAccountNumber, SS_BankAccountUpdate.EmployeeId, RequestSerial = SS_BankAccountUpdate.Id, RequestCode = SS_BankAccountUpdate.Code, SS_BankAccountUpdate.RequestDate, SS_BankAccountUpdate.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_BankAccountUpdate in _context.SS_BankAccountUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_BankAccountUpdate.EmployeeId equals Hrs_Employees.id
                                          join sys_Banks in _context.sys_Banks
                                          on SS_BankAccountUpdate.BankId equals sys_Banks.ID
                                          where (SS_BankAccountUpdate.Id == ReauestID )
                                          select new {  BankName = sys_Banks.EngName, SS_BankAccountUpdate.BankAccountNumber, SS_BankAccountUpdate.EmployeeId, RequestSerial = SS_BankAccountUpdate.Id, RequestCode = SS_BankAccountUpdate.Code, SS_BankAccountUpdate.RequestDate, SS_BankAccountUpdate.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };


                    Result.ErrorMessage = "";
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
        private object GetMyContactInfoUpdateDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_ContactInformationUpdate in _context.SS_ContactInformationUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ContactInformationUpdate.EmployeeId equals Hrs_Employees.id
                                          where (SS_ContactInformationUpdate.Id == ReauestID)
                                          select new {  SS_ContactInformationUpdate.EmployeeId, RequestSerial = SS_ContactInformationUpdate.Id, RequestCode = SS_ContactInformationUpdate.Code, SS_ContactInformationUpdate.RequestDate, SS_ContactInformationUpdate.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_ContactInformationUpdate in _context.SS_ContactInformationUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_ContactInformationUpdate.EmployeeId equals Hrs_Employees.id
                                          where (SS_ContactInformationUpdate.Id == ReauestID)
                                          select new {SS_ContactInformationUpdate.EmployeeId, RequestSerial = SS_ContactInformationUpdate.Id, RequestCode = SS_ContactInformationUpdate.Code, SS_ContactInformationUpdate.RequestDate, SS_ContactInformationUpdate.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyDependentsInfoUpdateDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_DependentsInformationUpdate in _context.SS_DependentsInformationUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_DependentsInformationUpdate.EmployeeId equals Hrs_Employees.id
                                          where (SS_DependentsInformationUpdate.Id == ReauestID )
                                          select new {  SS_DependentsInformationUpdate.EmployeeId, RequestSerial = SS_DependentsInformationUpdate.Id, RequestCode = SS_DependentsInformationUpdate.Code, SS_DependentsInformationUpdate.RequestDate, SS_DependentsInformationUpdate.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_DependentsInformationUpdate in _context.SS_DependentsInformationUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_DependentsInformationUpdate.EmployeeId equals Hrs_Employees.id
                                          where (SS_DependentsInformationUpdate.Id == ReauestID)
                                          select new {  SS_DependentsInformationUpdate.EmployeeId, RequestSerial = SS_DependentsInformationUpdate.Id, RequestCode = SS_DependentsInformationUpdate.Code, SS_DependentsInformationUpdate.RequestDate, SS_DependentsInformationUpdate.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyMedicalInsuranceUpdateDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_MedicalInsuranceAdjustments in _context.SS_MedicalInsuranceAdjustments
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_MedicalInsuranceAdjustments.EmployeeId equals Hrs_Employees.id
                                          where (SS_MedicalInsuranceAdjustments.Id == ReauestID)
                                          select new { SS_MedicalInsuranceAdjustments.EmployeeId, RequestSerial = SS_MedicalInsuranceAdjustments.Id, RequestCode = SS_MedicalInsuranceAdjustments.Code, SS_MedicalInsuranceAdjustments.RequestDate, SS_MedicalInsuranceAdjustments.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_MedicalInsuranceAdjustments in _context.SS_MedicalInsuranceAdjustments
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_MedicalInsuranceAdjustments.EmployeeId equals Hrs_Employees.id
                                          where (SS_MedicalInsuranceAdjustments.Id == ReauestID)
                                          select new { SS_MedicalInsuranceAdjustments.EmployeeId, RequestSerial = SS_MedicalInsuranceAdjustments.Id, RequestCode = SS_MedicalInsuranceAdjustments.Code, SS_MedicalInsuranceAdjustments.RequestDate, SS_MedicalInsuranceAdjustments.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyLegalDocumentsUpdateDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_OtherLegalDocumentUpdates in _context.SS_OtherLegalDocumentUpdates
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_OtherLegalDocumentUpdates.EmployeeId equals Hrs_Employees.id
                                          where (SS_OtherLegalDocumentUpdates.Id == ReauestID)
                                          select new {  SS_OtherLegalDocumentUpdates.EmployeeId, RequestSerial = SS_OtherLegalDocumentUpdates.Id, RequestCode = SS_OtherLegalDocumentUpdates.Code, SS_OtherLegalDocumentUpdates.RequestDate, SS_OtherLegalDocumentUpdates.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_OtherLegalDocumentUpdates in _context.SS_OtherLegalDocumentUpdates
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_OtherLegalDocumentUpdates.EmployeeId equals Hrs_Employees.id
                                          where (SS_OtherLegalDocumentUpdates.Id == ReauestID)
                                          select new {  SS_OtherLegalDocumentUpdates.EmployeeId, RequestSerial = SS_OtherLegalDocumentUpdates.Id, RequestCode = SS_OtherLegalDocumentUpdates.Code, SS_OtherLegalDocumentUpdates.RequestDate, SS_OtherLegalDocumentUpdates.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        private object GetMyEmployeeFileUpdateDetails(int ReauestID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (Lang == 1)
                {
                    Result.ResultObject = from SS_EmployeeFileUpdate in _context.SS_EmployeeFileUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_EmployeeFileUpdate.EmployeeId equals Hrs_Employees.id
                                          where (SS_EmployeeFileUpdate.Id == ReauestID )
                                          select new {  SS_EmployeeFileUpdate.EmployeeId, RequestSerial = SS_EmployeeFileUpdate.Id, RequestCode = SS_EmployeeFileUpdate.Code, SS_EmployeeFileUpdate.RequestDate, SS_EmployeeFileUpdate.Remarks, EmployeeName = Hrs_Employees.ArbName + " " + Hrs_Employees.FatherArbName + " " + Hrs_Employees.GrandArbName + " " + Hrs_Employees.FamilyArbName };
                }
                else
                {
                    Result.ResultObject = from SS_EmployeeFileUpdate in _context.SS_EmployeeFileUpdate
                                          join Hrs_Employees in _context.Hrs_Employees
                                          on SS_EmployeeFileUpdate.EmployeeId equals Hrs_Employees.id
                                          where (SS_EmployeeFileUpdate.Id == ReauestID )
                                          select new {SS_EmployeeFileUpdate.EmployeeId, RequestSerial = SS_EmployeeFileUpdate.Id, RequestCode = SS_EmployeeFileUpdate.Code, SS_EmployeeFileUpdate.RequestDate, SS_EmployeeFileUpdate.Remarks, EmployeeName = Hrs_Employees.EngName + " " + Hrs_Employees.FatherEngName + " " + Hrs_Employees.GrandEngName + " " + Hrs_Employees.FamilyEngName };
                }

                Result.ErrorMessage = "";
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
        #endregion





        private object SaveVacationRequest(SS_VacationRequest request, string requestType)
        {
            try
            {
                Result = new GeneralOutputClass<object>();

                var conflictingRequest = _context.SS_VacationRequest
                          .Where(x => x.EmployeeID == request.EmployeeID)  
                          .Where(x => x.RequestStautsTypeId != 2)  
                          .Where(x => x.RequestStautsTypeId != 5)  
                          .FirstOrDefault(x =>
                              (request.StartDate >= x.StartDate && request.StartDate <= x.EndDate) ||  
                              (request.EndDate >= x.StartDate && request.EndDate <= x.EndDate) ||      
                              (x.StartDate >= request.StartDate && x.StartDate <= request.EndDate)     
                          );

                if (conflictingRequest != null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "There is a conflicting leave request in the period from " + conflictingRequest.StartDate.ToString("yyyy-MM-dd") + " to " + conflictingRequest.EndDate.ToString("yyyy-MM-dd") + "";
                    return Result;
                }
                else
                {
                    var conflictingVacations = _context.Hrs_EmployeesVacations
                          .Where(x => x.EmployeeId == request.EmployeeID)
                          .FirstOrDefault(x =>
                              (request.StartDate >= x.ActualStartDate && request.StartDate < x.ExpectedEndDate) ||
                              (request.EndDate >= x.ActualStartDate && request.EndDate <= x.ExpectedEndDate) ||
                              (x.ActualStartDate >= request.StartDate && x.ActualStartDate < request.EndDate)
                          );
                    if (conflictingVacations != null)
                    {
                        Result.ErrorCode = 0;
                        Result.ErrorMessage = "There is a conflicting with a vacation in the period from " + conflictingVacations.ActualStartDate + " to " + conflictingVacations.ExpectedEndDate  + "";
                        return Result;
                    }
                    else
                    {
                        request.VacationType = requestType;
                        if (requestType != "SS_0013")
                        {

                            request.VacationTypeID = 1;

                        }
                        request.RequestStautsTypeId = 3;
                        request.RegDate = DateTime.Now;

                        _context.SS_VacationRequest.Add(request);
                        _context.SaveChanges();

                        request.Code = GenerateVacationCode(requestType, request.ID);
                        _context.SaveChanges();

                        var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeID, request.ID);
                        return CreateSuccessResultWithRecipients(actionResult, request.ID, request.Code);
                    }
                    
                }
     

             }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        private GeneralOutputClass<object> CreateSuccessResultWithRecipients(object actionResult, int requestId, string requestCode)
        {
            var actionData = (GeneralOutputClass<object>)actionResult;

            return new GeneralOutputClass<object>
            {   
                ErrorMessage = "Transaction Done Successfully",
                ErrorCode = 1,
                ResultObject = new
                {
                    RequestId = requestId,
                    RequestCode = requestCode,
                    Recipients = actionData.ResultObject  
                }
            };
        }
        private object SaveExcuseRequest(SS_ExecuseRequest request, string requestType)
        {
            Result = new GeneralOutputClass<object>();
            var companySettings = _context.Sys_Companies
                   .Select(c => new
                   {
                       c.ID,
                       c.Code,
                       c.EngName,
                       c.ArbName,
                       c.ExecuseRequestHoursallowed
                   })
            .FirstOrDefault();
            var hoursAllowed = companySettings?.ExecuseRequestHoursallowed ?? 0;

            if (hoursAllowed > 0)
            {
                 if (!decimal.TryParse(request.ExecuseTime, out decimal excuseTime))
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Invalid excuse time format";
                    return Result;
                }

                if (excuseTime > hoursAllowed)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = $"Excuse hours ({excuseTime}) exceed the allowed limit ({hoursAllowed} hours)";
                    return Result;
                }
                else
                {
                    SaveGenericRequest(
                        request,
                        requestType,
                        r =>
                        {
                            r.RequestDate = DateTime.Now;
                            r.RequestStautsTypeId = 3;
                            _context.SS_ExecuseRequest.Add(r);
                        },
                        r => $"EX_{r.Id}",
                        r => r.EmployeeId
                    );
                    var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
                    return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
                }
            }
            else
            {
                 if (!int.TryParse(request.ExecuseTime, out int excuseTime))
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Invalid excuse time format";
                    return Result;
                }

                SaveGenericRequest(
                    request,
                    requestType,
                    r =>
                    {
                        r.RequestDate = DateTime.Now;
                        r.RequestStautsTypeId = 3;
                        _context.SS_ExecuseRequest.Add(r);
                    },
                    r => $"EX_{r.Id}",
                    r => r.EmployeeId
                );
                var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
                return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
            }
        }
     
        private object SaveEndOfServiceRequest(SS_EndOfServiceRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;

                    _context.SS_EndOfServiceRequest.Add(r);
                },
                r => $"EOS_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

        private object SaveLoanLetterRequest(SS_LoanLetterRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;

                    _context.SS_LoanLetterRequest.Add(r);
                },
                r => $"LNTR_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }


         private object SaveExitReentryRequest(SS_ExitEntryRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_ExitEntryRequest.Add(r);
                },
                r => $"EXIT_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);

        }

        private object SaveVisaRequest(SS_VisaRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_VisaRequest.Add(r);
                },
                r => $"VISA_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

 

        private object SaveAnnualTicketRequest(SS_AnnualTicketRelatedRequests request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_AnnualTicketRelatedRequests.Add(r);
                },
                r => $"TKT_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

 

        private object SaveOtherLetterRequest(SS_OtherLetterRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_OtherLetterRequest.Add(r);
                },
                r => $"OLTR_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

        private object SaveChamberCommerceRequest(SS_ChamberofCommerceLetterRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_ChamberofCommerceLetterRequest.Add(r);
                },
                r => $"CHMBR_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

        private object SaveSCFHSLetterRequest(SS_ScfhsletterRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_ScfhsletterRequest.Add(r);
                },
                r => $"SCFHS_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

        private object SavePayslipLetterRequest(SS_PaySlipRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_PaySlipRequest.Add(r);
                },
                r => $"PAYSLP_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

        private object SaveOccurrenceVarianceRequest(SS_OccurrenceVarianceReportingLetter request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_OccurrenceVarianceReportingLetter.Add(r);
                },
                r => $"OVR_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

         private object SaveTrainingRequest(SS_TrainingRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_TrainingRequest.Add(r);
                },
                r => $"TRN_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

        private object SaveDaycareSupportRequest(SS_DaycareSupportReaquest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_DaycareSupportReaquest.Add(r);
                },
                r => $"DAYCARE_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

        private object SaveEducationSupportRequest(SS_EducationSupportRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_EducationSupportRequest.Add(r);
                },
                r => $"EDU_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

        private object SaveAdvanceHousingRequest(SS_AdvanceHousingRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_AdvanceHousingRequest.Add(r);
                },
                r => $"ADV_HOUS_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

        private object SaveAdvanceSalaryRequest(SS_AdvanceSalaryRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_AdvanceSalaryRequest.Add(r);
                },
                r => $"ADV_SAL_{r.Id}",
                r => r.EmployeeId

            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

        private object SaveOvertimeRequest(SS_OvertimeRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_OvertimeRequest.Add(r);
                },
                r => $"OT_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

        private object SaveEducationFeesRequest(SS_EducationFeesCompensationApplication request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_EducationFeesCompensationApplication.Add(r);
                },
                r => $"EDU_FEES_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

         private object SaveGrievanceRequest(SS_GrievanceFormRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_GrievanceFormRequest.Add(r);
                },
                r => $"GRV_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

        private object SaveInterviewEvaluationRequest(SS_InterviewEvaluationFormRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_InterviewEvaluationFormRequest.Add(r);
                },
                r => $"INT_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

        private object SaveAssaultEscalationRequest(SS_AssaultEscalationFormRequest request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_AssaultEscalationFormRequest.Add(r);
                },
                r => $"ASLT_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

        private object SavePhysiciansPrivilegingRequest(SS_PhysiciansPrivilegingFormRequest request, string requestType)
        {
            return SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_PhysiciansPrivilegingFormRequest.Add(r);
                },
                r => $"PHY_{r.Id}",
                r => r.EmployeeId
            );
        }

        private object SaveBankAccountUpdateRequest(SS_BankAccountUpdate request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_BankAccountUpdate.Add(r);
                },
                r => $"BANK_UPD_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }
        private object SaveSS_ContactInformationUpdateRequest(SS_ContactInformationUpdate request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_ContactInformationUpdate.Add(r);
                },
                r => $"COnt_UPD_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }
        private object SaveDependentsUpdateRequest(SS_DependentsInformationUpdate request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r =>
                {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_DependentsInformationUpdate.Add(r);
                },
                r => $"DEP_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }
        private object SaveMedicalInsuranceUpdateRequest(SS_MedicalInsuranceAdjustments request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_MedicalInsuranceAdjustments.Add(r);
                },
                r => $"MED_INS_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }
        private object SaveLegalDocumentsUpdateRequest(SS_OtherLegalDocumentUpdates request, string requestType)
        {
              SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_OtherLegalDocumentUpdates.Add(r);
                },
                r => $"LEGAL_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }
        private object SaveEmployeeFileUpdateRequest(SS_EmployeeFileUpdate request, string requestType)
        {
             SaveGenericRequest(
                request,
                requestType,
                r => {
                    r.RequestDate = DateTime.Now;
                    r.RequestStautsTypeId = 3;
                    _context.SS_EmployeeFileUpdate.Add(r);
                },
                r => $"FILE_{r.Id}",
                r => r.EmployeeId
            );
            var actionResult = SaveRequestActionFirstLevel(requestType, request.EmployeeId, request.Id);
            return CreateSuccessResultWithRecipients(actionResult, request.Id, request.Code);
        }

        private object SaveGenericRequest<T>(
            T request,
            string requestType,
            Action<T> addToContext,
            Func<T, string> generateCode,
            Func<T, int> getEmployeeId) where T : class
        {
            try
            {
                addToContext(request);
                _context.SaveChanges();

                 int id = 0;
                var idProperty = request.GetType().GetProperty("Id") ?? request.GetType().GetProperty("ID");
                if (idProperty != null)
                {
                    id = (int)idProperty.GetValue(request);
                }
                else
                {
                    throw new InvalidOperationException("ID property not found on request object");
                }

                var code = generateCode(request);
                request.GetType().GetProperty("Code")?.SetValue(request, code);

                _context.SaveChanges();
              //  SaveRequestActionFirstLevel(requestType, getEmployeeId(request), id);

                return CreateSuccessResult();
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        private string GenerateVacationCode(string requestType, int id)
        {
            return requestType switch
            {
                "SS_0011" => $"V_ADM_{id}",
                "SS_0012" => $"V_Med_{id}",
                "SS_0013" => $"V_Oth_{id}",
                "SS_0018"=> $"V_Oth_Med_{id}",
                "SS_00193" => $"LNTR_{id}",
                "SS_001915" => $"CHMB_{id}",
                "SS_001916" => $"SCFHS_{id}",
                _ => $"SS_{id}"
            };
        }

        private GeneralOutputClass<object> CreateSuccessResult()
        {
            return new GeneralOutputClass<object>
            {
                ErrorMessage = "Transaction Done Successfully",
                ErrorCode = 1
            };
        }

        private GeneralOutputClass<object> HandleError(Exception ex)
        {
            return new GeneralOutputClass<object>
            {
                ResultObject = null,
                ErrorCode = 0,
                ErrorMessage = ex.Message
            };
        }






        public object SaveAnnulavacationMedicalRequest(SS_VacationRequest Request)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                string FormCode = "SS_0012";
                Request.VacationType = FormCode;
                Request.VacationTypeID = 1;
                Request.RegDate = DateTime.Now;


                _context.SS_VacationRequest.Add(Request);
                _context.SaveChanges();
                int id = Request.ID;
                string Code = "V_MED_" + id + "";
                Request.Code = Code;
                _context.SaveChanges();
                SaveRequestActionFirstLevel(FormCode, Request.EmployeeID, id);


                Result.ErrorMessage = "Transaction Done Successfully With Serial:- "+Code+"";
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
        
        public object SaveRequestActionFirstLevel(string FormCode ,int EmpID,int RequestSerial)
        {
            Result = new GeneralOutputClass<object>();
            var recipients = new List<SysUser>();


            var Configuration = _context.SS_Configuration.Where(C => C.FormCode == FormCode && C.Rank == 1).FirstOrDefault();
            int UserTypeID = int.Parse(Configuration.UserTypeID);
            if (UserTypeID == 1)
            {
                int DitrectManager = (int)_context.Hrs_Employees.Where(E => E.id == EmpID).Select(E => E.ManagerId).FirstOrDefault();
                if (DitrectManager>0)
                {
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

            }





            
            //Position
            else if (UserTypeID==2 && Configuration.PositionID!=null)
            {
                int ssEmpID = 0;

                var EmployssinPosition = _context.hrs_Contracts.Where(E => E.PositionID ==int.Parse(Configuration.PositionID) && E.CancelDate ==null && (E.EndDate>DateTime.Now || E.EndDate==null)).Select(C => new { C.EmployeeID }).ToList();
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
 
                     newAction.ActionSerial = newAction.ID;
                    _context.SaveChanges();  
                }
                if (ssEmpID>0)
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
            else if (UserTypeID == 3 && Configuration.EmployeeID!= null)
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

            Result.ErrorCode = 1;
            Result.ErrorMessage = "Success";
            Result.ResultObject = new
            {
                Recipients = recipients,
                RequestSerial = RequestSerial
            };

            return Result;

        }

       

        public object SaveAnnualVacationRequest(SS_VacationRequest Request)
        {
            throw new NotImplementedException();
        }

        public object GeatServicePeriod(int EmployeeID,DateTime EndServiceDate)
        {
            Result = new GeneralOutputClass<object>();

            try
            {
                DateTime? JoinDate = _context.Hrs_Employees
                    .Where(E => E.id == EmployeeID)
                    .Select(E => E.JoinDate)
                    .FirstOrDefault();

                if (JoinDate == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Employee not found";
                    return Result;
                }

                DateTime currentDate = EndServiceDate;
                DateTime startDate = JoinDate.Value;

                // حساب الفرق بدقة
                int years = currentDate.Year - startDate.Year;
                int months = 0;
                int days = 0;

                 if (currentDate.Month < startDate.Month)
                {
                    years--;
                    months = 12 - startDate.Month + currentDate.Month;
                }
                else
                {
                    months = currentDate.Month - startDate.Month;
                }

                 if (currentDate.Day < startDate.Day)
                {
                    months--;
                    DateTime lastMonth = currentDate.AddMonths(-1);
                    days = (currentDate - lastMonth.AddDays(startDate.Day - currentDate.Day)).Days;
                }
                else
                {
                    days = currentDate.Day - startDate.Day;
                }

                 if (months < 0)
                {
                    years--;
                    months += 12;
                }

                var servicePeriod = new
                {
                    Years = years,
                    Months = months,
                    Days = days,
                    TotalDays = (currentDate - startDate).Days,
                    JoinDate = startDate.ToString("yyyy-MM-dd"),
                    CurrentDate = currentDate.ToString("yyyy-MM-dd"),
                    ServicePeriod = $"{years} سنة, {months} شهر, {days} يوم"
                };

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Service period calculated successfully";
                Result.ResultObject = servicePeriod;
            }
            catch (Exception ex)
            {
                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error: {ex.Message}";
            }

            return Result;
        }

        public object GetEmployeeRequests(int EmployeeID, int Lang)
        {
            Result = new GeneralOutputClass<object>();

            try
            {
                Result.ResultObject = from SS_VFollowup in _context.SS_VFollowup
                                      join statusType in _context.SS_RequestStatuesTypes
                                      on SS_VFollowup.RequestStautsTypeID equals statusType.Id
                                      where SS_VFollowup.EmployeeID == EmployeeID
                                      orderby SS_VFollowup.RequestDate descending
                                      select new
                                      {
                                          Id = SS_VFollowup.ID,
                                          RequestCode = SS_VFollowup.RequestSerial,
                                          RequestType = Lang == 1 ?
                                              SS_VFollowup.RequestArbName :
                                              SS_VFollowup.RequestEngName,
                                          RequestDate = SS_VFollowup.RequestDate.ToString("yyyy-MM-dd"),
                                          Status = Lang == 1 ?
                                              statusType.AraName :
                                              statusType.EngName,
                                         RequestName = Lang == 1 ?
                                              SS_VFollowup.RequestArbName :
                                              SS_VFollowup.RequestEngName,
                                          //VacationType = SS_VFollowup.VacationType,
                                          FormCode = SS_VFollowup.FormCode,
                                             CanViewDetails = true,
                                          HasStages = true
                                      };

                Result.ErrorCode = 1;
                Result.ErrorMessage = "Success";
            }
            catch (Exception ex)
            {

                Result.ErrorCode = 0;
                Result.ErrorMessage = $"Error: {ex.Message}";
            }
            return Result;

        }
    }
    }

