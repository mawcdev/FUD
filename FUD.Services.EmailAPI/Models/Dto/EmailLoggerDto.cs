using System.ComponentModel.DataAnnotations;

namespace FUD.Services.EmailAPI.Models.Dto
{
    public class EmailLoggerDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime? EmailSent { get; set; }
    }
}
