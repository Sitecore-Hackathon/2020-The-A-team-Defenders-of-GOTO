using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;
using Sitecore.Security.Accounts;
using Sitecore.Security.Authentication;
using System.Linq;

namespace GOTO_Usergroup.Feature.Account.SubmitActions.LoginUser
{
    public class LoginUser : SubmitActionBase<string>
    {
        public LoginUser(ISubmitActionData submitActionData) : base(submitActionData)
        {
        }

        protected override bool Execute(string data, FormSubmitContext formSubmitContext)
        {

            var shortname = ((StringInputViewModel)formSubmitContext.Fields.FirstOrDefault(f => f.Name == "ShortName")).Value;
            var title = ((StringInputViewModel)formSubmitContext.Fields.FirstOrDefault(f => f.Name == "Title")).Value;


            return true;
        }



        protected virtual User Login(string userName, string password)
        {
            var accountName = string.Empty;
            var domain = global::Sitecore.Context.Domain;
            if (domain != null)
            {
                accountName = domain.GetFullName(userName);
            }

            var result = AuthenticationManager.Login(accountName, password);
            if (!result)
            {
                return null;
            }

            var user = AuthenticationManager.GetActiveUser();
            return user;
        }


        private bool UsernameOrPasswordFieldIsNull(LoginUserFormFields field)
        {
            Assert.ArgumentNotNull(field, nameof(field));
            return field.Username == null || field.Password == null;
        }

        private bool UsernameOrPasswordValueIsNull(LoginUserFieldValues values)
        {
            Assert.ArgumentNotNull(values, nameof(values));
            return string.IsNullOrEmpty(values.Username) || string.IsNullOrEmpty(values.Password);
        }

        private bool AbortForm(FormSubmitContext formSubmitContext)
        {
            formSubmitContext.Abort();
            return false;
        }

        protected override bool TryParse(string value, out string target)
        {
            target = string.Empty;
            return true;
        }

        internal class LoginUserFormFields
        {
            public IViewModel Username { get; set; }
            public IViewModel Password { get; set; }

            public LoginUserFieldValues GetFieldValues()
            {
                return new LoginUserFieldValues
                {
                    Username = Helper.FieldHelper.GetValue(Username),
                    Password = Helper.FieldHelper.GetValue(Password)
                };
            }
        }

        internal class LoginUserFieldValues
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}