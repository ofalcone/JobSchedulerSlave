using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slave1.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Argument { get; set; }
        public  List<int> IdNodeList { get; set; }
    }
}
