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
        public string Password { get; set; }
        [Required]
        [MaxLength(20)]
        public string Login { get; set; }

        public string FbId { get; set; }
        public User (string email, string password, string login)
        {
            Id = Guid.NewGuid();
            Email = email;
            Password = password;
            Login = login;
        }
        public User()
        {

        }

        public User(string email, string login, string fbId, string password)
        {
            Id = Guid.NewGuid();
            Email = email;
            Login = login;
            FbId = fbId;
        }
    }
}
