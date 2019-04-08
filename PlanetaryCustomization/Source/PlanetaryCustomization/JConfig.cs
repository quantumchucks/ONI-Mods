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
            DirectoryInfo dirInfo = new DirectoryInfo(pathfinder.Directory.FullName+"\\Planets\\");
            if (!dirInfo.Exists)
            {
                firstSetup(dirInfo);

                //lets try that again
                dirInfo = new DirectoryInfo(pathfinder.Directory.FullName + "\\Planets\\");
            }
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

        //first time setup until the workshop uploader respects my json
        public static void firstSetup(DirectoryInfo dirInfo)
        {
            dirInfo.Create();

            Dictionary<string, string> files = new Dictionary<string, string>
            { 
                {"CarbonaceousAsteroid.json","{\n    \"ID\":\"CarbonaceousAsteroid\",\n    \"distanceRange\":{ \"min\":40000.0,\"max\":60000.0}\n}" },
                {"DustyMoon.json","{\n    \"ID\":\"DustyMoon\",\n    \"distanceRange\":{\"min\":70000.0,\"max\":90000.0}\n}" },
                {"GasGiant.json","{\n    \"ID\":\"GasGiant\",\n    \"distanceRange\":{\"min\":100000.0,\"max\":110000.0}\n}" },
                {"IceGiant.json","{\n    \"ID\":\"IceGiant\",\n    \"distanceRange\":{\"min\":100000.0,\"max\":110000.0}\n}" },
                {"IcyDwarf.json","{\n    \"ID\":\"IcyDwarf\",\n     \"distanceRange\":{\"min\":60000.0,\"max\":80000.0}\n}" },
                {"MetallicAsteroid.json","{\n    \"ID\":\"MetallicAsteroid\",\n    \"distanceRange\":{\"min\":40000.0,\"max\":60000.0}\n}" },
                {"OrganicDwarf.json","{\n    \"ID\":\"OrganicDwarf\",\n    \"distanceRange\":{\"min\":60000.0,\"max\":80000.0}\n}" },
                {"RockyAsteroid.json","{\n    \"ID\":\"RockyAsteroid\",\n    \"distanceRange\":{\"min\":40000.0,\"max\":60000.0}\n}" },
                {"Satellite.json","{\n    \"ID\":\"Satellite\",\n    \"distanceRange\":{\"min\":30000.0,\"max\":40000.0}\n}" },
                {"TerraPlanet.json","{\n    \"ID\":\"TerraPlanet\",\n    \"distanceRange\":{\"min\":90000.0,\"max\":100000.0}\n}" },
                {"VolcanoPlanet.json","{\n    \"ID\":\"VolcanoPlanet\",\n    \"distanceRange\":{\"min\":90000.0,\"max\":100000.0}\n}" }
            };

            foreach (var file in files)
            {
                StreamWriter fwrite = new StreamWriter(dirInfo.FullName + "\\" + file.Key);
                fwrite.Write(file.Value);
                fwrite.Flush();
                fwrite.Close();
            }
        }

        public static Dictionary<SimHashes,MathUtil.MinMax> convertElementTable(Dictionary<string,JConfig.MinMax> parsedTable)
        {
            if (parsedTable == null)
            {
                return null;
            }

            Dictionary<SimHashes, MathUtil.MinMax> result = new Dictionary<SimHashes, MathUtil.MinMax>();

            foreach (var kvPair in parsedTable)
            {
                result.Add((SimHashes)Enum.Parse(typeof(SimHashes), kvPair.Key, true), kvPair.Value.getUnityRep());
            }


            return result;
        }
    }
}
