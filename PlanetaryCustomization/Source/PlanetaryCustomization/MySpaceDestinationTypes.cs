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

            destinationDictionary = new Dictionary<string, SpaceDestinationType>();
            foreach (var dest in resources)
            {
                destinationDictionary.Add(dest.Id, dest);
            }
            //flush out the old resources now that they have been added
            resources.Clear();

            var planetDefs=new Dictionary<string, JConfig.PlanetDefinition>();

            foreach (string planetDefString in JConfig.ReadPlanetFiles())
            {
                JConfig.PlanetDefinition pDef = JsonConvert.DeserializeObject<JConfig.PlanetDefinition>(planetDefString);

                bool isNew = (destinationDictionary[pDef.ID]==null);

                //Debug.Log("pDef: " + pDef.ID + " table: ");
                //foreach (var kvpair in pDef.elementTable)
                //    Debug.Log("element: " + kvpair.Key + " min: "+kvpair.Value.min+" max: "+kvpair.Value.max);

                planetDefs.Add(pDef.ID,pDef);
                ArtifactDropRate artifactDropRate=null;
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
                    case null:
                        if (isNew)
                            artifactDropRate = Db.Get().ArtifactDropRates.Bad;
                        break;
                }
                SpaceDestinationType planet;
                if (isNew)
                {
                    Debug.Log("new");

                    //If planet is new, create new definition
                    planet = new SpaceDestinationType(pDef.ID, parent, pDef.typeName, pDef.description, pDef.iconSize, pDef.spriteName, JConfig.convertElementTable(pDef.elementTable), pDef.recoverableEntities, artifactDropRate);

                    destinationDictionary.Add(pDef.ID, planet);
                }
                else
                {
                    planet = destinationDictionary[pDef.ID];
                    if (pDef.description!=null)
                        destinationDictionary[pDef.ID].description = pDef.description;
                    if (pDef.iconSize != 0)
                        destinationDictionary[pDef.ID].iconSize = pDef.iconSize;
                    if (pDef.spriteName != null)
                        destinationDictionary[pDef.ID].spriteName = pDef.spriteName;
                    if (pDef.elementTable != null)
                        destinationDictionary[pDef.ID].elementTable = JConfig.convertElementTable(pDef.elementTable);
                    if (pDef.recoverableEntities != null)
                        destinationDictionary[pDef.ID].recoverableEntities = pDef.recoverableEntities;
                    if (artifactDropRate != null)
                        destinationDictionary[pDef.ID].artifactDropTable = artifactDropRate;
                }

                base.Add(planet);
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

                foreach (var pDef in planetDefs)
                {
                    if (pDef.Value.distanceRange.min<dist && pDef.Value.distanceRange.max>dist)
                    {
                        distPool.Add(pDef.Value.ID);
                    }
                }

                destPools.Add(distPool);
            }

            /*
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

            base.IceGiant = destinationDictionary["IceGiant"];*/
        }

        public List<List<string>> GetDestinationPools()
        {
            return destPools;
        }
    }
}