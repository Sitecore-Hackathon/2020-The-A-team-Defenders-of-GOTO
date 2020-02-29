using GOTO_Usergroup.Foundation.XConnect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOTO_Usergroup.Foundation.XConnect.Interface
{
    public interface IXConnectService
    {
        Task<bool> SubscribeContact(string email, IEnumerable<Guid> listIds);
        Task<bool> Unsubscribe(string email, Guid listId);
        Task<bool> IsSubscribed(string email, Guid listId);
        Task<bool> SaveContactDetails(string email, string fullname);
        Guid CreateList(string name);
        List<Subscriber> GetSubscribersFromList(Guid listId);
    }
}
