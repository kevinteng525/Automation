using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using ES1Common.Exceptions;
using Core.Providers;
using System.Xml.Linq;

namespace Core.Model
{
    public enum ProviderCategory
    {
        Environment = 0,
        Build = 1,
        TestCase = 2,
    }

    public partial class Provider
    {
        public ProviderCategory ProviderCategory
        {
            get { return (ProviderCategory)Category; }
            set { Category = (int)value; }
        }

        public static Provider GetProviderById(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Providers.Find(id);
            }
        }

        /// <summary>
        /// Use reflect to create IProvider instance
        /// </summary>
        /// <returns> IProvider instance </returns>
        public virtual IProvider CreateProvider()
        {
            object obj = null;

            try
            {
                obj = Assembly.GetAssembly(typeof(IProvider)).CreateInstance(Type);
            }
            catch (FileNotFoundException)
            {
                throw new FrameworkException("Provider", string.Format("{0} not find", Path));
            }
            catch (Exception ex)
            {
                throw new FrameworkException("Provider", ex.Message);
            }

            IProvider provider = obj as IProvider;

            if (provider != null)
            {
                provider.Provider = this;
                provider.ApplyConfig(Config);
            }
            else
            {
                throw new FrameworkException("Provider", string.Format("Failed to created provider: {0} ", Type));
            }

            return provider;
        }

        public static IList<Provider> GetProvidersByCategory(ProviderCategory categroy)
        {
            using (var context = new ES1AutomationEntities())
            {
                return (
                            from provider in context.Providers.Where(p => p.Category == (int)categroy)
                            select provider
                       ).ToList();
            }
        }
    }
}
