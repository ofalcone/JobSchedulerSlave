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
        string readOut ="";

        [HttpPost]
        public async Task<IActionResult> StartJob(Job job)
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
                    process.StartInfo.Arguments = job.Argument;

                    //process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);//(sender, args) => jobResult.StandardOutput = args.Data;
                    //process.OutputDataReceived += (s, ea) => jobResult.StandardOutput=ea.Data;
                    process.OutputDataReceived += new DataReceivedEventHandler(HandleOutputData);
                    process.Start();
                    process.BeginOutputReadLine();
                    //jobResult.StandardOutput = await process.StandardOutput.ReadToEndAsync();
                    //process.WaitForExit();

                    jobResult.Pid = process.Id;
                    ////jobResult.ExitCode = process.ExitCode;
                   
                    jobResult.IdNode = idNode;
                    jobResult.StandardOutput = readOut;
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


        private void HandleOutputData(object sender, DataReceivedEventArgs e)
        {
            readOut=e.Data;
        }
    }
}
