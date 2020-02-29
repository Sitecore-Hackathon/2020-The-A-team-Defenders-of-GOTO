using GOTO_Usergroup.Foundation.XConnect.Interface;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
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
    public class CreateEvent : SubmitActionBase<string>
    {
        private TemplateID _templateId;
        private Database _database;
        private IXConnectService _xconnectService;

        public CreateEvent(ISubmitActionData submitActionData) : base(submitActionData)
        {
            _templateId = new TemplateID(new ID(Settings.GetSetting("GOTO-Usergroup.Feature.Usergroup.CreateEvent.TemplateId", "{EB03188C-C8DA-49C5-8A41-9B3AE8DC2A75}")));
            _database = Database.GetDatabase(Settings.GetSetting("GOTO-Usergroup.Feature.Usergroup.CreateEvent.Database", "master"));            
            _xconnectService = ServiceLocator.ServiceProvider.GetService<IXConnectService>();
        }

        protected override bool Execute(string data, FormSubmitContext formSubmitContext)
        {
            var itemId = ((StringInputViewModel)formSubmitContext.Fields.FirstOrDefault(f => f.Name == "ItemId")).Value;
            var host = ((StringInputViewModel)formSubmitContext.Fields.FirstOrDefault(f => f.Name == "Host")).Value;
            var title = ((StringInputViewModel)formSubmitContext.Fields.FirstOrDefault(f => f.Name == "Title")).Value;
            var description = ((MultipleLineTextViewModel)formSubmitContext.Fields.FirstOrDefault(f => f.Name == "Description")).Value;
            var date = ((DateViewModel)formSubmitContext.Fields.FirstOrDefault(f => f.Name == "Date")).Value;
            var address = ((MultipleLineTextViewModel)formSubmitContext.Fields.FirstOrDefault(f => f.Name == "Address")).Value;

            using (new SecurityDisabler())
            {
                var rootItem = _database.GetItem(itemId);
                var item = rootItem.Add(title, _templateId);
                var attendeesListId = _xconnectService.CreateList($"{item.Name} - {title} Attendees");
                item.Editing.BeginEdit();
                item["Host"] = host;
                item["Title"] = title;
                item["Description"] = description;
                item["Date"] = Sitecore.DateUtil.ToIsoDate(date.Value);
                item["Address"] = address;
                item["Attendees"] = new ID(attendeesListId).ToString();
                item.Editing.AcceptChanges();

                _xconnectService.SubscribeContact("test", "test", new List<Guid> { attendeesListId });
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