using System;
using System.IO;
using TaskScheduler;

namespace TestApp {
	/// <remarks>
	/// Tester for the TaskScheduler library
	/// </remarks>
	class TestApp {
		static ScheduledTasks st = null;

		static void ListTriggers(Task t) {
			Console.WriteLine("  " + t.ToString());
			foreach (Trigger tr in t.Triggers) {
				Console.WriteLine("    " + tr.ToString());
			}
		}

		static void ListTasks(ScheduledTasks st) {
			string[] taskNames = st.GetTaskNames();
			// Open each task, dump info to console
			foreach (string name in taskNames) {
				Task t = st.OpenTask(name);
				//Console.WriteLine("--> " + name + " ");
				Console.WriteLine(t.ToString() + "\n");
				t.Close();
			}
		}

		static void GoComputer(string machine) {
			try {
				if (machine == "") 
					st = new ScheduledTasks();
				else
					st = new ScheduledTasks(machine);
			} catch (Exception e) {
				Console.WriteLine("Could not access task scheduler on " + machine + 
					"\n  >> " + e.Message);
				st = null;
			}
		}

		static void ShowProperties(Task t) {
			Console.WriteLine(t.Name);
			try {
				Console.WriteLine("  Acct- {0}", t.AccountName);
			} catch (Exception e) {
				Console.WriteLine("  Acct- Exception: " + e.Message);
			}
			Console.WriteLine("  App- {0}", t.ApplicationName);
			Console.WriteLine("  Parms- {0}", t.Parameters);
			Console.WriteLine("  Comment- {0}", t.Comment);
			Console.WriteLine("  Creator- {0}", t.Creator);
			try {
				Console.WriteLine("  ExitCode- {0}", t.ExitCode);
			} catch (Exception e) {
				Console.WriteLine("  ExitCode- Exception: " + e.Message);
			}
			if (t.Hidden)
				Console.WriteLine("  Hidden");
			Console.WriteLine("  Flags- {0}", t.Flags);
			Console.WriteLine("  IdleWaitDeadline- {0}", t.IdleWaitDeadlineMinutes);
			Console.WriteLine("  IdleWait- {0}", t.IdleWaitMinutes);
			Console.WriteLine("  MaxRunTime- {0}", t.MaxRunTime);
			Console.WriteLine("  LastRun- {0}", t.MostRecentRunTime);
			Console.WriteLine("  NextRun- {0}", t.NextRunTime);
			Console.WriteLine("  Priority- {0}", t.Priority);
			Console.WriteLine("  Status- {0}", t.Status.ToString());
			Console.WriteLine("  WorkingDir- {0}", t.WorkingDirectory);
		}

		static Task CreateTask(string name) {
			Task t;
			try {
				t = st.CreateTask(name);
			} catch (ArgumentException) {
				Console.WriteLine("Task already exists");
				return null;
			}
			Console.Write("Appication Name: ");
			t.ApplicationName = Console.ReadLine();
			Console.Write("Parameters: ");
			t.Parameters = Console.ReadLine();
			Console.Write("Comment: ");
			t.Comment = Console.ReadLine();
			Console.Write("Creator: ");
			t.Creator = Console.ReadLine();
			Console.Write("Working Directory: ");
			t.WorkingDirectory = Console.ReadLine();
			Console.Write("Account: ");
			string acct = Console.ReadLine();
			if (acct=="") {
				t.SetAccountInformation(acct, (string)null);
			} else if (acct == Environment.UserName) {
				t.Flags = TaskFlags.RunOnlyIfLoggedOn;
				t.SetAccountInformation(acct, (string)null);
				Console.WriteLine("cur user is " + Environment.UserName + "; No password needed.");
			} else {
				Console.Write("Password: ");
				t.SetAccountInformation(acct, Console.ReadLine());
			}
			//t.Hidden = true;
			t.IdleWaitDeadlineMinutes = 20;
			t.IdleWaitMinutes = 10;
			t.MaxRunTime = new TimeSpan(1, 0, 0);
			t.Priority = System.Diagnostics.ProcessPriorityClass.High;
			t.Triggers.Add(new RunOnceTrigger(DateTime.Now + TimeSpan.FromMinutes(3.0)));
			t.Triggers.Add(new DailyTrigger(8, 30, 1));
			t.Triggers.Add(new WeeklyTrigger(6, 0, DaysOfTheWeek.Sunday));
			t.Triggers.Add(new MonthlyDOWTrigger(8, 0, DaysOfTheWeek.Monday | DaysOfTheWeek.Thursday, WhichWeek.FirstWeek | WhichWeek.ThirdWeek));
			int[] days = {1,8,15,22,29};
			t.Triggers.Add(new MonthlyTrigger(9, 0, days, MonthsOfTheYear.July));
			t.Triggers.Add(new OnIdleTrigger());
			t.Triggers.Add(new OnLogonTrigger());
			t.Triggers.Add(new OnSystemStartTrigger());
			return t;
		}
	

		static void Hack() {
			// Use this to add ad hoc tests.  

			//----------------------
			// Create a task
			Task t;
			try {
				t = st.CreateTask("D checker");
			} catch (ArgumentException) {
				Console.WriteLine("Task name already exists");
				return;
			}

			// Fill in the program info
			t.ApplicationName = "ChkDsk.exe";
			t.Parameters = "d: /f";
			t.Comment = "Checks and fixes errors on D: drive";

			// Set the account under which the task should run.
			// Passing an empty string and null sets the task to be run under the local system account.
			t.SetAccountInformation("", (string)null);

			// Declare that the system must have been idle for ten minutes before the task will start
			t.IdleWaitMinutes = 10;

			// Allow the task to run for no more than 2 hours, 30 minutes.
			t.MaxRunTime = new TimeSpan(2, 30, 0);

			// Set priority to only run when system is idle.
			t.Priority = System.Diagnostics.ProcessPriorityClass.Idle;

			// Create a trigger to start the task every Sunday at 6:30 AM.
			t.Triggers.Add(new WeeklyTrigger(6, 30, DaysOfTheWeek.Sunday));

			// Save and close.
			t.Save();
			t.Close();

			// -----------------------------------------------
			// Open a task and change its triggers
			Task task = st.OpenTask("D checker");
			// Be sure the task was found before proceeding
			if (task != null) {
				// Enumerate each trigger in the TriggerList of this task
				foreach (Trigger tr in task.Triggers) {
					// If this trigger has a start time, change it to 4:15 AM.
					if (tr is StartableTrigger) {
						(tr as StartableTrigger).StartHour = 4;
						(tr as StartableTrigger).StartMinute = 15;
					}
				}
				task.Save();
				task.Close();
			}
		}
			
		static void Hack(Task t) {
			// Use this to put in ad hoc operations on a named task.  A few old ones are below.
			if (t != null) {

				
				t.Flags = t.Flags ^ TaskFlags.Hidden;  //toggle attribute
				//t.Hidden = ! t.Hidden;

				// Test GetRunTimeAfter

//				DateTime rAfter = new DateTime();
//				DateTime rNext = new DateTime();
//				rAfter = DateTime.Now;
//				for (int i=0; i<10; i++) {
//					rNext = t.NextRunTimeAfter(rAfter);
//					Console.WriteLine("Next Run Time after {0} is {1}", rAfter, rNext);
//					rAfter = rNext;
//				}



				//Test MonthlyTrigger days property
//				int[] initial = {12, 6};
//				int[] modified = {1, 5, 11, 17, 19};
//				MonthlyTrigger tr = new MonthlyTrigger(12, 5, initial);
//				tr.Days = modified;
//				t.Triggers.Add(tr);
//				int[] res = tr.Days;
//				foreach (int i in res) Console.Write(" {0}", i);
//				Console.WriteLine();

//				// Clear trigger list and create a new one
//				t.Triggers.Clear();
//				t.Triggers.Add(new OnIdleTrigger());
//				// Create a tag object
//				string s = "This is a tag attached to a task.";
//				t.Tag = (object)s;
//
//				// Replace the trigger with a different one
//				t.Triggers.Clear();
//				t.Triggers.Add(new OnSystemStartTrigger());
//				//Save with a new name
//				Console.Write("Save As: ");
//				string newname = Console.ReadLine();
//				try{
//					t.Save(newname);
//				}
//				catch {
//					Console.WriteLine("Task couldn't be saved as" + newname);
//				}
//				// Fetch the tag back and dump it out
//				s = null;
//				s = (string)t.Tag;
//				Console.WriteLine("Tag retrieved: " + s);
//
//				// Test trigger lookup and removal
//				Trigger trigger = new OnIdleTrigger();
//				t.Triggers.Add(trigger);
//				int idx = t.Triggers.IndexOf(trigger);
//				if (idx != -1)
//					t.Triggers.RemoveAt(idx);
//
//				//Test trigger copying
//				trigger = new OnIdleTrigger();
//				t.Triggers.Add(trigger);
//				Trigger[] tgs = new Trigger[12];
//				t.Triggers.CopyTo(tgs, 0);
//				Console.WriteLine("{0} Triggers in an array:", t.Triggers.Count);
//				for (int i=0; i<t.Triggers.Count; i++) {
//					Console.WriteLine("  " + tgs[i].ToString() + "(" + tgs[i].GetType() + ")");
//				}
			}
		}

		static void Main(string[] args) {
			Task t = null;;
			string input = "?";
			st = new ScheduledTasks();
			ListTasks(st);
			Console.WriteLine();
			while (input != "q") {
				input = input.Trim(' ');
				int ib = input.IndexOf(" ");
				if (ib==-1) ib = input.Length;
				string cmd = input.Substring(0, ib);
				string arg = input.Substring(ib);
				arg = arg.Trim(' ');
				try {
					if (cmd == "g") {
						if (st != null) {
							st.Dispose();
						}
						if (arg != "") {
							GoComputer(arg);
						} else {
							GoComputer("");
						}
						ListTasks(st);
					} else if (cmd == "o") {
						if (arg != "")
							t = st.OpenTask(arg);
						if (t!=null) {
							Console.WriteLine("  Opened:  " + t.ToString());
						} else {
							Console.WriteLine("  Task couldn't be opened.");
						}
					} else if (cmd == "s") {
						if (t != null) {
							if (arg != "") {
								t.Save(arg);
							} else {
								t.Save();
							}
						} else {
							Console.Write("  No task is open");
						}
					} else if (cmd == "c") {
						if (t != null) {
							t.Close();
							t = null;
						} else {
							Console.Write("  No task is open");
						}
					} else if (cmd == "h") {
						if (t != null) {
							Hack(t);
						} else {
							Hack();
						}
					} else if (cmd == "t") {
						if (arg == "") {
							if (t != null) {
								ListTriggers(t);
							} else {
								Console.WriteLine("  No task is open");
							}
						} else {
							Task t2 = st.OpenTask(arg);
							ListTriggers(t2);
						}
					} else if (cmd == "p") {
						if (arg != "") {
							Task t2 = st.OpenTask(arg);
							if (t2 != null) {
								ShowProperties(t2);
								t2.Close();
							} else {
								Console.WriteLine("  Task couldn't be accessed.");
							}
						} else {
							if (t != null) {
								ShowProperties(t);
							} else {
								Console.Write("  No task is open");
							}
						}
					} else if (cmd == "e") {
						if (arg != "") {
							Task t2 = st.OpenTask(arg);
							if (t2 != null) {
								bool OK = t2.DisplayPropertySheet(Task.PropPages.Schedule);
								if (OK) {
									t2.Save();
									Console.WriteLine("  Saved");
								}
								t2.Close();
							} else {
								Console.WriteLine("  Task could not be found.");
							}
						} else {
							if (t != null) {
								bool OK = t.DisplayPropertySheet(Task.PropPages.Task |Task.PropPages.Settings);
								Console.WriteLine("  Returned {0}", OK?"OK":"Cancel");
							} else {
								Console.Write("  No task is open");
							}
						}
					} else if (cmd == "n") {
						if (arg != "") {
							if (t != null) {
								t.Close();
							}
							t = CreateTask(arg);
						} else {
							Console.Write("  A Task Name is requred.");
						}
					} else if (cmd == "d") {
						if (arg != "") {
							st.DeleteTask(arg);
							ListTasks(st);
						} else {
							Console.WriteLine("  A task name is required to delete.");
						}
					} else if (cmd == "r") {
						if (arg != "") {
							Task t2 = st.OpenTask(arg);
							if (t2 != null) {
								t2.Run();
								t2.Close();
							} else {
								Console.WriteLine("  Task couldn't be found.");
							}
						} else {
							if (t != null) {
								t.Run();
							} else {
								Console.WriteLine("  No task isopen.");
							}
						}
					} else if (cmd == "k") {
						if (arg != "") {
							Task t2 = st.OpenTask(arg);
							if (t2 != null) {
								t2.Terminate();
								t2.Close();
							} else {
								Console.WriteLine("  Task couldn't be found.");
							}
						} else {
							if (t != null) {
								t.Terminate();
							} else {
								Console.WriteLine("  No task open.");
							}
						}
					} else if (cmd == "?") {
						Console.Write(
							"g(o) [computer name]\n" + 
							"o(pen) name\n" +
							"s(ave) [name]\n" +
							"c(lose)\n" +
							"e(edit) [name]\n" +
							"p(roperties) [name]\n" +
							"t(riggers) [name]\n" +
							"n(ew) name\n" +
							"d(elete) name\n" +
							"r(un) [name]\n" +
							"k(ill) [name]\n" +
							"h(ack)\n" +
							"q(uit)" +
							"?"
							);
					} else if (cmd == "") {
						;
					} else {
						Console.WriteLine("Unknown command");
					}
				} catch (System.Runtime.InteropServices.COMException e) {
					Console.WriteLine("[COM Exception {0:X8}] {1}", e.ErrorCode, e.Message);
				} catch (Exception e) {
					Console.WriteLine("Exception " + e.ToString());
				}

				if (t != null) {
					Console.Write("\n" + t.Name + ">");
				} else {
					Console.Write("\n> ");
				}
				input = Console.ReadLine();

			} //loop
		} //method
	} //class
} //namespace
