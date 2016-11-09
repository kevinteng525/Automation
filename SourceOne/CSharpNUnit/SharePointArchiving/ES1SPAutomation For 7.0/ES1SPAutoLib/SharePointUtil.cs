using System;
using System.Collections.Generic;
using System.Text;

using EMC.Interop.ExSTLContainers;
using EMC.Interop.ExDataSet;
using System.Runtime.Serialization;
using System.IO;

namespace ES1.ES1SPAutoLib
{
    public class SharePointUtil
    {
        public static String GetFirstError(IExVector validationResult)
        {
            if (validationResult != null && validationResult.Count > 0)
            {
                foreach (IExPropertyValidationInfo error in validationResult)
                {
                    if (!error.isWarning) return error.errorDescription;
                }
            }
            return null;
        }

        public static string ToXml<T>(T value)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, value);
            ms.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(ms);
            return reader.ReadToEnd();
        }
    }
}
