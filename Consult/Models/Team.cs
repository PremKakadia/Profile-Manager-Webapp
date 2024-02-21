using System.ComponentModel.DataAnnotations;

namespace Consult.Models
{
    public class Team
    {
        private string? _FName;
        [Display(Name = "First Name")]
        public string? FName
        {
            get { return _FName; }
            set { _FName = value?.Trim(); }
        }

        private string? _LName;
        [Display(Name = "Last Name")]
        public string? LName
        {
            get { return _LName; }
            set { _LName = value?.Trim(); }
        }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        private string? _Qualifications;
        public string? Qualifications
        {
            get { return _Qualifications; }
            set { _Qualifications = value?.Trim(); }
        }

        private string? _Aos;
        [Display(Name = "Area of Specialization")]
        public string? Aos
        {
            get { return _Aos; }
            set { _Aos = value?.Trim(); }
        }

        private string? _AboutMe;
        [Display(Name = "Area of Specialization")]
        public string? AboutMe
        {
            get { return _AboutMe; }
            set { _AboutMe = value?.Trim(); }
        }

        public int? TeachingExp { get; set; }
        public int? IndustryExp { get; set; }

        private string? _LinkedIn;
        public string? LinkedIn
        {
            get { return _LinkedIn; }
            set { _LinkedIn = value?.Trim(); }
        }

        public IFormFile? Photo { get; set; }

        public string? PhotoUrl { get; set; }
    }
    public class TeamLogIn
    {
        public string? Id { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }
    }
}
