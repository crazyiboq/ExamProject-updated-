﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamProject_updated_
{
    internal class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Text: {Text}";
        }
    }
}
