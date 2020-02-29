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
        Task<bool> SaveContactDetails(string email, string fullname);
        Guid CreateList(string name);
    }
}
