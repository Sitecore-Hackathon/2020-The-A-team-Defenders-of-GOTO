using GOTO_Usergroup.Foundation.XConnect;
using GOTO_Usergroup.Foundation.XConnect.Interface;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GOTO_Usergroup.Feature.Usergroup.RenderingConditions
{
    public class ContactSubscribedToListFromField<T> : WhenCondition<T> where T : RuleContext
    {
        private IXConnectService _xconnectService;

        public ContactSubscribedToListFromField()
        {
            _xconnectService = ServiceLocator.ServiceProvider.GetService<IXConnectService>();
        }

        public string FieldName { get; set; }

        protected override bool Execute(T ruleContext)
        {
            if (string.IsNullOrWhiteSpace(FieldName) || string.IsNullOrWhiteSpace(ruleContext.Item[FieldName]))
                return false;

            var fieldId = Guid.Parse(ruleContext.Item[FieldName]);

            var email = Sitecore.Analytics.Tracker.Current?.Contact?.Identifiers?.FirstOrDefault(i => i.Source == Constants.XConnectSourceName)?.Identifier;
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return _xconnectService.IsSubscribed(email, fieldId).Result;
        }
    }
}