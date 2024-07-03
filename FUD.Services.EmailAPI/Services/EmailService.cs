using FUD.Services.EmailAPI.Data;
using FUD.Services.EmailAPI.Message;
using FUD.Services.EmailAPI.Models;
using FUD.Services.EmailAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FUD.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _options;

        public EmailService(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("<br/>Cart Email Requested ");
            message.AppendLine("<br/>Total " + cartDto.CartHeader.CartTotal);
            message.AppendLine("<br/>");
            message.AppendLine("<ul>");
            foreach(var item in cartDto.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product.Name + " x " + item.Count);
                message.Append("</li>");
            }
            message.AppendLine("</ul>");

            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);
        }

        public async Task EmailUserAndLog(UserDto userDto)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("<br/>New User Email Requested ");
            message.AppendLine("<br/>Email " + userDto.Email);
            message.AppendLine("<br/>");

            await LogAndEmail(message.ToString(), "dotnetmastery@gmail.com");
        }

        public async Task LogOrderPlaced(RewardMessage rewardMessage)
        {
            string message = "New Order Placed. <br/> Order Id: " + rewardMessage.OrderId;
            await LogAndEmail(message, "dotnetmastery@gmail.com");
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLog = new()
                {
                    Email = email,
                    Message = message,
                    EmailSent = DateTime.Now
                };
                await using var _context = new AppDbContext(_options);
                await _context.EmailLoggers.AddAsync(emailLog);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
