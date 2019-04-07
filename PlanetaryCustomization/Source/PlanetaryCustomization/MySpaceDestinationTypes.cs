using System;
using System.Collections.Generic;
using Database;
using Newtonsoft.Json;

namespace PlanetaryCustomization
{
    public class MySpaceDestinationTypes : Database.SpaceDestinationTypes
    {
        public Dictionary<string, SpaceDestinationType> destinationDictionary;
        private List<List<string>> destPools;

        public MySpaceDestinationTypes(ResourceSet parent) : base(parent)
        {
            //the base constructor has already added the vanilla planets as resources, so let's remove those to enable modification
            //I should make this more intricate later on
            resources.Clear();

            destinationDictionary = new Dictionary<string, SpaceDestinationType>();
            List<JConfig.PlanetDefinition> planetDefs=new List<JConfig.PlanetDefinition>();

            foreach (string planetDefString in JConfig.ReadPlanetFiles())
            {
                JConfig.PlanetDefinition pDef = JsonConvert.DeserializeObject<JConfig.PlanetDefinition>(planetDefString);

                //Debug.Log("pDef: " + pDef.ID + " table: ");
                //foreach (var kvpair in pDef.elementTable)
                //    Debug.Log("element: " + kvpair.Key + " min: "+kvpair.Value.min+" max: "+kvpair.Value.max);

                planetDefs.Add(pDef);
                ArtifactDropRate artifactDropRate;
                switch (pDef.artifactDropRate)
                {
                    case "Bad":
                        artifactDropRate = Db.Get().ArtifactDropRates.Bad;
                        break;
                    case "Mediocre":
                        artifactDropRate = Db.Get().ArtifactDropRates.Mediocre;
                        break;
                    case "Good":
                        artifactDropRate = Db.Get().ArtifactDropRates.Good;
                        break;
                    case "Great":
                        artifactDropRate = Db.Get().ArtifactDropRates.Great;
                        break;
                    case "Amazing":
                        artifactDropRate = Db.Get().ArtifactDropRates.Amazing;
                        break;
                    case "Perfect":
                        artifactDropRate = Db.Get().ArtifactDropRates.Perfect;
                        break;
                    default:
                        Debug.Log(pDef.ID + ": \"artifactDropRate\"=\"" + pDef.artifactDropRate + "\" is not a valid value, setting it to Bad instead.");
                        artifactDropRate = Db.Get().ArtifactDropRates.Bad;
                        break;
                }

                SpaceDestinationType planet = new SpaceDestinationType(pDef.ID, parent, pDef.typeName, pDef.description, pDef.iconSize, pDef.spriteName, JConfig.convertElementTable(pDef.elementTable), pDef.recoverableEntities, artifactDropRate);

                base.Add(planet);
                destinationDictionary.Add(pDef.ID, planet);
            }
            if (planetDefs.Count == 0)
                Debug.Log("PlanetaryCustomization: Could not find any planet definitions!");
            else
                Debug.Log("PlanetaryCustomization: Loaded " + planetDefs.Count + " planet definitions!");

            //create destination pools for random planet rolls
            destPools = new List<List<string>>();
            for (double dist = 9999.0; dist < 110001.0; dist += 10000.0)
            {
                var distPool = new List<string>();

                //debug MathUtil.MinMax mm=new MathUtil.MinMax;

                foreach (JConfig.PlanetDefinition pDef in planetDefs)
                {
                    if (pDef.distanceRange.min<dist && pDef.distanceRange.max>dist)
                    {
                        distPool.Add(pDef.ID);
                    }
                }

                destPools.Add(distPool);
            }


            //This exists for compatibility reasons so mods that modify existing planets _maybe_ don't get screwed (please don't smack me Cairath :( )
            base.Satellite = destinationDictionary["Satellite"];

            base.MetallicAsteroid = destinationDictionary["MetallicAsteroid"];

            base.RockyAsteroid = destinationDictionary["RockyAsteroid"];

            base.CarbonaceousAsteroid = destinationDictionary["CarbonaceousAsteroid"];

            base.IcyDwarf = destinationDictionary["IcyDwarf"];

            base.OrganicDwarf = destinationDictionary["OrganicDwarf"];

            base.DustyMoon = destinationDictionary["DustyMoon"];

            base.TerraPlanet = destinationDictionary["TerraPlanet"];

            base.VolcanoPlanet = destinationDictionary["VolcanoPlanet"];

            base.GasGiant = destinationDictionary["GasGiant"];

            base.IceGiant = destinationDictionary["IceGiant"];
        }

        public List<List<string>> GetDestinationPools()
        {
            return destPools;
        }
    }
}