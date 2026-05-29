using System;
using System.Collections.Generic;

namespace VenusHR.Core.SelfService
{
    public class EmployeePersonalInfoDto
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string ArabicName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public int? Age { get; set; }
        public string BirthCountry { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Religion { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
    }

    public class EmployeeContactInfoDto
    {
        public string MobileNo { get; set; } = string.Empty;
        public string WorkEmail { get; set; } = string.Empty;
        public string PersonalEmail { get; set; } = string.Empty;
        public string PhoneNo { get; set; } = string.Empty;
    }

    public class EmployeeOrganizationInfoDto
    {
        public string Branch { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Project { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class EmployeeEmploymentInfoDto
    {
        public DateTime? JoinDate { get; set; }
        public string Sponsor { get; set; } = string.Empty;
        public string ContractType { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string EmployeeClass { get; set; } = string.Empty;
        public string GradeSteps { get; set; } = string.Empty;
    }

    public class EmployeeBankingInfoDto
    {
        public string Bank { get; set; } = string.Empty;
        public string BankAccount { get; set; } = string.Empty;
    }

    public class EmployeeIdentityTravelInfoDto
    {
        public string IdentityNo { get; set; } = string.Empty;
        public string PassportNo { get; set; } = string.Empty;
        public string PassportIssueDate { get; set; } = string.Empty;
        public string PassportExpiryDate { get; set; } = string.Empty;
        public string IdentityIssueDate { get; set; } = string.Empty;
        public string IdentityExpiryDate { get; set; } = string.Empty;
    }

    public class EmployeeDocumentItemDto
    {
        public int Id { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Attachment { get; set; } = string.Empty;
    }

    public class EmployeeDependentItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string IdentityNo { get; set; } = string.Empty;
        public string PassportNo { get; set; } = string.Empty;
    }

    public class EmployeeProfileSectionsDto
    {
        public EmployeePersonalInfoDto PersonalInformation { get; set; } = new();
        public EmployeeContactInfoDto ContactInformation { get; set; } = new();
        public EmployeeOrganizationInfoDto OrganizationInformation { get; set; } = new();
        public EmployeeEmploymentInfoDto EmploymentInformation { get; set; } = new();
        public EmployeeBankingInfoDto BankingInformation { get; set; } = new();
        public EmployeeIdentityTravelInfoDto IdentityAndTravelInformation { get; set; } = new();
        public List<EmployeeDocumentItemDto> Documents { get; set; } = new();
        public List<EmployeeDependentItemDto> Dependents { get; set; } = new();
    }
}
