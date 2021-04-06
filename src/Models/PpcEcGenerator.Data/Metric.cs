﻿using System.Collections.Generic;
using System.IO;

namespace PpcEcGenerator.Data
{
    public abstract class Metric
    {
        private List<Requirement> requirements;

        protected Metric(string filePath)
        {
            CreateRequirementsFrom(File.ReadAllLines(filePath));
        }

        private void CreateRequirementsFrom(string[] fileReq)
        {
            requirements = new List<Requirement>();

            foreach (string req in fileReq)
            {
                string path = ExtractPathFrom(req);
                
                requirements.Add(new Requirement(path));
            }
        }

        private string ExtractPathFrom(string str)
        {
            // StartPoint is used to remove all char before the "["
            int startPoint = str.IndexOf("[");
            string path = str.Substring(startPoint);
            
            return path.Trim(new char[] { ' ', '[', ']', '\n' });
        }

        public void CountReqCovered(List<Test> listTestPath)
        {
            foreach (Requirement requirement in requirements)
            {
                foreach (Test test in listTestPath)
                {
                    if (!test.path.Contains(requirement.path) || requirement.feasible == false)
                        continue;

                    ParseTestPath(requirement, test);
                }
            }
        }

        protected abstract void ParseTestPath(Requirement requirement, Test test);

        public void ParseInfeasiblePath(List<string> listInfeasiblePaths)
        {
            foreach (string infeasiblePath in listInfeasiblePaths)
            {
                foreach (Requirement requirement in requirements)
                {
                    if (requirement.path.Contains(infeasiblePath))
                    {
                        requirement.feasible = false;
                    }
                }
            }
        }

        protected int GetTotalRequirements()
        {
            return requirements.Count;
        }
    }
}
