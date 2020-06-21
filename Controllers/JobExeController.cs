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
        string readOut = "";

        [HttpPost]
        public IActionResult StartJob(Job job)
        {
            if (string.IsNullOrWhiteSpace(job.Path)
                || job.IdNodeList == null
                || job.IdNodeList.Count < 1)
            {
                return BadRequest();
            }

            List<JobResult> jobResultList = new List<JobResult>();

            try
            {
                foreach (var idNode in job.IdNodeList)
                {
                    JobResult jobResult = new JobResult();

                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = job.Path;
                        process.StartInfo.Arguments = job.Argument;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.Arguments = job.Argument;

                        process.OutputDataReceived += new DataReceivedEventHandler(HandleOutputData);
                        process.Start();
                        process.BeginOutputReadLine();

                        jobResult.Pid = process.Id;
                        jobResult.IdNode = idNode;
                        jobResult.StandardOutput = readOut;
                        jobResultList.Add(jobResult);
                    }
                }
            }
            catch (Exception)
            {
                return NotFound();
            }

            return Ok(jobResultList);
        }

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
                var processFound = Process.GetProcessById(jobKill.Pid);
                processFound.Kill(killProcessTree);
            }
            catch (Exception)
            {
                return NotFound();
            }

            return Ok();
        }


        private void HandleOutputData(object sender, DataReceivedEventArgs e)
        {
            readOut = e.Data;
        }
    }
}
