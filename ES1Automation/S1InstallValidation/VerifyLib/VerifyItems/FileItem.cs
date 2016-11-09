using System.Xml.Linq;
using System;
using Common.FileCommon;

namespace VerifyLib.VerifyItems
{
    public class FileItem : VerifyItem
    {
        public string Type;

        public FileItem(XElement node, VerifyGroup group)
            : base(node, group)
        {
            Type = GetAttributeValue("type").ToUpper();

            if (Type != "FILE" && Type != "FOLDER")
            {
                throw new Exception("Config Error: In FileGroup, Type should be File or Folder");
            }
        }

        protected override void PrepareTest()
        {
            base.PrepareTest();

            ExpectValue = Exist;
        }

        protected override void Verify()
        {
            switch (Type)
            {
                case "FILE":
                    ActualValue = FileHelper.IsExistsFile(CheckValue) ? Exist : NotExist;
                    break;

                case "FOLDER":
                    ActualValue = FileHelper.IsExistsFolder(CheckValue) ? Exist : NotExist;
                    break;

                default:
                    VerifyResult = VerifyResult.Failed;
                    break;
            }

            VerifyResult = ActualValue == ExpectValue ? VerifyResult.Pass : VerifyResult.Failed;

            if (VerifyResult == VerifyResult.Failed)
            {
                Information = CheckValue;
            }
        }
    }
}
