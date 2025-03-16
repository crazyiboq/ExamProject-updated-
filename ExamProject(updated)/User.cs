using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamProject_updated_
{
    internal class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime Birthday { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }

        public Role Role { get; set; }
    }
}
