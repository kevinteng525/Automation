using System;
using System.Collections;
using System.Collections.Generic;
//using EMC.SourceOne.SP.Service.ES1Entity;
using EMC.Interop.ExJDFAPI;

using EMC.Interop.ExSTLContainers;


namespace Saber.S1CommonAPILib
{

    public class S1OrganizationalPolicyHelper 
    {
        /// <summary>
        /// Create S1 policy
        /// </summary>
        /// <param name="policy">The parameters needed to create the policy</param>
        /// <returns>the policy created</returns>
        public static int CreatePolicy(S1OrganizationalPolicy policy)
        {
            if(IsExistsByName(policy.Name))
            {
                return GetByName(policy.Name).id;
            }
            else
            {
                IExPolicy p = (IExPolicy)S1Context.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_Policy);
                p.name = policy.Name;
                p.description = policy.Description;
                try
                {
                    p.Save();
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to create policy: " + p.name, e);
                }
                return p.id;
            }

        }

        /// <summary>
        /// Check whether the policy with the name exist or not
        /// </summary>
        /// <param name="name">the policy name to check</param>
        /// <returns>id of the policy</returns>
        public static bool IsExistsByName(string name)
        {
            IExPolicy policy = GetByName(name);
            if (policy != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Delete the policy with the id
        /// </summary>
        /// <param name="id">the policy id to delete</param>
        public static void DeleteById(int id)
        {
            IExPolicy policy = GetById(id);
            if (policy != null)
            {
                policy.Delete();
            }
        } 

        /// <summary>
        /// Get the policy by id
        /// </summary>
        /// <param name="id">the policy id</param>
        /// <returns>the policy with the id</returns>
        internal static IExPolicy GetById(int id)
        {
            return (IExPolicy)S1Context.JDFAPIMgr.GetPolicyByID(id);
        }

        /// <summary>
        /// Get the policy by name
        /// </summary>
        /// <param name="name">the policy name</param>
        /// <returns>the policy with the name</returns>
        internal static IExPolicy GetByName(String name)
        {
            IExVector policies = (IExVector) S1Context.JDFAPIMgr.GetPolicies();
            foreach (IExPolicy nextPolicy in policies)
            {
                if (nextPolicy.name.ToLower() == name.ToLower())
                {
                    return nextPolicy;
                }
            }
            return null;
        }

        /// <summary>
        /// Get all the policies
        /// </summary>
        /// <returns>list of all the policies</returns>
        internal static List<IExPolicy> GetAll()
        {
            IExVector policies = (IExVector)S1Context.JDFAPIMgr.GetPolicies();
            List<IExPolicy> results = new List<IExPolicy>();

            if (policies != null)
            {
                foreach (IExPolicy policy in policies)
                {
                    results.Add(policy);
                }
            }

            return results;
        }

    }
}