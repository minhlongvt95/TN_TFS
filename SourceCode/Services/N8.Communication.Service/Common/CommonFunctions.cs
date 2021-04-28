using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace N8.Communication.Common
{
    public static class CommonFunctions
    {

        /// <summary>
        /// Common function to write into event viewer log
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="ErrorType"></param>
        public static void WriteIntoEventLog(String Source, String ErrorMessage, EventLogEntryType ErrorType)
        {
            if (!EventLog.SourceExists(ServiceProperties.ApplicationName))
            {
                EventLog.CreateEventSource(ServiceProperties.ApplicationName, "Application");
            }

            //Write the error to Windows System Event log
            //     EventLog.WriteEntry("N8 Communication Service", ErrorMessage + ".",
            //EventLogEntryType.Error);

            EventLog.WriteEntry(ServiceProperties.ApplicationName, Source + ". " +
               "Description: " + ErrorMessage + ".", ErrorType);
        }

    }
}
