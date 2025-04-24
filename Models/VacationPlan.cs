using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationManagement.Models
{
    public class VacationPlan:EntityBase
    {
        [Display(Name ="Vacation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:dd-mm-yyyy}")]
        public DateTime? VacationDate { get; set; }
        public int RequestVacationId { get; set; }
        [ForeignKey("RequestVacationId")]
        public RequestVacation? RequestVacation { get; set; }

    }
}
