using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ES1.ES1SPAutoLib
{
    public enum AgeUnit : int
    {
        Days = 1,
        Weeks = 7,
        Months = 30,
        Years = 365
    }

    public enum DateFilterMethod
    {
        IncludeAll = 0,
        Dated = 1,
        Aged = 2
    }

    public class ProcessName
    {
        public const String SPO_JBS_ProcessName = "ExSharePointCommonJBS";
        public const String SPO_JBC_ProcessName = "ExSharePointCommonJBC";
    }

    public enum ActivityID : int
    {
        SPO = 1704471942,
        SP = 286660062
    }

    public class ConstantItems
    {
        public const String Activity_Version_All = "all";
        public const String Activity_Version_Latest = "latest";
        public const String Activity_Date_Aged = "aged";
        public const String Activity_Date_Dated = "dated";
        public const String Activity_Date_All = "all";
        public const String Activity_Dated_After = "after";
        public const String Activity_Dated_Before = "before";
        public const String Activity_Dated_Between = "between";
        public const String Activity_Dated_On = "exactlyon";
        public const String Activity_Aged_Newer = "newer";
        public const String Activity_Aged_Between = "between";
        public const String Activity_Aged_Older = "older";
        public const String Activity_Aged_Days = "days";
        public const String Activity_Aged_Weeks = "weeks";
        public const String Activity_Aged_Months = "months";
        public const String Activity_Aged_Years = "years";
        public const String Activity_BaseUpon_Created = "created";
        public const String Activity_BaseUpon_Modified = "modified";
        public const String Activity_Action_Copy = "copy";
        public const String Activity_Action_Move = "move";
        public const String Activity_Schedule_Now = "now";
        public const String Activity_Schedule_Once = "once";
        public const String Activity_Schedule_Daily = "daily";
        public const String Activity_Schedule_Weekly = "weekly";
        public const String Activity_Schedule_Monthly = "monthly";
        public const String Activity_Schedule_Monday = "1";
        public const String Activity_Schedule_Tuesday = "2";
        public const String Activity_Schedule_Wednesday = "3";
        public const String Activity_Schedule_Thursday = "4";
        public const String Activity_Schedule_Friday = "5";
        public const String Activity_Schedule_Saturday = "6";
        public const String Activity_Schedule_Sunday = "7";
        
    }
}
