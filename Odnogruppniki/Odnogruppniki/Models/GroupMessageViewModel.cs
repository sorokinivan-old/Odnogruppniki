using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odnogruppniki.Models
{
    public class GroupMessageViewModel
    {
        public int id { get; set; }
        public int id_in { get; set; }
        public int id_out { get; set; }
        public string dateString { get; set; }
        public DateTime date { get; set; }
        public string message { get; set; }
        public string name { get; set; }
    }
}