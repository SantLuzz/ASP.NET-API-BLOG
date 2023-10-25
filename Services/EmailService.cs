using System.Net;
using System.Net.Mail;

namespace Blog.Services
{
    public class EmailService
    {
        /// <summary>
        /// envia um email de dentro do sistema usando uma conexão smtp
        /// </summary>
        /// <param name="toName">Nome Destinatario</param>
        /// <param name="toEmail">Email Destinatario</param>
        /// <param name="subject">Assunto</param>
        /// <param name="body">Corpo Email</param>
        /// <param name="fromName">Nome Remetente</param>
        /// <param name="fromEmail">Email Remetente</param>
        /// <returns>Retorna verdadeiro caso consiga e falso caso não</returns>
        public bool Send(
            string toName,
            string toEmail,
            string subject,
            string body,
            string fromName = "Luiz Felipe",
            string fromEmail = "luiz.santos1749@outlook.com")
        {
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //criando uma instancia do smtpClient
            var smtpClient = new SmtpClient(Configuration.Smtp.Host, Configuration.Smtp.Port);
            //passando as credenciais
            smtpClient.Credentials = new NetworkCredential(Configuration.Smtp.UserName, Configuration.Smtp.Password);
            //método de entrega
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //informando que to usando porta segura
            smtpClient.EnableSsl= true;
            smtpClient.UseDefaultCredentials = false;

            //criando a mensagem de email
            var mail = new MailMessage();

            mail.From = new MailAddress(fromEmail, fromName);
            mail.To.Add(new MailAddress(toEmail, toName));
            mail.Subject = subject; //assunto
            mail.Body = body; //corpo
            mail.IsBodyHtml = true; //marca que posso mandar como html

            try
            {
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
