using System.ComponentModel.DataAnnotations;

namespace MVC_POS.Models
{
    public class CardM
    {
        [Required(ErrorMessage = "Card ID is required")]
        [Display(Name = "Card ID")]
        public Guid CardId { get; set; }
        public Guid UserID { get; set; }
    }
}
