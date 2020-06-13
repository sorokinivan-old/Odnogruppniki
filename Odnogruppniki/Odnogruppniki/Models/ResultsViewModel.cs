using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odnogruppniki.Models
{
    public class ResultsViewModel
    {
        public int? id_answer { get; set; }
        public int? id_interview { get; set; }
        public int id_question { get; set; }
        public int id_user { get; set; }
        public string answer { get; set; }
    }
}