using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public object CheckInOut(int EmployeeID, double Latitude, double longitude, DateTime CheckingDatetime, string DeviceID, string deviceModel, string osVersion, string networkType, int Lang,string CheckType)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                 var GetDefaultLocation = from sys_Locations in _context.sys_Locations
                                         join Hrs_Employees in _context.Hrs_Employees
                                         on sys_Locations.ID equals Hrs_Employees.LocationId
                                         where (Hrs_Employees.id == EmployeeID)
                                         select new
                                         {
                                             sys_Locations.latitude,
                                             sys_Locations.longitude,
                                             sys_Locations.allowedRadius
                                         };

                var defaultLocation = GetDefaultLocation.FirstOrDefault();

                if (defaultLocation == null)
                {
                    Result.ErrorMessage = (Lang == 1) ? "لم يتم العثور على موقع افتراضي للموظف" : "No default location found for employee";
                    Result.ErrorCode = 0;
                    return Result;
                }

                 if (string.IsNullOrEmpty(defaultLocation.latitude.ToString()) || string.IsNullOrEmpty(defaultLocation.longitude.ToString()) || string.IsNullOrEmpty(defaultLocation.allowedRadius.ToString()))
                {
                    Result.ErrorMessage = (Lang == 1) ? "إحداثيات الموقع غير مكتملة" : "Location coordinates are incomplete";
                    Result.ErrorCode = 0;
                    return Result;
                }

                 if (!double.TryParse(defaultLocation.latitude.ToString(), out double allowedLatitude) ||
                    !double.TryParse(defaultLocation.longitude.ToString(), out double allowedLongitude) ||
                    !double.TryParse(defaultLocation.allowedRadius.ToString(), out double allowedRadius))
                {
                    Result.ErrorMessage = (Lang == 1) ? "خطأ في تنسيق إحداثيات الموقع" : "Error in location coordinates format";
                    Result.ErrorCode = 0;
                    return Result;
                }

                 double distance = CalculateDistance(Latitude, longitude, allowedLatitude, allowedLongitude);

                 if (distance <= allowedRadius)
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
                        DistanceFromLocation = distance,
                         CreatedDate = DateTime.Now,
                         CheckType=CheckType
                    };

                    _context.Hrs_Mobile_Attendance.Add(attendanceRecord);
                    _context.SaveChanges();

                    Result.ResultObject = new
                    {
                        CheckInId = attendanceRecord.ID,
                        CheckInTime = CheckingDatetime,
                        Distance = Math.Round(distance, 2),
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
                        AllowedRadius = allowedRadius,
                        ActualDistance = Math.Round(distance, 2),
                        Message = (Lang == 1) ?
                            $"أنت خارج نطاق الموقع المسموح. المسافة: {Math.Round(distance, 2)} متر (المسموح: {allowedRadius} متر)" :
                            $"You are outside the allowed location range. Distance: {Math.Round(distance, 2)} meters (Allowed: {allowedRadius} meters)"
                    };
                    Result.ErrorMessage = (Lang == 1) ?
                        $"أنت خارج نطاق الموقع المسموح. المسافة: {Math.Round(distance, 2)} متر" :
                        $"You are outside the allowed location range. Distance: {Math.Round(distance, 2)} meters";
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
                    Result.ErrorCode = 0;
                    Result.ResultObject = new { Records = new List<object>() };
                    return Result;
                }

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
                        Status = GetDayStatus(firstRecord, lastRecord),
                        TotalRecords = dayRecords.Count
                    };

                    dailyAttendance.Add(dailyRecord);
                }

                 var stats = new
                {
                    TotalDays = dailyGroups.Count,
                    PresentDays = dailyAttendance.Count(d => ((dynamic)d).Status == "Present"),
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

         private string GetDayStatus(Hrs_Mobile_Attendance checkIn, Hrs_Mobile_Attendance checkOut)
        {
            if (checkIn == null && checkOut == null)
                return "Absent";
            else if (checkIn != null && checkOut != null && checkIn != checkOut)
                return "Present";
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
