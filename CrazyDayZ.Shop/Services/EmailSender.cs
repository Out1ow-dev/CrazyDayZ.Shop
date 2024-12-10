using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit.Net.Smtp;
using MimeKit;
using CrazyDayZ.Shop.Models;

namespace CrazyDayZ.Shop.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                           ILogger<EmailSender> logger)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
        }

        public AuthMessageSenderOptions Options { get; } 

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if (string.IsNullOrEmpty(Options.SmtpServer))
            {
                throw new Exception("Null SMTP Server");
            }
            await Execute(subject, message, toEmail);
        }

        public async Task Execute(string subject, string message, string toEmail)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Of.Cloud", Options.SmtpUser));
            mailMessage.To.Add(new MailboxAddress("From Of.Cloud Tools", toEmail));
            mailMessage.Subject = "Подтверждение учетной записи OF.Cloud";

            var htmlBody = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Подтверждение учетной записи</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            margin: 0;
                            padding: 20px;
                            background-color: #f4f4f4;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: auto;
                            background: white;
                            padding: 20px;
                            border-radius: 5px;
                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        }}
                        h1 {{
                            color: #333;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            margin-top: 20px;
                            background-color: #007bff;
                            color: white;
                            text-decoration: none;
                            border-radius: 5px;
                            border: none;
                            cursor: pointer;
                        }}
                        .button:hover {{
                            background-color: #0056b3;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1>Подтвердите свою учетную запись OF.Cloud</h1>
                        <p>Подтвердите свою учетную запись OF.Cloud, нажав на кнопку ниже:</p>
                        {message}
                    </div>
                </body>
                </html>";

            mailMessage.Body = new TextPart("html")
            {
                Text = htmlBody
            };



            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(Options.SmtpServer, 465, true);

                client.Authenticate(Options.SmtpUser, Options.SmtpPassword);

                client.Send(mailMessage);
                //client.Dispose();
                client.Disconnect(true);
            }


        }
    }
}
