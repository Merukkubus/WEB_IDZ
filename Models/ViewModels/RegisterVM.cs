using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Укажите логин")]
        [MaxLength(30, ErrorMessage = "Логин должен быть меньше 30 символов")]
        [MinLength(5, ErrorMessage = "Логин должен быть больше 5 символов")]
        public string Login { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Введите пароль")]
        [MinLength(4, ErrorMessage = "Пароль должен быть больше 4 символов")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConf { get; set; }
    }
}