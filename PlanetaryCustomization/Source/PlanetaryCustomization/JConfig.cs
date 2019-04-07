using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using Database;
using UnityEngine;

namespace PlanetaryCustomization
{
    public static class JConfig
    {
        public class MinMax
        {
            public float min;
            public float max;
            public MathUtil.MinMax getUnityRep()
            {
                return new MathUtil.MinMax(min, max);
            }
        }
        public class PlanetDefinition
        {
            public string ID;
            public string typeName;
            public string description;
            public int iconSize;
            public string spriteName;
            public Dictionary<string, JConfig.MinMax> elementTable;
            public Dictionary<string, int> recoverableEntities;
            public string artifactDropRate;
            public JConfig.MinMax distanceRange;
        }

        public static IEnumerable<string> ReadPlanetFiles()
        {
            FileInfo pathfinder = new FileInfo(Assembly.GetExecutingAssembly().Location);
            DirectoryInfo dirInfo = new DirectoryInfo(pathfinder.Directory.FullName + "\\Planets\\");
            StreamReader fileReader;
            string fileContents;
            foreach (FileInfo file in dirInfo.GetFiles("*.json"))
            {
                fileReader = file.OpenText();
                fileContents = fileReader.ReadToEnd();
                fileReader.Close();
                yield return fileContents;
            }

        }

        public static Dictionary<SimHashes,MathUtil.MinMax> convertElementTable(Dictionary<string,JConfig.MinMax> parsedTable)
        {
            Dictionary<SimHashes, MathUtil.MinMax> result = new Dictionary<SimHashes, MathUtil.MinMax>();

            foreach (var kvPair in parsedTable)
            {
                result.Add((SimHashes)Enum.Parse(typeof(SimHashes), kvPair.Key, true), kvPair.Value.getUnityRep());
            }

            return result;
        }
    }
}
