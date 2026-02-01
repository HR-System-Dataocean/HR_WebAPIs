using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusHR.Core.Master
{


    //        [Table("hrs_VacationsBalance")]
    public class hrs_VacationsBalance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Employee ID")]
        public int? EmployeeID { get; set; }

        [Display(Name = "Year")]
        public int? Year { get; set; }

        [Display(Name = "Balance")]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Balance must be a positive number")]
        public decimal? Balance { get; set; }

        [Display(Name = "Consumed")]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Consumed must be a positive number")]
        public decimal? Consumed { get; set; }

        [Display(Name = "Remaining")]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Remaining must be a positive number")]
        public decimal? Remaining { get; set; }

        [Display(Name = "Balance Type ID")]
        public int? BalanceTypeID { get; set; }

        [Display(Name = "Expire Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpireDate { get; set; }

        [Display(Name = "Source")]
        [StringLength(150)]
        public string Src { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(500)]
        public string Remarks { get; set; }

        [Display(Name = "Registered User")]
        [StringLength(50)]
        public string Reguser { get; set; }

        [Display(Name = "Registration Date")]
        [DataType(DataType.DateTime)]
        public DateTime? RegDate { get; set; }

        [Display(Name = "End Service Date")]
        [DataType(DataType.Date)]
        public DateTime? EndServiceDate { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Display(Name = "Cancel Date")]
        [DataType(DataType.Date)]
        public DateTime? CancelDate { get; set; }
    }
    
 
 
}
