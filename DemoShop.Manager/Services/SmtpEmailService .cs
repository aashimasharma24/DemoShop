using DemoShop.Manager.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Manager.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendOrderConfirmationEmailAsync(int userId, int orderId)
        {
            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = Convert.ToInt32(_configuration["EmailSettings:SmtpPort"]);
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromPassword = _configuration["EmailSettings:FromPassword"];

                var userEmail = "customer@example.com";

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    // If SMTP requires credentials
                    client.Credentials = new NetworkCredential(fromEmail, fromPassword);
                    client.EnableSsl = true; // or false if not using SSL

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail),
                        Subject = $"Order #{orderId} Confirmation",
                        Body = $"Thank you for your order! Your order ID is {orderId}.",
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add(userEmail);

                    await client.SendMailAsync(mailMessage);
                }

                _logger.LogInformation($"Order confirmation email sent to User #{userId} for Order #{orderId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending order confirmation email.");
                // TODO: store failed emails in a queue or database for retry
            }
        }
    }
}
