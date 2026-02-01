// HRMasterEndpoints.cs
using Microsoft.AspNetCore.Mvc;
using VenusHR.Application.Common.Interfaces.HR_Master;
using VenusHR.Core.Master;
using WorkFlow_EF;

namespace YourNamespace
{
    public static class HRMasterEndpoints
    {
        public static void MapHRMasterEndpoints(this WebApplication app)
        {
            // 🔹 1. System Lookups
            app.MapGet("/api/hr-master/lookups/all/{lang}", GetAllMasterData);
            app.MapGet("/api/hr-master/lookups/by-type/{lookupType}/{lang}", GetLookupByType);
            app.MapGet("/api/hr-master/lookups/search", SearchLookups);

            // 🔹 2. Cities
            app.MapGet("/api/hr-master/cities/all/{lang}", GetAllCities);
            app.MapGet("/api/hr-master/cities/{id:int}/{lang}", GetCityById);

            // 🔹 3. Nationalities
            app.MapGet("/api/hr-master/nationalities/all/{lang}", GetAllNationalities);
            app.MapGet("/api/hr-master/nationalities/{id:int}/{lang}", GetNationalityById);
            app.MapPost("/api/hr-master/nationalities", CreateNationality);
            app.MapPut("/api/hr-master/nationalities/{id:int}", UpdateNationality);

            // 🔹 4. Banks
            app.MapGet("/api/hr-master/banks/all/{lang}", GetAllBanks);
            app.MapGet("/api/hr-master/banks/{id:int}/{lang}", GetBankById);
            app.MapPost("/api/hr-master/banks", CreateBank);
            app.MapPut("/api/hr-master/banks/{id:int}", UpdateBank);

            // 🔹 5. Religions
            app.MapGet("/api/hr-master/religions/all/{lang}", GetAllReligions);
            app.MapGet("/api/hr-master/religions/{id:int}/{lang}", GetReligionById);

            // 🔹 6. Marital Status
            app.MapGet("/api/hr-master/marital-status/all/{lang}", GetAllMaritalStatus);
            app.MapGet("/api/hr-master/marital-status/{id:int}/{lang}", GetMaritalStatusById);

            // 🔹 7. Blood Groups
            app.MapGet("/api/hr-master/blood-groups/all/{lang}", GetAllBloodGroups);
            app.MapGet("/api/hr-master/blood-groups/{id:int}/{lang}", GetBloodGroupById);
            app.MapPost("/api/hr-master/blood-groups", CreateBloodGroup);
            app.MapPut("/api/hr-master/blood-groups/{id:int}", UpdateBloodGroup);
            app.MapDelete("/api/hr-master/blood-groups/{id:int}", DeleteBloodGroup);

            // 🔹 8. Educations
            app.MapGet("/api/hr-master/educations/all/{lang}", GetAllEducations);
            app.MapGet("/api/hr-master/educations/{id:int}/{lang}", GetEducationById);
            app.MapPost("/api/hr-master/educations", CreateEducation);
            app.MapPut("/api/hr-master/educations/{id:int}", UpdateEducation);

            // 🔹 9. Professions
            app.MapGet("/api/hr-master/professions/all/{lang}", GetAllProfessions);
            app.MapGet("/api/hr-master/professions/{id:int}/{lang}", GetProfessionById);

            // 🔹 10. Companies
            app.MapGet("/api/hr-master/companies/all/{lang}", GetAllCompanies);
            app.MapGet("/api/hr-master/companies/{id:int}/{lang}", GetCompanyById);

            // 🔹 11. Employees
            app.MapPost("/api/hr-master/employees/new", SaveNewEmployeeForm);

            // 🔹 12. Health Check
            app.MapGet("/api/hr-master/health", TestConnection);
        }

        // =========== Implementation Methods ===========

        // System Lookups
        private static async Task<IResult> GetAllMasterData(
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetSystemLookupsAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> GetLookupByType(
            string lookupType,
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetLookupByTypeAsync(lookupType, lang);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> SearchLookups(
            [FromQuery] string searchTerm,
            [FromServices] IHRMaster service,
            [FromQuery] string? lookupType = null,
            [FromQuery] int lang = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return Results.BadRequest(new { error = "Search term is required" });
                }

                var result = await service.SearchLookupsAsync(searchTerm, lookupType, lang);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }
        // Cities
        private static async Task<IResult> GetAllCities(
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetAllCitiesAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> GetCityById(
            int id,
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetCityByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return Results.NotFound(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // Nationalities
        private static async Task<IResult> GetAllNationalities(
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetAllNationalitiesAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> GetNationalityById(
            int id,
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetNationalityByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return Results.NotFound(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> CreateNationality(
            [FromBody] sys_Nationalities nationality,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.CreateNationalityAsync(nationality);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                var createdNationality = (sys_Nationalities)result.ResultObject;
                return Results.Created(
                    $"/api/hr-master/nationalities/{createdNationality.ID}/{0}",
                    createdNationality
                );
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> UpdateNationality(
            int id,
            [FromBody] sys_Nationalities nationality,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.UpdateNationalityAsync(id, nationality);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // Banks
        private static async Task<IResult> GetAllBanks(
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetAllBanksAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> GetBankById(
            int id,
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetBankByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return Results.NotFound(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> CreateBank(
            [FromBody] sys_Banks bank,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.CreateBankAsync(bank);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                var createdBank = (sys_Banks)result.ResultObject;
                return Results.Created(
                    $"/api/hr-master/banks/{createdBank.ID}/{0}",
                    createdBank
                );
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> UpdateBank(
            int id,
            [FromBody] sys_Banks bank,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.UpdateBankAsync(id, bank);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // Religions
        private static async Task<IResult> GetAllReligions(
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetAllReligionsAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> GetReligionById(
            int id,
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetReligionByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return Results.NotFound(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // Marital Status
        private static async Task<IResult> GetAllMaritalStatus(
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetAllMaritalStatusAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> GetMaritalStatusById(
            int id,
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetMaritalStatusByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return Results.NotFound(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // Blood Groups
        private static async Task<IResult> GetAllBloodGroups(
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetAllBloodGroupsAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> GetBloodGroupById(
            int id,
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetBloodGroupByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return Results.NotFound(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> CreateBloodGroup(
            [FromBody] hrs_BloodGroups bloodGroup,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.CreateBloodGroupAsync(bloodGroup);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                var createdBloodGroup = (hrs_BloodGroups)result.ResultObject;
                return Results.Created(
                    $"/api/hr-master/blood-groups/{createdBloodGroup.ID}/{0}",
                    createdBloodGroup
                );
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> UpdateBloodGroup(
            int id,
            [FromBody] hrs_BloodGroups bloodGroup,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.UpdateBloodGroupAsync(id, bloodGroup);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> DeleteBloodGroup(
            int id,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.DeleteBloodGroupAsync(id);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(new { success = true, message = "Blood group deleted successfully" });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // Educations
        private static async Task<IResult> GetAllEducations(
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetAllEducationsAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> GetEducationById(
            int id,
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetEducationByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return Results.NotFound(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> CreateEducation(
            [FromBody] hrs_Educations education,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.CreateEducationAsync(education);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                var createdEducation = (hrs_Educations)result.ResultObject;
                return Results.Created(
                    $"/api/hr-master/educations/{createdEducation.ID}/{0}",
                    createdEducation
                );
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> UpdateEducation(
            int id,
            [FromBody] hrs_Educations education,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.UpdateEducationAsync(id, education);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // Professions
        private static async Task<IResult> GetAllProfessions(
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetAllProfessionsAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> GetProfessionById(
            int id,
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetProfessionByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return Results.NotFound(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // Companies
        private static async Task<IResult> GetAllCompanies(
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetAllCompaniesAsync(lang);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        private static async Task<IResult> GetCompanyById(
            int id,
            int lang,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.GetCompanyByIdAsync(id, lang);

                if (result.ErrorCode == 0)
                {
                    return Results.NotFound(new { error = result.ErrorMessage });
                }

                return Results.Ok(result.ResultObject);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // Employees
        private static async Task<IResult> SaveNewEmployeeForm(
            [FromBody] Hrs_NewEmployee newEmployee,
            [FromServices] IHRMaster service)
        {
            try
            {
                var result = await service.SaveNewEmployeeFormAsync(newEmployee);

                if (result.ErrorCode == 0)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }

                return Results.Ok(new
                {
                    success = true,
                    message = "Employee saved successfully",
                    data = result.ResultObject
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        // Health Check
        private static IResult TestConnection()
        {
            return Results.Ok(new
            {
                status = "API is running",
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            });
        }
    }
}