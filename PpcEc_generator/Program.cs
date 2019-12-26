﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestPathConsole
{

    class Requirement
    {
        public string path { set; get; }
        public List<string> testPaths;
        public bool covered;
        public bool feasible;

        public Requirement(string path)
        {
            this.path = path;
            this.covered = false;
            this.feasible = true;
            this.testPaths = new List<string>();
        }

    }

    class Test
    {
        public string path { set; get; }
        public List<string> requirements;
        public int newReqPpcCovered;
        public int overallReqPpcCovered;
        public int newReqEcCovered;
        public int overallReqEcCovered;
        public int pathLength;
        public Test(string path)
        {
            this.path = path;
            this.newReqPpcCovered = 0;
            this.overallReqPpcCovered = 0;
            this.newReqEcCovered = 0;
            this.overallReqEcCovered = 0;
            this.requirements = new List<string>();
            this.pathLength = 0;
        }
    }
    class Program
    {
        static void Main()
        {
            string projectPath = @"D:\GitHub\PPC_EC-generator\Projects\Mp3agic";


            CsvGeneratorTestCase(projectPath);
        }
        private static string[] OpenFile(string path)
        {
            string[] lines;
            try
            {
                lines = System.IO.File.ReadAllLines(path);
            }
            catch (FileNotFoundException execption)
            {
                throw execption;
            }
            return lines;
        }
        public static List<string> hasLoop(string path)
        {
            List<string> listPath = new List<string>();

            string[] test = path.Split(',');
            for (int i = 0; i < test.Length; i++)
            {
                for (int j = i + 1; j < test.Length; j++)
                {
                    if (test[i] == test[j] && !(i == (test.Length - test.Length) && j == test.Length - 1))
                    {
                        listPath.Add(path[i..j]);
                        i = j;
                        continue;
                    }
                }
            }
            return listPath;
        }
        public static void CreatListReq(string[] fileReq, List<Requirement> listReq)
        {
            foreach (string req in fileReq)
            {
                // StartPoint is used to remove all char before the "["
                int startPoint = req.IndexOf("[");
                string trProcessed = req.Substring(startPoint);
                Requirement requirement = new Requirement(trProcessed.Trim(new Char[] { ' ', '[', ']', '\n' }));
                listReq.Add(requirement);
            }
        }
        public static void CreateListTestPaths(string[] fileTestPath, List<Test> listTestPath)
        {
            // removing repeted test paths.
            String[] fileTestPathProcessed = fileTestPath.Distinct().ToArray();
            foreach (string item in fileTestPathProcessed.Skip(1))
            {
                string tmp = item.Trim(new Char[] { ' ', '[', ']', '\n' });
                Test test = new Test(tmp);

                listTestPath.Add(test);
            }
        }
        public static void CreateListInfeasiblePaths(string[] fileInfeasiblePaths, List<string> listInfeasiblePaths)
        {
            foreach (string item in fileInfeasiblePaths)
            {
                string tmp = item.Trim(new Char[] { ' ', '[', ']', '\n' });
                listInfeasiblePaths.Add(tmp);
            }
        }
        public static void CheckInfeasibleReq(List<string> listInfeasiblePaths, List<Requirement> listReq)
        {
            foreach (string infeasiblePath in listInfeasiblePaths)
            {
                foreach (Requirement requirement in listReq)
                {
                    if (requirement.path.Contains(infeasiblePath))
                    {
                        requirement.feasible = false;
                    }
                }
            }
        }
        public static void CountReqPpcCovered(List<Requirement> listReqPpc, List<Test> listTestPath)
        {
            foreach (Requirement requirement in listReqPpc)
            {
                foreach (Test test in listTestPath)
                {
                    if (test.path.Contains(requirement.path) && requirement.feasible == true)
                    {
                        if (requirement.covered == false)
                        {
                            test.newReqPpcCovered = test.newReqPpcCovered + 1;
                            requirement.covered = true;
                        }
                        test.overallReqPpcCovered = test.overallReqPpcCovered + 1;
                        test.requirements.Add(requirement.path);
                        requirement.testPaths.Add(test.path);
                    }
                }
            }
        }
        public static void CountReqNcCovered(List<Requirement> listReqNc, List<Test> listTestPath)
        {
            foreach (Requirement requirement in listReqNc)
            {
                foreach (Test test in listTestPath)
                {
                    if (test.path.Contains(requirement.path) && requirement.feasible == true)
                    {
                        if (requirement.covered == false)
                        {
                            test.newReqEcCovered = test.newReqEcCovered + 1;
                            requirement.covered = true;
                        }
                        test.overallReqEcCovered = test.overallReqEcCovered + 1;
                        test.requirements.Add(requirement.path);
                        requirement.testPaths.Add(test.path);
                    }
                }
            }
        }
        public static void SortListByPathLength(List<Test> listTestPath)
        {
            for (int i = 1; i < listTestPath.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (listTestPath[i].pathLength > listTestPath[j].pathLength)
                    {
                        Test test = listTestPath[i];
                        listTestPath[i] = listTestPath[j];
                        listTestPath[j] = test;
                    }
                }
            }
        }
        public static void CreateTestCaseMetrics(string filePath, List<Test> listTestPath, string idTestPathFile, List<Requirement> listReqPpc, List<Requirement> listReqEc)
        {
            string delimiter = ";";
            StringBuilder sb = new StringBuilder();

            double sumEdgeCoverage = 0;
            double sumPrimePath = 0;

            if (!File.Exists(filePath))
                sb.Append("Id;EdgeCoverage;PrimePathCoverage\n");

            foreach (Test test in listTestPath)
            {
                List<string> listLoop = hasLoop(test.path);

                sumEdgeCoverage += ((double)test.newReqEcCovered / (double)listReqEc.Count);

                sumPrimePath += ((double)test.newReqPpcCovered / (double)listReqPpc.Count);
            }

            sb.Append(idTestPathFile + delimiter + sumEdgeCoverage + delimiter + sumPrimePath + "\n");

            File.AppendAllText(filePath, sb.ToString());
        }

        public static string[] GetDirectories(string projectPath)
        {
            string[] directories = Directory.GetDirectories(projectPath);

            return directories;

        }
        public static string[] GetFiles(string directoryPath)
        {
            string[] files = Directory.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories);

            return files;
        }
        public static void CalculatePathLength(List<Test> listTestPath)
        {
            foreach (Test testPath in listTestPath)
            {
                testPath.pathLength = testPath.path.Split(',').Length;
            }
        }
        public static void ShowReqCoveredByEachTestPath(List<Test> listTestPath)
        {
            foreach (Test testPath in listTestPath)
            {
                Console.WriteLine(testPath.path);
                foreach (string reqCovered in testPath.requirements)
                {
                    Console.WriteLine(reqCovered);
                }
                Console.WriteLine("\n");
            }
        }

        public static void CsvGeneratorTestCase(string projectPath)
        {
            List<Requirement> listReqPpc;
            string[] fileReqPpc;
            List<Requirement> listReqEc;
            string[] fileReqEc;

            List<Test> listTestPath;
            String[] fileTestPath;

            List<string> listInfeasiblePaths = new List<string>();
            String[] fileInfeasiblePaths;
            string[] listMethodPaths = GetDirectories(projectPath);
            string[] listFiles;
            List<string> testPathsFiles;
            string reqFilePpc = string.Empty;
            string reqFileEc = string.Empty;
            string infPathFile = string.Empty;

            string filePath = projectPath + @"\TestCase_metrics.csv";

            if (File.Exists(filePath))
                File.Delete(filePath);

            foreach (string methodPath in listMethodPaths)
            {
                listTestPath = new List<Test>();
                testPathsFiles = new List<string>();
                listFiles = GetFiles(methodPath);
                foreach (string file in listFiles)
                {
                    if (file.Contains("TR_PPC"))
                    {
                        reqFilePpc = file;
                    }
                    else if (file.Contains("TR_EC"))
                    {
                        reqFileEc = file;
                    }
                    else if (file.Contains("TP"))
                    {
                        testPathsFiles.Add(file);
                    }
                    else if (file.Contains("INF"))
                    {
                        infPathFile = file;
                    }
                }

                string idTestPathFile;

                foreach (string testPathFile in testPathsFiles)
                {
                    listReqPpc = new List<Requirement>();
                    fileReqPpc = OpenFile(reqFilePpc);
                    CreatListReq(fileReqPpc, listReqPpc);

                    listReqEc = new List<Requirement>();
                    fileReqEc = OpenFile(reqFileEc);
                    CreatListReq(fileReqEc, listReqEc);

                    if (!string.IsNullOrEmpty(infPathFile))
                    {
                        fileInfeasiblePaths = OpenFile(infPathFile);
                        CreateListInfeasiblePaths(fileInfeasiblePaths, listInfeasiblePaths);
                        CheckInfeasibleReq(listInfeasiblePaths, listReqPpc);
                        CheckInfeasibleReq(listInfeasiblePaths, listReqEc);
                    }

                    fileTestPath = OpenFile(testPathFile);

                    idTestPathFile = fileTestPath.First();

                    CreateListTestPaths(fileTestPath, listTestPath);
                    CalculatePathLength(listTestPath);
                    SortListByPathLength(listTestPath);
                    CountReqPpcCovered(listReqPpc, listTestPath);
                    CountReqNcCovered(listReqEc, listTestPath);
                    CreateTestCaseMetrics(filePath, listTestPath, idTestPathFile, listReqPpc, listReqEc);

                    listTestPath.Clear();
                }
            }
        }
    }
}