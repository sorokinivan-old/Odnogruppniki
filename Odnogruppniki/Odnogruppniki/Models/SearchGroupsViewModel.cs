using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odnogruppniki.Models
{
    public class SearchGroupsViewModel
    {
        public int id_group { get; set; }
        public string name { get; set; }
        public int id_faculty { get; set; }
        public string faculty { get; set; }
        public int id_department { get; set; }
        public string department { get; set; }
    }
}