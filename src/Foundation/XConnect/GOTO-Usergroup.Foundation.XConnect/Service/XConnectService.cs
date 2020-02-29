using GOTO_Usergroup.Foundation.XConnect.Interface;
using Sitecore.DependencyInjection;
using Sitecore.ListManagement.Services.Model;
using Sitecore.ListManagement.Services.Repositories;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.Configuration;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Security.Accounts;
using Sitecore.Configuration;

namespace GOTO_Usergroup.Foundation.XConnect.Service
{
    public class XConnectService : IXConnectService
    {
        private string _apiUser;

        public XConnectService()
        {
            _apiUser = Settings.GetSetting("GOTO-Usergroup.Feature.Usergroup.ApiUser", "sitecore\\SystemListManager");
        }

        public virtual async Task<bool> SubscribeContact(string email, IEnumerable<Guid> listIds)
        {
            try
            {
                using (var client = SitecoreXConnectClientConfiguration.GetClient())
                {
                    var contact = await client.GetAsync(new IdentifiedContactReference(Constants.XConnectSourceName, email), new ExpandOptions(ListSubscriptions.DefaultFacetKey)).ConfigureAwait(false);

                    var listSubscriptions = contact.ListSubscriptions() ?? new ListSubscriptions();
                    foreach (var listId in listIds)
                    {
                        if (listSubscriptions.Subscriptions.Any(s => s.ListDefinitionId == listId))
                        {
                            continue;
                        }
                        var subscription = new ContactListSubscription(DateTime.UtcNow, true, listId);
                        listSubscriptions.Subscriptions.Add(subscription);
                    }
                    client.SetListSubscriptions(contact, listSubscriptions);
                    await client.SubmitAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual async Task<bool> SaveContactDetails(string email, string fullname)
        {
            try
            {
                using (var client = SitecoreXConnectClientConfiguration.GetClient())
                {
                    var contact = await client.GetAsync(new IdentifiedContactReference(Constants.XConnectSourceName, email), new ExpandOptions(EmailAddressList.DefaultFacetKey, PersonalInformation.DefaultFacetKey)).ConfigureAwait(false);

                    fullname = fullname.Trim();
                    var index = fullname.LastIndexOf(' ');
                    var firstname = index > 0 ? fullname.Substring(0, index - 1) : string.Empty;
                    var lastname = index > 0 ? fullname.Substring(index) : fullname;

                    var personalInfo = contact.Personal() ?? new PersonalInformation();
                    personalInfo.FirstName = firstname;
                    personalInfo.LastName = lastname;
                    client.SetPersonal(contact, personalInfo);

                    var emailList = contact.Emails();
                    if (emailList == null)
                    {
                        emailList = new EmailAddressList(new EmailAddress("email", true), "Preferred");
                    }
                    else
                    {
                        emailList.PreferredEmail.SmtpAddress = email;
                    }
                    client.SetEmails(contact, emailList);

                    await client.SubmitAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual Guid CreateList(string name)
        {
            using (new UserSwitcher(_apiUser, true))
            {
                var contactListRepository = ServiceLocator.ServiceProvider.GetService<IFetchRepository<ContactListModel>>();
                var id = Guid.NewGuid();
                var newContactList = new ContactListModel()
                {
                    Id = id.ToString("B"),
                    Name = name,
                    Description = name,
                    Owner = _apiUser
                };
                contactListRepository.Add(newContactList);
                return id;
            }
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
}