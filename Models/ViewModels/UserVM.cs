using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;

namespace WebApplication1.Models.ViewModels
{
    public class UserVM
    {
        [Required]
        [DisplayName("Логин")]
        public string Login { get; set; }

        [Required]
        [DisplayName("Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}