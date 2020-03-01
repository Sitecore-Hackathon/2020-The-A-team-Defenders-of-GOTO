using GOTO_Usergroup.Foundation.XConnect;
using GOTO_Usergroup.Foundation.XConnect.Interface;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GOTO_Usergroup.Feature.Usergroup.SubmitActions
{
    public class Reload : SubmitActionBase<string>
    {
        public Reload(ISubmitActionData submitActionData) : base(submitActionData)
        {
        }

        protected override bool Execute(string data, FormSubmitContext formSubmitContext)
        {
            var itemId = ((StringInputViewModel)formSubmitContext.Fields.FirstOrDefault(f => f.Name == "ItemId")).Value;
            formSubmitContext.RedirectUrl = Sitecore.Links.LinkManager.GetItemUrl(Sitecore.Context.Database.GetItem(itemId));
            formSubmitContext.RedirectOnSuccess = true;
            return true;
        }

        protected override bool TryParse(string value, out string target)
        {
            target = string.Empty;
            return true;
        }
    }
}