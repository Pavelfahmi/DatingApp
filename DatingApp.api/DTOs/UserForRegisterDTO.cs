using System.ComponentModel.DataAnnotations;
namespace DatingApp.api.DTOs
{
    public class UserForRegisterDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="The password must be between 4 an 8 characters")]
        public string Password{ get; set; }

    }
}