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
using Sitecore.ListManagement.Providers;
using Sitecore.ListManagement.XConnect.Web;
using GOTO_Usergroup.Foundation.XConnect.Models;

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

        public virtual async Task<bool> IsSubscribed(string email, Guid listId)
        {
            try
            {
                using (var client = SitecoreXConnectClientConfiguration.GetClient())
                {
                    var contact = await client.GetAsync(new IdentifiedContactReference(Constants.XConnectSourceName, email), new ExpandOptions(ListSubscriptions.DefaultFacetKey)).ConfigureAwait(false);

                    var listSubscriptions = contact.ListSubscriptions() ?? new ListSubscriptions();
                    return listSubscriptions.Subscriptions.Any(s => s.ListDefinitionId == listId);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual async Task<bool> Unsubscribe(string email, Guid listId)
        {
            try
            {
                using (var client = SitecoreXConnectClientConfiguration.GetClient())
                {
                    var contact = await client.GetAsync(new IdentifiedContactReference(Constants.XConnectSourceName, email), new ExpandOptions(ListSubscriptions.DefaultFacetKey)).ConfigureAwait(false);

                    var listSubscriptions = contact.ListSubscriptions() ?? new ListSubscriptions();
                    if (listSubscriptions.Subscriptions.Any(s => s.ListDefinitionId == listId))
                    {
                        listSubscriptions.Subscriptions.RemoveAll(s => s.ListDefinitionId == listId);
                        client.SetListSubscriptions(contact, listSubscriptions);
                        client.Submit();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                
            }
            return false;
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
                    var firstname = index > 0 ? fullname.Substring(0, index) : string.Empty;
                    var lastname = index > 0 ? fullname.Substring(index + 1) : fullname;

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

        public virtual List<Subscriber> GetSubscribersFromList(Guid listId)
        {
            var results = new List<Subscriber>();
            var batchSize = 200;
            string[] facets =
            {
                CollectionModel.FacetKeys.PersonalInformation,
                CollectionModel.FacetKeys.EmailAddressList,
                CollectionModel.FacetKeys.ListSubscriptions
            };
            var contactListProvider = ServiceLocator.ServiceProvider.GetService<IContactListProvider>();
            var contactProvider = ServiceLocator.ServiceProvider.GetService<IContactProvider>();
            var contactList = contactListProvider.Get(listId, Sitecore.Context.Culture);
            var contactBatchEnumerator = contactProvider.GetContactBatchEnumerator(contactList, batchSize, facets);
            while (contactBatchEnumerator.MoveNext())
            {
                var contacts = contactBatchEnumerator.Current;
                foreach (var contact in contacts)
                {
                    var email = contact.Emails()?.PreferredEmail?.SmtpAddress;

                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        var personal = contact.Personal();
                        results.Add(new Subscriber
                        {
                            Email = email,
                            FirstName = personal?.FirstName,
                            LastName = personal?.LastName
                        });
                    }
                }
            }

            return results;
        }

    }
}