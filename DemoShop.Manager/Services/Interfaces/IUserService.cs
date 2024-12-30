using DemoShop.Core.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Manager.Services.Interfaces
{
    public interface IUserService
    {
        IEnumerable<User> GetAll();
        User GetByGUID(String guid);
        void Add(User user);
        void Update(String guid, User user);
        void Delete(String guid);

    }
}
