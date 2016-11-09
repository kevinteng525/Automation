using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model
{
    public partial class DiagnosticLog
    {
        public static List<DiagnosticLog> GetAllDiagnosticLogs()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.DiagnosticLogs.ToList();
            }
        }

        public static DiagnosticLog GetDiagnosticLogById(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                return context.DiagnosticLogs.Find(id);
            }
        }

        public static DiagnosticLog AddDiagnosticLog(DiagnosticLog log)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                context.DiagnosticLogs.Add(log);
                context.SaveChanges();
                return log;
            }
        }

        public static DiagnosticLog UpdateDiagnosticLog(int id, object instance)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                DiagnosticLog log = context.DiagnosticLogs.Find(id);
                if (null != log)
                {
                    context.Entry(log).CurrentValues.SetValues(instance);
                    context.SaveChanges();
                    return log;
                }
                else
                {
                    return null;
                }
            }
        }

        public static void DeleteDiagnosticLog(int id)
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                DiagnosticLog log = context.DiagnosticLogs.Find(id);
                if (null != log)
                {
                    context.DiagnosticLogs.Remove(log);
                    context.SaveChanges();
                }
            }
        }

        public static void ClearDiagnosticLog()
        {
            using (ES1AutomationEntities context = new ES1AutomationEntities())
            {
                context.DiagnosticLogs.RemoveRange(context.DiagnosticLogs);
                context.SaveChanges();
            }
        }
    }
}
