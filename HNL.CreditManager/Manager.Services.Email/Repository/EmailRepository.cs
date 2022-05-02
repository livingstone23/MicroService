//using Manager.Services.Email.Messages;
//using Microsoft.EntityFrameworkCore;

using Manager.Services.Email.DbContexts;
using Manager.Services.Email.Messages;
using Manager.Services.Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Manager.Services.Email.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;

        public EmailRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
        {
            //implement an email sender or call some other class library
            //En esta seccion se hace el envio de correo y luego se registra en la base de datos
            EmailLog emailLog = new EmailLog()
            {
                Email = message.Email,
                EmailSent = DateTime.Now,
                Log = $"Order - {message.OrderId} has been created successfully."
            };

            await using var _db = new ApplicationDbContext(_dbContext);
            _db.EmailLogs.Add(emailLog);
            await _db.SaveChangesAsync();

        }
    }
}
