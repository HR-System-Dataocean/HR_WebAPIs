using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VenusHR.Application.Common.Interfaces.HR_Master;
using VenusHR.Core.Master;
using VenusHR.Infrastructure.Presistence;
using WorkFlow_EF;

namespace VenusHR.Infrastructure.Presistence.HRServices
{
    public class HRMasreService : IHRMaster
    {
        private readonly ApplicationDBContext _context;

        public HRMasreService(ApplicationDBContext context)
        {
            _context = context;
        }

        #region فصائل الدم
        public async Task<GeneralOutputClass<object>> GetAllBloodGroupsAsync(int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                result.ResultObject = lang == 1
                    ? await _context.hrs_BloodGroups
                        .Select(E => new { E.ID, E.Code, Name = E.ArbName })
                        .ToListAsync()
                    : await _context.hrs_BloodGroups
                        .Select(E => new { E.ID, E.Code, Name = E.EngName })
                        .ToListAsync();
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> GetBloodGroupByIdAsync(int id, int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var bloodGroup = await _context.hrs_BloodGroups.FindAsync(id);
                if (bloodGroup == null)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = $"Blood group with ID {id} not found";
                }
                else
                {
                    result.ResultObject = new
                    {
                        bloodGroup.ID,
                        bloodGroup.Code,
                        Name = lang == 1 ? bloodGroup.ArbName : bloodGroup.EngName,
                        bloodGroup.Remarks
                    };
                    result.ErrorCode = 1;
                }
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> CreateBloodGroupAsync(hrs_BloodGroups bloodGroup)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                bloodGroup.RegDate = DateTime.Now;
                // bloodGroup.RegUserID = GetCurrentUserId();
                // bloodGroup.RegComputerID = GetCurrentComputerId();

                _context.hrs_BloodGroups.Add(bloodGroup);
                await _context.SaveChangesAsync();

                result.ResultObject = bloodGroup;
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> UpdateBloodGroupAsync(int id, hrs_BloodGroups bloodGroup)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var existing = await _context.hrs_BloodGroups.FindAsync(id);
                if (existing == null)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = $"Blood group with ID {id} not found";
                    return result;
                }

                existing.Code = bloodGroup.Code;
                existing.EngName = bloodGroup.EngName;
                existing.ArbName = bloodGroup.ArbName;
                existing.ArbName4S = bloodGroup.ArbName4S;
                existing.Remarks = bloodGroup.Remarks;
                existing.CancelDate = bloodGroup.CancelDate;

                await _context.SaveChangesAsync();

                result.ResultObject = existing;
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> DeleteBloodGroupAsync(int id)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var bloodGroup = await _context.hrs_BloodGroups.FindAsync(id);
                if (bloodGroup == null)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = $"Blood group with ID {id} not found";
                    return result;
                }

                _context.hrs_BloodGroups.Remove(bloodGroup);
                await _context.SaveChangesAsync();

                result.ResultObject = true;
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        #endregion

        #region الجنسيات
        public async Task<GeneralOutputClass<object>> GetAllNationalitiesAsync(int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                result.ResultObject = lang == 1
                    ? await _context.sys_Nationalities
                        .Select(E => new { E.ID, E.Code, Name = E.ArbName })
                        .ToListAsync()
                    : await _context.sys_Nationalities
                        .Select(E => new { E.ID, E.Code, Name = E.EngName })
                        .ToListAsync();
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> GetNationalityByIdAsync(int id, int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var nationality = await _context.sys_Nationalities.FindAsync(id);
                if (nationality == null)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = $"Nationality with ID {id} not found";
                }
                else
                {
                    result.ResultObject = new
                    {
                        nationality.ID,
                        nationality.Code,
                        Name = lang == 1 ? nationality.ArbName : nationality.EngName,
                        nationality.IsMainNationality
                    };
                    result.ErrorCode = 1;
                }
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> CreateNationalityAsync(sys_Nationalities nationality)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                nationality.RegDate = DateTime.Now;
                // nationality.RegUserID = GetCurrentUserId();
                // nationality.RegComputerID = GetCurrentComputerId();

                _context.sys_Nationalities.Add(nationality);
                await _context.SaveChangesAsync();

                result.ResultObject = nationality;
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> UpdateNationalityAsync(int id, sys_Nationalities nationality)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var existing = await _context.sys_Nationalities.FindAsync(id);
                if (existing == null)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = $"Nationality with ID {id} not found";
                    return result;
                }

                existing.Code = nationality.Code;
                existing.EngName = nationality.EngName;
                existing.ArbName = nationality.ArbName;
                existing.ArbName4S = nationality.ArbName4S;
                existing.IsMainNationality = nationality.IsMainNationality;
                existing.Remarks = nationality.Remarks;
                existing.CancelDate = nationality.CancelDate;

                await _context.SaveChangesAsync();

                result.ResultObject = existing;
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        #endregion

        #region البنوك
        public async Task<GeneralOutputClass<object>> GetAllBanksAsync(int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                result.ResultObject = lang == 1
                    ? await _context.sys_Banks
                        .Select(E => new { E.ID, E.Code, Name = E.ArbName })
                        .ToListAsync()
                    : await _context.sys_Banks
                        .Select(E => new { E.ID, E.Code, Name = E.EngName })
                        .ToListAsync();
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> GetBankByIdAsync(int id, int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var bank = await _context.sys_Banks.FindAsync(id);
                if (bank == null)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = $"Bank with ID {id} not found";
                }
                else
                {
                    result.ResultObject = new
                    {
                        bank.ID,
                        bank.Code,
                        Name = lang == 1 ? bank.ArbName : bank.EngName,
                        //bank.BankAccountNo,
                        //bank.BankSwiftCode
                    };
                    result.ErrorCode = 1;
                }
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> CreateBankAsync(sys_Banks bank)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                bank.RegDate = DateTime.Now;
                // bank.RegUserID = GetCurrentUserId();
                // bank.RegComputerID = GetCurrentComputerId();

                _context.sys_Banks.Add(bank);
                await _context.SaveChangesAsync();

                result.ResultObject = bank;
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> UpdateBankAsync(int id, sys_Banks bank)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var existing = await _context.sys_Banks.FindAsync(id);
                if (existing == null)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = $"Bank with ID {id} not found";
                    return result;
                }

                existing.Code = bank.Code;
                existing.EngName = bank.EngName;
                existing.ArbName = bank.ArbName;
                existing.ArbName4S = bank.ArbName4S;
                //existing.BankAccountNo = bank.BankAccountNo;
                //existing.BankSwiftCode = bank.BankSwiftCode;
                existing.Remarks = bank.Remarks;
                existing.CancelDate = bank.CancelDate;

                await _context.SaveChangesAsync();

                result.ResultObject = existing;
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        #endregion

        #region الديانات
        public async Task<GeneralOutputClass<object>> GetAllReligionsAsync(int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                result.ResultObject = lang == 1
                    ? await _context.hrs_Religions
                        .Select(E => new { E.ID, E.Code, Name = E.ArbName })
                        .ToListAsync()
                    : await _context.hrs_Religions
                        .Select(E => new { E.ID, E.Code, Name = E.EngName })
                        .ToListAsync();
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> GetReligionByIdAsync(int id, int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var religion = await _context.hrs_Religions.FindAsync(id);
                if (religion == null)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = $"Religion with ID {id} not found";
                }
                else
                {
                    result.ResultObject = new
                    {
                        religion.ID,
                        religion.Code,
                        Name = lang == 1 ? religion.ArbName : religion.EngName
                    };
                    result.ErrorCode = 1;
                }
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        #endregion

        #region الحالة الاجتماعية
        public async Task<GeneralOutputClass<object>> GetAllMaritalStatusAsync(int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                result.ResultObject = lang == 1
                    ? await _context.hrs_MaritalStatus
                        .Select(E => new { E.ID, E.Code, Name = E.ArbName })
                        .ToListAsync()
                    : await _context.hrs_MaritalStatus
                        .Select(E => new { E.ID, E.Code, Name = E.EngName })
                        .ToListAsync();
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> GetMaritalStatusByIdAsync(int id, int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var maritalStatus = await _context.hrs_MaritalStatus.FindAsync(id);
                if (maritalStatus == null)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = $"Marital status with ID {id} not found";
                }
                else
                {
                    result.ResultObject = new
                    {
                        maritalStatus.ID,
                        maritalStatus.Code,
                        Name = lang == 1 ? maritalStatus.ArbName : maritalStatus.EngName
                    };
                    result.ErrorCode = 1;
                }
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        #endregion

        #region المؤهلات الدراسية
        public async Task<GeneralOutputClass<object>> GetAllEducationsAsync(int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                result.ResultObject = lang == 1
                    ? await _context.hrs_Educations
                        .Select(E => new { E.ID, E.Code, Name = E.ArbName })
                        .ToListAsync()
                    : await _context.hrs_Educations
                        .Select(E => new { E.ID, E.Code, Name = E.EngName })
                        .ToListAsync();
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> GetEducationByIdAsync(int id, int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var education = await _context.hrs_Educations.FindAsync(id);
                if (education == null)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = $"Education with ID {id} not found";
                }
                else
                {
                    result.ResultObject = new
                    {
                        education.ID,
                        education.Code,
                        Name = lang == 1 ? education.ArbName : education.EngName
                    };
                    result.ErrorCode = 1;
                }
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> CreateEducationAsync(hrs_Educations education)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                education.RegDate = DateTime.Now;
                // education.RegUserID = GetCurrentUserId();
                // education.RegComputerID = GetCurrentComputerId();

                _context.hrs_Educations.Add(education);
                await _context.SaveChangesAsync();

                result.ResultObject = education;
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> UpdateEducationAsync(int id, hrs_Educations education)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var existing = await _context.hrs_Educations.FindAsync(id);
                if (existing == null)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = $"Education with ID {id} not found";
                    return result;
                }

                existing.Code = education.Code;
                existing.EngName = education.EngName;
                existing.ArbName = education.ArbName;
                existing.ArbName4S = education.ArbName4S;
                existing.Remarks = education.Remarks;
                existing.CancelDate = education.CancelDate;

                await _context.SaveChangesAsync();

                result.ResultObject = existing;
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        #endregion

        #region المهن
        public async Task<GeneralOutputClass<object>> GetAllProfessionsAsync(int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                result.ResultObject = lang == 1
                    ? await _context.Hrs_Professions
                        .Select(E => new { E.Id, E.Code, Name = E.ArbName })
                        .ToListAsync()
                    : await _context.Hrs_Professions
                        .Select(E => new { E.Id, E.Code, Name = E.EngName })
                        .ToListAsync();
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> GetProfessionByIdAsync(int id, int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var profession = await _context.Hrs_Professions.FindAsync(id);
                if (profession == null)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = $"Profession with ID {id} not found";
                }
                else
                {
                    result.ResultObject = new
                    {
                        profession.Id,
                        profession.Code,
                        Name = lang == 1 ? profession.ArbName : profession.EngName
                    };
                    result.ErrorCode = 1;
                }
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        #endregion

        #region المدن
        public async Task<GeneralOutputClass<object>> GetAllCitiesAsync(int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                result.ResultObject = lang == 1
                    ? await _context.sys_Cities
                        .Select(E => new { E.ID, E.Code, Name = E.ArbName })
                        .ToListAsync()
                    : await _context.sys_Cities
                        .Select(E => new { E.ID, E.Code, Name = E.EngName })
                        .ToListAsync();
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> GetCityByIdAsync(int id, int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var city = await _context.sys_Cities.FindAsync(id);
                if (city == null)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = $"City with ID {id} not found";
                }
                else
                {
                    result.ResultObject = new
                    {
                        city.ID,
                        city.Code,
                        Name = lang == 1 ? city.ArbName : city.EngName
                    };
                    result.ErrorCode = 1;
                }
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        #endregion

        #region الشركات
        public async Task<GeneralOutputClass<object>> GetAllCompaniesAsync(int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                result.ResultObject = lang == 1
                    ? await _context.Sys_Companies
                        .Select(E => new { E.ID, E.Code, Name = E.ArbName })
                        .ToListAsync()
                    : await _context.Sys_Companies
                        .Select(E => new { E.ID, E.Code, Name = E.EngName })
                        .ToListAsync();
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> GetCompanyByIdAsync(int id, int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var company = await _context.Sys_Companies.FindAsync(id);
                if (company == null)
                {
                    result.ErrorCode = 0;
                    result.ErrorMessage = $"Company with ID {id} not found";
                }
                else
                {
                    result.ResultObject = new
                    {
                        company.ID,
                        company.Code,
                        Name = lang == 1 ? company.ArbName : company.EngName
                    };
                    result.ErrorCode = 1;
                }
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        #endregion

        #region إضافة موظف جديد
        public async Task<GeneralOutputClass<object>> SaveNewEmployeeFormAsync(Hrs_NewEmployee newEmployee)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var ssno = newEmployee.Ssno;
                int count = await _context.Hrs_NewEmployee
                    .Where(S => S.Ssno == ssno)
                    .CountAsync();

                if (count > 0)
                {
                    throw new Exception($"Employee with SSNO:{newEmployee.Ssno} Already Exists");
                }

                //newEmployee.RegDate = DateTime.Now;
                // newEmployee.RegUserID = GetCurrentUserId();
                // newEmployee.RegComputerID = GetCurrentComputerId();

                _context.Hrs_NewEmployee.Add(newEmployee);
                await _context.SaveChangesAsync();

                result.ErrorCode = 1;
                result.ResultObject = newEmployee;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        #endregion

        #region Lookups مركبة
        public async Task<GeneralOutputClass<object>> GetSystemLookupsAsync(int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                var lookups = new
                {
                    Nationalities = (await GetAllNationalitiesAsync(lang)).ResultObject,
                    Banks = (await GetAllBanksAsync(lang)).ResultObject,
                    Religions = (await GetAllReligionsAsync(lang)).ResultObject,
                    MaritalStatus = (await GetAllMaritalStatusAsync(lang)).ResultObject,
                    BloodGroups = (await GetAllBloodGroupsAsync(lang)).ResultObject,
                    Educations = (await GetAllEducationsAsync(lang)).ResultObject,
                    Professions = (await GetAllProfessionsAsync(lang)).ResultObject,
                    Cities = (await GetAllCitiesAsync(lang)).ResultObject,
                    Companies = (await GetAllCompaniesAsync(lang)).ResultObject
                };

                result.ResultObject = lookups;
                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public async Task<GeneralOutputClass<object>> GetLookupByTypeAsync(string lookupType, int lang = 0)
        {
            return lookupType.ToLower() switch
            {
                "nationalities" => await GetAllNationalitiesAsync(lang),
                "banks" => await GetAllBanksAsync(lang),
                "religions" => await GetAllReligionsAsync(lang),
                "maritalstatus" => await GetAllMaritalStatusAsync(lang),
                "bloodgroups" => await GetAllBloodGroupsAsync(lang),
                "educations" => await GetAllEducationsAsync(lang),
                "professions" => await GetAllProfessionsAsync(lang),
                "cities" => await GetAllCitiesAsync(lang),
                "companies" => await GetAllCompaniesAsync(lang),
                _ => new GeneralOutputClass<object>
                {
                    ResultObject = null,
                    ErrorCode = 0,
                    ErrorMessage = $"Lookup type '{lookupType}' not found"
                }
            };
        }

        public async Task<GeneralOutputClass<object>> SearchLookupsAsync(string searchTerm, string lookupType = null, int lang = 0)
        {
            var result = new GeneralOutputClass<object>();
            try
            {
                if (string.IsNullOrEmpty(lookupType))
                {
                    // بحث في جميع الجداول
                    var searchResults = new
                    {
                        Nationalities = await SearchInTable(_context.sys_Nationalities, searchTerm, lang),
                        Banks = await SearchInTable(_context.sys_Banks, searchTerm, lang),
                        Religions = await SearchInTable(_context.hrs_Religions, searchTerm, lang),
                        BloodGroups = await SearchInTable(_context.hrs_BloodGroups, searchTerm, lang)
                    };

                    result.ResultObject = searchResults;
                }
                else
                {
                    // بحث في جدول محدد
                    result = await GetLookupByTypeAsync(lookupType, lang);
                    if (result.ErrorCode == 1 && result.ResultObject is List<object> list)
                    {
                        var filtered = list.Where(item =>
                        {
                            var properties = item.GetType().GetProperties();
                            return properties.Any(p =>
                            {
                                var value = p.GetValue(item)?.ToString();
                                return !string.IsNullOrEmpty(value) &&
                                       value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
                            });
                        }).ToList();

                        result.ResultObject = filtered;
                    }
                }

                result.ErrorCode = 1;
            }
            catch (Exception ex)
            {
                result.ResultObject = null;
                result.ErrorCode = 0;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        private async Task<List<object>> SearchInTable<T>(IQueryable<T> table, string searchTerm, int lang) where T : class
        {
            var query = table.AsEnumerable(); // التحويل إلى IEnumerable للبحث في الذاكرة

            var results = query.Where(e =>
            {
                var engNameProp = e.GetType().GetProperty("EngName");
                var arbNameProp = e.GetType().GetProperty("ArbName");
                var codeProp = e.GetType().GetProperty("Code");

                var engName = engNameProp?.GetValue(e)?.ToString() ?? "";
                var arbName = arbNameProp?.GetValue(e)?.ToString() ?? "";
                var code = codeProp?.GetValue(e)?.ToString() ?? "";

                return engName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       arbName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
            })
            .Select(e =>
            {
                var idProp = e.GetType().GetProperty("ID") ?? e.GetType().GetProperty("Id");
                var codeProp = e.GetType().GetProperty("Code");
                var nameProp = lang == 1
                    ? e.GetType().GetProperty("ArbName")
                    : e.GetType().GetProperty("EngName");

                return new
                {
                    ID = idProp?.GetValue(e),
                    Code = codeProp?.GetValue(e),
                    Name = nameProp?.GetValue(e)
                };
            })
            .Cast<object>()
            .ToList();

            return results;
        }
        #endregion

        #region Methods غير منفذة (للتطوير المستقبلي)
        public Task<GeneralOutputClass<object>> GetAllDepartmentsAsync(int lang = 0)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralOutputClass<object>> GetDepartmentByIdAsync(int id, int lang = 0)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralOutputClass<object>> GetDepartmentsByCompanyAsync(int companyId, int lang = 0)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralOutputClass<object>> GetAllPositionsAsync(int lang = 0)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralOutputClass<object>> GetPositionByIdAsync(int id, int lang = 0)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralOutputClass<object>> GetAllProjectsAsync(int lang = 0)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralOutputClass<object>> GetProjectByIdAsync(int id, int lang = 0)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralOutputClass<object>> GetAllLocationsAsync(int lang = 0)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralOutputClass<object>> GetLocationByIdAsync(int id, int lang = 0)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralOutputClass<object>> GetLocationsByCityAsync(int cityId, int lang = 0)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralOutputClass<object>> GetAllVacationTypesAsync(int lang = 0)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralOutputClass<object>> GetVacationTypeByIdAsync(int id, int lang = 0)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralOutputClass<object>> GetAllContractsAsync(int lang = 0)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralOutputClass<object>> GetContractByIdAsync(int id, int lang = 0)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralOutputClass<object>> CreateContractAsync(hrs_Contracts contract)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralOutputClass<object>> UpdateContractAsync(int id, hrs_Contracts contract)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Methods مساعدة
        /*
        private int GetCurrentUserId()
        {
            // أضف منطق جلب الـ User ID من الـ Authentication
            return 1; // مؤقتاً
        }
        
        private int GetCurrentComputerId()
        {
            // أضف منطق جلب الـ Computer ID
            return 1; // مؤقتاً
        }
        */
        #endregion
    }
}