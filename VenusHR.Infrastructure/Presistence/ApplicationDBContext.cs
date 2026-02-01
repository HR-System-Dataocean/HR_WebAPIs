using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusHR.Application.Common.Interfaces;
using VenusHR.Core.Login;
using VenusHR.Core.Master;
using VenusHR.Core.SelfService;

namespace VenusHR.Infrastructure.Presistence
{
    public class ApplicationDBContext : DbContext, IApplicationDBContext
    {
        #region Ctor
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
         : base(options)
        {
        }
        #endregion
        #region DbSet
        public DbSet<AppSetting> AppSettings { get; set; }
        public DbSet<SS_RequestTypes> SS_RequestTypes { get; set; }
        public DbSet<Hrs_Employees> Hrs_Employees { get; set; }
        public DbSet<Hrs_Projects> Hrs_Projects { get; set; }
        public DbSet<sys_Locations> sys_Locations { get; set; }
        public DbSet<Sys_Companies> Sys_Companies { get; set; }
        public DbSet<SS_RequestAction> SS_RequestActions { get; set; }
        public virtual DbSet<Hrs_EmployeesVacations> Hrs_EmployeesVacations { get; set; }
        public virtual DbSet<Hrs_Mobile_Attendance> Hrs_Mobile_Attendance { get; set; }
        public virtual DbSet<hrs_VacationsTypes> hrs_VacationsTypes { get; set; }
        public virtual DbSet<hrs_EmployeeVacationOpenBalance> hrs_EmployeeVacationOpenBalance { get; set; }
        public virtual DbSet<hrs_Contracts> hrs_Contracts { get; set; }
        public virtual DbSet<hrs_ContractsVacations> hrs_ContractsVacations { get; set; }
        public virtual DbSet<hrs_EmployeesClasses> hrs_EmployeesClasses { get; set; }
        public virtual DbSet<hrs_VacationsBalance> hrs_VacationsBalance { get; set; }
        public virtual DbSet<hrs_Positions> hrs_Positions { get; set; }
   
        public virtual DbSet<sys_Cities> sys_Cities { get; set; }
        public virtual DbSet<sys_Departments> sys_Departments { get; set; }
        public virtual DbSet<sys_Nationalities> sys_Nationalities { get; set; }
        public virtual DbSet<sys_Banks> sys_Banks { get; set; }
        public virtual DbSet<hrs_Religions> hrs_Religions { get; set; }
        public virtual DbSet<hrs_MaritalStatus> hrs_MaritalStatus { get; set; }
        public virtual DbSet<hrs_BloodGroups> hrs_BloodGroups { get; set; }
        public virtual DbSet<hrs_Educations> hrs_Educations { get; set; }
        public virtual DbSet<Hrs_NewEmployee> Hrs_NewEmployee { get; set; }
        public virtual DbSet<Hrs_Profession> Hrs_Professions { get; set; }
        public virtual DbSet<SysUser> Sys_Users { get; set; }

        public virtual DbSet<SS_Configuration> SS_Configuration { get; set; }
        public virtual DbSet<SS_VacationRequest> SS_VacationRequest { get; set; }
        public virtual DbSet<SS_AdvanceHousingRequest> SS_AdvanceHousingRequest { get; set; }
        public virtual DbSet<SS_AdvanceSalaryRequest> SS_AdvanceSalaryRequest { get; set; }
        public virtual DbSet<SS_AnnualTicketRelatedRequests> SS_AnnualTicketRelatedRequests { get; set; }
        public virtual DbSet<SS_AssaultEscalationFormRequest> SS_AssaultEscalationFormRequest { get; set; }
        public virtual DbSet<SS_BankAccountUpdate> SS_BankAccountUpdate { get; set; }
        public virtual DbSet<SS_BusinessOrtrainingTravel> SS_BusinessOrtrainingTravel { get; set; }
        public virtual DbSet<SS_ContactInformationUpdate> SS_ContactInformationUpdate { get; set; }
        public virtual DbSet<SS_ChamberofCommerceLetterRequest> SS_ChamberofCommerceLetterRequest { get; set; }
        public virtual DbSet<SS_DependentsInformationUpdate> SS_DependentsInformationUpdate { get; set; }
        public virtual DbSet<SS_DaycareSupportReaquest> SS_DaycareSupportReaquest { get; set; }
        public virtual DbSet<SS_EducationFeesCompensationApplication> SS_EducationFeesCompensationApplication { get; set; }
        public virtual DbSet<SS_EducationSupportRequest> SS_EducationSupportRequest { get; set; }
        public virtual DbSet<SS_EmployeeFileUpdate> SS_EmployeeFileUpdate { get; set; }
        public virtual DbSet<SS_EndOfServiceRequest> SS_EndOfServiceRequest { get; set; }
        public virtual DbSet<SS_ExecuseRequest> SS_ExecuseRequest { get; set; }
        public virtual DbSet<SS_ExitEntryRequest> SS_ExitEntryRequest { get; set; }
        public virtual DbSet<SS_ExperienceRate> SS_ExperienceRate { get; set; }
        public virtual DbSet<SS_GrievanceFormRequest> SS_GrievanceFormRequest { get; set; }
        public virtual DbSet<SS_InterviewEvaluationFormRequest> SS_InterviewEvaluationFormRequest { get; set; }
        public virtual DbSet<SS_LoanLetterRequest> SS_LoanLetterRequest { get; set; }
        public virtual DbSet<SS_MedicalInsuranceAdjustments> SS_MedicalInsuranceAdjustments { get; set; }
        public virtual DbSet<SS_OccurrenceVarianceReportingLetter> SS_OccurrenceVarianceReportingLetter { get; set; }
        public virtual DbSet<SS_OtherLegalDocumentUpdates> SS_OtherLegalDocumentUpdates { get; set; }
        public virtual DbSet<SS_OtherLetterRequest> SS_OtherLetterRequest { get; set; }
        public virtual DbSet<SS_OvertimeRequest> SS_OvertimeRequest { get; set; }
        public virtual DbSet<SS_PaySlipRequest> SS_PaySlipRequest { get; set; }
        public virtual DbSet<SS_PhysiciansPrivilegingFormRequest> SS_PhysiciansPrivilegingFormRequest { get; set; }
         public virtual DbSet<SS_RequestStatuesTypes> SS_RequestStatuesTypes { get; set; }
        public virtual DbSet<SS_RequestTypes> SS_RequestType { get; set; }
        public virtual DbSet<SS_ResignationReason> SS_ResignationReason { get; set; }
        public virtual DbSet<SS_ScfhsletterRequest> SS_ScfhsletterRequest { get; set; }
        public virtual DbSet<SS_TrainingRequest> SS_TrainingRequest { get; set; }
        public virtual DbSet<SS_UserActions> SS_UserActions { get; set; }
        public virtual DbSet<SS_UserType> SS_UserType { get; set; }
        public virtual DbSet<SS_VisaRequest> SS_VisaRequest { get; set; }
        public virtual DbSet<SS_VisaType> SS_VisaType { get; set; }
        public virtual DbSet<SS_VFollowup> SS_VFollowup { get; set; }
        public virtual DbSet<sys_Documents> sys_Documents { get; set; }
        public virtual DbSet<sys_DocumentsDetails> sys_DocumentsDetails { get; set; }
        public virtual DbSet<sys_ObjectsAttachments> sys_ObjectsAttachments { get; set; }
 











        #endregion
        #region Methods
        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
        #endregion
    }
}
