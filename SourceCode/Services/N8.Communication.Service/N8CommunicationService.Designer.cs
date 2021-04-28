using System.Configuration;
namespace N8.Communication
{
    partial class N8CommunicationService
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.eventLog = new System.Diagnostics.EventLog();
            this.RefreshTimer = new System.Timers.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RefreshTimer)).BeginInit();
            // 
            // eventLog
            // 
            this.eventLog.Log = "Application";
            this.eventLog.Source = "N8.Communication.Service";
            // 
            // RefreshTimer
            // 
            this.RefreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.RefreshTimer_Elapsed);
            // 
            // N8CommunicationService
            // 
            //this.ServiceName = "N8 Communication Service";

            this.ServiceName = "N8ServiceCommunication";
            ((System.ComponentModel.ISupportInitialize)(this.eventLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RefreshTimer)).EndInit();

        }

        #endregion

        internal System.Timers.Timer RefreshTimer;
    }
}
