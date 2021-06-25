using Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DAO
{
    public class UserDao
    {
        private DbContextSV db;

        public UserDao()
        {
            db = new DbContextSV();
        }

        public User Find(string email)
        {
            var result = db.Users.SingleOrDefault(x => x.Email.Contains(email));

            return result;

        }

        public User login(string user, string pass)
        {
            var result = db.Users.SingleOrDefault(x => x.Username.Contains(user) && x.Password.Contains(pass));
            return result;
            //if (result == null)
            //{
            //    return 0;
            //}
            //else { return 1; }
        }

        public int loginoutlook(string email)
        {
            var result = db.Users.SingleOrDefault(x => x.Email.Contains(email));
            if (result == null)
            {
                return 0;
            }
            else { return 1; }
        }

        public List<User> ListAll()
        {
            return db.Users.ToList();
        }

        public IEnumerable<User> ListWhere(string keyword)
        {
            IQueryable<User> model = db.Users;
            if (!string.IsNullOrEmpty(keyword))
            return model.Where(x=>x.Username.Contains(keyword)|| x.FullName.Contains(keyword)).ToList();
            return model.ToList();
        }

        public string Insert(User ent_User)
        {
            db.Users.Add(ent_User);
            db.SaveChanges();
            return ent_User.Username;
        }

        public User FindUserName(string userName, string email)
        {
            return db.Users.SingleOrDefault(x => x.Email.Contains(email)|| x.Username.Contains(userName));
        }
    }
}
