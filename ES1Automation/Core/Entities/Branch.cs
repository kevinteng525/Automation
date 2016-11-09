using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model
{
    public partial class Branch
    {
        public static Branch GetBranchById(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Branches.Find(id);
            }
        }

        public static Branch GetBranchByName(string name)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Branches.Where(b => b.Name.ToLower() == name.ToLower()).FirstOrDefault();
            }
        }

        public static List<Branch> GetAllBranches()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Branches.ToList<Branch>();
            }
 
        }
        public static List<Branch> GetAllBranchesByType(int type)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return (
                           from branch in context.Branches.Where(p => p.Type == (int)type).OrderBy( p=> p.Name)
                           select branch
                      ).ToList();
            }

        }
        public static Branch UpdateBranch(int id, object instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Branch branch = context.Branches.Find(id);
                if (branch != null)
                {
                    context.Entry(branch).CurrentValues.SetValues(instance);
                    context.SaveChanges();
                    return branch;
                }
                else
                {
                    return null;
                }
            }
        }

        public static Branch AddOrUpdateBranch(Branch branch)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Branch existingBranch = context.Branches.Where(b => b.Name.ToLower() == branch.Name.ToLower() && b.Type==branch.Type && b.ProductId==branch.ProductId).FirstOrDefault();

                if (existingBranch != null)
                {
                    existingBranch.Description = branch.Description;
                    existingBranch.Path = branch.Path;
                    context.Entry(existingBranch).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return existingBranch;
                }
                else
                {
                    Branch b = context.Branches.Add(branch);
                    context.SaveChanges();
                    return b;
                }
            }
        }

        public static void DeleteBranchById(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Branch branch = context.Branches.Find(id);
                if (branch != null)
                {
                    context.Branches.Remove(branch);
                    context.SaveChanges();
                }
            }
        }
    }
}
