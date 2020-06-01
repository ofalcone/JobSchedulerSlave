using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using slave1.Models;

namespace slave1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobExeController : ControllerBase
    {
        // GET: api/JobExe
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/JobExe/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/JobExe
        [HttpPost]
        public async Task<IActionResult> Post(Job job)
        {
            
            List<JobResult> jobResultList = new List<JobResult>();

            foreach (var idNode in job.IdNodeList)
            {
                JobResult jobResult = new JobResult();

                using (Process process = new Process())
                {
                    process.StartInfo.FileName = job.Path;
                    process.StartInfo.Arguments = job.Argument;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();
                    process.WaitForExit();

                    jobResult.Pid = process.Id;
                    jobResult.ExitCode = process.ExitCode;
                    jobResult.StandardOutput = process.StandardOutput.ReadToEnd();
                    jobResult.IdNode = idNode;
                    jobResultList.Add(jobResult);

                }   
            }
            return Ok(jobResultList);
        }

        // PUT: api/JobExe/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
