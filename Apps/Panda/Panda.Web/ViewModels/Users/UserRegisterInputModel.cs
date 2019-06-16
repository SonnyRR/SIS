using SIS.MvcFramework.Attributes.Validation;

namespace Panda.Web.ViewModels.Users
{
    public class UserRegisterInputModel
    {
        [RequiredSis]
        [StringLengthSis(5,20, "Username must be between 5 and 20 chars")]
        public string Username { get; set; }

        [RequiredSis]
        [StringLengthSis(5, 20, "Email must be between 5 and 20 chars")]
        public string Email { get; set; }

        [RequiredSis]
        public string Password { get; set; }

        [RequiredSis]
        public string ConfirmPassword { get; set; }
    }
}
