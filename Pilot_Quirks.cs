using System;
using System.Reflection;
using BattleTech;
using Harmony;
using BattleTech.UI;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace Pilot_Quirks
{
    public static class Pre_Control
    {
        public const string ModName = "Pilot_Quirks";
        public const string ModId = "dZ.Zappo.Pilot_Quirks";

        internal static ModSettings settings;
        internal static string ModDirectory;

        public static void Init(string directory, string modSettings)
        {
            Helper.Logger.LogLine("Init");
            ModDirectory = directory;
            try
            {
                settings = JsonConvert.DeserializeObject<ModSettings>(modSettings);
            }
            catch (Exception)
            {
                settings = new ModSettings();
            }

            var harmony = HarmonyInstance.Create(ModId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        //This section works, but I'm not sure if I want the commander to utilize starting tags. They don't even start with Pilot Tags. 
        //[HarmonyPatch(typeof(SimGameState), "FirstTimeInitializeDataFromDefs")]
        //public static class Commander_Initial_Setup
        //{
        //    public static void Postfix(SimGameState __instance)
        //    {
        //        Helper.Logger.LogLine("Add Commander");
        //        if (__instance.Commander.pilotDef.PilotTags.Contains("commander_ancestry_steiner"))
        //        {
        //            Helper.Logger.LogLine("Tech Found");
        //            __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "MechTechSkill", StatCollection.StatOperation.Int_Add, settings.pilot_tech_TechBonus, -1, true);
        //        }
        //    }
        //}

        [HarmonyPatch(typeof(SimGameState), "AddPilotToRoster", new Type[] { typeof(PilotDef), typeof(bool) } )]
        public static class Pilot_Gained
        {
            public static void Postfix(SimGameState __instance, PilotDef def, bool updatePilotDiscardPile = false)
            {
                Helper.Logger.LogLine("Add Pilot");
                Helper.Logger.LogLine(def.ToString());
                Helper.Logger.LogLine(def.PilotTags.Contains("pilot_tech").ToString());
                if (updatePilotDiscardPile == true)
                {
                    if (def.PilotTags.Contains("pilot_tech"))
                    {
                        __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "MechTechSkill", StatCollection.StatOperation.Int_Add, settings.pilot_tech_TechBonus, -1, true);
                    }
                }
               
            }
        }

        [HarmonyPatch(typeof(SimGameState), "KillPilot", new Type[] { typeof(Pilot) })]
        public static class Pilot_Died
        {
            public static void Postfix(SimGameState __instance, Pilot p)
            {
                Helper.Logger.LogLine("Pilot_Died");
                Helper.Logger.LogLine(p.ToString());
                Helper.Logger.LogLine(p.pilotDef.PilotTags.Contains("pilot_tech").ToString());
                if (p.pilotDef.PilotTags.Contains("pilot_tech"))
                {
                    __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "MechTechSkill", StatCollection.StatOperation.Int_Subtract, settings.pilot_tech_TechBonus, -1, true);
                }
            }
        }

        [HarmonyPatch(typeof(SimGameState), "DismissPilot", new Type[] { typeof(Pilot) })]
        public static class Pilot_Dismissed
        {
            public static void Prefix(SimGameState __instance, Pilot p)
            {
                Helper.Logger.LogLine("FirePilot");
                Helper.Logger.LogLine(p.ToString());
                Helper.Logger.LogLine(p.pilotDef.PilotTags.Contains("pilot_tech").ToString());
                if (p.pilotDef.PilotTags.Contains("pilot_tech"))
                {
                    __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "MechTechSkill", StatCollection.StatOperation.Int_Subtract, settings.pilot_tech_TechBonus, -1, true);
                }
            }
        }

        [HarmonyPatch(typeof(SimGameState), "_OnDefsLoadComplete")]
        public static class Initialize_New_Game
        {
            public static void Postfix(SimGameState __instance)
            {
                Helper.Logger.LogLine("Initialize");
                foreach (Pilot pilot in __instance.PilotRoster)
                {
                    Helper.Logger.LogLine(pilot.ToString());
                    if (pilot.pilotDef.PilotTags.Contains("pilot_tech"))
                    {
                        Helper.Logger.LogLine("Tech Found");
                        __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "MechTechSkill", StatCollection.StatOperation.Int_Add, settings.pilot_tech_TechBonus, -1, true);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SimGameState), "OnDayPassed")]
        public static class SaveGameUpdater
        {
            public static void Postfix(SimGameState __instance)
            {
                Helper.Logger.LogLine("SaveGameUpdater");
                if (settings.IsSaveGame)
                {
                    Helper.Logger.LogLine("Got Here");
                    foreach (Pilot pilot in __instance.PilotRoster)
                        if (pilot.pilotDef.PilotTags.Contains("pilot_tech"))
                        {
                            Helper.Logger.LogLine("But Did I get here? ");
                            __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "MechTechSkill", StatCollection.StatOperation.Int_Add, settings.pilot_tech_TechBonus, -1, true);
                        }
                }
            }
        }

        /// <summary>
        /// Following is to-hit modifiers area.
        /// </summary>

        [HarmonyPatch(typeof(ToHit), "GetAllModifiers")]
        public static class ToHit_GetAllModifiers_Patch
        {
            private static void Postfix(ToHit __instance, ref float __result, AbstractActor attacker, Weapon weapon, ICombatant target, Vector3 attackPosition, Vector3 targetPosition, LineOfFireLevel lofLevel, bool isCalledShot)
            {
                Pilot pilot = attacker.GetPilot();
                Pilot TargetPilot = target.GetPilot();

                if (pilot.pilotDef.PilotTags.Contains("pilot_reckless"))
                {
                    __result = __result + (float)settings.pilot_reckless_ToHitBonus;
                }
                if (TargetPilot.pilotDef.PilotTags.Contains("pilot_reckless"))
                {
                    __result = __result + (float)settings.pilot_reckless_ToBeHitBonus;
                }

                if (pilot.pilotDef.PilotTags.Contains("pilot_cautious"))
                {
                    __result = __result + (float)settings.pilot_reckless_ToHitBonus;
                }
                if (TargetPilot.pilotDef.PilotTags.Contains("pilot_cautious"))
                {
                    __result = __result + (float)settings.pilot_reckless_ToBeHitBonus;
                }
            }
        }

        //public string GetAllModifiersDescription(AbstractActor attacker, Weapon weapon, ICombatant target, Vector3 attackPosition, Vector3 targetPosition, LineOfFireLevel lofLevel, bool isCalledShot)
        [HarmonyPatch(typeof(ToHit), "GetAllModifiersDescription")]
        public static class ToHit_GetAllModifiersDescription_Patch
        {
            private static void Postfix(ToHit __instance, ref string __result, AbstractActor attacker, Weapon weapon, ICombatant target, Vector3 attackPosition, Vector3 targetPosition, LineOfFireLevel lofLevel, bool isCalledShot)
            {
                Pilot pilot = attacker.GetPilot();
                if (pilot.pilotDef.PilotTags.Contains("pilot_reckless"))
                {
                    __result = string.Format("{0}RECKLESS {1:+#;-#}; ", __result, settings.pilot_reckless_ToHitBonus);
                }
            }
        }

        //private void UpdateToolTipsSelf()
        [HarmonyPatch(typeof(CombatHUDWeaponSlot), "SetHitChance", new Type[] { typeof(ICombatant) })]
        public static class CombatHUDWeaponSlot_SetHitChance_Patch
        {
            private static void Postfix(CombatHUDWeaponSlot __instance, ICombatant target)
            {
                AbstractActor actor = __instance.DisplayedWeapon.parent;
                var _this = Traverse.Create(__instance);
                Pilot pilot = actor.GetPilot();
                if (pilot.pilotDef.PilotTags.Contains("pilot_reckless"))
                {
                    _this.Method("AddToolTipDetail", "RECKLESS", settings.pilot_reckless_ToHitBonus).GetValue();
                }
            }
        }


        // G untested but compiles
        [HarmonyPatch(typeof(Shop), "GetPrice")]
        public static class Patch_GetPrice
        {
            public static void Postfix(Shop __instance, ref int __result)
            {
                int tagCount = 0;
                int tagValue = 2; //  TODO implement actual variables MerchantTagValue;
                int cbills = __result;
                var pilots = Traverse.Create("SimGameState").Field("PilotRoster").GetValue<WeightedList<Pilot>>();
               
                foreach (var p in pilots)
                {
                    if (p.pilotDef.PilotTags.Contains("pilot_klutz"))
                    {
                        tagCount++;
                    }
                }

                __result = cbills - (tagCount * tagValue / 100 * cbills);
            }
        }

        [HarmonyPatch(typeof(ShopDefItem))]
        public class Patch_SellPrice
        {
            static void Postfix(ShopDefItem __instance, ref float __result)
            {
                int tagCount = 0;
                int tagValue = 2; //  TODO implement actual variables MerchantTagValue;
                var pilots = Traverse.Create("SimGameState").Field("PilotRoster").GetValue<WeightedList<Pilot>>();
                foreach (var p in pilots)
                {
                    if (p.pilotDef.PilotTags.Contains("pilot_klutz"))
                    {
                        tagCount++;
                    }
                }
                __result = __result + (tagCount * tagValue / 100f);
            }
        }

        public static class Helper
        {
            public static Settings LoadSettings()
            {
                Settings result;
                try
                {
                    using (StreamReader streamReader = new StreamReader("Mods/Pilot_Quirks/settings.json"))
                    {
                        result = JsonConvert.DeserializeObject<Settings>(streamReader.ReadToEnd());
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    result = null;
                }
                return result;
            }
            public class Logger
            {
                public static void LogError(Exception ex)
                {
                    using (StreamWriter streamWriter = new StreamWriter("Mods/Pilot_Quirks/Log.txt", true))
                    {
                        streamWriter.WriteLine(string.Concat(new string[]
                        {
                        "Message :",
                        ex.Message,
                        "<br/>",
                        Environment.NewLine,
                        "StackTrace :",
                        ex.StackTrace,
                        Environment.NewLine,
                        "Date :",
                        DateTime.Now.ToString()
                        }));
                        streamWriter.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }
                }

                public static void LogLine(string line)
                {
                    string path = "Mods/Pilot_Quirks/Log.txt";
                    using (StreamWriter streamWriter = new StreamWriter(path, true))
                    {
                        streamWriter.WriteLine(line + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                        streamWriter.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }
                }
            }
        }
        internal class ModSettings
        {
            public int pilot_tech_TechBonus = 100;
            public int pilot_reckless_ToHitBonus = -1;
            public int pilot_reckless_ToBeHitBonus = -1;
            public int pilot_cautious_ToHitBonus = 1;
            public int pilot_cautious_ToBeHitBonus = 1;
            public bool IsSaveGame = false;


            public int FatigueTimeStart = 7;
            public int MoraleModifier = 5;
            public int StartingMorale = 25;
            public int FatigueMinimum = 0;
            public int MoralePositiveTierOne = 5;
            public int MoralePositiveTierTwo = 15;
            public int MoraleNegativeTierOne = -5;
            public int MoraleNegativeTierTwo = -15;
            public double FatigueFactor = 2.5;
            public bool InjuriesHurt = true;
        }
    }
}