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
    public class CreateUsergroup : SubmitActionBase<string>
    {
        private TemplateID _templateId;
        private Item _rootItem;
        private Database _database;
        private IXConnectService _xconnectService;

        public CreateUsergroup(ISubmitActionData submitActionData) : base(submitActionData)
        {
            _templateId = new TemplateID(new ID(Settings.GetSetting("GOTO-Usergroup.Feature.Usergroup.CreateUsergroup.TemplateId", "{B6EF3982-5312-46CA-8934-E3DAD6F0D53A}")));
            _database = Database.GetDatabase(Settings.GetSetting("GOTO-Usergroup.Feature.UsergroupCreateUsergroup..Database", "master"));
            _rootItem = _database.GetItem(new ID(Settings.GetSetting("GOTO-Usergroup.Feature.Usergroup.CreateUsergroup.RootId", "{8EDAE83B-BB11-49FF-831D-7EA394C9738C}")));
            
            _xconnectService = ServiceLocator.ServiceProvider.GetService<IXConnectService>();
        }

        protected override bool Execute(string data, FormSubmitContext formSubmitContext)
        {
            var shortname = ((StringInputViewModel)formSubmitContext.Fields.FirstOrDefault(f => f.Name == "ShortName")).Value;
            var title = ((StringInputViewModel)formSubmitContext.Fields.FirstOrDefault(f => f.Name == "Title")).Value;
            
            using (new SecurityDisabler())
            {
                var item = _rootItem.Add(shortname, _templateId);
                var membersListId = _xconnectService.CreateList($"{shortname} Members");
                var organizersListId = _xconnectService.CreateList($"{shortname} Organizers");
                item.Editing.BeginEdit();
                item["Title"] = title;
                item["Members"] = membersListId.ToString();
                item["Organizers"] = organizersListId.ToString();
                item.Editing.AcceptChanges();

                _xconnectService.SubscribeContact("test", "test", new List<Guid> { membersListId, organizersListId });
            }
            return true;
        }

        protected override bool TryParse(string value, out string target)
        {
            target = string.Empty;
            return true;
        }
    }
}