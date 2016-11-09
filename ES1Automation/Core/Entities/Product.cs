using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model
{
    
    public partial class Product
    {
        #region basic operations for products

        public static List<Product> GetAllProducts()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Products.ToList<Product>();
            }
        }

        public static Product GetProductByID(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Products.Find(id);
            }
        }

        public static Product GetProductByName(string name)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Products.Where(p => p.Name == name).FirstOrDefault();
            }
        }

        public static Product AddProduct(Product instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                int productId = (from p in context.Products select p.ProductId).Max() + 1;

                if (Product.GetProductByID(productId) != null)
                {
                    return null;
                }

                instance.ProductId = productId;

                Product product = context.Products.Add(instance);
                context.SaveChanges();
                return product;
            }
        }

        public static Product UpdateProduct(int productID, Object instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Product product = context.Products.Find(productID);
                context.Entry(product).CurrentValues.SetValues(instance);
                context.SaveChanges();
                return product;
            }
        }

        public static void DeleteProduct(int productID)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                Product p = GetProductByID(productID);
                if (p != null)
                {
                    context.Products.Attach(p);
                    context.Products.Remove(p);
                    context.SaveChanges();
                }
            }
        }

        #endregion


        #region get all the test cases( include test suites) for this product
        /// <summary>
        /// Get all the root test suites for the product
        /// </summary>
        /// <param name="productID">Product Id</param>
        /// <returns>List<Build>List of root test suites</returns>
        public static List<TestSuite> GetAllRootTestSuitesForProduct(int productID)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Get all the root test cases for the product
        /// </summary>
        /// <param name="productID">Product Id</param>
        /// <returns>List<Build>List of root test cases</returns>
        public static List<TestCase> GetAllRootTestCasesForProduct(int productID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region get all the builds for this project

        /// <summary>
        /// Get all the builds for the product
        /// </summary>
        /// <param name="productID">product ID</param>
        /// <returns>List<Build>List of builds</returns>
        public static List<Build> GetAllBuildsForProduct(int productID)
        {
            return Build.GetAllBuildsByProductAndStatus(productID, BuildStatus.Success).ToList<Build>();
        }
        
        #endregion

        #region get all the builds for this project

        /// <summary>
        /// Get all the branches for the product
        /// </summary>
        /// <param name="productID">product ID</param>
        /// <returns>List<Build>List of branches</returns>
        public static List<Branch> GetAllBranchesForProduct(int productID, int type)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Branches.Where(b => b.ProductId == productID && b.Type== type).OrderBy(b => b.Name).ToList();
            }
        }

        #endregion

        #region get all the releases for this project

        /// <summary>
        /// Get all the releases for the product
        /// </summary>
        /// <param name="productID">product ID</param>
        /// <returns>List<Build>List of releases</returns>
        public static List<Release> GetAllReleasesForProduct(int productID, int type)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.Releases.Where(r => r.ProductId == productID && r.Type == type).OrderBy(r => r.Name).ToList();
            }
        }

        #endregion
    }
}
