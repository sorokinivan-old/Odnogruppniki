using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odnogruppniki.Models
{
    public class SearchUserViewModel
    {
        public int id_user { get; set; }
        public string photo { get; set; }
        public string name { get; set; }
        public int id_university { get; set; }
        public string university { get; set; }
        public int? id_faculty { get; set; }
        public string faculty { get; set; }
        public int? id_department { get; set; }
        public string department { get; set; }
        public int? id_group { get; set; }
        public string @group { get; set; }
    }
}