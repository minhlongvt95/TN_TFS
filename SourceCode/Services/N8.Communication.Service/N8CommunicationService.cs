using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Collections;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Configuration;
using DecTech.Library;
using System.IO;
using System.Data.SqlClient;
using N8.Communication.Common;
using RestSharp;


namespace N8.Communication
{
	public partial class N8CommunicationService : ServiceBase
	{

		#region Property

		private Object SyncRoot = new Object(); // needed for sync between threads

		//in built memory cache for the transactions
		private Queue _messagesQueue = new Queue();

		private static SmtpClient SmtpServer;
		private static NetworkCredential SmtpCredential;

		private bool stopping;
		private ManualResetEvent stoppedEvent;

		private System.Diagnostics.EventLog eventLog;

		private string TimeForRefresh;
		Schedule.ReportTimer clsSch;

		
		#endregion


		public N8CommunicationService()
		{
			InitializeComponent();

			this.stopping = false;
			this.stoppedEvent = new ManualResetEvent(false);
		}

		protected override void OnStart(string[] args)
		{
			try
			{
				// Read all configuration from config file
				Thread.Sleep(ServiceProperties.SleepInterval);
				InitialServiceProcessing();

				RefreshTimer.Enabled = true;

				// Queue the main service function for execution in a worker thread.
				ThreadPool.QueueUserWorkItem(new WaitCallback(ServiceWorkerThread));
			}
			catch (Exception ex)
			{
				// Write entry
				CommonFunctions.WriteIntoEventLog("Sub = OnStart", ex.ToString(), EventLogEntryType.Error);
			}
		}

		private void InitialServiceProcessing()
		{
            try
            {
                String initialCatalog;
                String dataSource;
                String useWindowsAuthentication;
                String dBUserId;
                String dBPassword;

                String company;
                String serviceSuffix;
                int sleepInterval;
                short queueReceivedNo;

                //use this as default value
                company = ConfigurationManager.AppSettings["N8.Company"].Trim();
                serviceSuffix = ConfigurationManager.AppSettings["N8.ServiceSuffix"].Trim().ToUpper();

                ServiceProperties.ApplicationName = "N8ServiceCommunication" + company + serviceSuffix;


                //getting the data from the config file

                /******************** Connection String *********************/
                initialCatalog = ConfigurationManager.AppSettings["N8.InitialCatalog"].Trim();
                dataSource = ConfigurationManager.AppSettings["N8.DataSource"].Trim();
                useWindowsAuthentication = ConfigurationManager.AppSettings["N8.IntegratedSecurity"].Trim().ToUpper();
                dBUserId = ConfigurationManager.AppSettings["N8.DBUserId"].Trim();
                dBPassword = ConfigurationManager.AppSettings["N8.DBPassword"].Trim();


                ServiceProperties.ConnectionString = "Data Source=" + dataSource +
                                                     ";Initial Catalog=" + initialCatalog +
                                                     ";Integrated Security=False;" +
                                                     "User Id=" + dBUserId +
                                                     ";Password=" + dBPassword + ";";


                if (int.TryParse(ConfigurationManager.AppSettings["Sleep.Interval"], out sleepInterval))
                {
                    ServiceProperties.SleepInterval = sleepInterval;
                }


                if (short.TryParse(ConfigurationManager.AppSettings["Queue.Received.Number"], out queueReceivedNo))
                {
                    ServiceProperties.QueueReceivedNo = queueReceivedNo;
                }
                else
                {
                    ServiceProperties.QueueReceivedNo = 100;

                    CommonFunctions.WriteIntoEventLog("Sub = InitialServiceProcessing",
                        "Invalid Queue.Received.Number, set to default value (" +
                        ServiceProperties.QueueReceivedNo.ToString() + ")", EventLogEntryType.Warning);
                }


                // Email - SMTP Configuration.
                ServiceProperties.EmailSMTPServer =
                    ConfigurationManager.AppSettings["Communication.Email.SMTP.Server"].Trim();
                ServiceProperties.EmailSMTPPort =
                    ConfigurationManager.AppSettings["Communication.Email.SMTP.Port"].Trim();
                ServiceProperties.EmailSMTPUserName =
                    ConfigurationManager.AppSettings["Communication.Email.SMTP.UserName"].Trim();
                string SMTPPass = ConfigurationManager.AppSettings["Communication.Email.SMTP.Password"].Trim();

                ServiceProperties.EmailSMTPPassword = SMTPPass;



                ServiceProperties.EmailFromAddress =
                    ConfigurationManager.AppSettings["Communication.Email.FromAddress"].Trim();
                ServiceProperties.EmailFrom = ConfigurationManager.AppSettings["Communication.Email.From"].Trim();
                int portNo;
                if (int.TryParse(ConfigurationManager.AppSettings["Communication.Email.SMTP.Port"], out portNo))
                {
                    SmtpServer =
                        new SmtpClient(ConfigurationManager.AppSettings["Communication.Email.SMTP.Server"].Trim(),
                            portNo);
                }
                else
                {
                    SmtpServer =
                        new SmtpClient(ConfigurationManager.AppSettings["Communication.Email.SMTP.Server"].Trim(), 25);
                }

                if (ServiceProperties.EmailSMTPPassword != "" || ServiceProperties.EmailSMTPUserName != "")
                {
                    SmtpCredential = new NetworkCredential(ServiceProperties.EmailSMTPUserName,
                        ServiceProperties.EmailSMTPPassword);
                    SmtpServer.Credentials = SmtpCredential;
                }

                bool ssL;
                if (bool.TryParse(ConfigurationManager.AppSettings["Communication.Email.SMTP.SSL"].Trim(), out ssL))
                {
                    SmtpServer.EnableSsl = ssL;
                }
                else
                {
                    SmtpServer.EnableSsl = false;
                }

                bool useFromAddress;
                if (bool.TryParse(ConfigurationManager.AppSettings["Communication.Email.UseFromAddress"].Trim(), out useFromAddress))
                {
                    ServiceProperties.UseFromAddress = useFromAddress;
                }
                else
                {
                    ServiceProperties.UseFromAddress = false;
                }
                //UserId for Diary Notes
                ServiceProperties.ServiceUser = ConfigurationManager.AppSettings["Communication.ServiceUser"];

                //SMS setting
                int ISTELCOSUB;
                if (int.TryParse(ConfigurationManager.AppSettings["Communication.SMS.ISTELCOSUB"], out ISTELCOSUB))
                {
                    ServiceProperties.ISTELCOSUB = ISTELCOSUB;
                }
                else
                {
                    ServiceProperties.ISTELCOSUB = 0;
                }
                int dataCoding;
                if (int.TryParse(ConfigurationManager.AppSettings["Communication.SMS.DATACODING"], out dataCoding))
                {
                    ServiceProperties.DATACODING = dataCoding;
                }
                else
                {
                    ServiceProperties.DATACODING = 0;
                }

                ServiceProperties.AGENTID = ConfigurationManager.AppSettings["Communication.SMS.AGENTID"].Trim();
                ServiceProperties.APIUSER = ConfigurationManager.AppSettings["Communication.SMS.APIUSER"].Trim();
                ServiceProperties.APIPASS = ConfigurationManager.AppSettings["Communication.SMS.APIPASS"].Trim();
                ServiceProperties.USERNAME = ConfigurationManager.AppSettings["Communication.SMS.USERNAME"].Trim();
                ServiceProperties.APIURL = ConfigurationManager.AppSettings["Communication.SMS.APIURL"].Trim();
                
                ServiceProperties.LABELID = ConfigurationManager.AppSettings["Communication.SMS.LABELID"].Trim();
                ServiceProperties.CONTRACTID = ConfigurationManager.AppSettings["Communication.SMS.CONTRACTID"].Trim();
                ServiceProperties.TEMPLATEID = ConfigurationManager.AppSettings["Communication.SMS.TEMPLATEID"].Trim();


            }
			catch (Exception ex)
			{
				//write entry
				CommonFunctions.WriteIntoEventLog("Sub = InitialServiceProcessing", ex.ToString(), EventLogEntryType.Error);

				// NEED TO STOP SERVICE
				Stop();
			}
		}

		private void ServiceWorkerThread(object state)
		{
			// Periodically check if the service is stopping.
			while (!this.stopping)
			{
				// 1. Get the dataset from the Communication_Service_Queue Table
				DataSet ds = RetrieveDataFromCommunicationQueue();

				try
				{
					if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
					{
						//put all the messages in the queue
						foreach (DataRow dr in ds.Tables[0].Rows)
						{
						
								//put the value in
								N8Message currentMessage = new N8Message();

								currentMessage.QueueId = (Guid)dr["QueueId"];
								currentMessage.From = dr["From"] + string.Empty;
								currentMessage.To = dr["To"] + string.Empty;
                                currentMessage.Content = (dr["Content"]).ToString().Replace(System.Environment.NewLine, string.Empty) + string.Empty;
								currentMessage.Title = dr["Title"] + string.Empty;
								currentMessage.Method = dr["Method"] + string.Empty;

								lock (this._messagesQueue.SyncRoot)
								{
									this._messagesQueue.Enqueue(currentMessage);
								}
							
						}
					}
				}
				catch (Exception ex)
				{
					CommonFunctions.WriteIntoEventLog("Sub = ServiceWorkerThread - Loop records from Database", ex.ToString(), EventLogEntryType.Error);
				}

				// 2. Send Emails / SMS (Disabled)

				N8Message messageToSend;

				while (this._messagesQueue.Count > 0)
				{
					try
					{
						lock (this._messagesQueue.SyncRoot)
						{
							//if nothing in the queue then wait
							while (this._messagesQueue.Count == 0)
							{
								//waiting for the next available data
								Monitor.Wait(this._messagesQueue.SyncRoot);
							}

							//get the data from the queue
							messageToSend = (N8Message)this._messagesQueue.Dequeue();
						}

						if (messageToSend != null)
						{
							//Checking if the message to be sent will be an email OR SMS
							if (messageToSend.Method == "Email")
							{
								//Build the email message using defaults and values from the CSQ
								MailMessage mail = new MailMessage();
                                if(!ServiceProperties.UseFromAddress)
								    mail.From = new MailAddress(messageToSend.From);
                                else
                                    mail.From = new MailAddress(ServiceProperties.EmailFromAddress);
                                mail.To.Add(messageToSend.To.Replace(';', ','));
								mail.Subject = messageToSend.Title;
								mail.Body = messageToSend.Content;
                                mail.IsBodyHtml = true;
								//Send the email message
								SmtpServer.Send(mail);

								//Message sent, Remove from database and insert Diary Note.
								MessageSent(messageToSend.QueueId);
							}
							if (messageToSend.Method == "SMS")
                            {
                                if (!string.IsNullOrEmpty(messageToSend.To))
                                {
                                    if (messageToSend.To.Substring(0, 1) == "0")
                                    {
                                        messageToSend.To = "84" + messageToSend.To.Remove(0, 1);
                                    }

                                    var requestBody =
                                        "{\"RQST\":{\"name\":\"send_sms_list\",\"REQID\":\"1\",\"LABELID\":\"" +
                                        ServiceProperties.LABELID + "\",\"CONTRACTID\":\"" +
                                        ServiceProperties.CONTRACTID +
                                        "\",\"CONTRACTTYPEID\":\"1\",\"TEMPLATEID\":\"" + ServiceProperties.TEMPLATEID +
                                        "\",\"PARAMS\":[{\"NUM\":\"1\",\"CONTENT\":\"" + messageToSend.Content +
                                        "\"}],\"SCHEDULETIME\":\"\",\"MOBILELIST\":\"" + messageToSend.To +
                                        "\",\"ISTELCOSUB\":\"0\",\"AGENTID\":\"" + ServiceProperties.AGENTID +
                                        "\",\"APIUSER\":\"" + ServiceProperties.APIUSER + "\",\"APIPASS\":\"" +
                                        ServiceProperties.APIPASS + "\",\"USERNAME\":\"" +
                                        ServiceProperties.USERNAME + "\",\"DATACODING\":\"" +
                                        ServiceProperties.DATACODING + "\"}}";
                                    //sent SMS
                                    var client = new RestClient(ServiceProperties.APIURL);
                                    var request = new RestRequest(Method.POST);
                                    request.AddHeader("Content-Type", "application/json");
                                    request.AddParameter("undefined", requestBody, ParameterType.RequestBody);
                                    IRestResponse response = client.Execute(request);
                                }

                                //Message sent, Remove from database and insert Diary Note.
                                MessageSent(messageToSend.QueueId);
							}
						}
					}
					catch (Exception ex)
					{
						CommonFunctions.WriteIntoEventLog("Sub = ServiceWorkerThread - Loop queue and send emails", ex.ToString(), EventLogEntryType.Error);
					}
				}
				Thread.Sleep(ServiceProperties.SleepInterval);  // Sleep according to time specified if not stopping
			}

			// Signal the stopped event.
			this.stoppedEvent.Set();
		}

		private DataSet RetrieveDataFromCommunicationQueue()
		{
			DataSet resultDS = new DataSet();
			SqlConnection conn = new SqlConnection(ServiceProperties.ConnectionString);

			try
			{
				using (SqlCommand cmd = new SqlCommand("[N8_Communication_Service_Queue_Select]", conn))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					conn.Open();
					using (SqlDataAdapter da = new SqlDataAdapter(cmd))
					{
						da.Fill(resultDS);
						conn.Close();
					}
				}
			}
			catch (Exception ex)
			{
				CommonFunctions.WriteIntoEventLog("Sub = RetrieveDataFromCommunicationQueue", ex.ToString(), EventLogEntryType.Error);
			}
			finally
			{
				if (conn.State == ConnectionState.Open)
				{
					conn.Close();
				}
			}

			return resultDS;
		}

		private void MessageSent(Guid queueId)
		{

			SqlParameter[] sqlParam = new SqlParameter[1];
			SqlConnection conn = new SqlConnection(ServiceProperties.ConnectionString);

			try
			{
				sqlParam[0] = new SqlParameter("@QueueId", SqlDbType.UniqueIdentifier);
				sqlParam[0].Value = queueId;

				using (SqlCommand cmd = new SqlCommand("[N8_Communication_Service_Queue_Complete]", conn))
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					cmd.Parameters.AddRange(sqlParam);
					conn.Open();
					cmd.ExecuteNonQuery();
					conn.Close();
				}
			}
			catch (Exception ex)
			{
				CommonFunctions.WriteIntoEventLog("Sub = MessageSent", ex.ToString(), EventLogEntryType.Error);
			}
			finally
			{
				if (conn.State == ConnectionState.Open)
				{
					conn.Close();
				}
			}

		}

		protected override void OnStop()
		{
			try
			{
				if (clsSch != null)
				{
					clsSch.ClearJobs();
					clsSch.Dispose();
					clsSch = null;
				}
				// Indicate that the service is stopping and wait for the finish 
				// of the main service function (ServiceWorkerThread).
				this.stopping = true;
				this.stoppedEvent.WaitOne(30000);
			}
			catch (Exception ex)
			{
				// Write entry
				CommonFunctions.WriteIntoEventLog("Sub = OnStop", ex.ToString(), EventLogEntryType.Error);
			}
		}

		public static String EncodeHTML(String value)
		{
			return System.Web.HttpUtility.UrlEncode(value);
		}
        private void RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (DateTime.Now.ToString("HH:mm:ss") == TimeForRefresh)
                {
                    if (clsSch != null)
                    {
                        clsSch.ClearJobs();
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.WriteIntoEventLog("Sub = RefreshTimer_Elapsed", ex.ToString(), EventLogEntryType.Error);
            }

        }

    }
}
