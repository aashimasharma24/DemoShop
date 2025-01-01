using DemoShop.Core.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Manager.Repositories.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();
        User GetByGUID(String guid);
        User GetByUsername(String username);
        void Add(User user);
        void Update(User user);
        void Delete(User user);
    }
}
