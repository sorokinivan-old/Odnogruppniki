using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odnogruppniki.Models
{
    public class ResultCountViewModel
    {
        public int idQuestion { get; set; }
        public int idAnswer { get; set; }
        public int count { get; set; }
    }
}