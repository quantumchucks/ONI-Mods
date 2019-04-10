using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using UnityEngine;
using OpCodes = System.Reflection.Emit.OpCodes;
using Database;

namespace PlanetaryCustomization
{
    //Replace Database.SpaceDestinationTypes with custom class here (presumably in postfix?)
    //Modify SpacecraftManager.GenerateRandomDestinations() to dynamically load ids
    public static class PlanetaryCustomization
    {
        public static List<string> additionalPlanetFolders;

        //add Planets-folders to search for <planet>.json
        public static void addPlanets(string location)
        {
            if (additionalPlanetFolders==null)
            {
                additionalPlanetFolders = new List<string>();
            }

            additionalPlanetFolders.Add(location);
        }

        public static List<List<string>> GetDestinationList()
        {
            SpaceDestinationTypes spaceDestinationTypes = Db.Get().SpaceDestinationTypes;

            return ((MySpaceDestinationTypes)spaceDestinationTypes).GetDestinationPools();
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public static class Db_Initialize_Patch
        {
            public static void Postfix(Db __instance)
            {
                __instance.SpaceDestinationTypes = new MySpaceDestinationTypes(__instance.Root);
            }
        }

        [HarmonyPatch(typeof(SpacecraftManager))]
        [HarmonyPatch("GenerateRandomDestinations")]
        public static class SpacecraftManager_GenerateRandomDestinations_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                Debug.Log("PlanetaryCustomization: Transpiler active.");
                
                ConstructorInfo listCtor = AccessTools.Constructor(typeof(List<int>));
                MethodInfo getDstList = AccessTools.Method(typeof(PlanetaryCustomization), nameof(PlanetaryCustomization.GetDestinationList));
                foreach (CodeInstruction instruction in instructions)
                {
                    if (instruction.opcode == OpCodes.Newobj && instruction.operand == listCtor)
                    {
                        Debug.Log("PlanetaryCustomization: Transpiler entry point found, injecting code...");

                        yield return new CodeInstruction(opcode: OpCodes.Call, operand: getDstList);

                        yield return new CodeInstruction(opcode: OpCodes.Stloc_2);

                        yield return instruction;
                    }
                    else
                    {
                        //Debug.Log("nope");

                        yield return instruction;
                    }
                }
            }
        }
    }
}