using System;
using System.Collections;
using System.Collections.Generic;
//using EMC.SourceOne.SP.Service.ES1Entity;
using EMC.Interop.ExJDFAPI;

using EMC.Interop.ExSTLContainers;


namespace ES1.ES1SPAutoLib
{
    public class PolicyManager 
    {

        //public static bool IsReplaceTheFormerDataWhenCreate = true;

        private static IExPolicy PrparePolicy()
        {
            return  (IExPolicy)SourceOneContext.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_Policy);
        }

        public static IExPolicy CreatePolicy(IExPolicy policy)
        {
            IExPolicy tempPolicy = GetPolicyByName(policy.name);

            if (tempPolicy != null)
            {
                return tempPolicy;
            }
            try
            {
                policy.Save();
            }
            catch(Exception e)
            {
                throw new Exception("Failed to create policy: " + policy.name, e);
            }
            policy = GetPolicyById(policy.id);
            return policy;
        }

        public static IExPolicy CreatePolicy(String name, String description = "Auto test")
        {
            IExPolicy policy = (IExPolicy)SourceOneContext.JDFAPIMgr.CreateNewObject(exJDFObjectType.exJDFObjectType_Policy);
            policy.name = name;
            policy.description = description;

            return CreatePolicy(policy);
        }

        public static void DeletePolicy(String name)
        {
            IExPolicy policy = GetPolicyByName(name);
            if (policy != null)
            {
                policy.Delete();
            }
        }

        /*public static IExPolicy GetPolicyIdByName(String name)
        {
            IExPolicy policy = GetPolicyByName(name);
            if (policy == null)
            {
                return null;
            }
            return policy;
        }*/

        private static IExPolicy GetPolicyById(int policyId)
        {
            return (IExPolicy)SourceOneContext.JDFAPIMgr.GetPolicyByID(policyId);
        }

        public static IExPolicy GetPolicyByName(String policyName)
        {
            IExVector policies = (IExVector) SourceOneContext.JDFAPIMgr.GetPolicies();
            foreach (IExPolicy nextPolicy in policies)
            {
                if (nextPolicy.name == policyName)
                {
                    return nextPolicy;
                }
            }
            return null;
        }

        /*public static List<IExPolicy> GetAllPolicy()
        {
            IExVector policies = (IExVector)SourceOneContext.JDFAPIMgr.GetPolicies();
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

        public static void DeletePolicy(IExPolicy policy)
        {
            policy.Delete();
        }*/
    }
}