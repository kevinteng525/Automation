using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;


namespace NeilTest
{
    [TestFixture]
    public class Class1
    {

        [Category("WebId=971")]
        [Test]
        public void Test1()
        {
            Console.WriteLine("Test 1");
            Assert.Pass();
        }
        [Test]
        [Category("2")]
        public void Test2()
        {
            Console.WriteLine("Test 2");
            Assert.Pass();
        }
    }
}
