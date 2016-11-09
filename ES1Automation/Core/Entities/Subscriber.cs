using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model
{
    public partial class Subscriber
    {
        public static List<Subscriber> GetAllSubscribers()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Subscribers.ToList();
            }
        }

        public static Subscriber GetSubscriberById(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Subscribers.Find(id);
            }
        }

        public static List<string> GetSubscribersEmailByProjectId(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                List<string> l = (from u in context.Users
                                  where (from s in context.Subscribers
                                         where s.ProjectId == id
                                         select s.UserId).Contains(u.UserId)
                                  select u.Email).ToList();
                l.RemoveAll(e => e == string.Empty);
                return l;
            }
        }
        public static Subscriber AddOrUpdateSubscriber(Subscriber instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Subscriber existingSubscriber = context.Subscribers.Where(s => s.ProjectId == instance.ProjectId && s.UserId == instance.UserId).FirstOrDefault();
                if (null == existingSubscriber)
                {
                    context.Subscribers.Add(instance);
                    context.SaveChanges();
                    return instance;
                }
                return existingSubscriber;
            }
        }

        public static void DeleteSubscriber(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Subscriber subscriber = context.Subscribers.Find(id);
                if (null != subscriber)
                {
                    context.Subscribers.Remove(subscriber);
                    context.SaveChanges();
                }
            }
        }
    }
}
