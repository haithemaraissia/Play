namespace MySnapps.Generator
{
    /// <summary>
    /// Convert Html page at a given URL to a JPEG file using open-source tool wkhtml2pdf
    ///   wkhtml2pdf can be found at: http://code.google.com/p/wkhtmltopdf/
    ///   Useful code used in the creation of this I love the good folk of StackOverflow!: http://stackoverflow.com/questions/1331926/calling-wkhtmltopdf-to-generate-pdf-from-html/1698839
    ///   An online manual can be found here: http://madalgo.au.dk/~jakobt/wkhtmltoxdoc/wkhtmltopdf-0.9.9-doc.html
    /// Ensure that the output folder specified is writeable by the ASP.NET process of IIS running on your server
    /// This code requires that the Windows installer is installed on the relevant server / client.  This can either be found at:
    ///   http://code.google.com/p/wkhtmltopdf/downloads/list - download wkhtmltopdf-0.9.9-installer.exe
    /// </summary>
    /// <param name="jpegOutputLocation"></param>
    /// <param name="outputFilenamePrefix"></param>
    /// <param name="urls"></param>
    /// <param name="options"></param>
    /// <returns>the URL of the generated PDF</returns>
    public interface IGenerator
    {
        void GenerateDocument(string outputLocation, string outputFilenamePrefix, string[] urls, string[] options = null);
    }
}