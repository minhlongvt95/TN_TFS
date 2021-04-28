using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Net.Mail;

namespace N8.Communication.Common
{
    public static class ServiceProperties
    {
        /// <summary>
        /// Application name for writing into event viewer log purposes.
        /// </summary>
        public static String ApplicationName { get; set; }

        /// <summary>
        /// Connection String used for the all the code to connect to database.
        /// </summary>
        public static String ConnectionString { get; set; }

        /// <summary>
        /// Polling Interval that the service will use when pooling the data from Service Broker Queue.
        /// </summary>
        public static int PollingInterval { get; set; }

        /// <summary>
        /// Long Polling Interval that the service will use when pooling the data from Service Broker Queue.
        /// </summary>
        public static int SleepInterval { get; set; }

        /// <summary>
        /// Number of worker Threads that the service will create and maintain at same time.
        /// </summary>
        public static short NoOfThreads { get; set; }

        /// <summary>
        /// Number of retries that the service will do for each Thread everytime there is an error occured during Loading/Fraud check.
        /// </summary>
        public static short MaxRetry { get; set; }

        /// <summary>
        /// Maximum records that service pool from Service Broker queue per polling interval.
        /// </summary>
        public static short QueueReceivedNo { get; set; }

        /// <summary>
        /// Minimum number of records of transactions that the service needs to maintain in memory to avoid starving worker threads.
        /// </summary>
        public static int MinimumBuffer { get; set; }

        /// <summary>
        /// Specify the user to save into the system diary 
        /// </summary>
        public static String ServiceUser { get; set; }

        /// <summary>
        /// Specify the URL for the SMS http post
        /// </summary>
        public static String APIURL { get; set; }
        public static int ISTELCOSUB { get; set; }
        /// <summary>
        /// User ID passed in the SMS
        /// </summary>
        public static String AGENTID { get; set; }

        /// <summary>
        /// Password passed in the SMS
        /// </summary>
        public static String APIUSER { get; set; }

        /// <summary>
        /// SMS Type passed in the SMS
        /// </summary>
        public static String APIPASS { get; set; }

        /// <summary>
        /// Channel passed in the SMS
        /// </summary>
        public static String USERNAME { get; set; }

        /// <summary>
        /// First SMS Cut Off time (in 24 hours format).
        /// </summary>
        public static int DATACODING { get; set; }

        /// <summary>
        /// First SMS Cut Off time (in 24 hours format).
        /// </summary>
        public static string LABELID { get; set; }

        /// <summary>
        /// First SMS Cut Off time (in 24 hours format).
        /// </summary>
        public static string CONTRACTID { get; set; }

        /// <summary>
        /// First SMS Cut Off time (in 24 hours format).
        /// </summary>
        public static string TEMPLATEID { get; set; }

        /// <summary>
        /// Email SMTP server used to send email messages 
        /// </summary>
        public static String EmailSMTPServer { get; set; }

        /// <summary>
        /// The port number of the mail server
        /// </summary>
        public static String EmailSMTPPort { get; set; }

        /// <summary>
        /// Username used to authenticate against the mail server
        /// </summary>
        public static String EmailSMTPUserName { get; set; }

        /// <summary>
        /// Password used to authenticate against the mail server
        /// </summary>
        public static String EmailSMTPPassword { get; set; }

        /// <summary>
        /// The email address to be shown on the messages being sent
        /// </summary>
        public static String EmailFromAddress { get; set; }

        /// <summary>
        /// The name to be shown on the messages being sent
        /// </summary>
        public static String EmailFrom { get; set; }

        public static bool UseFromAddress { get; set; }
        /// <summary>
        /// Site with special functions
        /// </summary>
        public static String SiteWithSpecialFunction { get; set; }

      
    }
}
