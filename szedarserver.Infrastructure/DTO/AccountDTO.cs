using System;
using System.Collections.Generic;
using System.Text;

namespace szedarserver.Infrastructure.DTO
{
    public class AccountDTO
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
