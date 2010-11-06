using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Aont
{
    class Gnuplot : IDisposable
    {
        Process GnuplotProcess;
        public Gnuplot(string Path, bool Start)
        {
            this.GnuplotProcess = new Process();
            var StartInfo = this.GnuplotProcess.StartInfo;
            StartInfo.FileName = Path;
            StartInfo.Arguments = "-persist";
            StartInfo.RedirectStandardInput =
                StartInfo.RedirectStandardError =
                StartInfo.RedirectStandardOutput =
                    true;
            StartInfo.UseShellExecute = false;

            if (Start) this.Start();
        }
        public Gnuplot(string Path) : this(Path, true) { }


        StreamWriter Input;
        StreamReader Output;
        //StreamReader Error;
        public void Start()
        {
            this.GnuplotProcess.Start();
            this.Input = this.GnuplotProcess.StandardInput;
            this.Output = this.GnuplotProcess.StandardOutput;
            //this.Error = this.GnuplotProcess.StandardError;
            this.GnuplotProcess.ErrorDataReceived += new DataReceivedEventHandler(GnuplotProcess_ErrorDataReceived);
            this.ErrorData = new List<string>();
            this.GnuplotProcess.BeginErrorReadLine();

        }

        List<string> ErrorData;
        void GnuplotProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e) { ErrorData.Add(e.Data); }
        bool ThrowErrorException = false;

        public void WriteLineNoWait(string format, params object[] args) { this.Input.WriteLine(format, args); }

        public string WriteLine(string format, params object[] args)
        {
            return this.WriteLine(true, format, args);
        }
        public string WriteLine(bool WaitResponse, string format, params object[] args)
        {
            bool ErrorFlag = false;
            WriteLineNoWait(format, args);
            if (!WaitResponse)
                return null;

            StringBuilder ErrorMessage = new StringBuilder();
            while (true)
            {
                if (ErrorData.Count == 0)
                    continue;
                else
                {
                    var mes = ErrorData[0]; ErrorData.RemoveAt(0);
                    if (mes == null)
                        continue;
                    //Error.ReadLine();
                    //Console.WriteLine(mes);
                    if (mes.EndsWith("gnuplot> ")) { break; }
                    else
                    {
                        ErrorMessage.Append(mes);
                        ErrorMessage.Append('\n');
                        if (!ErrorFlag)
                        {
                            if (//!mes.StartsWith("input data ('e' ends) >") && 
                                !mes.StartsWith("gnuplot> "))
                                ErrorFlag = true;
                        }
                    }
                }
            }
            if (ErrorFlag)
            {
                var ErrorMessageString = ErrorMessage.ToString();
                if (ThrowErrorException)
                    throw new Exception(ErrorMessageString);
                return ErrorMessageString;
            }
            else { return null; }
        }

        public void Dispose()
        {
            if (!GnuplotProcess.HasExited)
            {
                this.Input.WriteLine();
                this.Input.WriteLine("quit");
                this.Input.Dispose();
                this.Output.Dispose();
                //this.Error.Dispose();
            }
        }
    }
}
