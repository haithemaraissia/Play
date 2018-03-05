using System;
using System.Diagnostics;
using System.IO;

namespace MySnapps.Generator
{
    public class PdfGenerator : IGenerator
    {
        public void GenerateDocument(string pdfOutputLocation, string outputFilenamePrefix, string[] urls, string[] options = null)
        {
            var pdfHtmlToPdfExePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + @"\wkhtmltopdf\wkhtmltopdf.exe";
            var urlsSeparatedBySpaces = string.Empty;
            try
            {
                //Determine inputs
                if ((urls == null) || (urls.Length == 0))
                    throw new Exception("No input URLs provided for HtmlToPdf");
                urlsSeparatedBySpaces = String.Join(" ", urls);

                //Assemble destination PDF file name
                string outputFilename = outputFilenamePrefix + "_" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fff") + ".PDF";

                //--page-size A4 --viewport-size 1366x768
                // options = new[] { "--page-size A4 --viewport-size 1280x1024 --disable-smart-shrinking" };

                //Execute Process
                var p = new Process
                {
                    StartInfo =
                    {
                        FileName = pdfHtmlToPdfExePath,
                        Arguments = ((options == null) ? "" : String.Join(" ", options)) + " " + urlsSeparatedBySpaces + " " + outputFilename,
                        UseShellExecute = false, // needs to be false in order to redirect output
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true, // redirect all 3, as it should be all 3 or none
                        CreateNoWindow = true,
                        WorkingDirectory = pdfOutputLocation
                    }
                };
                p.Start();

                // read the output here...
                var errorOutput = p.StandardError.ReadToEnd();

                // ...then wait n milliseconds for exit (as after exit, it can't read the output)
                p.WaitForExit(60000);

                // read the exit code, close process
                int returnCode = p.ExitCode;
                p.Close();

                // if 0 or 2, it worked so return path of pdf
                if (returnCode == 0 || returnCode == 2) return;
                throw new Exception(errorOutput);
            }
            catch (Exception exc)
            {
                throw new Exception("Problem generating PDF from HTML, URLs: " + urlsSeparatedBySpaces + ", outputFilename: " + outputFilenamePrefix, exc);
            }
        }
    }
}