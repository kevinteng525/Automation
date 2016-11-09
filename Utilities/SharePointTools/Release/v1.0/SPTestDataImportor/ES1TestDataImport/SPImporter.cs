using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPUtilities.Extensions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.IO;

namespace ES1TestDataImport
{
    public static class SPImporter
    {
        private static Dictionary<string, byte[]> Dict = new Dictionary<string, byte[]>();

        public static void AddYear(StreamWriter log, SPList lists, string fileNames, int year)
        {
            for (int i = 1; i <= 12; i++)
            {
                AddMonth(log, lists, fileNames, year, i);
            }
        }

        public static void AddMonth(StreamWriter log, SPList lists, string fileNames, int year, int month)
        {
            int Days = DateTime.DaysInMonth(year, month);
            for (int i = 1; i <= Days; i++)
            {
                AddFile(log, lists, fileNames, year, month, i);

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305")]
        public static void AddFile(StreamWriter log, SPList list, string fileName, int year, int month, int day)
        {
            string fileExt = "";
            string fileSize = "";
            switch (day % 5)
            {
                case 0:
                    fileExt = "doc";
                    break;
                case 1:
                    fileExt = "jpg";
                    break;
                case 2:
                    fileExt = "tif";
                    break;
                case 3:
                    fileExt = "xlsx";
                    break;
                case 4:
                    fileExt = "docx";
                    break;
            }

            switch (day % 6)
            {
                case 0:
                    fileSize = "K20";
                    break;
                case 1:
                    fileSize = "K50";
                    break;
                case 2:
                    fileSize = "K100";
                    break;
                case 3:
                    fileSize = "K200";
                    break;
                case 4:
                    fileSize = "K500";
                    break;
                case 5:
                    fileSize = "K1000";
                    break;
            }

            DateTime createdDate = new DateTime(year, month, day, (year + month + day) % 24, (year + month + day) % 60, (year * month * day) % 60);
            fileName = fileName + year.ToString("D4") + month.ToString("D2") + day.ToString("D2");

            DateTime modifiedDate = createdDate.AddDays(5);
            Guid UniqueId = list.CreateFile(fileName + "." + fileExt, Dict[fileSize + "." + fileExt]).ChangeCreatedDate(createdDate).ChangeModifiedDate(modifiedDate).UniqueId;

            log.WriteLine(UniqueId.ToString() + "\t" + fileName + "\t" + list.Title + "\t" + fileExt.ToString() + "\t" + fileSize.ToString() + "\t" + createdDate.ToString() + "\t" + modifiedDate.ToString());

        }

        public static void GetAllFiles(string dirPath)
        {
            DirectoryInfo DI = new DirectoryInfo(dirPath);
            FileInfo[] FIs = DI.GetFiles();
            string FileName;
            for (int i = 0; i < FIs.Length; i++)
            {
                FileName = FIs[i].Name;
                Dict[FileName] = GetTemplate(FIs[i].FullName);
            }
        }
        public static byte[] GetTemplate(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] buffur = new byte[fs.Length];
            fs.Read(buffur, 0, (int)fs.Length);
            return buffur;
        }
    }
}
