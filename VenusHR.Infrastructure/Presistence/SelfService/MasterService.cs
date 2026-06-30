 using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
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
                                              RequestID=SS_RequestActions.ActionSerial,
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
                                              RequestID = SS_RequestActions.ActionSerial,
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

        public object GetEmployeeByID(int employeeId, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                var employeeExists = _context.Hrs_Employees.Any(e => e.id == employeeId);
                if (!employeeExists)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = Lang == 1 ? "الموظف غير موجود" : "Employee not found";
                    return Result;
                }

                var todayAttendance = BuildTodayAttendance(employeeId);
                var legacyEmployee = BuildLegacyEmployeeSummary(employeeId, Lang);
                var profileSections = BuildEmployeeProfileSections(employeeId, Lang);

                Result.ResultObject = new
                {
                    Employee = legacyEmployee,
                    profileSections.PersonalInformation,
                    profileSections.ContactInformation,
                    profileSections.OrganizationInformation,
                    profileSections.EmploymentInformation,
                    profileSections.BankingInformation,
                    profileSections.IdentityAndTravelInformation,
                    profileSections.Documents,
                    profileSections.Dependents,
                    TodayAttendance = todayAttendance
                };
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

        public object GetEmployeeMonthlyTransactions(int employeeId, int month, int year, int lang, bool hideNotPaid = true)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (month < 1 || month > 12)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = lang == 1 ? "الشهر غير صالح (يجب أن يكون من 1 إلى 12)" : "Invalid month (must be between 1 and 12)";
                    return Result;
                }

                var employee = _context.Hrs_Employees
                    .Where(e => e.id == employeeId && e.CancelDate == null)
                    .Select(e => new { e.id, e.Code, e.ArbName, e.FatherArbName, e.GrandArbName, e.FamilyArbName, e.EngName, e.FatherEngName, e.GrandEngName, e.FamilyEngName })
                    .FirstOrDefault();

                if (employee == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = lang == 1 ? "الموظف غير موجود" : "Employee not found";
                    return Result;
                }

                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var fiscalPeriod = ResolveFiscalPeriodByMonth(connection, month, year, lang);
                if (fiscalPeriod == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = lang == 1 ? "الفترة المالية غير موجودة لهذا الشهر" : "Fiscal period not found for this month";
                    return Result;
                }

                var (fiscalPeriodId, periodFrom, periodTo, periodName) = fiscalPeriod.Value;

                int? transactionId = null;
                decimal financialWorkingUnits = 0;
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT TOP 1 ID, ISNULL(FinancialWorkingUnits, 0)
                        FROM hrs_EmployeesTransactions
                        WHERE EmployeeID = @EmployeeId
                          AND FiscalYearPeriodID = @FiscalPeriodId
                          AND PrepareType = 'N'";
                    cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));
                    cmd.Parameters.Add(new SqlParameter("@FiscalPeriodId", fiscalPeriodId));
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        transactionId = reader.GetInt32(0);
                        financialWorkingUnits = Convert.ToDecimal(reader.GetValue(1));
                    }
                }

                var dto = new EmployeeMonthlyTransactionDto
                {
                    EmployeeId = employeeId,
                    EmployeeCode = employee.Code,
                    EmployeeName = lang == 1
                        ? $"{employee.ArbName} {employee.FatherArbName} {employee.GrandArbName} {employee.FamilyArbName}".Trim()
                        : $"{employee.EngName} {employee.FatherEngName} {employee.GrandEngName} {employee.FamilyEngName}".Trim(),
                    Month = month,
                    Year = year,
                    FinancialPeriod = periodName,
                    IsPrepared = transactionId.HasValue,
                    HideNotPaid = hideNotPaid,
                    NumberOfWorkDays = financialWorkingUnits
                };

                if (transactionId.HasValue)
                {
                    dto.Entitlements = GetMonthlyTransactionLines(connection, transactionId.Value, 1, lang, hideNotPaid);
                    dto.Deductions = GetMonthlyTransactionLines(connection, transactionId.Value, -1, lang, hideNotPaid);

                    dto.TotalEntitlements = dto.Entitlements
                        .Where(x => x.PaidStatus == "Paid")
                        .Sum(x => x.Value ?? 0);
                    dto.TotalDeductions = dto.Deductions
                        .Where(x => x.PaidStatus == "Paid")
                        .Sum(x => x.Value ?? 0);
                    dto.NetSalary = Math.Round(dto.TotalEntitlements - dto.TotalDeductions, 2);
                }
                if (transactionId.HasValue)
                {
                    ApplyAttendanceAndSalaryMetrics(connection, dto, employeeId, periodFrom, periodTo, fiscalPeriodId, lang);

                    Result.ErrorCode = 1;
                    Result.ErrorMessage =  (lang == 1 ? "تم جلب المعاملة الشهرية بنجاح" : "Monthly transaction retrieved successfully");
                    Result.ResultObject = dto;
                }
                else
                {
                    Result.ResultObject = null;
                    Result.ErrorCode = 1;
                    Result.ErrorMessage = (lang == 1
                            ? "لا توجد بيانات راتب متاحة ..لم يتم الانتهاء من تجهيز الرواتب للفترة المحددة حتى الأن"
                            : "No Salary Data Available..Payroll processing for the selected period has not been completed yet");
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

        private static List<MonthlyTransactionLineItemDto> GetMonthlyTransactionLines(
            System.Data.Common.DbConnection connection,
            int transactionId,
            int sign,
            int lang,
            bool hideNotPaid)
        {
            var items = new List<MonthlyTransactionLineItemDto>();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT
                    d.TransactionTypeID,
                    CASE WHEN @Lang = 1 THEN t.ArbName ELSE t.EngName END AS ItemType,
                    SUM(d.NumericValue) AS Amount,
                    MAX(ISNULL(d.TextValue, '')) AS Description,
                    ISNULL(d.EmployeePayabilityScheduleID, 0) AS EmpSchId,
                    CASE WHEN t.IsPaid = 1 THEN 'Paid' ELSE 'Not Paid' END AS PaidStatus
                FROM hrs_EmployeesTransactionsDetails d
                INNER JOIN hrs_EmployeesTransactionsProjects p ON d.EmpTransProjID = p.ID
                INNER JOIN hrs_TransactionsTypes t ON t.ID = d.TransactionTypeID
                WHERE ISNULL(d.CancelDate, '') = ''
                  AND p.EmployeeTransactionID = @TransactionId
                  AND t.Sign = @Sign
                  AND (@HideNotPaid = 0 OR t.IsPaid = 1)
                GROUP BY d.TransactionTypeID, d.EmployeePayabilityScheduleID,
                         CASE WHEN @Lang = 1 THEN t.ArbName ELSE t.EngName END, t.IsPaid
                ORDER BY d.TransactionTypeID";
            cmd.Parameters.Add(new SqlParameter("@TransactionId", transactionId));
            cmd.Parameters.Add(new SqlParameter("@Sign", sign));
            cmd.Parameters.Add(new SqlParameter("@Lang", lang));
            cmd.Parameters.Add(new SqlParameter("@HideNotPaid", hideNotPaid ? 1 : 0));

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var amount = reader.IsDBNull(2) ? (decimal?)null : Convert.ToDecimal(reader.GetValue(2));
                items.Add(new MonthlyTransactionLineItemDto
                {
                    TransactionTypeId = reader.GetInt32(0),
                    ItemType = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Value = amount == 0 ? null : amount,
                    Notes = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    EmpSchId = reader.GetInt32(4),
                    PaidStatus = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
                });
            }

            return items;
        }

        private static (int Id, DateTime From, DateTime To, string Name)? ResolveFiscalPeriodByMonth(
            System.Data.Common.DbConnection connection,
            int month,
            int year,
            int lang)
        {
            var periodStart = new DateTime(year, month, 1);
            var periodEnd = periodStart.AddMonths(1).AddDays(-1);

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT TOP 1 ID, FromDate, ToDate,
                       CASE WHEN @Lang = 1 THEN ISNULL(ArbName, EngName) ELSE EngName END
                FROM sys_FiscalYearsPeriods
                WHERE ISNULL(CancelDate, '') = ''
                  AND FromDate <= @PeriodEnd
                  AND ToDate >= @PeriodStart
                ORDER BY FromDate DESC";
            cmd.Parameters.Add(new SqlParameter("@PeriodStart", periodStart));
            cmd.Parameters.Add(new SqlParameter("@PeriodEnd", periodEnd));
            cmd.Parameters.Add(new SqlParameter("@Lang", lang));

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            return (
                reader.GetInt32(0),
                reader.GetDateTime(1),
                reader.GetDateTime(2),
                reader.IsDBNull(3) ? string.Empty : reader.GetString(3));
        }

        private static void ApplyAttendanceAndSalaryMetrics(
            System.Data.Common.DbConnection connection,
            EmployeeMonthlyTransactionDto dto,
            int employeeId,
            DateTime periodFrom,
            DateTime periodTo,
            int fiscalPeriodId,
            int lang)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                    SET DATEFORMAT DMY;
                    SELECT
                        ISNULL(SUM(ap.NotpermitLate), 0) AS DelayHours,
                        ISNULL(SUM(ap.Overtime), 0) AS OvertimeHours,
                        ISNULL(SUM(ap.HolidayHours), 0) AS HolidayWorkHours,
                        ISNULL(MAX(ap.OTSalary), 0) AS OvertimeValueHourly,
                        ISNULL(MAX(ap.HOTSalary), 0) AS HolidayWorkValue,
                        ISNULL(MAX(ap.SalaryPerHour), 0) AS HourlySalary,
                        ISNULL(MAX(ap.SalaryPerDay), 0) AS DailySalary
                    FROM Att_AttendancePreparationProjects ap
                    WHERE ap.TrnsID IN (
                        SELECT ID FROM Att_AttendancePreparationDetails
                        WHERE EmployeeID = @EmployeeId
                          AND CancelDate IS NULL
                          AND CONVERT(date, GAttendDate, 103) >= CONVERT(date, @FromDate, 103)
                          AND CONVERT(date, GAttendDate, 103) <= CONVERT(date, @ToDate, 103)
                    )";
                cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));
                cmd.Parameters.Add(new SqlParameter("@FromDate", periodFrom.ToString("dd/MM/yyyy")));
                cmd.Parameters.Add(new SqlParameter("@ToDate", periodTo.ToString("dd/MM/yyyy")));
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dto.DelayHours = Convert.ToDecimal(reader.GetValue(0));
                    dto.OvertimeHours = Convert.ToDecimal(reader.GetValue(1));
                    dto.HolidayWorkHours = Convert.ToDecimal(reader.GetValue(2));
                    dto.OvertimeValueHourly = Convert.ToDecimal(reader.GetValue(3));
                    dto.HolidayWorkValue = Convert.ToDecimal(reader.GetValue(4));
                    dto.HourlySalary = Convert.ToDecimal(reader.GetValue(5));
                    dto.DailySalary = Convert.ToDecimal(reader.GetValue(6));
                }
            }

            int contractId = 0;
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT TOP 1 ID
                    FROM hrs_Contracts
                    WHERE EmployeeID = @EmployeeId
                      AND CancelDate IS NULL
                      AND StartDate <= @PeriodTo
                      AND (EndDate IS NULL OR EndDate >= @PeriodFrom)
                    ORDER BY StartDate DESC";
                cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));
                cmd.Parameters.Add(new SqlParameter("@PeriodFrom", periodFrom));
                cmd.Parameters.Add(new SqlParameter("@PeriodTo", periodTo));
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    contractId = Convert.ToInt32(result);
            }

            if (contractId > 0)
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    SET DATEFORMAT DMY;
                    DECLARE @SalaryCalc INT = (SELECT TOP 1 ISNULL(SalaryCalculation, 0) FROM sys_Companies);
                    IF @SalaryCalc = 0
                        SELECT dbo.fn_GetBasicSalary(@ContractId, @ToDate);
                    ELSE
                        SELECT dbo.fn_GetTotalAdditions(@ContractId, @ToDate);";
                cmd.Parameters.Add(new SqlParameter("@ContractId", contractId));
                cmd.Parameters.Add(new SqlParameter("@ToDate", periodTo.ToString("dd/MM/yyyy")));
                var basicSalary = cmd.ExecuteScalar();
                if (basicSalary != null && basicSalary != DBNull.Value)
                    dto.BasicSalary = Math.Round(Convert.ToDecimal(basicSalary), 2);
            }

            if (dto.BasicSalary > 0 && dto.NumberOfWorkDays > 0 && dto.DailySalary == 0)
            {
                dto.DailySalary = Math.Round(dto.BasicSalary / dto.NumberOfWorkDays, 2);
            }
        }

        public object GetEmployeeDependants(int employeeId, int lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                var employee = _context.Hrs_Employees
                    .Where(e => e.id == employeeId && e.CancelDate == null)
                    .Select(e => new { e.id, e.Code, e.ArbName, e.FatherArbName, e.GrandArbName, e.FamilyArbName, e.EngName, e.FatherEngName, e.GrandEngName, e.FamilyEngName })
                    .FirstOrDefault();

                if (employee == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = lang == 1 ? "الموظف غير موجود" : "Employee not found";
                    return Result;
                }

                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                int objectId;
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT TOP 1 ID
                        FROM sys_Objects
                        WHERE Code = REPLACE('hrs_EmployeesDependants', ' ', '')
                          AND ISNULL(CancelDate, '') = ''";
                    var result = cmd.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                    {
                        Result.ErrorCode = 0;
                        Result.ErrorMessage = lang == 1 ? "تعريف كائن المرافقين غير موجود" : "Dependants object definition not found";
                        return Result;
                    }
                    objectId = Convert.ToInt32(result);
                }

                var dependants = new List<EmployeeDependantDto>();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "hrs_GetEmployeesDependantsData";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));
                    cmd.Parameters.Add(new SqlParameter("@ObjectId", objectId));

                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        dependants.Add(MapDependantFromReader(reader, objectId));
                    }
                }

                var dto = new EmployeeDependantsResultDto
                {
                    EmployeeId = employeeId,
                    EmployeeCode = employee.Code,
                    EmployeeName = lang == 1
                        ? $"{employee.ArbName} {employee.FatherArbName} {employee.GrandArbName} {employee.FamilyArbName}".Trim()
                        : $"{employee.EngName} {employee.FatherEngName} {employee.GrandEngName} {employee.FamilyEngName}".Trim(),
                    ObjectId = objectId,
                    Dependants = dependants
                };

                Result.ErrorCode = 1;
                Result.ErrorMessage = lang == 1 ? "تم جلب بيانات المرافقين بنجاح" : "Employee dependants retrieved successfully";
                Result.ResultObject = dto;
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }

            return Result;
        }

        public object GetEmployeeVacationBalance(
            int employeeId,
            int lang,
            int? vacationTypeId = null,
            DateTime? balanceDate = null,
            DateTime? vacationEndDate = null,
            int? vacationId = null)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                var asOfDate = (balanceDate ?? DateTime.Today).Date;
                var employee = _context.Hrs_Employees
                    .Where(e => e.id == employeeId && e.CancelDate == null)
                    .Select(e => new
                    {
                        e.id,
                        e.Code,
                        e.ArbName,
                        e.FatherArbName,
                        e.GrandArbName,
                        e.FamilyArbName,
                        e.EngName,
                        e.FatherEngName,
                        e.GrandEngName,
                        e.FamilyEngName,
                        e.NationalityId
                    })
                    .FirstOrDefault();

                if (employee == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = lang == 1 ? "الموظف غير موجود" : "Employee not found";
                    return Result;
                }

                var resolvedVacationTypeId = vacationTypeId ?? _context.hrs_VacationsTypes
                    .Where(v => v.IsAnnual == true && v.CancelDate == null)
                    .OrderBy(v => v.Id)
                    .Select(v => v.Id)
                    .FirstOrDefault();

                if (resolvedVacationTypeId <= 0)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = lang == 1 ? "نوع الإجازة غير موجود" : "Vacation type not found";
                    return Result;
                }

                var vacationType = _context.hrs_VacationsTypes
                    .Where(v => v.Id == resolvedVacationTypeId)
                    .Select(v => new { v.Id, v.EngName, v.ArbName })
                    .FirstOrDefault();

                string nationality = string.Empty;
                if (employee.NationalityId.HasValue)
                {
                    nationality = _context.sys_Nationalities
                        .Where(n => n.ID == employee.NationalityId.Value)
                        .Select(n => lang == 1 ? n.ArbName : n.EngName)
                        .FirstOrDefault() ?? string.Empty;
                }

                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                bool roundAnnualVacBalance = false;
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT ISNULL(RoundAnnualVacBalance, 0) FROM hrs_VacationsTypes WHERE ID = @VacationTypeId";
                    cmd.Parameters.Add(new SqlParameter("@VacationTypeId", resolvedVacationTypeId));
                    var roundResult = cmd.ExecuteScalar();
                    if (roundResult != null && roundResult != DBNull.Value)
                        roundAnnualVacBalance = Convert.ToBoolean(roundResult);
                }

                var contractId = VacationBalanceCalculator.GetValidContractId(connection, employeeId, asOfDate);
                decimal entitledBalance = 0;
                if (contractId.HasValue)
                {
                    entitledBalance = VacationBalanceCalculator.CalculateAnnualVacationDays(
                        connection, contractId.Value, employeeId, asOfDate, resolvedVacationTypeId);
                }

                decimal requestedDays = 0;
                if (vacationEndDate.HasValue)
                    requestedDays = Math.Max(0, (decimal)(vacationEndDate.Value.Date - asOfDate).TotalDays);

                decimal totalBalance = roundAnnualVacBalance
                    ? VacationBalanceCalculator.RoundVacationBalance(Math.Round(entitledBalance, 2))
                    : Math.Round(entitledBalance, 0);
                decimal remainingBalance = roundAnnualVacBalance
                    ? VacationBalanceCalculator.RoundVacationBalance(Math.Round(entitledBalance - requestedDays, 2))
                    : Math.Round(entitledBalance - requestedDays, 2);

                var dto = new EmployeeVacationBalanceResultDto
                {
                    EmployeeId = employeeId,
                    EmployeeCode = employee.Code,
                    EmployeeName = lang == 1
                        ? $"{employee.ArbName} {employee.FatherArbName} {employee.GrandArbName} {employee.FamilyArbName}".Trim()
                        : $"{employee.EngName} {employee.FatherEngName} {employee.GrandEngName} {employee.FamilyEngName}".Trim(),
                    Nationality = nationality ?? string.Empty,
                    VacationTypeId = resolvedVacationTypeId,
                    VacationTypeName = lang == 1 ? vacationType?.ArbName ?? string.Empty : vacationType?.EngName ?? string.Empty,
                    ContractId = contractId,
                    BalanceDate = asOfDate,
                    EntitledBalance = Math.Round(entitledBalance, 2),
                    TotalBalance = totalBalance,
                    RemainingBalance = remainingBalance,
                    VacationDays = requestedDays,
                    RoundAnnualVacBalance = roundAnnualVacBalance,
                    PreviousVacations = GetEmployeeVacationHistory(connection, employeeId, resolvedVacationTypeId)
                };

                if (vacationId.HasValue && vacationId.Value > 0)
                {
                    dto.SelectedVacation = GetSelectedVacationDetails(connection, vacationId.Value, employeeId);
                    if (dto.SelectedVacation != null)
                    {
                        dto.EntitledBalance = dto.SelectedVacation.EntitledBalance;
                        dto.TotalBalance = dto.SelectedVacation.TotalBalance;
                        dto.RemainingBalance = dto.SelectedVacation.RemainingBalance;
                        dto.VacationDays = dto.SelectedVacation.VacationDays;
                    }
                }

                Result.ErrorCode = 1;
                Result.ErrorMessage = lang == 1 ? "تم جلب رصيد الإجازة بنجاح" : "Vacation balance retrieved successfully";
                Result.ResultObject = dto;
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }

            return Result;
        }

        public object GetEmployeeHealthInsurance(int employeeId, int lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                var employee = _context.Hrs_Employees
                    .Where(e => e.id == employeeId && e.CancelDate == null)
                    .Select(e => new
                    {
                        e.id,
                        e.Code,
                        e.ArbName,
                        e.FatherArbName,
                        e.GrandArbName,
                        e.FamilyArbName,
                        e.EngName,
                        e.FatherEngName,
                        e.GrandEngName,
                        e.FamilyEngName,
                        e.Whours,
                        e.IsProjectRelated,
                        e.IsSpecialForce
                    })
                    .FirstOrDefault();

                if (employee == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = lang == 1 ? "الموظف غير موجود" : "Employee not found";
                    return Result;
                }

                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var contractId = VacationBalanceCalculator.GetValidContractId(connection, employeeId, DateTime.Today);
                var fieldName = lang == 1 ? "ArbName" : "EngName";

                var dto = new EmployeeHealthInsuranceResultDto
                {
                    EmployeeId = employeeId,
                    EmployeeCode = employee.Code,
                    EmployeeName = lang == 1
                        ? $"{employee.ArbName} {employee.FatherArbName} {employee.GrandArbName} {employee.FamilyArbName}".Trim()
                        : $"{employee.EngName} {employee.FatherEngName} {employee.GrandEngName} {employee.FamilyEngName}".Trim(),
                    ContractId = contractId,
                    WorkHours = employee.Whours,
                    IsProjectRelated = employee.IsProjectRelated,
                    IsSpecialForce = employee.IsSpecialForce
                };

                if (contractId.HasValue)
                {
                    dto.HealthInsurance = GetHealthInsuranceData(connection, contractId.Value, fieldName);
                    dto.TravelTicket = GetTravelTicketData(connection, contractId.Value, fieldName);
                }

                Result.ErrorCode = 1;
                Result.ErrorMessage = lang == 1 ? "تم جلب بيانات التأمين الصحي بنجاح" : "Health insurance data retrieved successfully";
                Result.ResultObject = dto;
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }

            return Result;
        }

        private static HealthInsuranceDto GetHealthInsuranceData(
            System.Data.Common.DbConnection connection,
            int contractId,
            string fieldName)
        {
            var healthInsurance = new HealthInsuranceDto();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $@"
                SELECT
                    (SELECT TOP 1 HIC.[{fieldName}]
                     FROM hrs_HIPolicyContract HIPC
                     INNER JOIN hrs_HICompanyClasses HICC ON HICC.ID = HIPC.HICompanyClasses
                     INNER JOIN hrs_HICompanies HIC ON HIC.ID = HICC.HICompanyID
                     WHERE HIPC.ContractID = @ContractId
                       AND HIPC.CancelDate IS NULL
                       AND HIPC.ActiveDate <= GETDATE()
                     ORDER BY HIPC.ActiveDate DESC) AS CompanyName,
                    (SELECT TOP 1 HICC.[{fieldName}]
                     FROM hrs_HIPolicyContract HIPC
                     INNER JOIN hrs_HICompanyClasses HICC ON HICC.ID = HIPC.HICompanyClasses
                     INNER JOIN hrs_HICompanies HIC ON HIC.ID = HICC.HICompanyID
                     WHERE HIPC.ContractID = @ContractId
                       AND HIPC.CancelDate IS NULL
                       AND HIPC.ActiveDate <= GETDATE()
                     ORDER BY HIPC.ActiveDate DESC) AS ClassName,
                    (SELECT SUM(HIPC.EmployeeAmt)
                     FROM hrs_HIPolicyContract HIPC
                     INNER JOIN hrs_HICompanyClasses HICC ON HICC.ID = HIPC.HICompanyClasses
                     INNER JOIN hrs_HICompanies HIC ON HIC.ID = HICC.HICompanyID
                     WHERE HIPC.ContractID = @ContractId
                       AND HIPC.CancelDate IS NULL) AS EmployeeAmt,
                    (SELECT SUM(HIPC.CompanyAmt)
                     FROM hrs_HIPolicyContract HIPC
                     INNER JOIN hrs_HICompanyClasses HICC ON HICC.ID = HIPC.HICompanyClasses
                     INNER JOIN hrs_HICompanies HIC ON HIC.ID = HICC.HICompanyID
                     WHERE HIPC.ContractID = @ContractId
                       AND HIPC.CancelDate IS NULL) AS CompanyAmt,
                    (SELECT TOP 1 CONVERT(Date, HIPC.ActiveDate)
                     FROM hrs_HIPolicyContract HIPC
                     INNER JOIN hrs_HICompanyClasses HICC ON HICC.ID = HIPC.HICompanyClasses
                     INNER JOIN hrs_HICompanies HIC ON HIC.ID = HICC.HICompanyID
                     WHERE HIPC.ContractID = @ContractId
                       AND HIPC.CancelDate IS NULL
                       AND HIPC.ActiveDate <= GETDATE()
                     ORDER BY HIPC.ActiveDate DESC) AS ActiveDate";
            cmd.Parameters.Add(new SqlParameter("@ContractId", contractId));

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                healthInsurance.InsuranceCompany = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                healthInsurance.InsuranceClass = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                healthInsurance.EmployeeAmount = reader.IsDBNull(2) ? null : Convert.ToDecimal(reader.GetValue(2));
                healthInsurance.CompanyAmount = reader.IsDBNull(3) ? null : Convert.ToDecimal(reader.GetValue(3));
                healthInsurance.ActivationDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4);
            }

            return healthInsurance;
        }

        private static TravelTicketDto GetTravelTicketData(
            System.Data.Common.DbConnection connection,
            int contractId,
            string fieldName)
        {
            var travelTicket = new TravelTicketDto();
            int? mainCompanyId = null;
            using (var companyCmd = connection.CreateCommand())
            {
                companyCmd.CommandText = "SELECT TOP 1 ID FROM sys_Companies ORDER BY ID";
                var result = companyCmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    mainCompanyId = Convert.ToInt32(result);
            }

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT TC.TicketsClassID, TC.TicketsRouteID, TC.TotalCost, TC.IsPaid
                FROM hrs_TicketsContarct TC
                WHERE TC.ContractID = @ContractId
                  AND TC.CancelDate IS NULL
                  AND (@CompanyId IS NULL OR TC.CompanyID = @CompanyId)";
            cmd.Parameters.Add(new SqlParameter("@ContractId", contractId));
            cmd.Parameters.Add(new SqlParameter("@CompanyId", (object?)mainCompanyId ?? DBNull.Value));

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return travelTicket;

            travelTicket.TicketClassId = reader.IsDBNull(0) ? null : reader.GetInt32(0);
            travelTicket.TicketRouteId = reader.IsDBNull(1) ? null : reader.GetInt32(1);
            travelTicket.TotalCost = reader.IsDBNull(2) ? null : Convert.ToDecimal(reader.GetValue(2));
            travelTicket.IsPaid = reader.IsDBNull(3) ? null : Convert.ToBoolean(reader.GetValue(3));
            reader.Close();

            if (travelTicket.TicketClassId.HasValue)
            {
                using var classCmd = connection.CreateCommand();
                classCmd.CommandText = $"SELECT [{fieldName}] FROM hrs_TicketsClasses WHERE ID = @Id AND CancelDate IS NULL";
                classCmd.Parameters.Add(new SqlParameter("@Id", travelTicket.TicketClassId.Value));
                var className = classCmd.ExecuteScalar();
                travelTicket.TicketClassName = className == null || className == DBNull.Value ? string.Empty : className.ToString()!;
            }

            if (travelTicket.TicketRouteId.HasValue)
            {
                using var routeCmd = connection.CreateCommand();
                routeCmd.CommandText = $"SELECT [{fieldName}] FROM hrs_TicketsRoutes WHERE ID = @Id AND CancelDate IS NULL";
                routeCmd.Parameters.Add(new SqlParameter("@Id", travelTicket.TicketRouteId.Value));
                var routeName = routeCmd.ExecuteScalar();
                travelTicket.TicketRouteName = routeName == null || routeName == DBNull.Value ? string.Empty : routeName.ToString()!;
            }

            return travelTicket;
        }

        private static List<EmployeeVacationHistoryDto> GetEmployeeVacationHistory(
            System.Data.Common.DbConnection connection,
            int employeeId,
            int vacationTypeId)
        {
            var history = new List<EmployeeVacationHistoryDto>();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT
                    A.ID,
                    A.ActualStartDate,
                    ISNULL((
                        SELECT Due.ActualEndDate
                        FROM hrs_EmployeesVacations AS Due
                        WHERE Due.ID = A.OverDueVacation AND Due.CancelDate IS NULL
                    ), A.ActualEndDate) AS ActualEndDate,
                    ISNULL(A.TotalDays, 0) AS TotalDays,
                    ISNULL(A.ConsumDays, 0) AS ConsumDays,
                    ISNULL(A.vactiondays, 0) AS vactiondays,
                    ISNULL(A.OverdueDays, 0) AS OverdueDays,
                    ISNULL(A.RemainingDays, 0) AS RemainingDays,
                    ISNULL(A.ZeroingBalance, 0) AS ZeroingBalance,
                    A.PaymentTrnID
                FROM hrs_EmployeesVacations AS A
                WHERE A.CancelDate IS NULL
                  AND A.EmployeeID = @EmployeeId
                  AND A.VacationTypeID = @VacationTypeId
                ORDER BY A.ActualStartDate DESC";
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));
            cmd.Parameters.Add(new SqlParameter("@VacationTypeId", vacationTypeId));

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                history.Add(new EmployeeVacationHistoryDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                    ActualStartDate = reader.IsDBNull(reader.GetOrdinal("ActualStartDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ActualStartDate")),
                    ActualEndDate = reader.IsDBNull(reader.GetOrdinal("ActualEndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ActualEndDate")),
                    TotalDays = Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("TotalDays"))),
                    ConsumDays = Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("ConsumDays"))),
                    VacationDays = Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("vactiondays"))),
                    OverdueDays = Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("OverdueDays"))),
                    RemainingDays = Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("RemainingDays"))),
                    ZeroingBalance = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("ZeroingBalance"))),
                    PaymentTrnId = reader.IsDBNull(reader.GetOrdinal("PaymentTrnID")) ? null : reader.GetInt32(reader.GetOrdinal("PaymentTrnID"))
                });
            }

            return history;
        }

        private static EmployeeVacationDetailDto? GetSelectedVacationDetails(
            System.Data.Common.DbConnection connection,
            int vacationId,
            int employeeId)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT
                    V.ID,
                    V.EmployeeID,
                    V.ActualStartDate,
                    ISNULL((
                        SELECT TOP 1 O.ActualEndDate
                        FROM hrs_EmployeesVacations O
                        WHERE O.ParentVacationID = V.ID AND O.CancelDate IS NULL
                    ), V.ActualEndDate) AS ActualEndDate,
                    ISNULL(V.TotalDays, 0) AS TotalDays,
                    ISNULL(V.RemainingDays, 0) AS RemainingDays,
                    ISNULL(V.vactiondays, 0) AS vactiondays,
                    ISNULL(V.ZeroingBalance, 0) AS ZeroingBalance,
                    V.OverDueVacation,
                    V.VacationRequestID,
                    V.RegDate
                FROM hrs_EmployeesVacations V
                WHERE V.ID = @VacationId
                  AND V.EmployeeID = @EmployeeId
                  AND V.CancelDate IS NULL";
            cmd.Parameters.Add(new SqlParameter("@VacationId", vacationId));
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            var remainingDays = Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("RemainingDays")));
            var vacationDays = Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("vactiondays")));
            var detail = new EmployeeVacationDetailDto
            {
                VacationId = reader.GetInt32(reader.GetOrdinal("ID")),
                ActualStartDate = reader.IsDBNull(reader.GetOrdinal("ActualStartDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ActualStartDate")),
                ActualEndDate = reader.IsDBNull(reader.GetOrdinal("ActualEndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ActualEndDate")),
                TotalBalance = Math.Round(Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("TotalDays"))), 2),
                EntitledBalance = Math.Round(remainingDays + vacationDays, 2),
                RemainingBalance = Math.Round(remainingDays, 2),
                VacationDays = Math.Round(vacationDays, 2),
                ZeroingBalance = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("ZeroingBalance"))),
                OverDueVacationId = reader.IsDBNull(reader.GetOrdinal("OverDueVacation")) ? null : reader.GetInt32(reader.GetOrdinal("OverDueVacation")),
                VacationRequestId = reader.IsDBNull(reader.GetOrdinal("VacationRequestID")) ? null : reader.GetInt32(reader.GetOrdinal("VacationRequestID")),
                RegDate = reader.IsDBNull(reader.GetOrdinal("RegDate")) ? null : reader.GetDateTime(reader.GetOrdinal("RegDate"))
            };

            if (detail.OverDueVacationId.HasValue && detail.OverDueVacationId.Value > 0)
            {
                reader.Close();
                using var overdueCmd = connection.CreateCommand();
                overdueCmd.CommandText = @"
                    SELECT ActualStartDate, ActualEndDate
                    FROM hrs_EmployeesVacations
                    WHERE ID = @OverDueVacationId AND CancelDate IS NULL";
                overdueCmd.Parameters.Add(new SqlParameter("@OverDueVacationId", detail.OverDueVacationId.Value));
                using var overdueReader = overdueCmd.ExecuteReader();
                if (overdueReader.Read()
                    && !overdueReader.IsDBNull(0)
                    && !overdueReader.IsDBNull(1))
                {
                    var start = overdueReader.GetDateTime(0);
                    var end = overdueReader.GetDateTime(1);
                    detail.OverDueVacationDays = (end.Date - start.Date).Days + 1;
                }
            }

            return detail;
        }

        private static bool? ReadNullableBool(IDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal))
                return null;
            var value = reader.GetValue(ordinal);
            return value switch
            {
                bool b => b,
                int i => i != 0,
                short s => s != 0,
                byte by => by != 0,
                string str => str == "1" || str.Equals("true", StringComparison.OrdinalIgnoreCase) || str.Equals("Y", StringComparison.OrdinalIgnoreCase),
                _ => Convert.ToBoolean(value)
            };
        }

        private static string ReadNullableString(IDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return string.Empty;
            return Convert.ToString(reader.GetValue(ordinal)) ?? string.Empty;
        }

        private static string? ReadOptionalString(IDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return null;
            return Convert.ToString(reader.GetValue(ordinal));
        }

        private static int ReadInt32(IDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return 0;
            return Convert.ToInt32(reader.GetValue(ordinal));
        }

        private static int? ReadNullableInt32(IDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return null;
            return Convert.ToInt32(reader.GetValue(ordinal));
        }

        private static DateTime? ReadNullableDateTime(IDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return null;

            var value = reader.GetValue(ordinal);
            if (value is DateTime dateTime)
                return dateTime;
            if (value is string text && DateTime.TryParse(text, out var parsed))
                return parsed;
            return null;
        }

        private static EmployeeDependantDto MapDependantFromReader(IDataReader reader, int defaultObjectId)
        {
            return new EmployeeDependantDto
            {
                Id = ReadInt32(reader, "ID"),
                EmployeeId = ReadInt32(reader, "EmployeeID"),
                DependantTypeId = ReadNullableInt32(reader, "DependantTypeID"),
                EnglishName = ReadNullableString(reader, "EngName"),
                ArabicName = ReadNullableString(reader, "ArbName"),
                ArabicName4S = ReadOptionalString(reader, "ArbName4S"),
                BirthDate = ReadNullableDateTime(reader, "BirthDate"),
                BirthCityId = ReadNullableInt32(reader, "BirthCityID"),
                Sex = ReadOptionalString(reader, "Sex"),
                NationalityId = ReadNullableInt32(reader, "NationalityID"),
                InsuranceCovered = ReadNullableBool(reader, "InsuranceCovered"),
                InsurancePercentage = reader.IsDBNull(reader.GetOrdinal("InsurancePercentage")) ? null : Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("InsurancePercentage"))),
                TicketCovered = ReadNullableBool(reader, "TicketCovered"),
                TicketPercentage = reader.IsDBNull(reader.GetOrdinal("TicketPercentage")) ? null : Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("TicketPercentage"))),
                NationalIdOrIqamaNo = ReadOptionalString(reader, "NationalIDORIqamano"),
                Remarks = ReadOptionalString(reader, "Remarks"),
                RegDate = ReadNullableDateTime(reader, "RegDate"),
                FileName = ReadOptionalString(reader, "FileName"),
                ObjectId = ReadNullableInt32(reader, "ObjectId") ?? defaultObjectId
            };
        }

        private object BuildTodayAttendance(int employeeId)
        {
            var today = DateTime.Now.Date;
            var todayRecords = _context.Hrs_Mobile_Attendance
                .Where(a => a.EmployeeID == employeeId && a.CheckingDatetime.Date == today)
                .OrderBy(a => a.CheckingDatetime)
                .ToList();

            var firstRecord = todayRecords.FirstOrDefault(a => a.CheckType?.ToUpper() == "IN") ?? todayRecords.FirstOrDefault();
            var lastRecord = todayRecords.LastOrDefault(a => a.CheckType?.ToUpper() == "OUT") ?? todayRecords.LastOrDefault();

            TimeSpan? workingHours = null;
            if (firstRecord != null && lastRecord != null && firstRecord != lastRecord)
            {
                workingHours = lastRecord.CheckingDatetime - firstRecord.CheckingDatetime;
            }

            return new
            {
                Date = today.ToString("yyyy-MM-dd"),
                CheckIn = firstRecord != null ? new
                {
                    Time = firstRecord.CheckingDatetime.ToString("HH:mm:ss"),
                    Type = firstRecord.CheckType,
                    Latitude = firstRecord.Latitude,
                    Longitude = firstRecord.Longitude,
                    Distance = firstRecord.DistanceFromLocation,
                    Device = firstRecord.DeviceModel,
                    Location = $"{firstRecord.Latitude:F4}, {firstRecord.Longitude:F4}"
                } : null,
                CheckOut = lastRecord != null && lastRecord != firstRecord ? new
                {
                    Time = lastRecord.CheckingDatetime.ToString("HH:mm:ss"),
                    Type = lastRecord.CheckType,
                    Latitude = lastRecord.Latitude,
                    Longitude = lastRecord.Longitude,
                    Distance = lastRecord.DistanceFromLocation,
                    Device = lastRecord.DeviceModel,
                    Location = $"{lastRecord.Latitude:F4}, {lastRecord.Longitude:F4}"
                } : null,
                WorkingHours = workingHours?.ToString(@"hh\:mm\:ss"),
                Status = GetDayStatus(firstRecord, lastRecord),
                TotalRecords = todayRecords.Count
            };
        }

        private object? BuildLegacyEmployeeSummary(int employeeId, int lang)
        {
            var isArabic = lang == 1;
            var today = DateTime.Now.Date;

            var emp = _context.Hrs_Employees
                .Where(e => e.id == employeeId)
                .Select(e => new
                {
                    e.Code,
                    e.Phone,
                    e.Mobile,
                    e.WorkE_Mail,
                    e.E_Mail,
                    e.ArbName,
                    e.FatherArbName,
                    e.GrandArbName,
                    e.FamilyArbName,
                    e.EngName,
                    e.FatherEngName,
                    e.GrandEngName,
                    e.FamilyEngName,
                    e.JoinDate,
                    e.SsnNo,
                    e.PassPortNo,
                    e.DepartmentId,
                    e.BranchId,
                    e.LocationId,
                    e.ManagerId
                })
                .FirstOrDefault();

            if (emp == null)
                return null;

            var positionId = _context.hrs_Contracts
                .Where(c => c.EmployeeID == employeeId
                    && c.CancelDate == null
                    && c.StartDate <= today
                    && (c.EndDate == null || c.EndDate >= today))
                .OrderByDescending(c => c.StartDate)
                .Select(c => (int?)c.PositionID)
                .FirstOrDefault();

            string? positionName = null;
            if (positionId.HasValue)
            {
                positionName = _context.hrs_Positions
                    .Where(p => p.Id == positionId.Value)
                    .Select(p => isArabic ? p.ArbName : p.EngName)
                    .FirstOrDefault();
            }

            string? departmentName = emp.DepartmentId.HasValue
                ? GetLookupName("sys_Departments", emp.DepartmentId.Value, isArabic)
                : null;
            string? branchName = emp.BranchId.HasValue
                ? GetLookupName("sys_Branches", emp.BranchId.Value, isArabic)
                : null;
            string? locationName = emp.LocationId.HasValue
                ? GetLookupName("sys_Locations", emp.LocationId.Value, isArabic)
                : null;

            string? managerName = null;
            if (emp.ManagerId.HasValue)
            {
                var manager = _context.Hrs_Employees
                    .Where(m => m.id == emp.ManagerId.Value)
                    .Select(m => new
                    {
                        m.EngName,
                        m.FatherEngName,
                        m.GrandEngName,
                        m.FamilyEngName,
                        m.ArbName,
                        m.FatherArbName,
                        m.GrandArbName,
                        m.FamilyArbName
                    })
                    .FirstOrDefault();

                if (manager != null)
                {
                    managerName = isArabic
                        ? string.Join(" ", new[] { manager.ArbName, manager.FatherArbName, manager.GrandArbName, manager.FamilyArbName }.Where(s => !string.IsNullOrWhiteSpace(s)))
                        : string.Join(" ", new[] { manager.EngName, manager.FatherEngName, manager.GrandEngName, manager.FamilyEngName }.Where(s => !string.IsNullOrWhiteSpace(s)));
                }
            }

            if (isArabic)
            {
                return new
                {
                    emp.Code,
                    PositionName = positionName,
                    DepartmentName = departmentName,
                    PhoneNo = emp.Phone,
                    Mobile = emp.Mobile,
                    Email = emp.WorkE_Mail,
                    PersonalEmail = emp.E_Mail,
                    EmployeeName = string.Join(" ", new[] { emp.ArbName, emp.FatherArbName, emp.GrandArbName, emp.FamilyArbName }.Where(s => !string.IsNullOrWhiteSpace(s))),
                    emp.JoinDate,
                    ManagerName = managerName,
                    BranchName = branchName,
                    LocationName = locationName,
                    SsnNo = emp.SsnNo,
                    PassPortNo = emp.PassPortNo
                };
            }

            return new
            {
                emp.Code,
                PositionName = positionName,
                DepartmentName = departmentName,
                PhoneNo = emp.Phone,
                Mobile = emp.Mobile,
                Email = emp.WorkE_Mail,
                PersonalEmail = emp.E_Mail,
                EmployeeName = string.Join(" ", new[] { emp.EngName, emp.FatherEngName, emp.GrandEngName, emp.FamilyEngName }.Where(s => !string.IsNullOrWhiteSpace(s))),
                emp.JoinDate,
                ManagerName = managerName,
                BranchName = branchName,
                LocationName = locationName,
                SsnNo = emp.SsnNo,
                PassPortNo = emp.PassPortNo
            };
        }

        private EmployeeProfileSectionsDto BuildEmployeeProfileSections(int employeeId, int lang)
        {
            var isArabic = lang == 1;
            var today = DateTime.Now.Date;

            var emp = _context.Hrs_Employees
                .Where(e => e.id == employeeId)
                .Select(e => new
                {
                    e.Code,
                    e.EngName,
                    e.FatherEngName,
                    e.GrandEngName,
                    e.FamilyEngName,
                    e.ArbName,
                    e.FatherArbName,
                    e.GrandArbName,
                    e.FamilyArbName,
                    e.BirthDate,
                    e.BirthCityId,
                    e.NationalityId,
                    e.ReligionId,
                    e.MaritalStatusId,
                    e.BloodGroupId,
                    e.Sex,
                    e.Mobile,
                    e.WorkE_Mail,
                    e.E_Mail,
                    e.Phone,
                    e.DepartmentId,
                    e.BranchId,
                    e.LocationId,
                    e.ManagerId,
                    e.SponsorId,
                    e.JoinDate,
                    e.BankId,
                    e.BankAccountNumber,
                    e.SsnNo,
                    e.PassPortNo
                })
                .First();

            var activeContract = _context.hrs_Contracts
                .Where(c => c.EmployeeID == employeeId
                    && c.CancelDate == null
                    && c.StartDate <= today
                    && (c.EndDate == null || c.EndDate >= today))
                .OrderByDescending(c => c.StartDate)
                .Select(c => new
                {
                    c.ContractTypeID,
                    c.PositionID,
                    c.ProfessionID,
                    c.EmployeeClassID,
                    c.GradeStepID
                })
                .FirstOrDefault();

            string BuildFullName(string? first, string? father, string? grand, string? family) =>
                string.Join(" ", new[] { first, father, grand, family }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim();

            int? age = null;
            if (emp.BirthDate.HasValue)
            {
                age = today.Year - emp.BirthDate.Value.Date.Year;
                if (emp.BirthDate.Value.Date > today.AddYears(-age.Value))
                    age--;
            }

            string? sponsorName = null;
            string? contractTypeName = null;
            string? professionName = null;
            string? positionName = null;
            string? employeeClassName = null;
            string? gradeStepsName = null;

            if (activeContract != null)
            {
                positionName = _context.hrs_Positions
                    .Where(p => p.Id == activeContract.PositionID)
                    .Select(p => isArabic ? p.ArbName : p.EngName)
                    .FirstOrDefault();

                professionName = _context.Hrs_Professions
                    .Where(p => p.Id == activeContract.ProfessionID)
                    .Select(p => isArabic ? p.ArbName : p.EngName)
                    .FirstOrDefault();

                employeeClassName = GetLookupName("hrs_EmployeesClasses", activeContract.EmployeeClassID, isArabic);
                gradeStepsName = GetGradeStepDisplayName(activeContract.GradeStepID, isArabic);
                contractTypeName = GetLookupName("hrs_ContractsTypes", activeContract.ContractTypeID, isArabic);
            }

            if (emp.SponsorId.HasValue)
                sponsorName = GetLookupName("hrs_Sponsors", emp.SponsorId.Value, isArabic);

            var manager = emp.ManagerId.HasValue
                ? _context.Hrs_Employees
                    .Where(m => m.id == emp.ManagerId.Value)
                    .Select(m => new
                    {
                        m.EngName,
                        m.FatherEngName,
                        m.GrandEngName,
                        m.FamilyEngName,
                        m.ArbName,
                        m.FatherArbName,
                        m.GrandArbName,
                        m.FamilyArbName
                    })
                    .FirstOrDefault()
                : null;

            var identityDates = GetEmployeeIdentityDates(employeeId);
            var birthCountry = GetBirthCountryName(emp.BirthCityId, isArabic);
            var projectName = emp.LocationId.HasValue
                        ? GetLookupName("sys_Locations", emp.LocationId.Value, isArabic) : string.Empty;

            List<EmployeeDocumentItemDto> documents;
            List<EmployeeDependentItemDto> dependents;
            try
            {
                documents = GetEmployeeProfileDocuments(employeeId, isArabic);
            }
            catch
            {
                documents = new List<EmployeeDocumentItemDto>();
            }

            try
            {
                dependents = GetEmployeeProfileDependents(employeeId, isArabic);
            }
            catch
            {
                dependents = new List<EmployeeDependentItemDto>();
            }

            return new EmployeeProfileSectionsDto
            {
                PersonalInformation = new EmployeePersonalInfoDto
                {
                    EmployeeCode = emp.Code ?? string.Empty,
                    EnglishName = BuildFullName(emp.EngName, emp.FatherEngName, emp.GrandEngName, emp.FamilyEngName),
                    ArabicName = BuildFullName(emp.ArbName, emp.FatherArbName, emp.GrandArbName, emp.FamilyArbName),
                    BirthDate = emp.BirthDate,
                    Age = age,
                    BirthCountry = birthCountry,
                    Nationality = emp.NationalityId.HasValue
                        ? GetLookupName("sys_Nationalities", emp.NationalityId.Value, isArabic)
                        : string.Empty,
                    Gender = FormatGender(emp.Sex, isArabic),
                    Religion = emp.ReligionId.HasValue
                        ? GetLookupName("hrs_Religions", emp.ReligionId.Value, isArabic)
                        : string.Empty,
                    MaritalStatus = emp.MaritalStatusId.HasValue
                        ? GetLookupName("hrs_MaritalStatus", emp.MaritalStatusId.Value, isArabic)
                        : string.Empty,
                    BloodGroup = emp.BloodGroupId.HasValue
                        ? GetLookupName("hrs_BloodGroups", emp.BloodGroupId.Value, isArabic)
                        : string.Empty
                },
                ContactInformation = new EmployeeContactInfoDto
                {
                    MobileNo = emp.Mobile ?? string.Empty,
                    WorkEmail = emp.WorkE_Mail ?? string.Empty,
                    PersonalEmail = emp.E_Mail ?? string.Empty,
                    PhoneNo = emp.Phone ?? string.Empty
                },
                OrganizationInformation = new EmployeeOrganizationInfoDto
                {
                    Branch = emp.BranchId.HasValue
                ? GetLookupName("sys_Branches", emp.BranchId.Value, isArabic)
                : string.Empty,
                    Department = emp.DepartmentId.HasValue
                        ? GetLookupName("sys_Departments", emp.DepartmentId.Value, isArabic)
                        : string.Empty,
                    Project = projectName,
                    Manager = manager == null
                        ? string.Empty
                        : (isArabic
                            ? BuildFullName(manager.ArbName, manager.FatherArbName, manager.GrandArbName, manager.FamilyArbName)
                            : BuildFullName(manager.EngName, manager.FatherEngName, manager.GrandEngName, manager.FamilyEngName)),
                    Location = emp.LocationId.HasValue
                        ? GetLookupName("sys_Locations", emp.LocationId.Value, isArabic)
                        : string.Empty
                },
                EmploymentInformation = new EmployeeEmploymentInfoDto
                {
                    JoinDate = emp.JoinDate,
                    Sponsor = sponsorName ?? string.Empty,
                    ContractType = contractTypeName ?? string.Empty,
                    Profession = professionName ?? string.Empty,
                    Position = positionName ?? string.Empty,
                    EmployeeClass = employeeClassName ?? string.Empty,
                    GradeSteps = gradeStepsName ?? string.Empty
                },
                BankingInformation = new EmployeeBankingInfoDto
                {
                    Bank = emp.BankId.HasValue
                        ? GetLookupName("sys_Banks", emp.BankId.Value, isArabic)
                        : string.Empty,
                    BankAccount = emp.BankAccountNumber ?? string.Empty
                },
                IdentityAndTravelInformation = new EmployeeIdentityTravelInfoDto
                {
                    IdentityNo = emp.SsnNo ?? string.Empty,
                    PassportNo = emp.PassPortNo ?? string.Empty,
                    PassportIssueDate = identityDates.PassportIssueDate,
                    PassportExpiryDate = identityDates.PassportExpiryDate,
                    IdentityIssueDate = identityDates.IdentityIssueDate,
                    IdentityExpiryDate = identityDates.IdentityExpiryDate
                },
                Documents = documents,
                Dependents = dependents
            };
        }

        private (string PassportIssueDate, string PassportExpiryDate, string IdentityIssueDate, string IdentityExpiryDate)
            GetEmployeeIdentityDates(int employeeId)
        {
            var empty = (string.Empty, string.Empty, string.Empty, string.Empty);
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    SELECT PassportIssueDate, PassportExpireDate, SSNOIssueDate, SSNOExpireDate
                    FROM hrs_Employees
                    WHERE ID = @EmployeeId";
                cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));

                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                    return empty;

                return (
                    ReadNullableString(reader, "PassportIssueDate"),
                    ReadNullableString(reader, "PassportExpireDate"),
                    ReadNullableString(reader, "SSNOIssueDate"),
                    ReadNullableString(reader, "SSNOExpireDate"));
            }
            catch
            {
                return empty;
            }
        }

        private static string FormatGender(string? sex, bool isArabic)
        {
            if (string.IsNullOrWhiteSpace(sex))
                return string.Empty;

            var normalized = sex.Trim().ToUpperInvariant();
            if (normalized is "M" or "1")
                return isArabic ? "ذكر" : "Male";
            if (normalized is "F" or "2")
                return isArabic ? "أنثى" : "Female";
            return sex;
        }

        private string GetLookupName(string tableName, int id, bool isArabic)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = $@"
                SELECT TOP 1 CASE WHEN @Lang = 1 THEN ISNULL(ArbName, EngName) ELSE ISNULL(EngName, ArbName) END
                FROM {tableName}
                WHERE ID = @Id AND ISNULL(CancelDate, '') = ''";
            cmd.Parameters.Add(new SqlParameter("@Id", id));
            cmd.Parameters.Add(new SqlParameter("@Lang", isArabic ? 1 : 0));
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? string.Empty : Convert.ToString(result) ?? string.Empty;
        }

        private string GetBirthCountryName(int? birthCityId, bool isArabic)
        {
            if (!birthCityId.HasValue)
                return string.Empty;

            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT TOP 1 CASE WHEN @Lang = 1 THEN ISNULL(c.ArbName, c.EngName) ELSE ISNULL(c.EngName, c.ArbName) END
                FROM sys_Cities city
                LEFT JOIN sys_Countries c ON c.ID = city.CountryID
                WHERE city.ID = @CityId";
            cmd.Parameters.Add(new SqlParameter("@CityId", birthCityId.Value));
            cmd.Parameters.Add(new SqlParameter("@Lang", isArabic ? 1 : 0));
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? string.Empty : Convert.ToString(result) ?? string.Empty;
        }

       

        private string GetGradeStepDisplayName(int gradeStepId, bool isArabic)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT TOP 1
                    CASE WHEN @Lang = 1
                        THEN ISNULL(g.ArbName, g.EngName) + N' - ' + CAST(ISNULL(gs.Step, 0) AS NVARCHAR(20))
                        ELSE ISNULL(g.EngName, g.ArbName) + ' - Step ' + CAST(ISNULL(gs.Step, 0) AS NVARCHAR(20))
                    END
                FROM hrs_GradesSteps gs
                INNER JOIN hrs_Grades g ON g.ID = gs.GradeID
                WHERE gs.ID = @GradeStepId AND ISNULL(gs.CancelDate, '') = ''";
            cmd.Parameters.Add(new SqlParameter("@GradeStepId", gradeStepId));
            cmd.Parameters.Add(new SqlParameter("@Lang", isArabic ? 1 : 0));
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? string.Empty : Convert.ToString(result) ?? string.Empty;
        }

        private static string GetDocumentStatus(DateTime? expiryDate, bool isArabic)
        {
            if (!expiryDate.HasValue)
                return isArabic ? "غير محدد" : "Not Set";

            var today = DateTime.Today;
            if (expiryDate.Value.Date < today)
                return isArabic ? "منتهي" : "Expired";
            if (expiryDate.Value.Date <= today.AddDays(30))
                return isArabic ? "قارب على الانتهاء" : "Expiring Soon";
            return isArabic ? "ساري" : "Valid";
        }

        private List<EmployeeDocumentItemDto> GetEmployeeProfileDocuments(int employeeId, bool isArabic)
        {
            var documents = new List<EmployeeDocumentItemDto>();
            var employeeObjectId = GetSysObjectId("hrs_Employees");
            var documentDetailsObjectId = GetSysObjectId("sys_DocumentsDetails");
            if (employeeObjectId <= 0)
                return documents;

            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT
                    dd.ID,
                    CASE WHEN @Lang = 1 THEN ISNULL(doc.ArbName, doc.EngName) ELSE ISNULL(doc.EngName, doc.ArbName) END AS DocumentTypeName,
                    dd.DocumentNumber,
                    dd.IssueDate,
                    dd.ExpiryDate,
                    (
                        SELECT TOP 1
                            CASE
                                WHEN ISNULL(att.FolderName, '') <> '' AND ISNULL(att.FileName, '') <> ''
                                    THEN att.FolderName + '/' + att.FileName
                                ELSE ISNULL(att.FileName, '')
                            END
                        FROM sys_ObjectsAttachments att
                        WHERE att.RecordID = dd.ID
                          AND att.ObjectID = @DocDetailsObjectId
                          AND ISNULL(att.CancelDate, '') = ''
                        ORDER BY att.ID DESC
                    ) AS AttachmentPath
                FROM sys_DocumentsDetails dd
                INNER JOIN sys_Documents doc ON doc.ID = dd.DocumentID
                WHERE dd.RecordID = @EmployeeId
                  AND dd.ObjectID = @EmployeeObjectId
                  AND ISNULL(dd.CancelDate, '') = ''
                ORDER BY dd.ID";
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));
            cmd.Parameters.Add(new SqlParameter("@EmployeeObjectId", employeeObjectId));
            cmd.Parameters.Add(new SqlParameter("@DocDetailsObjectId", documentDetailsObjectId > 0 ? documentDetailsObjectId : employeeObjectId));
            cmd.Parameters.Add(new SqlParameter("@Lang", isArabic ? 1 : 0));

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var issueDate = ReadNullableDateTime(reader, "IssueDate");
                var expiryDate = ReadNullableDateTime(reader, "ExpiryDate");
                var typeName = ReadNullableString(reader, "DocumentTypeName");

                documents.Add(new EmployeeDocumentItemDto
                {
                    Id = ReadInt32(reader, "ID"),
                    DocumentName = typeName,
                    DocumentType = typeName,
                    DocumentNumber = ReadNullableString(reader, "DocumentNumber"),
                    IssueDate = issueDate,
                    ExpiryDate = expiryDate,
                    Status = GetDocumentStatus(expiryDate, isArabic),
                    Attachment = ReadNullableString(reader, "AttachmentPath")
                });
            }

            return documents;
        }

        private List<EmployeeDependentItemDto> GetEmployeeProfileDependents(int employeeId, bool isArabic)
        {
            var dependents = new List<EmployeeDependentItemDto>();
            var dependantsObjectId = GetSysObjectId("hrs_EmployeesDependants");
            if (dependantsObjectId <= 0)
                return dependents;

            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "hrs_GetEmployeesDependantsData";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));
            cmd.Parameters.Add(new SqlParameter("@ObjectId", dependantsObjectId));

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var dependant = MapDependantFromReader(reader, dependantsObjectId);
                var relationship = dependant.DependantTypeId.HasValue
                    ? GetLookupName("hrs_DependantsTypes", dependant.DependantTypeId.Value, isArabic)
                    : string.Empty;

                var nationality = dependant.NationalityId.HasValue
                    ? GetLookupName("sys_Nationalities", dependant.NationalityId.Value, isArabic)
                    : string.Empty;

                dependents.Add(new EmployeeDependentItemDto
                {
                    Id = dependant.Id,
                    Name = isArabic
                        ? (string.IsNullOrWhiteSpace(dependant.ArabicName) ? dependant.EnglishName : dependant.ArabicName)
                        : (string.IsNullOrWhiteSpace(dependant.EnglishName) ? dependant.ArabicName : dependant.EnglishName),
                    Relationship = relationship,
                    Gender = FormatGender(dependant.Sex, isArabic),
                    BirthDate = dependant.BirthDate,
                    Nationality = nationality,
                    IdentityNo = dependant.NationalIdOrIqamaNo ?? string.Empty,
                    PassportNo = GetDependantPassportNo(dependant.Id, dependantsObjectId, isArabic)
                });
            }

            return dependents;
        }

        private string GetDependantPassportNo(int dependantId, int dependantsObjectId, bool isArabic)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT TOP 1 dd.DocumentNumber
                FROM sys_DocumentsDetails dd
                INNER JOIN sys_Documents doc ON doc.ID = dd.DocumentID
                WHERE dd.RecordID = @DependantId
                  AND dd.ObjectID = @DependantsObjectId
                  AND ISNULL(dd.CancelDate, '') = ''
                  AND (
                        doc.EngName LIKE '%Passport%'
                     OR doc.ArbName LIKE N'%جواز%'
                     OR doc.Code LIKE '%Passport%'
                  )
                ORDER BY dd.ID DESC";
            cmd.Parameters.Add(new SqlParameter("@DependantId", dependantId));
            cmd.Parameters.Add(new SqlParameter("@DependantsObjectId", dependantsObjectId));
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? string.Empty : Convert.ToString(result) ?? string.Empty;
        }

        public int GetSysObjectId(string objectCode)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT TOP 1 ID
                FROM sys_Objects
                WHERE Code = REPLACE(@ObjectCode, ' ', '')
                  AND ISNULL(CancelDate, '') = ''";
            cmd.Parameters.Add(new SqlParameter("@ObjectCode", objectCode));
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }

        private string GetDayStatus(VenusHR.Core.Master.Hrs_Mobile_Attendance checkIn, VenusHR.Core.Master.Hrs_Mobile_Attendance checkOut)
        {
            if (checkIn == null && checkOut == null)
                return "Absent";
            else if (checkIn != null && checkOut != null && checkIn != checkOut)
                return "Present";
            else if (checkIn != null || checkOut != null)
                return "Partial";
            else
                return "Unknown";
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
               .Select(C => new { C.Code, C.ArbName })
                    .ToList();


                    Result.ErrorCode = 1;
                }
                else
                {
                    Result.ResultObject = _context.SS_ResignationReason
               .Select(C => new { C.Code, C.EngName })
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

        public object GetUserMenusByPermission(int userId)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (userId <= 0)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "userId must be greater than zero";
                    return Result;
                }

                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                int? groupId;
                using (var groupCommand = connection.CreateCommand())
                {
                    groupCommand.CommandText = @"
                        SELECT TOP 1 GroupID
                        FROM sys_GroupsUsers
                        WHERE UserID = @UserId
                          AND CancelDate IS NULL
                        ORDER BY ID";
                    groupCommand.Parameters.Add(new SqlParameter("@UserId", userId));
                    var groupResult = groupCommand.ExecuteScalar();
                    groupId = groupResult == null || groupResult == DBNull.Value
                        ? null
                        : Convert.ToInt32(groupResult);
                }

                if (!groupId.HasValue || groupId.Value <= 0)
                {
                    groupId = 0;
                }

                var modules = new List<ModuleMenuPermissionResult>();
                using (var modulesCommand = connection.CreateCommand())
                {
                    modulesCommand.CommandText = @"
                        SELECT ID, Code, EngName, ArbName
                        FROM sys_Modules
                        WHERE CancelDate IS NULL
                        ORDER BY ISNULL(Rank, 0), ID";
                    modulesCommand.CommandType = CommandType.Text;

                    using var moduleReader = modulesCommand.ExecuteReader();
                    while (moduleReader.Read())
                    {
                        var moduleRow = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                        for (var i = 0; i < moduleReader.FieldCount; i++)
                        {
                            moduleRow[moduleReader.GetName(i)] = moduleReader.IsDBNull(i) ? null : moduleReader.GetValue(i);
                        }

                        var moduleId = GetIntFromDictionary(moduleRow, "ID");
                        if (!moduleId.HasValue)
                            continue;

                        modules.Add(new ModuleMenuPermissionResult
                        {
                            ModuleId = moduleId.Value,
                            Code = GetStringFromDictionary(moduleRow, "Code"),
                            EngName = GetStringFromDictionary(moduleRow, "EngName"),
                            ArbName = GetStringFromDictionary(moduleRow, "ArbName"),
                            Menus = new List<MenuPermissionNode>()
                        });
                    }
                }

                foreach (var module in modules)
                {
                    var flatMenuRows = new List<Dictionary<string, object?>>();
                    using (var menuCommand = connection.CreateCommand())
                    {
                        menuCommand.CommandText = "hrs_GetMenuPermissionsAll";
                        menuCommand.CommandType = CommandType.StoredProcedure;
                        menuCommand.Parameters.Add(new SqlParameter("@UserID", userId));
                        menuCommand.Parameters.Add(new SqlParameter("@GroupID", groupId.Value));
                        menuCommand.Parameters.Add(new SqlParameter("@ModuleID", module.ModuleId));

                        using var reader = menuCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            flatMenuRows.Add(row);
                        }
                    }

                    module.Menus = BuildMenuTree(flatMenuRows);
                }

                modules = modules
                    .Where(m => m.Menus.Count > 0)
                    .ToList();

                var allMenus = modules.SelectMany(m => m.Menus).ToList();

                Result.ResultObject = new
                {
                    UserId = userId,
                    GroupId = groupId.Value,
                    ModulesCount = modules.Count,
                    Modules = modules,
                    Menus = allMenus
                };
                Result.ErrorCode = 1;
                Result.ErrorMessage = "Menu permissions retrieved successfully";
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }

            return Result;
        }

        private static List<MenuPermissionNode> BuildMenuTree(List<Dictionary<string, object?>> flatMenuRows)
        {
            var nodesById = new Dictionary<int, MenuPermissionNode>();
            var childrenByParentId = new Dictionary<int, List<MenuPermissionNode>>();

            foreach (var row in flatMenuRows)
            {
                var id = GetIntFromDictionary(row, "ID");
                if (!id.HasValue)
                    continue;

                var node = new MenuPermissionNode
                {
                    Id = id.Value,
                    ParentId = GetIntFromDictionary(row, "ParentID"),
                    IsHide = GetBoolFromDictionary(row, "IsHide"),
                    ArbName = GetStringFromDictionary(row, "ArbName"),
                    EngName = GetStringFromDictionary(row, "EngName"),
                    Tag = GetStringFromDictionary(row, "Tag"),
                    LinkTarget = GetStringFromDictionary(row, "LinkTarget"),
                    LinkUrl = GetStringFromDictionary(row, "LinkUrl"),
                    Height = GetIntFromDictionary(row, "Height"),
                    Width = GetIntFromDictionary(row, "Width"),
                    TargetFormId = GetIntFromDictionary(row, "TargetFormID"),
                    MainId = GetIntFromDictionary(row, "MainID")
                };

                nodesById[node.Id] = node;
                if (node.ParentId.HasValue)
                {
                    if (!childrenByParentId.TryGetValue(node.ParentId.Value, out var children))
                    {
                        children = new List<MenuPermissionNode>();
                        childrenByParentId[node.ParentId.Value] = children;
                    }
                    children.Add(node);
                }
            }

            foreach (var entry in childrenByParentId)
            {
                if (nodesById.TryGetValue(entry.Key, out var parent))
                {
                    parent.Children = entry.Value
                        .OrderBy(c => c.Id)
                        .ToList();
                }
            }

            return nodesById.Values
                .Where(n => !n.ParentId.HasValue && !n.IsHide)
                .OrderBy(n => n.Id)
                .ToList();
        }

        private static int? GetIntFromDictionary(IDictionary<string, object?> row, string key)
        {
            if (!row.TryGetValue(key, out var value) || value == null)
                return null;

            return value switch
            {
                int i => i,
                long l => Convert.ToInt32(l),
                short s => s,
                byte b => b,
                decimal d => Convert.ToInt32(d),
                _ => int.TryParse(Convert.ToString(value), out var parsed) ? parsed : null
            };
        }

        private static bool GetBoolFromDictionary(IDictionary<string, object?> row, string key)
        {
            if (!row.TryGetValue(key, out var value) || value == null)
                return false;

            return value switch
            {
                bool b => b,
                int i => i != 0,
                long l => l != 0,
                short s => s != 0,
                byte by => by != 0,
                string s when s == "1" || s.Equals("true", StringComparison.OrdinalIgnoreCase) || s.Equals("Y", StringComparison.OrdinalIgnoreCase) => true,
                _ => Convert.ToBoolean(value)
            };
        }

        private static string? GetStringFromDictionary(IDictionary<string, object?> row, string key)
        {
            if (!row.TryGetValue(key, out var value) || value == null)
                return null;

            return Convert.ToString(value);
        }

        private sealed class ModuleMenuPermissionResult
        {
            public int ModuleId { get; set; }
            public string? Code { get; set; }
            public string? EngName { get; set; }
            public string? ArbName { get; set; }
            public List<MenuPermissionNode> Menus { get; set; } = new();
        }

        private sealed class MenuPermissionNode
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
            public bool IsHide { get; set; }
            public string? ArbName { get; set; }
            public string? EngName { get; set; }
            public string? Tag { get; set; }
            public string? LinkTarget { get; set; }
            public string? LinkUrl { get; set; }
            public int? Height { get; set; }
            public int? Width { get; set; }
            public int? TargetFormId { get; set; }
            public int? MainId { get; set; }
            public List<MenuPermissionNode> Children { get; set; } = new();
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
                                     && r.ActionSerial == RequestAction.ActionSerial);


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
    .OrderByDescending(S => S.ActionSerial)  
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
                                    newAction.ActionSerial = newAction.ActionSerial;
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
                        newAction.ActionSerial = newAction.ActionSerial;
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

                        newAction.ActionSerial = newAction.ActionSerial;
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

                    newAction.ActionSerial = newAction.ActionSerial;
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

        object IMaster.GetSysObjectId(string v)
        {
            return GetSysObjectId(v);
        }
    }
}
