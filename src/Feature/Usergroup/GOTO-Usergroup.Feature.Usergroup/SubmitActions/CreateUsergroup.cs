using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;
using Sitecore.ListManagement.Services.Model;
using Sitecore.ListManagement.Services.Repositories;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.ListManagement.XConnect.Web;
using Sitecore.Analytics;

namespace GOTO_Usergroup.Feature.Usergroup.SubmitActions
{
    public class CreateUsergroup : SubmitActionBase<string>
    {
        private TemplateID _templateId;
        private Item _rootItem;
        private Database _database;

        public CreateUsergroup(ISubmitActionData submitActionData) : base(submitActionData)
        {
            _templateId = new TemplateID(new ID(Settings.GetSetting("GOTO-Usergroup.Feature.Usergroup.TemplateId", "{B6EF3982-5312-46CA-8934-E3DAD6F0D53A}")));
            _database = Database.GetDatabase(Settings.GetSetting("GOTO-Usergroup.Feature.Usergroup.Database", "master"));
            _rootItem = _database.GetItem(new ID(Settings.GetSetting("GOTO-Usergroup.Feature.Usergroup.RootId", "{8EDAE83B-BB11-49FF-831D-7EA394C9738C}")));
        }

        protected override bool Execute(string data, FormSubmitContext formSubmitContext)
        {
            var subscriptionService = ServiceLocator.ServiceProvider.GetService<ISubscriptionService>();
            using (new SecurityDisabler())
            {
                var item = _rootItem.Add(data, _templateId);
                var membersListId = CreateList($"{data} Members");
                var organizersListId = CreateList($"{data} Organizers");
                item.Editing.BeginEdit();
                item["Title"] = data;
                item["Members"] = membersListId.ToString();
                item["Organizers"] = organizersListId.ToString();
                item.Editing.AcceptChanges();

                subscriptionService.Subscribe(membersListId, Tracker.Current.Contact.ContactId);
                subscriptionService.Subscribe(organizersListId, Tracker.Current.Contact.ContactId);
            }
            return true;
            
        }

        protected override bool TryParse(string value, out string target)
        {
            target = string.Empty;
            return true;
        }

        protected virtual Guid CreateList(string name)
        {
            var contactListRepository = ServiceLocator.ServiceProvider.GetService<IFetchRepository<ContactListModel>>();
            var id = Guid.NewGuid();
            var newContactList = new ContactListModel()
            {
                Id = id.ToString("B"),
                Name = name,
                Description = name,
                Owner = "Administrator"
            };
            contactListRepository.Add(newContactList);
            return id;
        }

        //get contacts (for later)
        //https://doc.sitecore.com/developers/93/sitecore-experience-manager/en/the-list-manager-api.html
        //int batchSize = 200; // Size of the batch
        //string[] facets =
        //{
        //    CollectionModel.FacetKeys.PersonalInformation,
        //    CollectionModel.FacetKeys.ListSubscriptions
        //}; // Contact facets to retrieve
        //        var contactListProvider = ServiceLocator.ServiceProvider.GetService<IContactListProvider>();
        //        var contactProvider = ServiceLocator.ServiceProvider.GetService<IContactProvider>();
        //        var contactList = contactListProvider.Get(contactListId, cultureInfo);
        //        var contactBatchEnumerator = contactProvider.GetContactBatchEnumerator(
        //            contactList,
        //            batchSize,
        //             facets);
        //while (contactBatchEnumerator.MoveNext())
        //{    
        //var contacts = contactBatchEnumerator.Current;
        //        // write your logic here
        //        contacts.ToList();
        //}
    }

    public class CreateUsergroupData
    {
        public string ShortName { get; set; }
        public string Title { get; set; }
    }
}