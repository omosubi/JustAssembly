using JustAssembly.API.Analytics;
using JustAssembly.Core;
using JustAssembly.Infrastructure.Analytics;
using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using JustAssembly.CommandLineInterface;

namespace JustAssembly.CommandLineTool
{
    public class Startup
    {
        private static IAnalyticsService analytics;

        static void Main(string[] args)
        {
            analytics = AnalyticsServiceImporter.Instance.Import();

            analytics.Start();
            analytics.TrackFeature("Mode.CommandLine");

            try
            {
                RunMain(args);
            }
            catch (Exception ex)
            {
                analytics.TrackException(ex);
                analytics.Stop();

                throw;
            }

            analytics.Stop();
        }

        private static void RunMain(string[] args)
        {
            Options options = new Options();
            try
            {
                Parser.Default.ParseArguments(args, options);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    WriteErrorAndSetErrorCode(e.InnerException.Message);
                }
                return;
            }

            if (options.Help)
            {
                Console.WriteLine(CommandLine.Text.HelpText.AutoBuild(options));
                return;
            }

            string xml = string.Empty;
            try
            {
                IDiffItem diffItem = APIDiffHelper.GetAPIDifferences(options.BaseAssemblyPath, options.ComparisonAssemblyPath);
                if (diffItem != null)
                {
                    xml = diffItem.ToXml();
                }
            }
            catch (Exception ex)
            {
                WriteExceptionAndSetErrorCode("A problem occurred while creating the API diff.", ex);
                return;
            }

            try
            {
                string outputFileName = Path.Combine(options.OutputPath, "results.xml");
                if (!File.Exists(outputFileName))
                {
                    File.Create(outputFileName);
                }

                using (StreamWriter writer = new StreamWriter(outputFileName))
                {
                    writer.Write(xml);
                }
            }
            catch (Exception ex)
            {
                WriteExceptionAndSetErrorCode("There was a problem while writing output file.", ex);
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"API differences calculated successfully. Find output here: {options.OutputPath}");
            Console.ResetColor();
        }

        private static void WriteErrorAndSetErrorCode(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();

            Environment.ExitCode = 1;
        }

        private static void WriteExceptionAndSetErrorCode(string message, Exception ex)
        {
            analytics.TrackException(ex);

            WriteErrorAndSetErrorCode($"{message}{Environment.NewLine}{ex}");
        }
    }
}
