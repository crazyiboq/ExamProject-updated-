using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamProject_updated_
{
    internal class TestQuestion
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int QuestionId { get; set; }

        public Test Test { get; set; }
        public Question Question { get; set; }

        public override string ToString()
        {
            return $"Id: {Id} - TestId: {TestId} - QuestionId: {QuestionId}";
        }
    }
}
