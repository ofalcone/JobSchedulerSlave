using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slave1.Models
{
    public class JobResult
    {
        public int Pid { get; set; }
        public int ExitCode { get; set; }
        public int IdNode { get; set; }
        public string StandardOutput { get; set; }
    }
}
