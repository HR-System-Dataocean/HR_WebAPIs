using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VenusHR.Application.Common.Interfaces.Attendance;
using VenusHR.Core.Attendance;
using VenusHR.Core.Master;
using WorkFlow_EF;
 


namespace VenusHR.Infrastructure.Presistence.Attendance
{
    public class AttendanceSercives : IAttendance
    {
        private readonly ApplicationDBContext _context;
        private GeneralOutputClass<object> Result;

        public AttendanceSercives(ApplicationDBContext context)
        {
            _context = context;

        }

        public object CheckInOut(int EmployeeID, double Latitude, double longitude, DateTime CheckingDatetime, string DeviceID, string deviceModel, string osVersion, string networkType, int Lang, string CheckType, string? macAddress = null)
        {
            Result = new GeneralOutputClass<object>();
            bool isCheckOut = string.Equals(CheckType?.Trim(), "Out", StringComparison.OrdinalIgnoreCase);
            try
            {
                var employee = _context.Hrs_Employees.FirstOrDefault(e => e.id == EmployeeID);
                if (employee == null)
                {
                    Result.ErrorMessage = (Lang == 1) ? "الموظف غير موجود" : "Employee not found";
                    Result.ErrorCode = 0;
                    return Result;
                }

                if (string.IsNullOrWhiteSpace(DeviceID))
                {
                    Result.ErrorMessage = (Lang == 1) ? "معرف الجهاز مطلوب" : "Device ID is required";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var incomingDeviceId = DeviceID.Trim();
                if (IsDeviceIdUsedByAnotherEmployee(EmployeeID, incomingDeviceId))
                {
                    Result.ErrorMessage = (Lang == 1)
                        ? "هذا الجهاز مستخدم لموظف آخر"
                        : "This device is used for another employee";
                    Result.ErrorCode = 0;
                    return Result;
                }

                 // Get location from hrs_LocationGeoPoints (employee-specific geo points)
                var allLocations = (from geoPoints in _context.hrs_LocationGeoPoints
                                            join sys_Locations in _context.sys_Locations on geoPoints.LocationID equals sys_Locations.ID
                                            join Hrs_Employees in _context.Hrs_Employees
                                               on sys_Locations.ID equals Hrs_Employees.LocationId
                                            where Hrs_Employees.id == EmployeeID
                                            && (geoPoints.Active == null || geoPoints.Active == true)
                                            select new
                                            {
                                                latitude = (double?)geoPoints.Latitude,
                                                longitude = (double?)geoPoints.Longitude,
                                                allowedRadius = (double?)geoPoints.AllowedRadius
                                            }).AsNoTracking().ToList();

                // Ensure connection is closed after reading
                _context.Database.CloseConnection();

                if (allLocations == null || !allLocations.Any())
                {
                    Result.ErrorMessage = (Lang == 1) ? "لم يتم العثور على موقع افتراضي للموظف" : "No default location found for employee";
                    Result.ErrorCode = 0;
                    return Result;
                }

                // Loop through all locations to check if employee is within range of any
                bool isWithinAnyRange = false;
                double? closestDistance = null;
                double? matchedAllowedRadius = null;

                foreach (var location in allLocations)
                {
                    if (location.latitude == null || location.longitude == null || location.allowedRadius == null)
                        continue;

                    if (!double.TryParse(location.latitude.ToString(), out double allowedLatitude) ||
                        !double.TryParse(location.longitude.ToString(), out double allowedLongitude) ||
                        !double.TryParse(location.allowedRadius.ToString(), out double allowedRadius))
                        continue;

                    double distance = CalculateDistance(Latitude, longitude, allowedLatitude, allowedLongitude);

                    // Track the closest distance for reporting
                    if (closestDistance == null || distance < closestDistance)
                    {
                        closestDistance = distance;
                        matchedAllowedRadius = allowedRadius;
                    }

                    // If within range of this location, allow check-in
                    if (distance <= allowedRadius)
                    {
                        isWithinAnyRange = true;
                        matchedAllowedRadius = allowedRadius;
                        closestDistance = distance;
                        break; // Found a valid location, no need to check others
                    }
                }

                if (isWithinAnyRange)
                {
                     var attendanceRecord = new Hrs_Mobile_Attendance
                    {
                        EmployeeID = EmployeeID,
                        CheckingDatetime = CheckingDatetime,
                        Latitude = Latitude,
                        Longitude = longitude,
                        DeviceID = DeviceID,
                        DeviceModel = deviceModel,
                        OSVersion = osVersion,
                        NetworkType = networkType,
                        DistanceFromLocation = closestDistance ?? 0,
                         CreatedDate = DateTime.Now,
                         CheckType=CheckType
                    };

                    _context.Hrs_Mobile_Attendance.Add(attendanceRecord);
                    _context.SaveChanges();

                    var successMessage = isCheckOut
                        ? ((Lang == 1) ? "تم تسجيل المغادرة بنجاح" : "Check-out successful")
                        : ((Lang == 1) ? "تم تسجيل الحضور بنجاح" : "Check-in successful");

                    Result.ResultObject = new
                    {
                        CheckInId = attendanceRecord.ID,
                        CheckInTime = CheckingDatetime,
                        Distance = Math.Round(closestDistance ?? 0, 2),
                        Message = successMessage,
                        IsWithinRange = true
                    };
                    Result.ErrorMessage = successMessage;
                    Result.ErrorCode = 1;
                }
                else
                {
                     Result.ResultObject = new
                    {
                        IsWithinRange = false,
                        AllowedRadius = matchedAllowedRadius ?? 0,
                        ActualDistance = Math.Round(closestDistance ?? 0, 2),
                        Message = (Lang == 1) ?
                            $"أنت خارج نطاق الموقع المسموح. المسافة: {Math.Round(closestDistance ?? 0, 2)} متر (المسموح: {matchedAllowedRadius ?? 0} متر)" :
                            $"You are outside the allowed location range. Distance: {Math.Round(closestDistance ?? 0, 2)} meters (Allowed: {matchedAllowedRadius ?? 0} meters)"
                    };
                    Result.ErrorMessage = (Lang == 1) ?
                        $"أنت خارج نطاق الموقع المسموح. المسافة: {Math.Round(closestDistance ?? 0, 2)} متر" :
                        $"You are outside the allowed location range. Distance: {Math.Round(closestDistance ?? 0, 2)} meters";
                    Result.ErrorCode = 0;
                }
            }
            catch (Exception ex)
            {
                Result.ErrorMessage = isCheckOut
                    ? ((Lang == 1) ? "حدث خطأ أثناء تسجيل المغادرة" : "An error occurred during check-out")
                    : ((Lang == 1) ? "حدث خطأ أثناء تسجيل الحضور" : "An error occurred during check-in");
                Result.ErrorCode = 0;
                 Console.WriteLine($"CheckIn Error: {ex.Message}");
            }

            return Result;
        }

        public object ImportFingerprintUsers(List<hrs_Fingerprint_Users> users, int Lang)
        {
            Result = new GeneralOutputClass<object>();

            try
            {
                if (users == null || users.Count == 0)
                {
                    Result.ErrorMessage = (Lang == 1) ? "القائمة فارغة" : "Empty users list";
                    Result.ErrorCode = 0;
                    Result.ResultObject = new { InsertedRows = 0, SkippedDuplicates = 0 };
                    return Result;
                }

                var invalidRow = users.FirstOrDefault(u => u == null || u.USERID <= 0 || string.IsNullOrWhiteSpace(u.BADGENUMBER));
                if (invalidRow != null)
                {
                    Result.ErrorMessage = (Lang == 1) ? "بيانات المستخدم غير صحيحة (USERID/BADGENUMBER)" : "Invalid user data (USERID/BADGENUMBER)";
                    Result.ErrorCode = 0;
                    Result.ResultObject = new { InsertedRows = 0, SkippedDuplicates = 0 };
                    return Result;
                }

                var distinctUsers = users
                    .GroupBy(u => new { u.USERID, Badge = u.BADGENUMBER.Trim() })
                    .Select(g =>
                    {
                        var item = g.First();
                        item.BADGENUMBER = item.BADGENUMBER.Trim();
                        return item;
                    })
                    .ToList();

                var keys = distinctUsers.Select(u => new { u.USERID, u.BADGENUMBER }).ToList();

                var existingKeys = _context.hrs_Fingerprint_Users
                    .Where(x => keys.Any(k => k.USERID == x.USERID && k.BADGENUMBER == x.BADGENUMBER))
                    .Select(x => new { x.USERID, x.BADGENUMBER })
                    .ToList();

                var toInsert = distinctUsers
                    .Where(u => !existingKeys.Any(e => e.USERID == u.USERID && e.BADGENUMBER == u.BADGENUMBER))
                    .ToList();

                var skipped = distinctUsers.Count - toInsert.Count;

                if (toInsert.Count == 0)
                {
                    Result.ErrorMessage = (Lang == 1) ? "لا توجد سجلات جديدة للإضافة" : "No new rows to insert";
                    Result.ErrorCode = 1;
                    Result.ResultObject = new { InsertedRows = 0, SkippedDuplicates = skipped };
                    return Result;
                }

                using var transaction = _context.Database.BeginTransaction();

                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT hrs_Fingerprint_Users ON");
                _context.hrs_Fingerprint_Users.AddRange(toInsert);
                var insertedRows = _context.SaveChanges();
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT hrs_Fingerprint_Users OFF");

                transaction.Commit();

                Result.ErrorMessage = (Lang == 1) ? "تمت الإضافة بنجاح" : "Inserted successfully";
                Result.ErrorCode = 1;
                Result.ResultObject = new { InsertedRows = insertedRows, SkippedDuplicates = skipped };
            }
            catch (Exception ex)
            {
                Result.ErrorMessage = (Lang == 1) ? "حدث خطأ أثناء الاستيراد" : "Error during import";
                Result.ErrorCode = 0;
                Result.ResultObject = new { InsertedRows = 0, SkippedDuplicates = 0, Error = ex.Message };
            }

            return Result;
        }

        public object ImportCheckInOut(List<hrs_Fingerprint_CheckInOut> records, int Lang)
        {
            Result = new GeneralOutputClass<object>();

            try
            {
                if (records == null || records.Count == 0)
                {
                    Result.ErrorMessage = (Lang == 1) ? "القائمة فارغة" : "Empty records list";
                    Result.ErrorCode = 0;
                    Result.ResultObject = new { InsertedRows = 0, SkippedDuplicates = 0 };
                    return Result;
                }

                var invalidRow = records.FirstOrDefault(r => r == null || r.USERID <= 0 || r.CHECKTIME == default);
                if (invalidRow != null)
                {
                    Result.ErrorMessage = (Lang == 1) ? "بيانات السجل غير صحيحة (USERID/CHECKTIME)" : "Invalid record data (USERID/CHECKTIME)";
                    Result.ErrorCode = 0;
                    Result.ResultObject = new { InsertedRows = 0, SkippedDuplicates = 0 };
                    return Result;
                }

                var distinctRecords = records
                    .GroupBy(r => new { r.USERID, r.CHECKTIME })
                    .Select(g => g.First())
                    .ToList();

                var keys = distinctRecords.Select(r => new { r.USERID, r.CHECKTIME }).ToList();

                var existingKeys = _context.hrs_Fingerprint_CheckInOut
                    .Where(x => keys.Any(k => k.USERID == x.USERID && k.CHECKTIME == x.CHECKTIME))
                    .Select(x => new { x.USERID, x.CHECKTIME })
                    .ToList();

                var toInsert = distinctRecords
                    .Where(r => !existingKeys.Any(e => e.USERID == r.USERID && e.CHECKTIME == r.CHECKTIME))
                    .Select(r =>
                    {
                        r.Auto_Date = DateTime.Now;
                        return r;
                    })
                    .ToList();

                var skipped = distinctRecords.Count - toInsert.Count;

                if (toInsert.Count == 0)
                {
                    Result.ErrorMessage = (Lang == 1) ? "لا توجد سجلات جديدة للإضافة" : "No new rows to insert";
                    Result.ErrorCode = 1;
                    Result.ResultObject = new { InsertedRows = 0, SkippedDuplicates = skipped };
                    return Result;
                }

                _context.hrs_Fingerprint_CheckInOut.AddRange(toInsert);
                var insertedRows = _context.SaveChanges();

                Result.ErrorMessage = (Lang == 1) ? "تمت الإضافة بنجاح" : "Inserted successfully";
                Result.ErrorCode = 1;
                Result.ResultObject = new { InsertedRows = insertedRows, SkippedDuplicates = skipped };
            }
            catch (Exception ex)
            {
                Result.ErrorMessage = (Lang == 1) ? "حدث خطأ أثناء الاستيراد" : "Error during import";
                Result.ErrorCode = 0;
                Result.ResultObject = new { InsertedRows = 0, SkippedDuplicates = 0, Error = ex.Message };
            }

            return Result;
        }

        public object GetAttendanceHistory(int EmployeeID, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                 var startDate = FromDate ?? DateTime.Now.AddMonths(-1);  
                var endDate = ToDate ?? DateTime.Now;

                 var allRecords = _context.Hrs_Mobile_Attendance
                    .Where(a => a.EmployeeID == EmployeeID &&
                               a.CheckingDatetime >= startDate &&
                               a.CheckingDatetime <= endDate)
                    .OrderBy(a => a.CheckingDatetime)
                    .ToList();

                if (!allRecords.Any())
                {
                    Result.ErrorMessage = "No attendance records found";
                    Result.ErrorCode = 1;
                    Result.ResultObject = new { Records = new List<object>() };
                    return Result;
                }

                var (expectedStartTime, graceMinutes) = GetEmployeeWorkSchedule(EmployeeID, endDate);

                // تجميع السجلات حسب اليوم
                var dailyGroups = allRecords
                    .GroupBy(a => a.CheckingDatetime.Date)
                    .OrderBy(g => g.Key)
                    .ToList();

                var dailyAttendance = new List<object>();

                foreach (var dayGroup in dailyGroups)
                {
                    var dayRecords = dayGroup.OrderBy(a => a.CheckingDatetime).ToList();

                     var firstRecord = dayRecords.FirstOrDefault(a =>
                        a.CheckType?.ToUpper() == "IN");

                     var lastRecord = dayRecords.LastOrDefault(a =>
                        a.CheckType?.ToUpper() == "OUT");

                     if (firstRecord == null)
                        firstRecord = dayRecords.FirstOrDefault();

                    if (lastRecord == null)
                        lastRecord = dayRecords.LastOrDefault();

                    // حساب ساعات العمل
                    TimeSpan? workingHours = null;
                    if (firstRecord != null && lastRecord != null && firstRecord != lastRecord)
                    {
                        workingHours = lastRecord.CheckingDatetime - firstRecord.CheckingDatetime;
                    }

                    var dailyRecord = new
                    {
                        Date = dayGroup.Key.ToString("yyyy-MM-dd"),
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
                        Status = GetDayStatus(firstRecord, lastRecord, expectedStartTime, graceMinutes),
                        TotalRecords = dayRecords.Count
                    };

                    dailyAttendance.Add(dailyRecord);
                }

                 var stats = new
                {
                    TotalDays = dailyGroups.Count,
                    PresentDays = dailyAttendance.Count(d => ((dynamic)d).Status == "Present"),
                    LateDays = dailyAttendance.Count(d => ((dynamic)d).Status == "Late"),
                    PartialDays = dailyAttendance.Count(d => ((dynamic)d).Status == "Partial"),
                    TotalWorkingHours = CalculateTotalWorkingHours(dailyAttendance)
                };

                Result.ResultObject = new
                {
                    EmployeeID = EmployeeID,
                    Period = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
                    DailyAttendance = dailyAttendance,
                    Statistics = stats,
                    TotalRecords = allRecords.Count
                };

                Result.ErrorMessage = "Attendance history retrieved successfully";
                Result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                Result.ErrorMessage = $"Error retrieving attendance history: {ex.Message}";
                Result.ErrorCode = 0;
                Console.WriteLine($"GetAttendavceHistory Error: {ex.Message}");
            }

            return Result;
        }

        public object GetManagerTeamAttendance(int ManagerID, DateTime? Date, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                var manager = _context.Hrs_Employees.AsNoTracking().FirstOrDefault(e => e.id == ManagerID);
                if (manager == null)
                {
                    Result.ErrorMessage = (Lang == 1) ? "الموظف غير موجود" : "Employee not found";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var now = DateTime.Now;
                var day = (Date ?? now).Date;

                var employeesQuery = _context.Hrs_Employees.Where(e => e.ManagerId == ManagerID);
                var employees = BuildDailyAttendanceRows(employeesQuery, day, Lang);

                Result.ResultObject = new
                {
                    Date = day.ToString("yyyy-MM-dd"),
                    ManagerID = manager.id,
                    ManagerName = Lang == 1
                        ? JoinName(manager.ArbName, manager.FamilyArbName)
                        : JoinName(manager.EngName, manager.FamilyEngName),
                    AsOfTime = now.ToString("HH:mm"),
                    TeamCount = employees.Count,
                    CheckedInCount = employees.Count(e => e.Status != "NotCheckedIn"),
                    AtWorkCount = employees.Count(e => e.Status == "AtWork"),
                    CheckedOutCount = employees.Count(e => e.Status == "CheckedOut"),
                    NotCheckedInCount = employees.Count(e => e.Status == "NotCheckedIn"),
                    Employees = employees
                };
                Result.ErrorMessage = (Lang == 1) ? "تم جلب حضور فريق المدير بنجاح" : "Team attendance retrieved successfully";
                Result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                Result.ErrorMessage = (Lang == 1) ? "حدث خطأ أثناء جلب حضور الفريق" : "Error retrieving team attendance";
                Result.ErrorCode = 0;
                Console.WriteLine($"GetManagerTeamAttendance Error: {ex.Message}");
            }

            return Result;
        }

        public object GetManagerTeamAttendanceByTime(int ManagerID, DateTime? Date, string? Time, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (ManagerID <= 0)
                {
                    Result.ErrorMessage = (Lang == 1) ? "معرف المدير غير صحيح" : "Invalid manager ID";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var manager = _context.Hrs_Employees.AsNoTracking().FirstOrDefault(e => e.id == ManagerID);
                if (manager == null)
                {
                    Result.ErrorMessage = (Lang == 1) ? "الموظف غير موجود" : "Employee not found";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var now = DateTime.Now;
                var day = (Date ?? now).Date;

                TimeSpan asOfTime;
                if (string.IsNullOrWhiteSpace(Time))
                {
                    asOfTime = now.TimeOfDay;
                }
                else if (!TimeSpan.TryParse(Time.Trim(), out asOfTime)
                         || asOfTime < TimeSpan.Zero
                         || asOfTime >= TimeSpan.FromDays(1))
                {
                    Result.ErrorMessage = (Lang == 1) ? "الوقت غير صالح. استخدم صيغة مثل 14:30" : "Invalid time. Use format like 14:30";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var employeesQuery = _context.Hrs_Employees.Where(e => e.ManagerId == ManagerID);
                var employees = BuildDailyAttendanceRows(employeesQuery, day, Lang, asOfTime);

                Result.ResultObject = new
                {
                    Date = day.ToString("yyyy-MM-dd"),
                    ManagerID = manager.id,
                    ManagerName = Lang == 1
                        ? JoinName(manager.ArbName, manager.FamilyArbName)
                        : JoinName(manager.EngName, manager.FamilyEngName),
                    AsOfTime = $"{asOfTime.Hours:D2}:{asOfTime.Minutes:D2}",
                    TeamCount = employees.Count,
                    CheckedInCount = employees.Count(e => e.Status != "NotCheckedIn"),
                    AtWorkCount = employees.Count(e => e.Status == "AtWork"),
                    CheckedOutCount = employees.Count(e => e.Status == "CheckedOut"),
                    NotCheckedInCount = employees.Count(e => e.Status == "NotCheckedIn"),
                    Employees = employees
                };
                Result.ErrorMessage = (Lang == 1) ? "تم جلب حضور فريق المدير بنجاح" : "Team attendance retrieved successfully";
                Result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                Result.ErrorMessage = (Lang == 1) ? "حدث خطأ أثناء جلب حضور الفريق" : "Error retrieving team attendance";
                Result.ErrorCode = 0;
                Console.WriteLine($"GetManagerTeamAttendanceByTime Error: {ex.Message}");
            }

            return Result;
        }

        public object GetAllEmployeesAttendance(int RequesterID, DateTime? Date, int? DepartmentID, int Lang)
        {
            return BuildAllEmployeesAttendance(RequesterID, Date, null, DepartmentID, Lang, useAsOfTimeFilter: false);
        }

        public object GetAllEmployeesAttendanceByTime(int RequesterID, DateTime? Date, string? Time, int? DepartmentID, int Lang)
        {
            return BuildAllEmployeesAttendance(RequesterID, Date, Time, DepartmentID, Lang, useAsOfTimeFilter: true);
        }

        private object BuildAllEmployeesAttendance(
            int RequesterID,
            DateTime? Date,
            string? Time,
            int? DepartmentID,
            int Lang,
            bool useAsOfTimeFilter)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                var requester = _context.Hrs_Employees.AsNoTracking().FirstOrDefault(e => e.id == RequesterID);
                if (requester == null)
                {
                    Result.ErrorMessage = (Lang == 1) ? "الموظف غير موجود" : "Employee not found";
                    Result.ErrorCode = 0;
                    return Result;
                }

                if (requester.IsHRTeam != true)
                {
                    Result.ErrorMessage = (Lang == 1) ? "غير مسموح لك بعرض حضور كل الموظفين" : "You are not allowed to view all employees attendance";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var now = DateTime.Now;
                var day = (Date ?? now).Date;

                TimeSpan asOfTime;
                if (string.IsNullOrWhiteSpace(Time))
                {
                    asOfTime = now.TimeOfDay;
                }
                else if (!TimeSpan.TryParse(Time.Trim(), out asOfTime)
                         || asOfTime < TimeSpan.Zero
                         || asOfTime >= TimeSpan.FromDays(1))
                {
                    Result.ErrorMessage = (Lang == 1) ? "الوقت غير صالح. استخدم صيغة مثل 14:30" : "Invalid time. Use format like 14:30";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var employeesQuery = _context.Hrs_Employees.AsQueryable();
                string scope = Lang == 1 ? "كل الإدارات" : "All departments";
                if (DepartmentID.HasValue && DepartmentID.Value > 0)
                {
                    employeesQuery = employeesQuery.Where(e => e.DepartmentId == DepartmentID.Value);
                    var department = _context.sys_Departments.AsNoTracking().FirstOrDefault(d => d.ID == DepartmentID.Value);
                    scope = (Lang == 1 ? department?.ArbName : department?.EngName) ?? scope;
                }

                var rows = BuildDailyAttendanceRows(
                    employeesQuery,
                    day,
                    Lang,
                    useAsOfTimeFilter ? asOfTime : null);

                var atWorkCount = rows.Count(r => r.Status == "AtWork");
                var checkedOutCount = rows.Count(r => r.Status == "CheckedOut");
                var notCheckedInCount = rows.Count(r => r.Status == "NotCheckedIn");
                var checkedInCount = rows.Count(r => r.Status != "NotCheckedIn");

                var departments = rows
                    .GroupBy(r => new { r.DepartmentID, r.DepartmentCode, r.Department })
                    .OrderBy(g => g.Key.DepartmentID == null)
                    .ThenBy(g => g.Key.DepartmentCode)
                    .ThenBy(g => g.Key.DepartmentID)
                    .Select(g => new
                    {
                        DepartmentID = g.Key.DepartmentID,
                        DepartmentCode = g.Key.DepartmentCode,
                        Department = g.Key.Department ?? (Lang == 1 ? "بدون إدارة" : "No department"),
                        EmployeesCount = g.Count(),
                        CheckedInCount = g.Count(r => r.Status != "NotCheckedIn"),
                        AtWorkCount = g.Count(r => r.Status == "AtWork"),
                        CheckedOutCount = g.Count(r => r.Status == "CheckedOut"),
                        NotCheckedInCount = g.Count(r => r.Status == "NotCheckedIn"),
                        Employees = g.Select(e => new
                        {
                            e.EmployeeID,
                            e.EmployeeCode,
                            e.EmployeeName,
                            e.FirstCheckIn,
                            e.LastCheckOut,
                            e.Status,
                            e.StatusText
                        }).ToList()
                    })
                    .ToList();

                Result.ResultObject = new
                {
                    ReportType = Lang == 1 ? "تقرير يومي حسب الإدارات" : "Daily report by department",
                    Date = day.ToString("yyyy-MM-dd"),
                    DisplayDate = day.ToString("dd/MM/yyyy"),
                    DayName = GetDayName(day.DayOfWeek, Lang),
                    Scope = scope,
                    AsOfTime = $"{asOfTime.Hours:D2}:{asOfTime.Minutes:D2}",
                    TotalEmployees = rows.Count,
                    CheckedInCount = checkedInCount,
                    AtWorkCount = atWorkCount,
                    CheckedOutCount = checkedOutCount,
                    NotCheckedInCount = notCheckedInCount,
                    Summary = new
                    {
                        TotalEmployees = rows.Count,
                        AtWork = atWorkCount,
                        CheckedOut = checkedOutCount,
                        NotCheckedIn = notCheckedInCount
                    },
                    Departments = departments
                };
                Result.ErrorMessage = (Lang == 1) ? "تم جلب حضور الموظفين بنجاح" : "Employees attendance retrieved successfully";
                Result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                Result.ErrorMessage = (Lang == 1) ? "حدث خطأ أثناء جلب حضور الموظفين" : "Error retrieving employees attendance";
                Result.ErrorCode = 0;
                Console.WriteLine($"GetAllEmployeesAttendance Error: {ex.Message}");
            }

            return Result;
        }

        private static string GetDayName(DayOfWeek dayOfWeek, int Lang) => Lang == 1
            ? dayOfWeek switch
            {
                DayOfWeek.Saturday => "السبت",
                DayOfWeek.Sunday => "الأحد",
                DayOfWeek.Monday => "الإثنين",
                DayOfWeek.Tuesday => "الثلاثاء",
                DayOfWeek.Wednesday => "الأربعاء",
                DayOfWeek.Thursday => "الخميس",
                DayOfWeek.Friday => "الجمعة",
                _ => string.Empty
            }
            : dayOfWeek.ToString();

        public object GetAllEmployeesPeriodAttendance(
            int RequesterID,
            DateTime FromDate,
            DateTime ToDate,
            int? DepartmentID,
            int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                var requester = _context.Hrs_Employees.AsNoTracking().FirstOrDefault(e => e.id == RequesterID);
                if (requester == null)
                {
                    Result.ErrorMessage = (Lang == 1) ? "الموظف غير موجود" : "Employee not found";
                    Result.ErrorCode = 0;
                    return Result;
                }

                if (requester.IsHRTeam != true)
                {
                    Result.ErrorMessage = (Lang == 1) ? "غير مسموح لك بعرض حضور كل الموظفين" : "You are not allowed to view all employees attendance";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var from = FromDate.Date;
                var to = ToDate.Date;
                if (from > to)
                {
                    Result.ErrorMessage = (Lang == 1) ? "تاريخ البداية يجب أن يكون قبل تاريخ النهاية" : "From date must be before to date";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var workingDays = GetWorkingDays(from, to);
                var workDaysCount = workingDays.Count;
                var rangeEndExclusive = to.AddDays(1);

                var employeesQuery = _context.Hrs_Employees
                    .Where(e => e.CancelDate == null && (e.ExcludeDate == null || e.ExcludeDate >= from));

                string scope = Lang == 1 ? "كل الإدارات" : "All departments";
                if (DepartmentID.HasValue && DepartmentID.Value > 0)
                {
                    employeesQuery = employeesQuery.Where(e => e.DepartmentId == DepartmentID.Value);
                    var department = _context.sys_Departments.AsNoTracking().FirstOrDefault(d => d.ID == DepartmentID.Value);
                    scope = (Lang == 1 ? department?.ArbName : department?.EngName) ?? scope;
                }

                var employees = (from emp in employeesQuery
                                 join dep in _context.sys_Departments on emp.DepartmentId equals dep.ID into deps
                                 from dep in deps.DefaultIfEmpty()
                                 orderby emp.Code
                                 select new
                                 {
                                     emp.id,
                                     emp.Code,
                                     emp.ArbName,
                                     emp.FatherArbName,
                                     emp.FamilyArbName,
                                     emp.EngName,
                                     emp.FatherEngName,
                                     emp.FamilyEngName,
                                     DepartmentID = dep != null ? (int?)dep.ID : null,
                                     DepartmentCode = dep != null ? dep.Code : null,
                                     DepartmentArbName = dep != null ? dep.ArbName : null,
                                     DepartmentEngName = dep != null ? dep.EngName : null
                                 })
                                .AsNoTracking()
                                .ToList();

                var employeeIds = employees.Select(e => e.id).ToList();
                var punches = _context.Hrs_Mobile_Attendance
                    .AsNoTracking()
                    .Where(a => employeeIds.Contains(a.EmployeeID)
                        && a.CheckingDatetime >= from
                        && a.CheckingDatetime < rangeEndExclusive)
                    .Select(a => new { a.EmployeeID, a.CheckingDatetime, a.CheckType })
                    .ToList()
                    .GroupBy(a => a.EmployeeID)
                    .ToDictionary(
                        g => g.Key,
                        g => g.GroupBy(x => x.CheckingDatetime.Date)
                              .ToDictionary(dg => dg.Key, dg => dg.OrderBy(x => x.CheckingDatetime).ToList()));

                var rows = new List<PeriodAttendanceEmployeeDto>();

                foreach (var emp in employees)
                {
                    punches.TryGetValue(emp.id, out var byDay);
                    byDay ??= new();

                    int completed = 0;
                    int missingCheckout = 0;
                    int noMovement = 0;
                    long totalMinutes = 0;

                    foreach (var day in workingDays)
                    {
                        if (!byDay.TryGetValue(day, out var dayRecords) || dayRecords.Count == 0)
                        {
                            noMovement++;
                            continue;
                        }

                        var firstIn = dayRecords.FirstOrDefault(r =>
                                          string.Equals(r.CheckType?.Trim(), "IN", StringComparison.OrdinalIgnoreCase))
                                      ?? dayRecords.FirstOrDefault();
                        var lastOut = dayRecords.LastOrDefault(r =>
                            string.Equals(r.CheckType?.Trim(), "OUT", StringComparison.OrdinalIgnoreCase));

                        if (firstIn != null && lastOut != null && lastOut.CheckingDatetime > firstIn.CheckingDatetime)
                        {
                            completed++;
                            totalMinutes += (long)(lastOut.CheckingDatetime - firstIn.CheckingDatetime).TotalMinutes;
                        }
                        else if (firstIn != null)
                        {
                            missingCheckout++;
                        }
                        else
                        {
                            noMovement++;
                        }
                    }

                    rows.Add(new PeriodAttendanceEmployeeDto
                    {
                        EmployeeID = emp.id,
                        EmployeeCode = emp.Code,
                        EmployeeName = Lang == 1
                            ? JoinName(emp.ArbName, emp.FatherArbName, emp.FamilyArbName)
                            : JoinName(emp.EngName, emp.FatherEngName, emp.FamilyEngName),
                        DepartmentID = emp.DepartmentID,
                        DepartmentCode = emp.DepartmentCode,
                        Department = Lang == 1 ? emp.DepartmentArbName : emp.DepartmentEngName,
                        WorkDays = workDaysCount,
                        CompletedDays = completed,
                        MissingCheckoutDays = missingCheckout,
                        NoMovementDays = noMovement,
                        TotalDurationMinutes = totalMinutes,
                        TotalDuration = FormatDuration(totalMinutes)
                    });
                }

                var totalCompleted = rows.Sum(r => r.CompletedDays);
                var totalMissingCheckout = rows.Sum(r => r.MissingCheckoutDays);
                var totalNoMovement = rows.Sum(r => r.NoMovementDays);
                var totalMinutesAll = rows.Sum(r => r.TotalDurationMinutes);

                var departments = rows
                    .GroupBy(r => new { r.DepartmentID, r.DepartmentCode, r.Department })
                    .OrderBy(g => g.Key.DepartmentID == null)
                    .ThenBy(g => g.Key.DepartmentCode)
                    .ThenBy(g => g.Key.DepartmentID)
                    .Select(g => new
                    {
                        DepartmentID = g.Key.DepartmentID,
                        DepartmentCode = g.Key.DepartmentCode,
                        Department = g.Key.Department ?? (Lang == 1 ? "بدون إدارة" : "No department"),
                        EmployeesCount = g.Count(),
                        Employees = g.Select(e => new
                        {
                            e.EmployeeID,
                            e.EmployeeCode,
                            e.EmployeeName,
                            e.WorkDays,
                            e.CompletedDays,
                            e.MissingCheckoutDays,
                            e.NoMovementDays,
                            e.TotalDuration,
                            e.TotalDurationMinutes
                        }).ToList()
                    })
                    .ToList();

                Result.ResultObject = new
                {
                    ReportType = Lang == 1 ? "ملخص فترة - سطر لكل موظف" : "Period summary - one line per employee",
                    FromDate = from.ToString("yyyy-MM-dd"),
                    ToDate = to.ToString("yyyy-MM-dd"),
                    DisplayPeriod = $"{from:dd/MM/yyyy} – {to:dd/MM/yyyy}",
                    Scope = scope,
                    TotalEmployees = rows.Count,
                    WorkingDays = workDaysCount,
                    Summary = new
                    {
                        CompletedDays = totalCompleted,
                        MissingCheckoutDays = totalMissingCheckout,
                        NoMovementDays = totalNoMovement,
                        TotalAttendanceHours = (int)(totalMinutesAll / 60),
                        TotalDuration = FormatDuration(totalMinutesAll),
                        TotalDurationMinutes = totalMinutesAll
                    },
                    Departments = departments
                };
                Result.ErrorMessage = (Lang == 1) ? "تم جلب ملخص الفترة بنجاح" : "Period attendance summary retrieved successfully";
                Result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                Result.ErrorMessage = (Lang == 1) ? "حدث خطأ أثناء جلب ملخص الفترة" : "Error retrieving period attendance summary";
                Result.ErrorCode = 0;
                Console.WriteLine($"GetAllEmployeesPeriodAttendance Error: {ex.Message}");
            }

            return Result;
        }

        private static List<DateTime> GetWorkingDays(DateTime from, DateTime to)
        {
            var days = new List<DateTime>();
            for (var d = from.Date; d <= to.Date; d = d.AddDays(1))
            {
                // Friday + Saturday weekend (common MENA calendar)
                if (d.DayOfWeek == DayOfWeek.Friday || d.DayOfWeek == DayOfWeek.Saturday)
                    continue;
                days.Add(d);
            }
            return days;
        }

        private static string FormatDuration(long totalMinutes)
        {
            if (totalMinutes < 0) totalMinutes = 0;
            var hours = totalMinutes / 60;
            var minutes = totalMinutes % 60;
            return $"{hours}:{minutes:D2}";
        }

        private static string FormatTime12(DateTime dt, int Lang)
        {
            var hour24 = dt.Hour;
            var minute = dt.Minute;
            var isPm = hour24 >= 12;
            var hour12 = hour24 % 12;
            if (hour12 == 0) hour12 = 12;

            if (Lang == 1)
                return $"{hour12:D2}:{minute:D2} {(isPm ? "م" : "ص")}";

            return $"{hour12:D2}:{minute:D2} {(isPm ? "PM" : "AM")}";
        }

        public object GetEmployeePeriodAttendanceDetail(int EmployeeID, DateTime FromDate, DateTime ToDate, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (EmployeeID <= 0)
                {
                    Result.ErrorMessage = (Lang == 1) ? "معرف الموظف غير صحيح" : "Invalid employee ID";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var from = FromDate.Date;
                var to = ToDate.Date;
                if (from > to)
                {
                    Result.ErrorMessage = (Lang == 1) ? "تاريخ البداية يجب أن يكون قبل تاريخ النهاية" : "From date must be before to date";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var employee = (from emp in _context.Hrs_Employees.AsNoTracking()
                                join dep in _context.sys_Departments on emp.DepartmentId equals dep.ID into deps
                                from dep in deps.DefaultIfEmpty()
                                where emp.id == EmployeeID
                                select new
                                {
                                    emp.id,
                                    emp.Code,
                                    emp.ArbName,
                                    emp.FatherArbName,
                                    emp.FamilyArbName,
                                    emp.EngName,
                                    emp.FatherEngName,
                                    emp.FamilyEngName,
                                    emp.ManagerId,
                                    DepartmentID = dep != null ? (int?)dep.ID : null,
                                    DepartmentCode = dep != null ? dep.Code : null,
                                    DepartmentArbName = dep != null ? dep.ArbName : null,
                                    DepartmentEngName = dep != null ? dep.EngName : null
                                }).FirstOrDefault();

                if (employee == null)
                {
                    Result.ErrorMessage = (Lang == 1) ? "الموظف غير موجود" : "Employee not found";
                    Result.ErrorCode = 0;
                    return Result;
                }

                string? managerName = null;
                if (employee.ManagerId.HasValue && employee.ManagerId.Value > 0)
                {
                    var manager = _context.Hrs_Employees.AsNoTracking()
                        .Where(m => m.id == employee.ManagerId.Value)
                        .Select(m => new
                        {
                            m.ArbName,
                            m.FatherArbName,
                            m.FamilyArbName,
                            m.EngName,
                            m.FatherEngName,
                            m.FamilyEngName
                        })
                        .FirstOrDefault();

                    if (manager != null)
                    {
                        managerName = Lang == 1
                            ? JoinName(manager.ArbName, manager.FatherArbName, manager.FamilyArbName)
                            : JoinName(manager.EngName, manager.FatherEngName, manager.FamilyEngName);
                    }
                }

                var rangeEndExclusive = to.AddDays(1);
                var punchesByDay = _context.Hrs_Mobile_Attendance
                    .AsNoTracking()
                    .Where(a => a.EmployeeID == EmployeeID
                        && a.CheckingDatetime >= from
                        && a.CheckingDatetime < rangeEndExclusive)
                    .Select(a => new { a.CheckingDatetime, a.CheckType })
                    .ToList()
                    .GroupBy(a => a.CheckingDatetime.Date)
                    .ToDictionary(g => g.Key, g => g.OrderBy(x => x.CheckingDatetime).ToList());

                int completedDays = 0;
                int missingCheckoutDays = 0;
                int noMovementDays = 0;
                long totalCompletedMinutes = 0;

                var days = new List<object>();

                for (var day = from; day <= to; day = day.AddDays(1))
                {
                    bool isWeeklyRest = day.DayOfWeek == DayOfWeek.Friday || day.DayOfWeek == DayOfWeek.Saturday;
                    punchesByDay.TryGetValue(day, out var dayRecords);
                    dayRecords ??= new();

                    string? firstCheckIn = null;
                    string? firstCheckIn24 = null;
                    string? lastCheckOut = null;
                    string? lastCheckOut24 = null;
                    string? duration = null;
                    long? durationMinutes = null;
                    string status;
                    string statusText;

                    if (isWeeklyRest && dayRecords.Count == 0)
                    {
                        status = "WeeklyRest";
                        statusText = Lang == 1 ? "راحة أسبوعية" : "Weekly rest";
                    }
                    else if (dayRecords.Count == 0)
                    {
                        noMovementDays++;
                        status = "NoMovement";
                        statusText = Lang == 1 ? "لا توجد حركات" : "No movements";
                    }
                    else
                    {
                        var firstIn = dayRecords.FirstOrDefault(r =>
                                          string.Equals(r.CheckType?.Trim(), "IN", StringComparison.OrdinalIgnoreCase))
                                      ?? dayRecords.FirstOrDefault();
                        var lastOut = dayRecords.LastOrDefault(r =>
                            string.Equals(r.CheckType?.Trim(), "OUT", StringComparison.OrdinalIgnoreCase));

                        if (firstIn != null)
                        {
                            firstCheckIn24 = firstIn.CheckingDatetime.ToString("HH:mm");
                            firstCheckIn = FormatTime12(firstIn.CheckingDatetime, Lang);
                        }

                        if (firstIn != null && lastOut != null && lastOut.CheckingDatetime > firstIn.CheckingDatetime)
                        {
                            lastCheckOut24 = lastOut.CheckingDatetime.ToString("HH:mm");
                            lastCheckOut = FormatTime12(lastOut.CheckingDatetime, Lang);
                            var spanMinutes = (long)(lastOut.CheckingDatetime - firstIn.CheckingDatetime).TotalMinutes;
                            durationMinutes = spanMinutes;
                            duration = FormatDuration(spanMinutes);

                            // Count completed only on working days for summary cards
                            if (!isWeeklyRest)
                            {
                                completedDays++;
                                totalCompletedMinutes += spanMinutes;
                            }
                            else
                            {
                                totalCompletedMinutes += spanMinutes;
                            }

                            status = "Completed";
                            statusText = Lang == 1 ? "مكتمل" : "Completed";
                        }
                        else if (firstIn != null)
                        {
                            if (!isWeeklyRest) missingCheckoutDays++;
                            status = "MissingCheckout";
                            statusText = Lang == 1 ? "دون انصراف" : "Missing checkout";
                        }
                        else if (isWeeklyRest)
                        {
                            status = "WeeklyRest";
                            statusText = Lang == 1 ? "راحة أسبوعية" : "Weekly rest";
                        }
                        else
                        {
                            noMovementDays++;
                            status = "NoMovement";
                            statusText = Lang == 1 ? "لا توجد حركات" : "No movements";
                        }
                    }

                    days.Add(new
                    {
                        Date = day.ToString("yyyy-MM-dd"),
                        DisplayDate = day.ToString("dd/MM/yyyy"),
                        DayName = GetDayName(day.DayOfWeek, Lang),
                        DayAndDate = $"{GetDayName(day.DayOfWeek, Lang)} {day:dd/MM/yyyy}",
                        IsWeeklyRest = isWeeklyRest,
                        FirstCheckIn = firstCheckIn,
                        FirstCheckIn24 = firstCheckIn24,
                        LastCheckOut = lastCheckOut,
                        LastCheckOut24 = lastCheckOut24,
                        Duration = duration,
                        DurationMinutes = durationMinutes,
                        Status = status,
                        StatusText = statusText
                    });
                }

                var workingDays = GetWorkingDays(from, to).Count;
                var restDays = Enumerable.Range(0, (to - from).Days + 1)
                    .Select(i => from.AddDays(i))
                    .Count(d => d.DayOfWeek == DayOfWeek.Friday || d.DayOfWeek == DayOfWeek.Saturday);
                const int officialHolidays = 0; // reserved until official holiday calendar is available

                Result.ResultObject = new
                {
                    ReportType = Lang == 1 ? "ملخص يومي لسجل الموظف" : "Daily summary of employee record",
                    TimeZoneNote = Lang == 1 ? "التوقيت المحلي (السعودية)" : "Local time (Saudi Arabia)",
                    Employee = new
                    {
                        EmployeeID = employee.id,
                        EmployeeCode = employee.Code,
                        EmployeeName = Lang == 1
                            ? JoinName(employee.ArbName, employee.FatherArbName, employee.FamilyArbName)
                            : JoinName(employee.EngName, employee.FatherEngName, employee.FamilyEngName),
                        DepartmentID = employee.DepartmentID,
                        DepartmentCode = employee.DepartmentCode,
                        Department = Lang == 1 ? employee.DepartmentArbName : employee.DepartmentEngName,
                        ManagerID = employee.ManagerId,
                        ManagerName = managerName
                    },
                    FromDate = from.ToString("yyyy-MM-dd"),
                    ToDate = to.ToString("yyyy-MM-dd"),
                    DisplayPeriod = $"{from:dd/MM/yyyy} – {to:dd/MM/yyyy}",
                    Calendar = new
                    {
                        WorkingDays = workingDays,
                        RestDays = restDays,
                        OfficialHolidays = officialHolidays
                    },
                    Summary = new
                    {
                        CompletedDays = completedDays,
                        MissingCheckoutDays = missingCheckoutDays,
                        NoMovementDays = noMovementDays,
                        TotalDuration = FormatDuration(totalCompletedMinutes),
                        TotalDurationMinutes = totalCompletedMinutes
                    },
                    Days = days
                };
                Result.ErrorMessage = (Lang == 1) ? "تم جلب تفاصيل حضور الموظف بنجاح" : "Employee attendance detail retrieved successfully";
                Result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                Result.ErrorMessage = (Lang == 1) ? "حدث خطأ أثناء جلب تفاصيل حضور الموظف" : "Error retrieving employee attendance detail";
                Result.ErrorCode = 0;
                Console.WriteLine($"GetEmployeePeriodAttendanceDetail Error: {ex.Message}");
            }

            return Result;
        }

        private List<DailyAttendanceRowDto> BuildDailyAttendanceRows(
            IQueryable<Hrs_Employees> employeesQuery,
            DateTime day,
            int Lang,
            TimeSpan? asOfTime = null)
        {
            // Include the full target minute (e.g. Time=14:30 covers up to 14:30:59)
            var dayEnd = asOfTime.HasValue
                ? day.Add(asOfTime.Value).AddMinutes(1)
                : day.AddDays(1);
            var activeEmployees = employeesQuery
                .Where(e => e.CancelDate == null && (e.ExcludeDate == null || e.ExcludeDate >= day));

            var employees = (from emp in activeEmployees
                             join dep in _context.sys_Departments on emp.DepartmentId equals dep.ID into deps
                             from dep in deps.DefaultIfEmpty()
                             orderby emp.Code
                             select new
                             {
                                 emp.id,
                                 emp.Code,
                                 emp.ArbName,
                                 emp.FatherArbName,
                                 emp.FamilyArbName,
                                 emp.EngName,
                                 emp.FatherEngName,
                                 emp.FamilyEngName,
                                 DepartmentID = dep != null ? (int?)dep.ID : null,
                                 DepartmentCode = dep != null ? dep.Code : null,
                                 DepartmentArbName = dep != null ? dep.ArbName : null,
                                 DepartmentEngName = dep != null ? dep.EngName : null
                             })
                            .AsNoTracking()
                            .ToList();

            var punches = (from a in _context.Hrs_Mobile_Attendance
                           join e in activeEmployees on a.EmployeeID equals e.id
                           where a.CheckingDatetime >= day && a.CheckingDatetime < dayEnd
                           select new { a.EmployeeID, a.CheckingDatetime, a.CheckType })
                          .AsNoTracking()
                          .ToList()
                          .GroupBy(a => a.EmployeeID)
                          .ToDictionary(g => g.Key, g => g.OrderBy(a => a.CheckingDatetime).ToList());

            return employees.Select(t =>
            {
                punches.TryGetValue(t.id, out var records);
                records ??= new();

                var firstIn = records.FirstOrDefault(r => string.Equals(r.CheckType?.Trim(), "IN", StringComparison.OrdinalIgnoreCase))
                              ?? records.FirstOrDefault();
                var lastRecord = records.LastOrDefault();
                bool isCheckedOut = lastRecord != null
                    && string.Equals(lastRecord.CheckType?.Trim(), "OUT", StringComparison.OrdinalIgnoreCase);

                string status = firstIn == null ? "NotCheckedIn" : isCheckedOut ? "CheckedOut" : "AtWork";

                return new DailyAttendanceRowDto
                {
                    EmployeeID = t.id,
                    EmployeeCode = t.Code,
                    EmployeeName = Lang == 1
                        ? JoinName(t.ArbName, t.FatherArbName, t.FamilyArbName)
                        : JoinName(t.EngName, t.FatherEngName, t.FamilyEngName),
                    DepartmentID = t.DepartmentID,
                    DepartmentCode = t.DepartmentCode,
                    Department = Lang == 1 ? t.DepartmentArbName : t.DepartmentEngName,
                    FirstCheckIn = firstIn?.CheckingDatetime.ToString("HH:mm"),
                    LastCheckOut = isCheckedOut ? lastRecord!.CheckingDatetime.ToString("HH:mm") : null,
                    Status = status,
                    StatusText = status switch
                    {
                        "AtWork" => Lang == 1 ? "في العمل" : "At work",
                        "CheckedOut" => Lang == 1 ? "منصرف" : "Checked out",
                        _ => Lang == 1 ? "لم يسجل حضوراً" : "Not checked in"
                    }
                };
            }).ToList();
        }

        private static string JoinName(params string?[] parts) =>
            string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p!.Trim()));

        private (TimeSpan? ExpectedStart, int GraceMinutes) GetEmployeeWorkSchedule(int employeeId, DateTime referenceDate)
        {
            var employeeClassId = _context.hrs_Contracts
                .Where(c => c.EmployeeID == employeeId
                    && c.CancelDate == null
                    && c.StartDate <= referenceDate
                    && (c.EndDate == null || c.EndDate >= referenceDate))
                .OrderByDescending(c => c.StartDate)
                .Select(c => c.EmployeeClassID)
                .FirstOrDefault();

            if (employeeClassId == 0)
                return (null, 0);

            var schedule = _context.hrs_EmployeesClasses
                .Where(ec => ec.ID == employeeClassId && ec.CancelDate == null)
                .Select(ec => new { ec.DefultStartTime, ec.PerDailyDelaying })
                .FirstOrDefault();

            if (schedule?.DefultStartTime == null)
                return (null, schedule?.PerDailyDelaying ?? 0);

            return (schedule.DefultStartTime.Value.TimeOfDay, schedule.PerDailyDelaying ?? 0);
        }

        private static string GetDayStatus(
            Hrs_Mobile_Attendance checkIn,
            Hrs_Mobile_Attendance checkOut,
            TimeSpan? expectedStartTime = null,
            int graceMinutes = 0)
        {
            if (checkIn == null && checkOut == null)
                return "Absent";
            else if (checkIn != null && checkOut != null && checkIn != checkOut)
            {
                if (expectedStartTime.HasValue)
                {
                    var allowedCheckIn = expectedStartTime.Value.Add(TimeSpan.FromMinutes(graceMinutes));
                    if (checkIn.CheckingDatetime.TimeOfDay > allowedCheckIn)
                        return "Late";
                }

                return "Present";
            }
            else if (checkIn != null || checkOut != null)
                return "Partial"; // حضور بدون انصراف أو انصراف بدون حضور
            else
                return "Unknown";
        }

        private string CalculateTotalWorkingHours(List<object> dailyAttendance)
        {
            try
            {
                var totalTicks = dailyAttendance
                    .Where(d => ((dynamic)d).WorkingHours != null)
                    .Sum(d =>
                    {
                        var timeStr = (string)((dynamic)d).WorkingHours;
                        if (TimeSpan.TryParse(timeStr, out var timeSpan))
                            return timeSpan.Ticks;
                        return 0;
                    });

                var totalTime = new TimeSpan(totalTicks);
                return totalTime.ToString(@"hh\:mm\:ss");
            }
            catch
            {
                return "00:00:00";
            }
        }


        public object GetRegisteredDevice(int EmployeeID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                var employee = _context.Hrs_Employees.FirstOrDefault(e => e.id == EmployeeID);
                if (employee == null)
                {
                    Result.ErrorMessage = (Lang == 1) ? "الموظف غير موجود" : "Employee not found";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var deviceId = GetEmployeeDeviceId(EmployeeID);

                Result.ResultObject = new
                {
                    EmployeeID = employee.id,
                    EmployeeCode = employee.Code,
                    DeviceID = deviceId,
                    IsDeviceRegistered = !string.IsNullOrWhiteSpace(deviceId)
                };
                Result.ErrorMessage = (Lang == 1) ? "تم جلب بيانات الجهاز بنجاح" : "Device info retrieved successfully";
                Result.ErrorCode = 1;
            }
            catch (Exception)
            {
                Result.ErrorMessage = (Lang == 1) ? "حدث خطأ أثناء جلب بيانات الجهاز" : "An error occurred while retrieving device info";
                Result.ErrorCode = 0;
            }

            return Result;
        }

        public object ChangeDevice(int EmployeeID, string DeviceID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (string.IsNullOrWhiteSpace(DeviceID))
                {
                    Result.ErrorMessage = (Lang == 1) ? "معرف الجهاز مطلوب" : "Device ID is required";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var employee = _context.Hrs_Employees.FirstOrDefault(e => e.id == EmployeeID);
                if (employee == null)
                {
                    Result.ErrorMessage = (Lang == 1) ? "الموظف غير موجود" : "Employee not found";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var previousDeviceId = GetEmployeeDeviceId(EmployeeID);
                var newDeviceId = DeviceID.Trim();
                if (IsDeviceIdUsedByAnotherEmployee(EmployeeID, newDeviceId))
                {
                    Result.ErrorMessage = (Lang == 1)
                        ? "هذا الجهاز مستخدم لموظف آخر"
                        : "This device is used for another employee";
                    Result.ErrorCode = 0;
                    return Result;
                }

                SetEmployeeDeviceId(EmployeeID, newDeviceId);

                Result.ResultObject = new
                {
                    EmployeeID = employee.id,
                    EmployeeCode = employee.Code,
                    PreviousDeviceID = previousDeviceId,
                    NewDeviceID = newDeviceId
                };
                Result.ErrorMessage = (Lang == 1) ? "تم تغيير الجهاز بنجاح" : "Device changed successfully";
                Result.ErrorCode = 1;
            }
            catch (Exception)
            {
                Result.ErrorMessage = (Lang == 1) ? "حدث خطأ أثناء تغيير الجهاز" : "An error occurred while changing device";
                Result.ErrorCode = 0;
            }

            return Result;
        }

        public object ClearDevice(int EmployeeID, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                var employee = _context.Hrs_Employees.FirstOrDefault(e => e.id == EmployeeID);
                if (employee == null)
                {
                    Result.ErrorMessage = (Lang == 1) ? "الموظف غير موجود" : "Employee not found";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var previousDeviceId = GetEmployeeDeviceId(EmployeeID);
                SetEmployeeDeviceId(EmployeeID, null);

                Result.ResultObject = new
                {
                    EmployeeID = employee.id,
                    EmployeeCode = employee.Code,
                    ClearedDeviceID = previousDeviceId
                };
                Result.ErrorMessage = (Lang == 1) ? "تم إلغاء تسجيل الجهاز بنجاح" : "Device registration cleared successfully";
                Result.ErrorCode = 1;
            }
            catch (Exception)
            {
                Result.ErrorMessage = (Lang == 1) ? "حدث خطأ أثناء إلغاء تسجيل الجهاز" : "An error occurred while clearing device registration";
                Result.ErrorCode = 0;
            }

            return Result;
        }

        public object GetEmployeesExpectedStartBeforeTime(int? hour, int? minute, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                var now = DateTime.Now;
                var effectiveHour = hour ?? now.Hour;
                var effectiveMinute = minute ?? now.Minute;

                if (effectiveHour < 0 || effectiveHour > 23 || effectiveMinute < 0 || effectiveMinute > 59)
                {
                    Result.ErrorMessage = (Lang == 1) ? "الوقت غير صالح" : "Invalid time";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var employees = GetExpectedStartEmployeesBeforeTime(effectiveHour, effectiveMinute);
                var targetTime = new TimeSpan(effectiveHour, effectiveMinute, 0).Add(TimeSpan.FromMinutes(5));

                Result.ResultObject = new
                {
                    TimeParameter = $"{effectiveHour:D2}:{effectiveMinute:D2}",
                    EvaluatedUntil = targetTime.ToString(@"hh\:mm"),
                    TotalEmployees = employees.Count,
                    Employees = employees
                };
                Result.ErrorMessage = (Lang == 1) ? "تم جلب الموظفين بنجاح" : "Employees retrieved successfully";
                Result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                Result.ErrorMessage = (Lang == 1) ? "حدث خطأ أثناء جلب الموظفين" : "Error retrieving employees";
                Result.ErrorCode = 0;
                Result.ResultObject = new { Error = ex.Message };
            }

            return Result;
        }

        public List<ExpectedStartEmployeeDto> GetExpectedStartEmployeesBeforeTime(int? hour, int? minute)
        {
            var now = DateTime.Now;
            var effectiveHour = hour ?? now.Hour;
            var effectiveMinute = minute ?? now.Minute;
            var referenceDate = now.Date;
            var targetTime = new TimeSpan(effectiveHour, effectiveMinute, 0).Add(TimeSpan.FromMinutes(5));

            var activeContracts = (from contract in _context.hrs_Contracts
                                   join employee in _context.Hrs_Employees on contract.EmployeeID equals employee.id
                                   join employeeClass in _context.hrs_EmployeesClasses on contract.EmployeeClassID equals employeeClass.ID
                                   where contract.CancelDate == null
                                       && contract.StartDate <= referenceDate
                                       && (contract.EndDate == null || contract.EndDate >= referenceDate)
                                       && employee.CancelDate == null
                                       && employeeClass.CancelDate == null
                                   select new
                                   {
                                       EmployeeId = employee.id,
                                       EmployeeCode = employee.Code,
                                       EmployeeNameAr = employee.ArbName,
                                       EmployeeNameEn = employee.EngName,
                                       ContractStartDate = contract.StartDate,
                                       employeeClass.DefultStartTime,
                                       GraceMinutes = employeeClass.PerDailyDelaying ?? 0
                                   })
                                   .AsNoTracking()
                                   .ToList();

            return activeContracts
                .GroupBy(x => x.EmployeeId)
                .Select(g => g.OrderByDescending(x => x.ContractStartDate).First())
                .Where(x => x.DefultStartTime.HasValue)
                .Select(x =>
                {
                    var expectedStart = x.DefultStartTime!.Value.TimeOfDay;
                    var allowedStart = expectedStart.Add(TimeSpan.FromMinutes(x.GraceMinutes));
                    return new
                    {
                        x.EmployeeId,
                        x.EmployeeCode,
                        EmployeeNameAr = x.EmployeeNameAr ?? string.Empty,
                        EmployeeNameEn = x.EmployeeNameEn ?? string.Empty,
                        ExpectedStartTime = expectedStart.ToString(@"hh\:mm"),
                        GraceMinutes = x.GraceMinutes,
                        AllowedStartTime = allowedStart.ToString(@"hh\:mm"),
                        IsMatched = allowedStart <= targetTime
                    };
                })
                .Where(x => x.IsMatched)
                .OrderBy(x => x.AllowedStartTime)
                .Select(x => new ExpectedStartEmployeeDto
                {
                    EmployeeId = x.EmployeeId,
                    EmployeeCode = x.EmployeeCode ?? string.Empty,
                    EmployeeNameAr = x.EmployeeNameAr,
                    EmployeeNameEn = x.EmployeeNameEn,
                    ExpectedStartTime = x.ExpectedStartTime,
                    GraceMinutes = x.GraceMinutes,
                    AllowedStartTime = x.AllowedStartTime
                })
                .ToList();
        }

        private string? GetEmployeeDeviceId(int employeeId)
        {
            return _context.Hrs_Mobile_Attendance
                .Where(a => a.EmployeeID == employeeId && a.DeviceID != null && a.DeviceID != "")
                .OrderByDescending(a => a.CheckingDatetime)
                .Select(a => a.DeviceID)
                .FirstOrDefault();
        }

        private bool IsDeviceIdUsedByAnotherEmployee(int employeeId, string deviceId)
        {
            return _context.Hrs_Mobile_Attendance.Any(a =>
                a.EmployeeID != employeeId &&
                a.DeviceID != null &&
                a.DeviceID.ToLower() == deviceId.ToLower());
        }

        private void SetEmployeeDeviceId(int employeeId, string? deviceId)
        {
            _context.Database.ExecuteSqlInterpolated(
                $"UPDATE Hrs_Mobile_Attendance SET DeviceID = {deviceId} WHERE EmployeeID = {employeeId}");
        }

         private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // نصف قطر الأرض بالمتر

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;

            return distance;
        }
 
     

        private double ToRadians(double degree)
        {
            return degree * (Math.PI / 180);
        }
    }
}
