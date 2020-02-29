using GOTO_Usergroup.Feature.Account.Helper;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;
using Sitecore.Security.Accounts;
using System;


namespace GOTO_Usergroup.Feature.Account.SubmitActions.RegisterUser
{
    public class RegisterUser : SubmitActionBase<RegisterUserData>
    {
        public RegisterUser(ISubmitActionData submitActionData) : base(submitActionData)
        {
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
                Email = FieldHelper.GetFieldById(data.EmailFieldId, formSubmitContext.Fields),
                Password = FieldHelper.GetFieldById(data.PasswordFieldId, formSubmitContext.Fields),
                FullName = FieldHelper.GetFieldById(data.FullNameFieldId, formSubmitContext.Fields),
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