using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VenusHR.Application.Common.Interfaces.Attendance;
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
            try
            {
                var employee = _context.Hrs_Employees.FirstOrDefault(e => e.id == EmployeeID);
                if (employee == null)
                {
                    Result.ErrorMessage = (Lang == 1) ? "الموظف غير موجود" : "Employee not found";
                    Result.ErrorCode = 0;
                    return Result;
                }

                var incomingMac = !string.IsNullOrWhiteSpace(macAddress) ? macAddress.Trim() :
                                  !string.IsNullOrWhiteSpace(DeviceID) ? DeviceID.Trim() : null;

                if (string.IsNullOrWhiteSpace(incomingMac))
                {
                    Result.ErrorMessage = (Lang == 1) ? "معرف الجهاز (MAC Address) مطلوب" : "Device identifier (MAC Address) is required";
                    Result.ErrorCode = 0;
                    return Result;
                }

                if (!string.IsNullOrWhiteSpace(employee.MacAddress))
                {
                    if (!string.Equals(employee.MacAddress.Trim(), incomingMac, StringComparison.OrdinalIgnoreCase))
                    {
                        Result.ErrorMessage = (Lang == 1)
                            ? "هذا الجهاز غير مسجل. يرجى التواصل مع إدارة الموارد البشرية لتغيير الجهاز"
                            : "This device is not registered. Please contact HR to change your device";
                        Result.ErrorCode = 0;
                        return Result;
                    }
                }
                else
                {
                    SetEmployeeMacAddress(EmployeeID, incomingMac);
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

                    Result.ResultObject = new
                    {
                        CheckInId = attendanceRecord.ID,
                        CheckInTime = CheckingDatetime,
                        Distance = Math.Round(closestDistance ?? 0, 2),
                        Message = (Lang == 1) ? "تم تسجيل الحضور بنجاح" : "Check-in successful",
                        IsWithinRange = true
                    };
                    Result.ErrorMessage = (Lang == 1) ? "تم تسجيل الحضور بنجاح" : "Check-in successful";
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
                Result.ErrorMessage = (Lang == 1) ? "حدث خطأ أثناء تسجيل الحضور" : "An error occurred during check-in";
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

                Result.ResultObject = new
                {
                    EmployeeID = employee.id,
                    EmployeeCode = employee.Code,
                    MacAddress = employee.MacAddress,
                    IsDeviceRegistered = !string.IsNullOrWhiteSpace(employee.MacAddress)
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

        public object ChangeDevice(int EmployeeID, string MacAddress, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (string.IsNullOrWhiteSpace(MacAddress))
                {
                    Result.ErrorMessage = (Lang == 1) ? "عنوان MAC مطلوب" : "MAC Address is required";
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

                var previousMac = employee.MacAddress;
                var newMac = MacAddress.Trim();
                SetEmployeeMacAddress(EmployeeID, newMac);

                Result.ResultObject = new
                {
                    EmployeeID = employee.id,
                    EmployeeCode = employee.Code,
                    PreviousMacAddress = previousMac,
                    NewMacAddress = newMac
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

                var previousMac = employee.MacAddress;
                SetEmployeeMacAddress(EmployeeID, null);

                Result.ResultObject = new
                {
                    EmployeeID = employee.id,
                    EmployeeCode = employee.Code,
                    ClearedMacAddress = previousMac
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

         private void SetEmployeeMacAddress(int employeeId, string? macAddress)
        {
            _context.Database.ExecuteSqlInterpolated(
                $"UPDATE Hrs_Employees SET MacAddress = {macAddress} WHERE id = {employeeId}");
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
