using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Harmony;
using System.Reflection;
using Newtonsoft.Json;

namespace ExtraPlanets
{
    public static class ExtraPlanets
    {
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public static class Db_Initialize_Patch
        {
            public static void Prefix(Db __instance)
            {
                FileInfo pathfinder = new FileInfo(Assembly.GetExecutingAssembly().Location);
                string pPath = pathfinder.Directory.FullName + "/Planets/";
                PlanetaryCustomization.PlanetaryCustomization.addPlanets(pPath);
                DirectoryInfo dirInfo = new DirectoryInfo(pPath);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                    generateJSON(pPath);
                }
            }
        }

        public static void generateJSON(string folder)
        {
            var dyingSun = new PlanetaryCustomization.JConfig.PlanetDefinition();
            dyingSun.ID = "DyingSun";
            dyingSun.typeName = "Dying Sun";
            dyingSun.description = "A star at the end of it's lifecycle. It's fusion has mostly stopped, allowing for careful harvest of superheated fusion products.";
            dyingSun.iconSize = 96;
            dyingSun.spriteName = "nebula";
            dyingSun.elementTable = new Dictionary<string, PlanetaryCustomization.JConfig.MinMax>{
                { "TungstenGas", new PlanetaryCustomization.JConfig.MinMax(100, 200) },
                { "RockGas", new PlanetaryCustomization.JConfig.MinMax(100,200)},
                { "CarbonGas", new PlanetaryCustomization.JConfig.MinMax(100,200)},
                };
            dyingSun.recoverableEntities = null;
            dyingSun.artifactDropRate = "Bad";
            dyingSun.distanceRange = new PlanetaryCustomization.JConfig.MinMax(80000, 110000);

            StreamWriter file = new StreamWriter(folder + "dyingSun.json");
            file.Write(JsonConvert.SerializeObject(dyingSun));
            file.Close();

            var tradeStation = new PlanetaryCustomization.JConfig.PlanetDefinition();
            tradeStation.ID = "TradeStation";
            tradeStation.typeName = "Trading Outpost";
            tradeStation.description = "An unmanned station controlled by a lone trade bot. Fortunately, it appears to be broken, giving out supplies and scrap for free.";
            tradeStation.iconSize = 16;
            tradeStation.spriteName = "asteroid";
            tradeStation.elementTable = new Dictionary<string, PlanetaryCustomization.JConfig.MinMax>{
                { "Cuprite", new PlanetaryCustomization.JConfig.MinMax(100, 200) },
                { "IronOre", new PlanetaryCustomization.JConfig.MinMax(100, 200) },
                { "Water", new PlanetaryCustomization.JConfig.MinMax(100, 200) },
                { "LiquidHydrogen", new PlanetaryCustomization.JConfig.MinMax(100, 200) },
                { "LiquidOxygen", new PlanetaryCustomization.JConfig.MinMax(100, 200) },
            };
            tradeStation.recoverableEntities = new Dictionary<string, int>
            {
                { "Atmo_Suit",2 },
                { "Funky_Vest",2 },
                { "BasicCure",12 },
                { "IntermediateCure",12 },
                { "AdvancedCure",12 }
            };
            tradeStation.artifactDropRate = "Perfect";
            tradeStation.distanceRange = new PlanetaryCustomization.JConfig.MinMax(10000, 50000);

            file = new StreamWriter(folder + "tradeStation.json");
            file.Write(JsonConvert.SerializeObject(tradeStation));
            file.Close();
        }
    }
}
