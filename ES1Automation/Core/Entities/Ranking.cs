using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model
{
    public partial class Ranking
    {
        public static List<Ranking> GetAllRankings()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Rankings.OrderBy(r => r.Name).ToList<Ranking>();
            }
        }

        public static Ranking AddOrUpdateRanking(Ranking ranking)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Ranking existingRanking = context.Rankings.Where(r=>r.Name.ToLower() == ranking.Name.ToLower()).FirstOrDefault();
                if (existingRanking != null)
                {
                    existingRanking.Description = ranking.Description;
                    context.Entry(existingRanking).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return existingRanking;
                }
                else
                {
                    context.Rankings.Add(ranking);
                    context.SaveChanges();
                    return ranking;
                }
            }
        }

        public static Ranking GetRankingById(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Rankings.Find(id);
            }
        }

        public static Ranking UpdateRanking(int id, object instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Ranking ranking = context.Rankings.Find(id);
                if (ranking != null)
                {
                    context.Entry(ranking).CurrentValues.SetValues(instance);
                    context.SaveChanges();
                    return ranking;
                }
                else
                {
                    return null;
                }
            }
        }

        public static void DeleteRanking(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Ranking ranking = context.Rankings.Find(id);
                if (null != ranking)
                {
                    context.Rankings.Remove(ranking);
                    context.SaveChanges();
                }
            }
        }
    }
}
