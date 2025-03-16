using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamProject_updated_
{
    internal class GlobalLeaderboard
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TotalScore { get; set; } 

        public User User { get; set; }
    }
}
