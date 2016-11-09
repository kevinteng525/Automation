using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Common.SystemCommon
{
    public static class WindwosEventLog
    {
        public static bool CreateEvent(string sourceName, string logName)
        {
            try
            {
                if (DeleteSourceEvent(sourceName))
                {
                    // Logs and Sources are created as a pair.
                    EventLog.CreateEventSource(sourceName, logName);
                    EventLog eventLog = CreateEventLog(sourceName, logName);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;

                throw;
            }
        }

        public static bool IsSourceExist(string sourceName)
        {
            return EventLog.SourceExists(sourceName);
        }

        public static bool IsLogExist(string logName)
        {
            return EventLog.Exists(logName);
        }

        public static bool DeleteSourceEvent(string sourceName)
        {
            try
            {
                if (IsSourceExist(sourceName))
                {
                    EventLog.DeleteEventSource(sourceName);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static EventLog CreateEventLog(string sourceName, string logName)
        {
            return new EventLog
            {
                Log = logName,
                Source = sourceName,
                MachineName = "."
            };
        }

        public static bool WriteEvent(string sourceName, string logName, string logMsg, EventLogEntryType type, int eventID, short category)
        {
            if (!IsSourceExist(sourceName))
            {
                CreateEvent(sourceName, logName);
            }

            EventLog eventLog = CreateEventLog(sourceName, logName);

            if (EventLog.Exists(logName))
            {
                try
                {
                    eventLog.WriteEntry(logMsg, type, eventID, category);

                    return true;
                }
                catch (Exception)
                {
                    return false;

                    throw;
                }
            }
            else
            {
                return false;
            }
        }

        public static List<string> ReadEvent(string sourceName, string logName)
        {
            EventLog eventLog = CreateEventLog(sourceName, logName);

            List<string> events = new List<string>();

            foreach (EventLogEntry entry in eventLog.Entries)
            {
                events.Add(entry.Message);
            }

            return events;
        }

        public static bool ClearEvent(string sourceName, string logName)
        {
            EventLog eventLog = CreateEventLog(sourceName, logName);

            if (EventLog.Exists(logName))
            {
                try
                {
                    eventLog.Clear();

                    return true;
                }
                catch (Exception)
                {
                    return false;

                    throw;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
