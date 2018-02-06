using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;

namespace JustAssembly.CommandLineInterface
{
    public class Options
    {


        private static readonly List<string> ValidInputFileExtensions = new List<string>() { ".dll", ".exe" };
        private string baseAssemblyPath;
        private string comparisonAssemblyPath;
        private string outputPath;

        //[Option("comparedirectory", DefaultValue = false, HelpText = "Compare all dlls found in 2 directories. Will only compare if an exact filename match is found.")]
        //public bool CompareDirectory { get; set; }

        [Option('b', "basePath", HelpText = "Base Assembly Path", Required = true)]
        public string BaseAssemblyPath
        {
            get { return baseAssemblyPath; }
            set
            {
                if (!File.Exists(value) && ValidInputFileExtensions.Contains(Path.GetExtension(value)))
                {
                    throw new ArgumentException("The base assembly path parameter (-b, --basePath) does not contain a valid path.");
                }

                baseAssemblyPath = value;
            }
        }

        [Option('c', "comparisonPath", HelpText = "Comparison Assembly Path", Required = true)]
        public string ComparisonAssemblyPath
        {
            get { return comparisonAssemblyPath; }
            set
            {
                if (!File.Exists(value) && ValidInputFileExtensions.Contains(Path.GetExtension(value)))
                {
                    throw new ArgumentException("The comparison assembly path parameter (-c, --comparisonPath) does not contain a valid path.");
                }

                comparisonAssemblyPath = value;
            }
        }

        [Option('o', "outputDirectory", HelpText = "Output Directory for results", Required = true)]
        public string OutputPath
        {
            get { return outputPath; }
            set
            {
                if (!Directory.Exists(value))
                {
                    throw new ArgumentException("The output path parameter (-o, --output) does not contain a valid path.");
                }

                outputPath = value;
            }
        }

        [Option('?', "help", HelpText = "Prints help", Required = false, DefaultValue = false)]
        public bool Help { get; set; }
    }
}