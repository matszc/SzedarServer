using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace szedarserver.Core.Domain
{
    public class User: EntityGuid
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [MaxLength(20)]
        public string Login { get; set; }
        public User (string Email, string Password, string Login)
        {
            Id = Guid.NewGuid();
            this.Email = Email;
            this.Password = Password;
            this.Login = Login;
        }
        public User()
        {

        }
    }
}
