using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendDominoNotes
{
    public class Schedule
    {        
        int mIntervalTime = -1;
        DateTime mStartTime;
        DateTime mEndTime;
        //DateTime mStartDate;
        //DateTime mEndDate;
        //bool mAlldayFlag;

        public DateTime StartTime
        {
            get { return mStartTime; }
            set { mStartTime = value; }
        }

        public DateTime EndTime
        {
            get { return mEndTime; }
            set { mEndTime = value; }
        }

        //public DateTime StartDate
        //{
        //    get { return mStartDate; }
        //    set { mStartDate = value; }
        //}

        //public DateTime EndDate
        //{
        //    get { return mEndDate; }
        //    set { mEndDate = value; }
        //}

        //public bool AllDayFlag
        //{
        //    get { return mAlldayFlag; }
        //    set { mAlldayFlag = value; }
        //}

        public int IntervalTime
        {
            get { return mIntervalTime; }
            set { mIntervalTime = value; }
        }
    }
} 