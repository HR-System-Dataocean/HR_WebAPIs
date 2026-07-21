using System;
using System.Linq;
using VenusHR.Application.Common.Interfaces.SelfService;
using VenusHR.Core.SelfService;
using WorkFlow_EF;

namespace VenusHR.Infrastructure.Presistence.SelfService
{
    public class DependentsInformationUpdateService : IDependentsInformationUpdateService
    {
        private readonly ApplicationDBContext _context;
        private GeneralOutputClass<object> Result;

        public DependentsInformationUpdateService(ApplicationDBContext context)
        {
            _context = context;
        }

        public object GetAll(int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                Result.ResultObject = BuildQuery(Lang).ToList();
                Result.ErrorCode = 1;
                Result.ErrorMessage = "";
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }
            return Result;
        }

        public object GetById(int id, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                Result.ResultObject = BuildQuery(Lang).FirstOrDefault(r => r.Id == id);
                Result.ErrorCode = 1;
                Result.ErrorMessage = "";
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }
            return Result;
        }

        public object GetByEmployeeId(int employeeId, int Lang)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                Result.ResultObject = BuildQuery(Lang).Where(r => r.EmployeeId == employeeId).ToList();
                Result.ErrorCode = 1;
                Result.ErrorMessage = "";
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }
            return Result;
        }

        public object Create(SS_DependentsInformationUpdate request)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Request data is required";
                    return Result;
                }

                if (request.RequestDate == default)
                {
                    request.RequestDate = DateTime.Now;
                }

                if (request.RequestStautsTypeId == null || request.RequestStautsTypeId == 0)
                {
                    request.RequestStautsTypeId = 3;
                }

                if (string.IsNullOrWhiteSpace(request.Code))
                {
                    request.Code = "PENDING";
                }

                _context.SS_DependentsInformationUpdate.Add(request);
                _context.SaveChanges();

                request.Code = $"DEP_{request.Id}";
                _context.SaveChanges();

                Result.ResultObject = request;
                Result.ErrorCode = 1;
                Result.ErrorMessage = "";
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }
            return Result;
        }

        public object Update(SS_DependentsInformationUpdate request)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                if (request == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Request data is required";
                    return Result;
                }

                var existing = _context.SS_DependentsInformationUpdate.FirstOrDefault(r => r.Id == request.Id);
                if (existing == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Dependents information update request not found";
                    return Result;
                }

                existing.EmployeeId = request.EmployeeId;
                existing.RequestDate = request.RequestDate;
                existing.Remarks = request.Remarks;
                existing.RequesterUser = request.RequesterUser;
                existing.RequestStautsTypeId = request.RequestStautsTypeId;

                if (!string.IsNullOrWhiteSpace(request.Code))
                {
                    existing.Code = request.Code;
                }

                _context.SaveChanges();

                Result.ResultObject = existing;
                Result.ErrorCode = 1;
                Result.ErrorMessage = "";
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }
            return Result;
        }

        public object Delete(int id)
        {
            Result = new GeneralOutputClass<object>();
            try
            {
                var existing = _context.SS_DependentsInformationUpdate.FirstOrDefault(r => r.Id == id);
                if (existing == null)
                {
                    Result.ErrorCode = 0;
                    Result.ErrorMessage = "Dependents information update request not found";
                    return Result;
                }

                _context.SS_DependentsInformationUpdate.Remove(existing);
                _context.SaveChanges();

                Result.ResultObject = new { Id = id };
                Result.ErrorCode = 1;
                Result.ErrorMessage = "";
            }
            catch (Exception ex)
            {
                Result.ResultObject = null;
                Result.ErrorCode = 0;
                Result.ErrorMessage = ex.Message;
            }
            return Result;
        }

        private IQueryable<DependentsInformationUpdateDto> BuildQuery(int Lang)
        {
            return from request in _context.SS_DependentsInformationUpdate
                   join emp in _context.Hrs_Employees
                       on request.EmployeeId equals emp.id into empJoin
                   from emp in empJoin.DefaultIfEmpty()
                   select new DependentsInformationUpdateDto
                   {
                       Id = request.Id,
                       Code = request.Code,
                       EmployeeId = request.EmployeeId,
                       EmployeeName = emp == null
                           ? null
                           : (Lang == 1
                               ? emp.ArbName + " " + emp.FatherArbName + " " + emp.GrandArbName + " " + emp.FamilyArbName
                               : emp.EngName + " " + emp.FatherEngName + " " + emp.GrandEngName + " " + emp.FamilyEngName),
                       RequestDate = request.RequestDate,
                       Remarks = request.Remarks,
                       RequesterUser = request.RequesterUser,
                       RequestStautsTypeId = request.RequestStautsTypeId
                   };
        }

        private class DependentsInformationUpdateDto
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public DateTime RequestDate { get; set; }
            public string Remarks { get; set; }
            public string RequesterUser { get; set; }
            public int? RequestStautsTypeId { get; set; }
        }
    }
}
