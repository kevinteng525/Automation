using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.Windows.Forms;


namespace SendDominoNotes
{
    public class TaskItem 
    {
        #region private members

        private NotesMessage mMessage;
        private int mMessageCount;
        private Schedule mSchedule;
        private NotesGenerator mNotesGenerator = null;

        private TaskStatus mStatus = TaskStatus.Null;
        private string mName = String.Empty;

        private System.Timers.Timer timer = null;

        #endregion

        #region contructors

        public TaskItem(string name, NotesMessage message, int count, Schedule schedule, NotesGenerator notesGenerator)
        {
            mName = name;
            mMessage = message;
            mMessageCount = count;
            mSchedule = schedule;
            mNotesGenerator = notesGenerator;
            timer = new System.Timers.Timer();
            timer.Interval = mSchedule.IntervalTime * 60000;
            if (IsExpired())
            {
                mStatus = TaskStatus.Expired;
            }
        }
        
        #endregion

        #region properties

        public NotesMessage NotesMessage
        {
            get { return mMessage; }
        }

        public int MessageCount
        {
            get { return mMessageCount; }
        }

        public Schedule MessageSchedule
        { 
            get { return mSchedule; } 
        }

        public string TaskName
        {
            get { return mName; }
        }

        public TaskStatus Status
        {
            get { return mStatus; }
        }
        
        #endregion

        #region public methods

        public void Start()
        {            
            if (IsExpired())
            {
                mStatus = TaskStatus.Expired;
                MessageBox.Show("The schedule is expired.");
                return;
            }

            if (mStatus == TaskStatus.Null || mStatus == TaskStatus.Stopped)
            {
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                timer.Enabled = true;
                mStatus = TaskStatus.Running;
            }
        }

        public void Stop()
        {
            if (mStatus == TaskStatus.Running)
            {
                timer.Elapsed -= new ElapsedEventHandler(timer_Elapsed);
                timer.Enabled = false;
                mStatus = TaskStatus.Stopped;
            }
        }

        public void ResetSchedule()
        { 

        }

        #endregion

        #region private methods

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsInSchedule())
            {
                SendNotes();
            }
            if (IsExpired())
            {                
                mStatus = TaskStatus.Expired;
                timer.Elapsed -= new ElapsedEventHandler(timer_Elapsed);
                timer.Enabled = false;
            }
        }

        private void SendNotes()
        {                        
            if (mNotesGenerator != null)
            {
                mNotesGenerator.SendNotes(mMessage, mMessageCount);
            }                            
        }         

        private bool IsInSchedule()
        {
            DateTime currentDate = DateTime.Now;
            int spanStart = currentDate.CompareTo(mSchedule.StartTime);
            int spanEnd = currentDate.CompareTo(mSchedule.EndTime);
            if (spanStart < 0 || spanEnd > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }

        //private bool IsInDate()
        //{
        //    DateTime currentDate = DateTime.Now.Date;           

        //    if (currentDate.CompareTo(mSchedule.StartDate) < 0 ||
        //        currentDate.CompareTo(mSchedule.EndDate) > 0)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //private bool IsInTime()
        //{
        //    DateTime time = DateTime.Now;
        //    int h = time.Hour;
        //    int m = time.Minute;


        //    if (h < mSchedule.StartTime.Hour || h > mSchedule.EndTime.Hour)
        //    {
        //        return false;
        //    }

        //    if (h == mSchedule.StartTime.Hour && m < mSchedule.StartTime.Minute)
        //    {
        //        return false;
        //    }

        //    if (h == mSchedule.EndTime.Hour && m > mSchedule.EndTime.Minute)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        private bool IsExpired()
        {
            DateTime currentDate = DateTime.Now.Date;
            int span = currentDate.CompareTo(mSchedule.EndTime);
            
            if (span > 0)
            {
                return true;
            }                        
            return false;
        }
        
        #endregion

        public override bool Equals(object obj)
        {
            TaskItem item = obj as TaskItem;
            if (item != null)
            {
                return mName.Equals(item.TaskName);
            }
            else
            {
                return false;
            }
        }
    }
}
