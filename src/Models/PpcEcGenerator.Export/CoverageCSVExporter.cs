﻿using PpcEcGenerator.Data;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PpcEcGenerator.Export
{
    public class CoverageCSVExporter : IExporter
    {
        //---------------------------------------------------------------------
        //		Attributes
        //---------------------------------------------------------------------
        private static readonly string DELIMITER = ";";
        private readonly string output;
        private readonly IDictionary<string, List<Coverage>> coverage;


        //---------------------------------------------------------------------
        //		Constructor
        //---------------------------------------------------------------------
        public CoverageCSVExporter(string output, IDictionary<string, List<Coverage>> coverage)
        {
            this.output = output;
            this.coverage = coverage;
        }


        //---------------------------------------------------------------------
        //		Methods
        //---------------------------------------------------------------------
        public void Export()
        {
            StringBuilder sb = new StringBuilder();

            if (!File.Exists(output))
                sb.Append("TestMethod;CoveredMethod;EdgeCoverage;PrimePathCoverage\n");

            foreach (KeyValuePair<string, List<Coverage>> kvp in coverage)
            {
                foreach (Coverage c in kvp.Value)
                {
                    sb.Append(c.TestMethod);
                    sb.Append(DELIMITER);
                    sb.Append(c.CoveredMethod);
                    sb.Append(DELIMITER);
                    sb.Append(c.EdgeCoverage.ToString("0.0000"));
                    sb.Append(DELIMITER);
                    sb.Append(c.PrimePathCoverage.ToString("0.0000"));
                    sb.Append(DELIMITER);
                    sb.Append('\n');
                }
            }

            File.AppendAllText(output, sb.ToString());
        }
    }
}
