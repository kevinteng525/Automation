using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ES1.ES1SPAutoLib;

namespace ES1SPAutoLib.Activity
{
    public class ActivityFactory
    {
        public static ES1Activity CreateActivity(string activityXML, ActivityTypes activityType)
        {
            switch (activityType)
            {
                case ActivityTypes.SharepointOnline:
                    return new SPOActivity(activityXML);
                case ActivityTypes.Sharepoint:
                    return new SPActivity(activityXML);
            }
            throw new Exception("Pls. use correct activity type.");
        }
    }
}
