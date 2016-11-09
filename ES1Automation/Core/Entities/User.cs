using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Common.ActiveDirecotry;
using System.Runtime.Serialization;
using Common.FileCommon;

namespace Core.Model
{
    public enum UserType
    {
        ATF = 0,
        AD  = 1,
    }

    public enum UserRole
    {
        SystemAdmin = 0,
        User        = 1,
        Viewer      = 2,
    }

    public partial class User
    {
        public UserType UserType
        {
            get { return (UserType)Type; }
            set { Type = (int)value; }
        }
        public UserRole UserRole
        {
            get { return (UserRole)Role; }
            set { Role = (int)value; }
        }

        public string PlainText
        {
            get { return GZipHelper.Decompress(Password); }
            set { Password = GZipHelper.Compress(value); }
        }

        #region basic operations

        public static List<User> GetAllUsers()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Users.ToList<User>();
            }
        }

        public static User GetUserById(int userId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Users.Find(userId);
            }
        }

        public static User Add(User user)
        {
            try {
                using (ES1AutomationEntities context = new ES1AutomationEntities())
                {
                    User u = context.Users.Add(user);
                    context.SaveChanges();
                    return u;
                }
            }
            catch (Exception e)
            {
                ATFEnvironment.Log.logger.Error(e);
                return null;
            }
        }

        public void Update()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                context.Entry(this).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static User Update(int userId, Object instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                User user = context.Users.Find(userId);
                context.Entry(user).CurrentValues.SetValues(instance);
                context.SaveChanges();
                return user;
            }           
        }

        public static void Delete(User user)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                context.Users.Attach(user);
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }

        public static User GetUserByUserName(string name)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Users.Where(u => u.Username.ToLower() == name.ToLower()).FirstOrDefault();
            }
        }

        public static void Delete(int userId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Delete(GetUserById(userId));
            }
        }

        #endregion

        /// <summary>
        /// Get User by Username and Password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static User GetUser(string username, string password)
        {
            string defaultAuthenticationDomain = ATFConfiguration.GetStringValue("DefaultAuthenticationDomain");

            IList<string> pair = username.Split(new[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);

            string domain = string.Empty;

            if (pair.Count == 1)
            {
                username = pair[0];
            }
            else if (pair.Count == 2)
            {
                if (pair[0].ToLower() == "corp" || pair[0].ToLower() == defaultAuthenticationDomain.ToLower())
                {
                    domain = defaultAuthenticationDomain;
                }
                else
                {
                    //we could not support the authentication against other LDAP server.
                    return null;
                }
                username = pair[1];
            }
            else
            {
                return null;
            }

            User user = null;

            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                user = context.Users.Where(u => u.Username == username && u.IsActive).SingleOrDefault();
            }

            // add user to db if user not find
            if (user == null)
            {
                domain = defaultAuthenticationDomain;
                if (ADUser.Authenticate(username, password, domain))
                {
                    //add user to db
                    user = new User();
                    user.Description = string.Format("{0} from {1}", username, domain);
                    user.Username = username;
                    user.UserRole = UserRole.Viewer;
                    user.UserType = UserType.AD;
                    user.IsActive = true;
                    user.Email = ADUser.GetEmailAddress(username, password, domain);
                    User.Add(user);
                    return user;
                }
                else
                {
                    //no this user in db, failed to authenticate against the default domain
                    return null;
                }
            }
            else
            {
                // do validation
                switch (user.UserType)
                {
                    case UserType.ATF:

                        if (user.PlainText != password)
                        {
                            return null;
                        }

                        break;

                    case UserType.AD:

                        if (string.IsNullOrWhiteSpace(domain))
                        {
                            domain = defaultAuthenticationDomain;
                        }

                        if (!ADUser.Authenticate(username, password, domain))
                        {
                            return null;
                        }

                        break;

                    default:

                        return null;
                }
            }
            return user;
        }

        /// <summary>
        /// Validate a user
        /// </summary>
        /// <param name="username"> username of a user </param>
        /// <param name="password"> password of a user</param>
        /// <returns></returns>
        public static bool ValidateUser(string username, string password)
        {
            return GetUser(username, password) != null;
        }

        public static List<Project> GetSubscribedProjects(int userId)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
               return (from subscriber in context.Subscribers
                       where subscriber.UserId == userId
                       select subscriber.Project).ToList();
            }
        }
    }
}
