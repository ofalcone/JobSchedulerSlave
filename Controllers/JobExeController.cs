﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
        int exitCode= -999;

        [HttpPost]
        public IActionResult StartJob(LaunchJob launchJob)
        {
            if (launchJob == null
                || string.IsNullOrWhiteSpace(launchJob.Path)
                || launchJob.NodeId == 0)
            {
                return BadRequest();
            }
            JobResult jobResult = new JobResult();

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = launchJob.Path;
                    if (string.IsNullOrWhiteSpace(launchJob.Argomenti) == false)
                    {
                        process.StartInfo.Arguments =$"\"{launchJob.Argomenti}\"";
                    }
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;

                    process.OutputDataReceived += new DataReceivedEventHandler(HandleOutputData);
                    //process.OutputDataReceived += (sender, args) => readOut = args.Data;
                    process.Start();
                    process.BeginOutputReadLine();
                    //readOut = process.StandardOutput.ReadToEnd();

                    //var output = new List<string>();

                    //while (process.StandardOutput.Peek() > -1)
                    //{
                    //    output.Add(process.StandardOutput.ReadLine());
                    //}

                    jobResult.Pid = process.Id;
                    jobResult.IdNode = launchJob.NodeId;
                    if (string.IsNullOrWhiteSpace(readOut))
                    {
                        Thread.Sleep(500);
                    }
                    jobResult.StandardOutput = readOut;
                }
            }
            catch (Exception)
            {
                return NotFound();
            }

            return Ok(jobResult);
        }

        [HttpPost]
        public IActionResult KillJob(StopJob stopJob)
        {
            bool killProcessTree = false;
            JobResult jobResult = null;

            if (stopJob.Pid == 0)
            {
                return BadRequest();
            }

            try
            {
                var processFound = Process.GetProcessById(stopJob.Pid);
                processFound.EnableRaisingEvents = true;
                processFound.Exited += ProcessEnded;
                processFound.Kill(killProcessTree);
                processFound.WaitForExit();

                if (processFound != null)
                {
                    jobResult = new JobResult
                    {
                        Pid = stopJob.Pid,
                        ExitCode = exitCode
                    };
                }

                //processFound.Kill(killProcessTree);
            }
            catch (Exception )
            {
                return NotFound(null);
            }

            return Ok(jobResult);
        }


        private void HandleOutputData(object sender, DataReceivedEventArgs e)
        {
            readOut = e.Data;
        }

        private void ProcessEnded(object sender, EventArgs e)
        {
            var process = sender as Process;
            if (process != null)
            {
                exitCode = process.ExitCode;
            }
        }
    }
}
