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
            this.GnuplotProcess.StartInfo = new ProcessStartInfo()
            {
                FileName = Path,
                Arguments = "-persist",
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };
            if (Start)
                this.Start();
        }
        public Gnuplot(string Path) : this(Path, true) { }

        StreamWriter Input;
        StreamReader Output;
        public void Start()
        {
            this.GnuplotProcess.Start();
            this.Input = this.GnuplotProcess.StandardInput;
            this.Output = this.GnuplotProcess.StandardOutput;
            this.GnuplotProcess.ErrorDataReceived += new DataReceivedEventHandler(GnuplotProcess_ErrorDataReceived);
            this.ErrorData = new List<string>();
            this.GnuplotProcess.BeginErrorReadLine();
        }

        List<string> ErrorData;
        void GnuplotProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            ErrorData.Add(e.Data);
        }

        public void WriteLine(string format, params object[] args)
        {
            this.Input.WriteLine(format, args);
        }
        public void WriteLine(string format)
        {
            this.Input.WriteLine(format);
        }
        public void WriteLine(object obj)
        {
            this.Input.WriteLine(obj.ToString());
        }
        public void WriteLine()
        {
            this.Input.WriteLine();
        }

        public bool ThrowErrorException { get; set; }
        public string ReadError()
        {
            bool ErrorFlag = false;

            StringBuilder ErrorMessage = new StringBuilder();
            while (true)
            {
                if (ErrorData.Count == 0)
                    continue;
                else
                {
                    var mes = ErrorData[0];
                    ErrorData.RemoveAt(0);

                    if (mes.EndsWith("gnuplot> "))
                        break;
                    else
                    {
                        ErrorMessage.Append(mes);
                        ErrorMessage.Append('\n');
                        if (!ErrorFlag)
                        {
                            if (!mes.StartsWith("gnuplot> "))
                                ErrorFlag = true;
                        }
                    }
                }
            }
            if (ErrorFlag)
            {
                if (ThrowErrorException)
                    throw new GnuplotException(ErrorMessage.ToString());
                return ErrorMessage.ToString();
            }
            else
            {
                return null;
            }
        }

        public void Dispose()
        {
            if (!GnuplotProcess.HasExited)
            {
                this.Input.WriteLine();
                this.Input.WriteLine("quit");
                this.Input.Dispose();
                this.Output.Dispose();
                this.GnuplotProcess.CancelErrorRead();
            }
        }
        ~Gnuplot()
        {
            this.Dispose();
        }
    }
    public class GnuplotException : Exception
    {
        public GnuplotException(string mes) : base(mes) { }
    }
}
