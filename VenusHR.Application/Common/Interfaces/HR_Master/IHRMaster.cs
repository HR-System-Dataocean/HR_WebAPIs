using System.Threading.Tasks;
using VenusHR.Core.Master;
using WorkFlow_EF;

namespace VenusHR.Application.Common.Interfaces.HR_Master
{
    public interface IHRMaster
    {
        // 🔹 فصائل الدم
        Task<GeneralOutputClass<object>> GetAllBloodGroupsAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetBloodGroupByIdAsync(int id, int lang = 0);
        Task<GeneralOutputClass<object>> CreateBloodGroupAsync(hrs_BloodGroups bloodGroup);
        Task<GeneralOutputClass<object>> UpdateBloodGroupAsync(int id, hrs_BloodGroups bloodGroup);
        Task<GeneralOutputClass<object>> DeleteBloodGroupAsync(int id);

        // 🔹 الجنسيات
        Task<GeneralOutputClass<object>> GetAllNationalitiesAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetNationalityByIdAsync(int id, int lang = 0);
        Task<GeneralOutputClass<object>> CreateNationalityAsync(sys_Nationalities nationality);
        Task<GeneralOutputClass<object>> UpdateNationalityAsync(int id, sys_Nationalities nationality);

        // 🔹 البنوك
        Task<GeneralOutputClass<object>> GetAllBanksAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetBankByIdAsync(int id, int lang = 0);
        Task<GeneralOutputClass<object>> CreateBankAsync(sys_Banks bank);
        Task<GeneralOutputClass<object>> UpdateBankAsync(int id, sys_Banks bank);

        // 🔹 الديانات
        Task<GeneralOutputClass<object>> GetAllReligionsAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetReligionByIdAsync(int id, int lang = 0);

        // 🔹 الحالة الاجتماعية
        Task<GeneralOutputClass<object>> GetAllMaritalStatusAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetMaritalStatusByIdAsync(int id, int lang = 0);

        // 🔹 المؤهلات الدراسية
        Task<GeneralOutputClass<object>> GetAllEducationsAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetEducationByIdAsync(int id, int lang = 0);
        Task<GeneralOutputClass<object>> CreateEducationAsync(hrs_Educations education);
        Task<GeneralOutputClass<object>> UpdateEducationAsync(int id, hrs_Educations education);

        // 🔹 المهن
        Task<GeneralOutputClass<object>> GetAllProfessionsAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetProfessionByIdAsync(int id, int lang = 0);

        // 🔹 المدن
        Task<GeneralOutputClass<object>> GetAllCitiesAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetCityByIdAsync(int id, int lang = 0);

        // 🔹 الشركات
        Task<GeneralOutputClass<object>> GetAllCompaniesAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetCompanyByIdAsync(int id, int lang = 0);

        // 🔹 الإدارات
        Task<GeneralOutputClass<object>> GetAllDepartmentsAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetDepartmentByIdAsync(int id, int lang = 0);
        Task<GeneralOutputClass<object>> GetDepartmentsByCompanyAsync(int companyId, int lang = 0);

        // 🔹 الوظائف
        Task<GeneralOutputClass<object>> GetAllPositionsAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetPositionByIdAsync(int id, int lang = 0);

        // 🔹 المشاريع
        Task<GeneralOutputClass<object>> GetAllProjectsAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetProjectByIdAsync(int id, int lang = 0);

        // 🔹 المواقع
        Task<GeneralOutputClass<object>> GetAllLocationsAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetLocationByIdAsync(int id, int lang = 0);
        Task<GeneralOutputClass<object>> GetLocationsByCityAsync(int cityId, int lang = 0);

        // 🔹 أنواع الإجازات
        Task<GeneralOutputClass<object>> GetAllVacationTypesAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetVacationTypeByIdAsync(int id, int lang = 0);

        // 🔹 العقود
        Task<GeneralOutputClass<object>> GetAllContractsAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetContractByIdAsync(int id, int lang = 0);
        Task<GeneralOutputClass<object>> CreateContractAsync(hrs_Contracts contract);
        Task<GeneralOutputClass<object>> UpdateContractAsync(int id, hrs_Contracts contract);

        // 🔹 Lookups مركبة
        Task<GeneralOutputClass<object>> GetSystemLookupsAsync(int lang = 0);
        Task<GeneralOutputClass<object>> GetLookupByTypeAsync(string lookupType, int lang = 0);

        // 🔹 البحث
        Task<GeneralOutputClass<object>> SearchLookupsAsync(string searchTerm, string lookupType = null, int lang = 0);

        // 🔹 إضافة موظف جديد
        Task<GeneralOutputClass<object>> SaveNewEmployeeFormAsync(Hrs_NewEmployee newEmployee);
    }
}