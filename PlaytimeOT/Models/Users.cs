using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaytimeOT.Models
{
    public class Users
    {
        public int UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public byte[] UserPassword { get; set; }
        public Guid UserSalt { get; set; }
        public DateTime UserCreatedOn { get; set; }
    }
}
