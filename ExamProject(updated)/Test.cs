using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamProject_updated_
{
    internal class Test
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }

        public Category Category { get; set; }

        public ICollection<TestQuestion> TestQuestions { get; set; }

        public override string ToString()
        {
            return $"ID: {Id}, Title: {Title}, CreatedAt: {CreatedAt.ToShortDateString()}";
        }
    }
}
