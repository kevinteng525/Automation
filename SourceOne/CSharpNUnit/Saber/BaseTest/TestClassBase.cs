using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Saber.Common;
using Saber.TestData;
using Saber.TestEnvironment;
using Saber.TestMetadata;

namespace Saber.BaseTest
{
    public abstract class TestClassBase
    {
        public TestClassBase()
        {

        }
        static public string AssemblyDirectory
        {
            get
            {
                return AssemblyHelper.GetAssemblePath();
            }
        }
    }
}
