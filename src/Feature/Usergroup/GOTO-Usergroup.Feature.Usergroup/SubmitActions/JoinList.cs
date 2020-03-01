using GOTO_Usergroup.Foundation.XConnect;
using GOTO_Usergroup.Foundation.XConnect.Interface;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GOTO_Usergroup.Feature.Usergroup.SubmitActions
{
    public class JoinList : SubmitActionBase<string>
    {
        private IXConnectService _xconnectService;

        public JoinList(ISubmitActionData submitActionData) : base(submitActionData)
        {
            _xconnectService = ServiceLocator.ServiceProvider.GetService<IXConnectService>();
        }

        protected override bool Execute(string data, FormSubmitContext formSubmitContext)
        {
            var userGroupId = Guid.Parse(((StringInputViewModel)formSubmitContext.Fields.FirstOrDefault(f => f.Name == "ListId")).Value);
            var email = Sitecore.Analytics.Tracker.Current.Contact.Identifiers.FirstOrDefault(i => i.Source == Constants.XConnectSourceName)?.Identifier;
            _xconnectService.SubscribeContact(email, new List<Guid> { userGroupId });
            System.Threading.Thread.Sleep(2000);

            return true;
        }

        protected override bool TryParse(string value, out string target)
        {
            target = string.Empty;
            return true;
        }
    }
}