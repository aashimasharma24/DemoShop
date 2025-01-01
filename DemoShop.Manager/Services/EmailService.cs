using DemoShop.Manager.Services.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Manager.Services
{
    public class EmailService : IEmailService
    {
        public void Send(string to, string subject, string body)
        {
            Log.Information("Email sent to {Recipient} with subject: {Subject}", to, subject);
            Log.Information("Email body: {Body}", body);
        }
    }
}
