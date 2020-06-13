using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odnogruppniki.Models
{
    public class PersonalMessageViewModel
    {
        public int id { get; set; }
        public int id_in { get; set; }
        public string name_in { get; set; }
        public string photo_in { get; set; }
        public int id_out { get; set; }
        public string name_out { get; set; }
        public string photo_out { get; set; }
        public string dateString { get; set; }
        public DateTime date { get; set; }
        public string message { get; set; }
    }
}