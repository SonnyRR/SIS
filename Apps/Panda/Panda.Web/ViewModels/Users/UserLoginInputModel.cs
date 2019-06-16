using SIS.MvcFramework.Attributes.Validation;

namespace Panda.Web.ViewModels.Users
{
    public class UserLoginInputModel
    {
        [RequiredSis]
        [StringLengthSis(5, 20, "Username must be between 5 and 20 chars")]
        public string Username { get; set; }

        [RequiredSis]
        public string Password { get; set; }
    }
}
