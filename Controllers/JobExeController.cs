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
    [Route("api/[controller]/[action]")]
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
        public IActionResult StartJob(Job job)
        {

            List<JobResult> jobResultList = new List<JobResult>();

            foreach (var idNode in job.IdNodeList)
            {
                JobResult jobResult = new JobResult();

                //Task.Run(() =>
                //{
                //    using (Process process = new Process())
                //    {
                //        process.StartInfo.FileName = job.Path;
                //        process.StartInfo.Arguments = job.Argument;
                //        process.StartInfo.UseShellExecute = false;
                //        process.StartInfo.RedirectStandardOutput = true;
                //        jobResult.Pid = process.Id;

                //        process.Start();
                //        //process.WaitForExit();


                //        //jobResult.ExitCode = process.ExitCode;
                //        jobResult.StandardOutput = process.StandardOutput.ReadToEnd();
                //        jobResult.IdNode = idNode;
                //        jobResultList.Add(jobResult);

                //    }
                //});

                using (Process process = new Process())
                {
                    process.StartInfo.FileName = job.Path;
                    process.StartInfo.Arguments = job.Argument;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();
                    process.WaitForExit();

                    jobResult.Pid = process.Id;
                    //jobResult.ExitCode = process.ExitCode;
                    //jobResult.StandardOutput = process.StandardOutput.ReadToEnd();
                    jobResult.IdNode = idNode;
                    jobResultList.Add(jobResult);

                }

            }
            return Ok(jobResultList);
        }

        //[HttpPost, Route("/KillJob")]
        //[HttpPost,Route("api/[controller]/[action]")]
        //[HttpPost("{pid}")]
        [HttpPost]
        public IActionResult KillJob(JobKill jobKill)
        {
            bool killProcessTree = false;

            if (jobKill.Pid == 0)
            {
                return BadRequest();
            }
            try
            {
                var processFound = System.Diagnostics.Process.GetProcessById(jobKill.Pid);
                processFound.Kill(killProcessTree);
            }
            catch (Exception)
            {
                return NotFound();
            }

            return Ok();
        }

    }
}
