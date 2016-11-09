using System.Xml.Linq;
using Common.SystemCommon;

namespace VerifyLib.VerifyItems
{
    public class EventLogItem : VerifyItem
    {
        public string LogName;

        public EventLogItem(XElement node, VerifyGroup group)
            : base(node, group)
        {
            LogName = GetAttributeValue("logName");
        }

        protected override void PrepareTest()
        {
            base.PrepareTest();

            ExpectValue = Exist;
        }

        protected override void Verify()
        {
            try
            {
                ActualValue = WindwosEventLog.IsLogExist(LogName)? Exist : NotExist;
            }
            catch
            {
                ActualValue = NotExist;
            }

            VerifyResult = ActualValue == ExpectValue ? VerifyResult.Pass : VerifyResult.Failed;
        }
    }
}
