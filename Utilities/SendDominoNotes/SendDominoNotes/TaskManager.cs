using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SendDominoNotes
{
    public class TaskManager
    {
        private static TaskManager mTaskManager = null;
        private Dictionary<string, TaskItem> mTaskDic = null;

        private TaskManager()
        {

        }

        public static TaskManager Instance
        {
            get 
            {
                if(mTaskManager == null) mTaskManager = new TaskManager();
                return mTaskManager;
            }
        }

        public Dictionary<string, TaskItem> TaskDictionary
        {
            get 
            {
                if (mTaskDic == null)
                {
                    mTaskDic = new Dictionary<string, TaskItem>();
                }
                return mTaskDic; 
            }
        }

        public void AddTaskItem(TaskItem taskitem)
        {            
            if (mTaskDic == null)
            {
                mTaskDic = new Dictionary<string,TaskItem>();
            }

            if (mTaskDic.ContainsKey(taskitem.TaskName))
            {
                MessageBox.Show("The name has been existed, please change the task name.");
            }
            else
            {
                mTaskDic.Add(taskitem.TaskName, taskitem);
            }          
        }

        public void DeleteTaskItem(TaskItem taskitem)
        {
            if (mTaskDic.ContainsKey(taskitem.TaskName))
            {
                mTaskDic.Remove(taskitem.TaskName);
            }
            else
            {
                MessageBox.Show("This task doen't exist.");
            } 
        }
    }
}
