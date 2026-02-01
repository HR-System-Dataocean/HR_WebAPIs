using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VenusHR.Application.Common.Interfaces.HR_Master;
using VenusHR.Core.Master;
using WorkFlow_EF;

namespace VenusHR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRMasterController : ControllerBase
    {
        private readonly IHRMaster _hrMaster;

        public HRMasterController(IHRMaster hrMaster)
        {
            _hrMaster = hrMaster;
        }

        [HttpGet("GetAllMasterData/{lang}")]
        public async Task<IActionResult> GetAllMasterData(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetSystemLookupsAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetAllCities/{lang}")]
        public async Task<IActionResult> GetAllCities(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllCitiesAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetCityById/{id}/{lang}")]
        public async Task<IActionResult> GetCityById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetCityByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetAllNationalities/{lang}")]
        public async Task<IActionResult> GetAllNationalities(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllNationalitiesAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetNationalityById/{id}/{lang}")]
        public async Task<IActionResult> GetNationalityById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetNationalityByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("CreateNationality")]
        public async Task<IActionResult> CreateNationality([FromBody] sys_Nationalities nationality)
        {
            try
            {
                var result = await _hrMaster.CreateNationalityAsync(nationality);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return CreatedAtAction(nameof(GetNationalityById),
                    new { id = ((sys_Nationalities)result.ResultObject).ID },
                    result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("UpdateNationality/{id}")]
        public async Task<IActionResult> UpdateNationality(int id, [FromBody] sys_Nationalities nationality)
        {
            try
            {
                var result = await _hrMaster.UpdateNationalityAsync(id, nationality);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetAllBanks/{lang}")]
        public async Task<IActionResult> GetAllBanks(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllBanksAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetBankById/{id}/{lang}")]
        public async Task<IActionResult> GetBankById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetBankByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("CreateBank")]
        public async Task<IActionResult> CreateBank([FromBody] sys_Banks bank)
        {
            try
            {
                var result = await _hrMaster.CreateBankAsync(bank);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return CreatedAtAction(nameof(GetBankById),
                    new { id = ((sys_Banks)result.ResultObject).ID },
                    result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("UpdateBank/{id}")]
        public async Task<IActionResult> UpdateBank(int id, [FromBody] sys_Banks bank)
        {
            try
            {
                var result = await _hrMaster.UpdateBankAsync(id, bank);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetAllReligions/{lang}")]
        public async Task<IActionResult> GetAllReligions(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllReligionsAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetReligionById/{id}/{lang}")]
        public async Task<IActionResult> GetReligionById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetReligionByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetAllMaritalStatus/{lang}")]
        public async Task<IActionResult> GetAllMaritalStatus(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllMaritalStatusAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetMaritalStatusById/{id}/{lang}")]
        public async Task<IActionResult> GetMaritalStatusById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetMaritalStatusByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetAllBloodGroups/{lang}")]
        public async Task<IActionResult> GetAllBloodGroups(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllBloodGroupsAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetBloodGroupById/{id}/{lang}")]
        public async Task<IActionResult> GetBloodGroupById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetBloodGroupByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("CreateBloodGroup")]
        public async Task<IActionResult> CreateBloodGroup([FromBody] hrs_BloodGroups bloodGroup)
        {
            try
            {
                var result = await _hrMaster.CreateBloodGroupAsync(bloodGroup);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return CreatedAtAction(nameof(GetBloodGroupById),
                    new { id = ((hrs_BloodGroups)result.ResultObject).ID },
                    result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("UpdateBloodGroup/{id}")]
        public async Task<IActionResult> UpdateBloodGroup(int id, [FromBody] hrs_BloodGroups bloodGroup)
        {
            try
            {
                var result = await _hrMaster.UpdateBloodGroupAsync(id, bloodGroup);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("DeleteBloodGroup/{id}")]
        public async Task<IActionResult> DeleteBloodGroup(int id)
        {
            try
            {
                var result = await _hrMaster.DeleteBloodGroupAsync(id);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(new { success = true, message = "Blood group deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetAllEducations/{lang}")]
        public async Task<IActionResult> GetAllEducations(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllEducationsAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetEducationById/{id}/{lang}")]
        public async Task<IActionResult> GetEducationById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetEducationByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("CreateEducation")]
        public async Task<IActionResult> CreateEducation([FromBody] hrs_Educations education)
        {
            try
            {
                var result = await _hrMaster.CreateEducationAsync(education);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return CreatedAtAction(nameof(GetEducationById),
                    new { id = ((hrs_Educations)result.ResultObject).ID },
                    result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("UpdateEducation/{id}")]
        public async Task<IActionResult> UpdateEducation(int id, [FromBody] hrs_Educations education)
        {
            try
            {
                var result = await _hrMaster.UpdateEducationAsync(id, education);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetAllProfessions/{lang}")]
        public async Task<IActionResult> GetAllProfessions(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllProfessionsAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetProfessionById/{id}/{lang}")]
        public async Task<IActionResult> GetProfessionById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetProfessionByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetAllCompanies/{lang}")]
        public async Task<IActionResult> GetAllCompanies(int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetAllCompaniesAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetCompanyById/{id}/{lang}")]
        public async Task<IActionResult> GetCompanyById(int id, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetCompanyByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetLookupByType/{lookupType}/{lang}")]
        public async Task<IActionResult> GetLookupByType(string lookupType, int lang = 0)
        {
            try
            {
                var result = await _hrMaster.GetLookupByTypeAsync(lookupType, lang);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("SearchLookups")]
        public async Task<IActionResult> SearchLookups([FromQuery] string searchTerm, [FromQuery] string lookupType = null, [FromQuery] int lang = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return BadRequest(new { error = "Search term is required" });
                }

                var result = await _hrMaster.SearchLookupsAsync(searchTerm, lookupType, lang);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("SaveNewEmployeeForm")]
        public async Task<IActionResult> SaveNewEmployeeForm([FromBody] Hrs_NewEmployee newEmployee)
        {
            try
            {
                var result = await _hrMaster.SaveNewEmployeeFormAsync(newEmployee);

                if (result.ErrorCode == 0)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(new
                {
                    success = true,
                    message = "Employee saved successfully",
                    data = result.ResultObject
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("TestConnection")]
        public IActionResult TestConnection()
        {
            return Ok(new
            {
                status = "API is running",
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            });
        }
    }
}