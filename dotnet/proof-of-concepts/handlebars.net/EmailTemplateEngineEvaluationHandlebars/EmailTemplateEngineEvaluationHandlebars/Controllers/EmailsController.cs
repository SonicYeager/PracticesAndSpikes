using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HandlebarsDotNet;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace EmailTemplateEngineEvaluationHandlebars.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            //fetch templates
            using var sourceReader = new StreamReader("Templates/MainLayout.html");
            using var partialReader = new StreamReader("Templates/NewOrderNotifcationTemplate.html");

            var source = sourceReader.ReadToEnd();
            var partialSource = partialReader.ReadToEnd();

            //handle Templates
            var env = Handlebars.CreateSharedEnvironment();
            var env2 = Handlebars.CreateSharedEnvironment();
            env.RegisterTemplate("Body", partialSource);

            var template = Handlebars.Compile(source);

            var data = new {
                OrderLink = "https://localhost:3000"
            };

            var result = template(data);

            //send as mail;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Tobias Wollrab Relaxdays", "tobiaswollrab@relaxdays.de"));
            message.To.Add(new MailboxAddress("Tobias Wollrab GMail RecipientLastName", "tobiaswollrab@gmail.com"));
            message.Subject = "New order is ready for processing in Extraworld.";

            message.Body = new TextPart(TextFormat.Html)
            {
                Text = result
            };

            using var client = new SmtpClient
            {
                // For demo-purposes, accept all SSL certificates
                ServerCertificateValidationCallback = (_, _, _, _) => true,
            };

            client.Connect("localhost", 2525, false);

            client.Send(message);
            client.Disconnect(true);
            return NoContent();
        }
    }
}
