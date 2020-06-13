using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odnogruppniki.Models
{
    public class ListSaatiViewModel
    {
        public int idInterview { get; set; }
        public int? idCategory { get; set; }
        public string groupName { get; set; }
        public double Result { get; set; }
    }
}