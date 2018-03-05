using System.ComponentModel;

namespace MySnapps.Generator
{
    public enum GeneratorType { PDF, JPEG}

    public class GeneratorFactory 
    {
        public IGenerator GetGenerator(GeneratorType type)
        {
            switch (type)
            {
                case GeneratorType.PDF:
                    return  new PdfGenerator();
                case GeneratorType.JPEG:
                    return new JpegGenerator();
                default:
                    throw new InvalidEnumArgumentException(@"Unrecognized GeneratorType value.");
            }
        }
    }
}
