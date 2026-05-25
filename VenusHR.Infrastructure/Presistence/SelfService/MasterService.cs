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

                var todayAttendance = new
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

                if (Lang == 1)
                {
                    var employeeData = (from emp in _context.Hrs_Employees
                                          join dept in _context.sys_Departments
                                          on emp.DepartmentId equals dept.ID into deptJoin
                                          from dept in deptJoin.DefaultIfEmpty()
                                          join contract in _context.hrs_Contracts
                                          on emp.id equals contract.EmployeeID into contractJoin
                                          from contract in contractJoin.DefaultIfEmpty()
                                          join position in _context.hrs_Positions
                                          on contract.PositionID equals position.Id into positionJoin
                                          from position in positionJoin.DefaultIfEmpty()
                                          join manager in _context.Hrs_Employees
                                          on emp.ManagerId equals manager.id into managerJoin
                                          from manager in managerJoin.DefaultIfEmpty()
                                          join location in _context.sys_Locations
                                          on emp.LocationId equals location.ID into locationJoin
                                          from location in locationJoin.DefaultIfEmpty()
                                          join branch in _context.Sys_Companies
                                          on emp.BranchId equals branch.ID into branchJoin
                                          from branch in branchJoin.DefaultIfEmpty()
                                          where emp.id == employeeId
                                          select new {
                                              emp.Code,
                                              PositionName = position != null ? position.ArbName : null,
                                              DepartmentName = dept != null ? dept.ArbName : null,
                                              PhoneNo = emp.Phone,
                                              Mobile = emp.Mobile,
                                              Email = emp.WorkE_Mail,
                                              PersonalEmail = emp.E_Mail,
                                              EmployeeName = emp.ArbName + " " + emp.FatherArbName + " " + emp.GrandArbName + " " + emp.FamilyArbName,
                                              JoinDate = emp.JoinDate,
                                              ManagerName = manager != null
                                                  ? manager.ArbName + " " + manager.FatherArbName + " " + manager.GrandArbName + " " + manager.FamilyArbName
                                                  : null,
                                              BranchName = branch != null ? branch.ArbName : null,
                                              LocationName = location != null ? location.ArbName : null,
                                              SsnNo = emp.SsnNo,
                                              PassPortNo = emp.PassPortNo
                                          }).FirstOrDefault();

                    Result.ResultObject = new { Employee = employeeData, TodayAttendance = todayAttendance };
                }
                else
                {
                    var employeeData = (from emp in _context.Hrs_Employees
                                          join dept in _context.sys_Departments
                                          on emp.DepartmentId equals dept.ID into deptJoin
                                          from dept in deptJoin.DefaultIfEmpty()
                                          join contract in _context.hrs_Contracts
                                          on emp.id equals contract.EmployeeID into contractJoin
                                          from contract in contractJoin.DefaultIfEmpty()
                                          join position in _context.hrs_Positions
                                          on contract.PositionID equals position.Id into positionJoin
                                          from position in positionJoin.DefaultIfEmpty()
                                          join manager in _context.Hrs_Employees
                                          on emp.ManagerId equals manager.id into managerJoin
                                          from manager in managerJoin.DefaultIfEmpty()
                                          join location in _context.sys_Locations
                                          on emp.LocationId equals location.ID into locationJoin
                                          from location in locationJoin.DefaultIfEmpty()
                                          join branch in _context.Sys_Companies
                                          on emp.BranchId equals branch.ID into branchJoin
                                          from branch in branchJoin.DefaultIfEmpty()
                                          where emp.id == employeeId
                                          select new {
                                              emp.Code,
                                              PositionName = position != null ? position.EngName : null,
                                              DepartmentName = dept != null ? dept.EngName : null,
                                              PhoneNo = emp.Phone,
                                              Mobile = emp.Mobile,
                                              Email = emp.WorkE_Mail,
                                              PersonalEmail = emp.E_Mail,
                                              EmployeeName = emp.EngName + " " + emp.FatherEngName + " " + emp.GrandEngName + " " + emp.FamilyEngName,
                                              JoinDate = emp.JoinDate,
                                              ManagerName = manager != null
                                                  ? manager.EngName + " " + manager.FatherEngName + " " + manager.GrandEngName + " " + manager.FamilyEngName
                                                  : null,
                                              BranchName = branch != null ? branch.EngName : null,
                                              LocationName = location != null ? location.EngName : null,
                                              SsnNo = emp.SsnNo,
                                              PassPortNo = emp.PassPortNo
                                          }).FirstOrDefault();

                    Result.ResultObject = new { Employee = employeeData, TodayAttendance = todayAttendance };
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
                    Result.ErrorCode = 0;
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

        private static EmployeeDependantDto MapDependantFromReader(IDataReader reader, int defaultObjectId)
        {
            return new EmployeeDependantDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                DependantTypeId = reader.IsDBNull(reader.GetOrdinal("DependantTypeID")) ? null : reader.GetInt32(reader.GetOrdinal("DependantTypeID")),
                EnglishName = reader.IsDBNull(reader.GetOrdinal("EngName")) ? string.Empty : reader.GetString(reader.GetOrdinal("EngName")),
                ArabicName = reader.IsDBNull(reader.GetOrdinal("ArbName")) ? string.Empty : reader.GetString(reader.GetOrdinal("ArbName")),
                ArabicName4S = reader.IsDBNull(reader.GetOrdinal("ArbName4S")) ? null : reader.GetString(reader.GetOrdinal("ArbName4S")),
                BirthDate = reader.IsDBNull(reader.GetOrdinal("BirthDate")) ? null : reader.GetDateTime(reader.GetOrdinal("BirthDate")),
                BirthCityId = reader.IsDBNull(reader.GetOrdinal("BirthCityID")) ? null : reader.GetInt32(reader.GetOrdinal("BirthCityID")),
                Sex = reader.IsDBNull(reader.GetOrdinal("Sex")) ? null : reader.GetString(reader.GetOrdinal("Sex")),
                NationalityId = reader.IsDBNull(reader.GetOrdinal("NationalityID")) ? null : reader.GetInt32(reader.GetOrdinal("NationalityID")),
                InsuranceCovered = ReadNullableBool(reader, "InsuranceCovered"),
                InsurancePercentage = reader.IsDBNull(reader.GetOrdinal("InsurancePercentage")) ? null : Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("InsurancePercentage"))),
                TicketCovered = ReadNullableBool(reader, "TicketCovered"),
                TicketPercentage = reader.IsDBNull(reader.GetOrdinal("TicketPercentage")) ? null : Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("TicketPercentage"))),
                NationalIdOrIqamaNo = reader.IsDBNull(reader.GetOrdinal("NationalIDORIqamano")) ? null : reader.GetString(reader.GetOrdinal("NationalIDORIqamano")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                RegDate = reader.IsDBNull(reader.GetOrdinal("RegDate")) ? null : reader.GetDateTime(reader.GetOrdinal("RegDate")),
                FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                ObjectId = reader.IsDBNull(reader.GetOrdinal("ObjectId")) ? defaultObjectId : reader.GetInt32(reader.GetOrdinal("ObjectId"))
            };
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
