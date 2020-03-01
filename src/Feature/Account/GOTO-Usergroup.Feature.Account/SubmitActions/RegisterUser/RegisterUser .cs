using GOTO_Usergroup.Feature.Account.Helper;
using GOTO_Usergroup.Foundation.XConnect.Interface;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;
using Sitecore.Security.Accounts;
using Sitecore.Web;
using System;
using Microsoft.Extensions.DependencyInjection;
using GOTO_Usergroup.Foundation.XConnect;

namespace GOTO_Usergroup.Feature.Account.SubmitActions.RegisterUser
{
    public class RegisterUser : SubmitActionBase<RegisterUserData>
    {
        private IXConnectService _xconnectService;

        public RegisterUser(ISubmitActionData submitActionData) : base(submitActionData)
        {
            _xconnectService = ServiceLocator.ServiceProvider.GetService<IXConnectService>();
        }

        protected override bool Execute(RegisterUserData data, FormSubmitContext formSubmitContext)
        {
            Assert.ArgumentNotNull(data, nameof(data));
            Assert.ArgumentNotNull(formSubmitContext, nameof(formSubmitContext));

            var fields = GetFormFields(data, formSubmitContext);

            Assert.IsNotNull(fields, nameof(fields));

            if (EmailOrPasswordFieldsIsNull(fields))
            {
                return AbortForm(formSubmitContext);
            }

            var values = fields.GetFieldValues();

            if (EmailOrPasswordsIsNull(values))
            {
                return AbortForm(formSubmitContext);
            }

            // Fall back: If profile id is not provided - we set default:
            data.ProfileId = "{AE4C4969-5B7E-4B4E-9042-B2D8701CE214}";

            var result = Register(values.Email, values.Password, values.FullName, data.ProfileId);

            if (!result)
            {
                return AbortForm(formSubmitContext);
            }
            return true;
        }

        protected virtual bool Register(string email, string password, string name, string profileId)
        {
            Assert.ArgumentNotNullOrEmpty(email, nameof(email));
            Assert.ArgumentNotNullOrEmpty(password, nameof(password));

            try
            {
                var user = User.Create(global::Sitecore.Context.Domain.GetFullName(email), password);
                user.Profile.Email = email;

                if (!string.IsNullOrEmpty(profileId))
                {
                    user.Profile.ProfileItemId = profileId;
                }

                user.Profile.FullName = name;
                user.Profile.Save();

                //Identify the contact in XDB
                Sitecore.Analytics.Tracker.Current.Session.IdentifyAs(Constants.XConnectSourceName, email);
                //Save the email and name to XDB
                _xconnectService.SaveContactDetails(email, name);
            }
            catch (Exception ex)
            {
                Log.SingleError("Register user failed", ex);
                return false;
            }

            return true;
        }

        private RegisterUserFormFields GetFormFields(RegisterUserData data, FormSubmitContext formSubmitContext)
        {
            Assert.ArgumentNotNull(data, nameof(data));
            Assert.ArgumentNotNull(formSubmitContext, nameof(formSubmitContext));

            return new RegisterUserFormFields
            {
                Email = FieldHelper.GetFieldValueByName("Email", formSubmitContext.Fields),
                Password = FieldHelper.GetFieldValueByName("Password Confirmation", formSubmitContext.Fields),
                FullName = FieldHelper.GetFieldValueByName("Full Name", formSubmitContext.Fields),
            };
        }

        private bool EmailOrPasswordFieldsIsNull(RegisterUserFormFields field)
        {
            Assert.ArgumentNotNull(field, nameof(field));
            return field.Email == null || field.Password == null;
        }

        private bool EmailOrPasswordsIsNull(RegisterUserFieldValues values)
        {
            Assert.ArgumentNotNull(values, nameof(values));
            return string.IsNullOrEmpty(values.Email) || string.IsNullOrEmpty(values.Password);
        }

        private bool AbortForm(FormSubmitContext formSubmitContext)
        {
            formSubmitContext.Abort();
            return false;
        }

        internal class RegisterUserFormFields
        {
            public IViewModel Email { get; set; }
            public IViewModel Password { get; set; }
            public IViewModel FullName { get; set; }

            public RegisterUserFieldValues GetFieldValues()
            {
                return new RegisterUserFieldValues
                {
                    Email = FieldHelper.GetValue(Email),
                    Password = FieldHelper.GetValue(Password),
                    FullName = FieldHelper.GetValue(FullName)
                };
            }
        }

        internal class RegisterUserFieldValues
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FullName { get; set; }
        }
    }
}