using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace BLeaf.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _sendGridApiKey;

        public EmailSender(string sendGridApiKey)
        {
            _sendGridApiKey = sendGridApiKey;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress("bleafcaf@gmail.com", "BLeaf Cafe");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.OK &&
                response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new Exception($"Failed to send email: {response.StatusCode}");
            }
        }
    }
}