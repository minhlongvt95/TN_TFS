using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.HubConfig
{
    public class Notification : Hub
    {
        //public async Task ReceiveNotifications(Notifications listNotifications)
        //{
        //    await Clients.All.SendAsync("ReceiveNotifications", listNotifications);
        //}

        //you're going to invoke this method from the client app
        public void Echo(string message)
        {
            //you're going to configure your client app to listen for this
            Clients.All.SendAsync("Send", message);
        }
    }
}
