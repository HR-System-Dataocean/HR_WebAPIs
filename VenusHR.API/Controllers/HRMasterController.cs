using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VenusHR.Application.Common.Interfaces.HR_Master;
using VenusHR.Core.Master;
using WorkFlow_EF;
using VenusHR.API.Models;

namespace VenusHR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HRMasterController : ControllerBase
    {
        private readonly IHRMaster _hrMaster;

        public HRMasterController(IHRMaster hrMaster)
        {
            _hrMaster = hrMaster;
        }

        [HttpGet("GetAllMasterData/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllMasterData(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetSystemLookupsAsync(lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "فشل جلب البيانات" : "Failed to retrieve data";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب البيانات بنجاح" : "Data retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetAllCities/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllCities(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllCitiesAsync(lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "فشل جلب المدن" : "Failed to retrieve cities";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب المدن بنجاح" : "Cities retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetCityById/{id}/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetCityById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetCityByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "المدينة غير موجودة" : "City not found";
                    return NotFound(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب المدينة بنجاح" : "City retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetAllNationalities/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllNationalities(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllNationalitiesAsync(lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "فشل جلب الجنسيات" : "Failed to retrieve nationalities";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب الجنسيات بنجاح" : "Nationalities retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetNationalityById/{id}/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetNationalityById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetNationalityByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "الجنسية غير موجودة" : "Nationality not found";
                    return NotFound(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب الجنسية بنجاح" : "Nationality retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpPost("CreateNationality")]
        public async Task<ActionResult<ApiResponse<object>>> CreateNationality([FromBody] sys_Nationalities nationality)
        {
            try
            {
                var result = await _hrMaster.CreateNationalityAsync(nationality);

                if (result.ErrorCode == 0)
                {
                    var message = "Failed to create nationality";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = "Nationality created successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail("Server error occurred", 500, ex.Message));
            }
        }

        [HttpPut("UpdateNationality/{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateNationality(int id, [FromBody] sys_Nationalities nationality)
        {
            try
            {
                var result = await _hrMaster.UpdateNationalityAsync(id, nationality);

                if (result.ErrorCode == 0)
                {
                    var message = "Failed to update nationality";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = "Nationality updated successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail("Server error occurred", 500, ex.Message));
            }
        }

        [HttpGet("GetAllBanks/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllBanks(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllBanksAsync(lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "فشل جلب البنوك" : "Failed to retrieve banks";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب البنوك بنجاح" : "Banks retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetBankById/{id}/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetBankById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetBankByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "البنك غير موجود" : "Bank not found";
                    return NotFound(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب البنك بنجاح" : "Bank retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpPost("CreateBank")]
        public async Task<ActionResult<ApiResponse<object>>> CreateBank([FromBody] sys_Banks bank)
        {
            try
            {
                var result = await _hrMaster.CreateBankAsync(bank);

                if (result.ErrorCode == 0)
                {
                    var message = "Failed to create bank";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = "Bank created successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail("Server error occurred", 500, ex.Message));
            }
        }

        [HttpPut("UpdateBank/{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateBank(int id, [FromBody] sys_Banks bank)
        {
            try
            {
                var result = await _hrMaster.UpdateBankAsync(id, bank);

                if (result.ErrorCode == 0)
                {
                    var message = "Failed to update bank";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = "Bank updated successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail("Server error occurred", 500, ex.Message));
            }
        }

        [HttpGet("GetAllReligions/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllReligions(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllReligionsAsync(lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "فشل جلب الأديان" : "Failed to retrieve religions";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب الأديان بنجاح" : "Religions retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetReligionById/{id}/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetReligionById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetReligionByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "الدين غير موجود" : "Religion not found";
                    return NotFound(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب الدين بنجاح" : "Religion retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetAllMaritalStatus/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllMaritalStatus(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllMaritalStatusAsync(lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "فشل جلب الحالات الاجتماعية" : "Failed to retrieve marital statuses";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب الحالات الاجتماعية بنجاح" : "Marital statuses retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetMaritalStatusById/{id}/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetMaritalStatusById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetMaritalStatusByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "الحالة الاجتماعية غير موجودة" : "Marital status not found";
                    return NotFound(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب الحالة الاجتماعية بنجاح" : "Marital status retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetAllBloodGroups/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllBloodGroups(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllBloodGroupsAsync(lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "فشل جلب فصائل الدم" : "Failed to retrieve blood groups";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب فصائل الدم بنجاح" : "Blood groups retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetBloodGroupById/{id}/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetBloodGroupById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetBloodGroupByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "فصيلة الدم غير موجودة" : "Blood group not found";
                    return NotFound(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب فصيلة الدم بنجاح" : "Blood group retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpPost("CreateBloodGroup")]
        public async Task<ActionResult<ApiResponse<object>>> CreateBloodGroup([FromBody] hrs_BloodGroups bloodGroup)
        {
            try
            {
                var result = await _hrMaster.CreateBloodGroupAsync(bloodGroup);

                if (result.ErrorCode == 0)
                {
                    var message = "Failed to create blood group";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = "Blood group created successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail("Server error occurred", 500, ex.Message));
            }
        }

        [HttpPut("UpdateBloodGroup/{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateBloodGroup(int id, [FromBody] hrs_BloodGroups bloodGroup)
        {
            try
            {
                var result = await _hrMaster.UpdateBloodGroupAsync(id, bloodGroup);

                if (result.ErrorCode == 0)
                {
                    var message = "Failed to update blood group";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = "Blood group updated successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail("Server error occurred", 500, ex.Message));
            }
        }

        [HttpDelete("DeleteBloodGroup/{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteBloodGroup(int id)
        {
            try
            {
                var result = await _hrMaster.DeleteBloodGroupAsync(id);

                if (result.ErrorCode == 0)
                {
                    var message = "Failed to delete blood group";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = "Blood group deleted successfully";
                return Ok(ApiResponse<object>.Ok(null, successMsg));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail("Server error occurred", 500, ex.Message));
            }
        }

        [HttpGet("GetAllEducations/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllEducations(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllEducationsAsync(lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "فشل جلب المؤهلات" : "Failed to retrieve educations";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب المؤهلات بنجاح" : "Educations retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetEducationById/{id}/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetEducationById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetEducationByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "المؤهل غير موجود" : "Education not found";
                    return NotFound(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب المؤهل بنجاح" : "Education retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpPost("CreateEducation")]
        public async Task<ActionResult<ApiResponse<object>>> CreateEducation([FromBody] hrs_Educations education)
        {
            try
            {
                var result = await _hrMaster.CreateEducationAsync(education);

                if (result.ErrorCode == 0)
                {
                    var message = "Failed to create education";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = "Education created successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail("Server error occurred", 500, ex.Message));
            }
        }

        [HttpPut("UpdateEducation/{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateEducation(int id, [FromBody] hrs_Educations education)
        {
            try
            {
                var result = await _hrMaster.UpdateEducationAsync(id, education);

                if (result.ErrorCode == 0)
                {
                    var message = "Failed to update education";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = "Education updated successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail("Server error occurred", 500, ex.Message));
            }
        }

        [HttpGet("GetAllProfessions/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllProfessions(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllProfessionsAsync(lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "فشل جلب المهن" : "Failed to retrieve professions";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب المهن بنجاح" : "Professions retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetProfessionById/{id}/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetProfessionById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetProfessionByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "المهنة غير موجودة" : "Profession not found";
                    return NotFound(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب المهنة بنجاح" : "Profession retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetAllCompanies/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllCompanies(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllCompaniesAsync(lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "فشل جلب الشركات" : "Failed to retrieve companies";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب الشركات بنجاح" : "Companies retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetCompanyById/{id}/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetCompanyById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetCompanyByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "الشركة غير موجودة" : "Company not found";
                    return NotFound(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب الشركة بنجاح" : "Company retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("GetLookupByType/{lookupType}/{lang}")]
        public async Task<ActionResult<ApiResponse<object>>> GetLookupByType(string lookupType, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetLookupByTypeAsync(lookupType, lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "فشل جلب البيانات" : "Failed to retrieve lookup data";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم جلب البيانات بنجاح" : "Lookup data retrieved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpGet("SearchLookups")]
        public async Task<ActionResult<ApiResponse<object>>> SearchLookups([FromQuery] string searchTerm, [FromQuery] string lookupType = null, [FromQuery] int lang = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    var message = lang == 1 ? "مصطلح البحث مطلوب" : "Search term is required";
                    return BadRequest(ApiResponse<object>.Fail(message));
                }

                var result = await _hrMaster.SearchLookupsAsync(searchTerm, lookupType, lang);

                if (result.ErrorCode == 0)
                {
                    var message = lang == 1 ? "فشل البحث" : "Search failed";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = lang == 1 ? "تم البحث بنجاح" : "Search completed successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                var message = lang == 1 ? "حدث خطأ في الخادم" : "Server error occurred";
                return StatusCode(500, ApiResponse<object>.Fail(message, 500, ex.Message));
            }
        }

        [HttpPost("SaveNewEmployeeForm")]
        public async Task<ActionResult<ApiResponse<object>>> SaveNewEmployeeForm([FromBody] Hrs_NewEmployee newEmployee)
        {
            try
            {
                var result = await _hrMaster.SaveNewEmployeeFormAsync(newEmployee);

                if (result.ErrorCode == 0)
                {
                    var message = "Failed to save employee";
                    return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? message, result.ErrorCode));
                }

                var successMsg = "Employee saved successfully";
                return Ok(ApiResponse<object>.Ok(result.ResultObject, result.ErrorMessage ?? successMsg));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail("Server error occurred", 500, ex.Message));
            }
        }

        [HttpGet("TestConnection")]
        public ActionResult<ApiResponse<object>> TestConnection()
        {
            return Ok(ApiResponse<object>.Ok(new
            {
                status = "API is running",
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            }, "API is running"));
        }
    }
}