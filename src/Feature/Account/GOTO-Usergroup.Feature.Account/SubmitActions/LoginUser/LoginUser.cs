using Sitecore.Analytics;
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

            var email = ((StringInputViewModel)formSubmitContext.Fields.FirstOrDefault(f => f.Name == "Username")).Value;
            var password = ((StringInputViewModel)formSubmitContext.Fields.FirstOrDefault(f => f.Name == "Password")).Value;
            var user = Login(email, password);
            if(user==null)
            {
                // Login failed
            }
            else
            {
                // Identify
                Tracker.Current.Session.IdentifyAs(Foundation.XConnect.Constants.XConnectSourceName, email);
            }
            return user != null;
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

        protected override bool TryParse(string value, out string target)
        {
            target = string.Empty;
            return true;
        }
    
    }
}