//using System;
//using System.IO;
//using TaskScheduler;
//
//namespace TestApp
//{
//	/// <remarks>
//	/// Tester for the TaskScheduler library
//	/// </remarks>
//	class OldTestApp
//	{
//		static void OldMain(string[] args)
//		{
//			Scheduler sched = new Scheduler();
//			// Test task and trigger enumeration
//			foreach (Task t in sched.Tasks) 
//			{
//				Console.WriteLine(t.ToString());
//				foreach (Trigger tr in t.Triggers)
//					Console.WriteLine(tr.ToString());
//				Console.WriteLine();
//			}
//
//			// Test getting existing task (assumes you have a "Disk Cleanup" task already)
//			Task t1 = sched.Tasks["Disk Cleanup"];
//			if (t1 != null)
//			{
//				// Test trigger list
//				t1.Triggers.Clear();
//				t1.Triggers.Add(new OnIdleTrigger());
//				t1.Triggers[0] = new OnSystemStartTrigger();
//				// Test tag
//				string s = "This is an interesting comment.";
//				t1.Tag = (object)s;
//				t1.Save();
//				t1 = sched.Tasks["Disk Cleanup"];
//				s = null;
//				s = (string)t1.Tag;
//				Console.WriteLine(s);
//			}
//
//			// Test creating new task and all triggers
//			Task t2;
//			try 
//			{
//				t2 = sched.Tasks.NewTask("Testing");
//				t2.ApplicationName = "notepad.exe";
//				t2.Comment = "Testing Notepad";
//				t2.Creator = "Author";
//				t2.Flags = TaskFlags.Interactive;
//				t2.Hidden = true;
//				t2.IdleWaitDeadlineMinutes = 20;
//				t2.IdleWaitMinutes = 10;
//				t2.MaxRunTime = new TimeSpan(1, 0, 0);
//				t2.Parameters = @"c:\test.log";
//				t2.Priority = System.Diagnostics.ProcessPriorityClass.High;
//				t2.WorkingDirectory = @"c:\";
//				t2.Triggers.Add(new RunOnceTrigger(DateTime.Now + TimeSpan.FromMinutes(1.0)));
//				t2.Triggers.Add(new DailyTrigger(8, 30, 1));
//				t2.Triggers.Add(new WeeklyTrigger(6, 0, DaysOfTheWeek.Sunday));
//				t2.Triggers.Add(new MonthlyDOWTrigger(8, 0, DaysOfTheWeek.Monday | DaysOfTheWeek.Thursday, WhichWeek.FirstWeek | WhichWeek.ThirdWeek));
//				int[] days = {1,8,15,22,29};
//				t2.Triggers.Add(new MonthlyTrigger(9, 0, days, MonthsOfTheYear.July));
//				t2.Triggers.Add(new OnIdleTrigger());
//				t2.Triggers.Add(new OnLogonTrigger());
//				t2.Triggers.Add(new OnSystemStartTrigger());
//				// The following line needs to be set to a valid account and password
//				t2.SetAccountInformation("DOMAIN\\username", "password");
//				t2.Save();
//			}
//			catch
//			{
//			}
//
//			t2 = null;
//		
//			// Test getting all task properties
//			t2 = sched.Tasks["Testing"];
//			Console.WriteLine();
//			Console.WriteLine(t2.Name);
//			Console.WriteLine("  {0}", t2.AccountName);
//			Console.WriteLine("  {0}", t2.ApplicationName);
//			Console.WriteLine("  {0}", t2.Comment);
//			Console.WriteLine("  {0}", t2.Creator);
//			Console.WriteLine("  {0}", t2.ExitCode);
//			Console.WriteLine("  {0}", t2.Flags);
//			Console.WriteLine("  {0}", t2.Hidden);
//			Console.WriteLine("  {0}", t2.IdleWaitDeadlineMinutes);
//			Console.WriteLine("  {0}", t2.IdleWaitMinutes);
//			Console.WriteLine("  {0}", t2.MaxRunTime);
//			Console.WriteLine("  {0}", t2.MostRecentRunTime);
//			Console.WriteLine("  {0}", t2.NextRunTime);
//			Console.WriteLine("  {0}", t2.Parameters);
//			Console.WriteLine("  {0}", t2.Priority);
//			Console.WriteLine("  {0}", t2.Status);
//			Console.WriteLine("  {0}", t2.WorkingDirectory);
//
//			// Test trigger lookup and removal
//			Trigger trigger = new OnIdleTrigger();
//			int idx = t2.Triggers.IndexOf(trigger);
//			if (idx != -1)
//				t2.Triggers.RemoveAt(idx);
//
//			// Test task execution methods
//			t2.Run();
//			Console.In.ReadLine();
//			try 
//			{
//				t2.Terminate();
//			}
//			catch
//			{
//			}
//
//			// Test task deletion
//			sched.Tasks.Delete("Testing");
//		}
//	}
//}
