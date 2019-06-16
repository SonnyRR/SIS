namespace SULS.App.ViewModels.Users
{
    using SIS.MvcFramework.Attributes.Validation;

    public class UsersLoginInputModel
    {

        private const string PASSWORD_OUT_OF_RANGE_MSG = "Password must be between 6 and 20 characters long";
        private const string USERNAME_LENGTH_MSG = "Username must be between 6 and 20 characters long";

        [RequiredSis]        
        [StringLengthSis(5,20, USERNAME_LENGTH_MSG)]
        public string Username { get; set; }

        [RequiredSis]
        [StringLengthSis(6,20, PASSWORD_OUT_OF_RANGE_MSG)]
        public string  Password { get; set; }
    }
}
