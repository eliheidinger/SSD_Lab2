using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Lab1.Models
{
    public class Employer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Employer Name is required.")]
        [Display(Name = "Employer Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Website is required.")]
        [Url(ErrorMessage = "Invalid website URL.")]
        public string Website { get; set; }

        [Display(Name = "Incorporated Date")]
        public DateTime? IncorporatedDate { get; set; }
    }
}
