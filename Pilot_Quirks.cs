using System;
using System.Reflection;
using BattleTech;
using Harmony;
using BattleTech.UI;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using System.Collections.Generic;


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
                if (updatePilotDiscardPile == true)
                {
                    if (def.PilotTags.Contains("pilot_tech"))
                    {
                        __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "MechTechSkill", StatCollection.StatOperation.Int_Add, settings.pilot_tech_TechBonus, -1, true);
                    }

                    if (def.PilotTags.Contains("pilot_disgraced"))
                    {
                        __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "Morale", StatCollection.StatOperation.Int_Add, settings.pilot_disgraced_MoralePenalty, -1, true);
                    }

                    if (def.PilotTags.Contains("pilot_comstar"))
                    {
                        __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "MechTechSkill", StatCollection.StatOperation.Int_Add, settings.pilot_comstar_TechBonus, -1, true);
                    }

                    if (def.PilotTags.Contains("pilot_honest"))
                    {
                        __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "Morale", StatCollection.StatOperation.Int_Add, settings.pilot_honest_MoraleBonus, -1, true);
                    }

                    if (def.PilotTags.Contains("pilot_dishonest"))
                    {
                        __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "Morale", StatCollection.StatOperation.Int_Add, settings.pilot_dishonest_MoralePenalty, -1, true);
                    }

                }

            }
        }

        [HarmonyPatch(typeof(SimGameState), "KillPilot", new Type[] { typeof(Pilot) })]
        public static class Pilot_Died
        {
            public static void Postfix(SimGameState __instance, Pilot p)
            {
                if (p.pilotDef.PilotTags.Contains("pilot_tech"))
                {
                    __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "MechTechSkill", StatCollection.StatOperation.Int_Subtract, settings.pilot_tech_TechBonus, -1, true);
                }

                if (p.pilotDef.PilotTags.Contains("pilot_disgraced"))
                {
                    __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "Morale", StatCollection.StatOperation.Int_Subtract, settings.pilot_disgraced_MoralePenalty, -1, true);
                }

                if (p.pilotDef.PilotTags.Contains("pilot_comstar"))
                {
                    __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "MechTechSkill", StatCollection.StatOperation.Int_Subtract, settings.pilot_comstar_TechBonus, -1, true);
                }

                if (p.pilotDef.PilotTags.Contains("pilot_honest"))
                {
                    __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "Morale", StatCollection.StatOperation.Int_Subtract, settings.pilot_honest_MoraleBonus, -1, true);
                }

                if (p.pilotDef.PilotTags.Contains("pilot_honest"))
                {
                    __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "Morale", StatCollection.StatOperation.Int_Subtract, settings.pilot_dishonest_MoralePenalty, -1, true);
                }
            }
        }

        [HarmonyPatch(typeof(SimGameState), "DismissPilot", new Type[] { typeof(Pilot) })]
        public static class Pilot_Dismissed
        {
            public static void Prefix(SimGameState __instance, Pilot p)
            {
                if (p.pilotDef.PilotTags.Contains("pilot_tech"))
                {
                    __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "MechTechSkill", StatCollection.StatOperation.Int_Subtract, settings.pilot_tech_TechBonus, -1, true);
                }
               
                if (p.pilotDef.PilotTags.Contains("pilot_disgraced"))
                {
                    __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "Morale", StatCollection.StatOperation.Int_Subtract, settings.pilot_disgraced_MoralePenalty, -1, true);
                }

                if (p.pilotDef.PilotTags.Contains("pilot_comstar"))
                {
                    __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "MechTechSkill", StatCollection.StatOperation.Int_Subtract, settings.pilot_comstar_TechBonus, -1, true);
                }

                if (p.pilotDef.PilotTags.Contains("pilot_honest"))
                {
                    __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "Morale", StatCollection.StatOperation.Int_Subtract, settings.pilot_honest_MoraleBonus, -1, true);
                }

                if (p.pilotDef.PilotTags.Contains("pilot_dishonest"))
                {
                    __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "Morale", StatCollection.StatOperation.Int_Subtract, settings.pilot_dishonest_MoralePenalty, -1, true);
                }
            }
        }

        //[HarmonyPatch(typeof(SimGameState), "_OnDefsLoadComplete")]
        //public static class Initialize_New_Game
        //{
        //    public static void Postfix(SimGameState __instance)
        //    {
        //        foreach (Pilot pilot in __instance.PilotRoster)
        //        {
        //            if (pilot.pilotDef.PilotTags.Contains("pilot_disgraced"))
        //            {
        //                __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "Morale", StatCollection.StatOperation.Int_Add, settings.pilot_disgraced_MoralePenalty, -1, true);
        //            }

        //            if (pilot.pilotDef.PilotTags.Contains("pilot_honest"))
        //            {
        //                __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "Morale", StatCollection.StatOperation.Int_Add, settings.pilot_honest_MoraleBonus, -1, true);
        //            }

        //            if (pilot.pilotDef.PilotTags.Contains("pilot_dishonest"))
        //            {
        //                __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "Morale", StatCollection.StatOperation.Int_Add, settings.pilot_dishonest_MoralePenalty, -1, true);
        //            }
        //        }
        //    }
        //}

        [HarmonyPatch(typeof(SimGameState), "OnDayPassed")]
        public static class DayPasser
        {
            public static void Postfix(SimGameState __instance)
            {
                foreach (Pilot pilot in __instance.PilotRoster)
                {
                    var rng = new System.Random();
                    int Roll = rng.Next(1, 100);
                    if (pilot.pilotDef.PilotTags.Contains("pilot_unstable"))
                    {
                        if (Roll <= 33)
                        {
                            pilot.pilotDef.PilotTags.Add("pilot_morale_high");
                            pilot.pilotDef.PilotTags.Remove("pilot_morale_low");
                        }
                        else if (Roll > 33 && Roll <= 66)
                        {
                            pilot.pilotDef.PilotTags.Add("pilot_morale_low");
                            pilot.pilotDef.PilotTags.Remove("pilot_morale_high");
                        }
                        else
                        {
                            pilot.pilotDef.PilotTags.Remove("pilot_morale_low");
                            pilot.pilotDef.PilotTags.Remove("pilot_morale_high");
                        }
                    }

                }
                foreach (Pilot pilot in __instance.PilotRoster)
                {
                    if (pilot.pilotDef.PilotTags.Contains("pilot_criminal"))
                    {
                        var rng = new System.Random();
                        int Roll = rng.Next(1, 101);
                        if (Roll < settings.pilot_criminal_StealPercent)
                        {
                            __instance.AddFunds(settings.pilot_criminal_StealAmount, null, true);
                        } 
                    }
                }
                    if (settings.IsSaveGame)
                {
                    foreach (Pilot pilot in __instance.PilotRoster)
                    {
                        if (pilot.pilotDef.PilotTags.Contains("pilot_tech"))
                        {
                            __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "MechTechSkill", StatCollection.StatOperation.Int_Add, settings.pilot_tech_TechBonus, -1, true);
                        }

                        if (pilot.pilotDef.PilotTags.Contains("pilot_disgraced"))
                        {
                            __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "Morale", StatCollection.StatOperation.Int_Add, settings.pilot_disgraced_MoralePenalty, -1, true);
                        }


                        if (pilot.pilotDef.PilotTags.Contains("pilot_honest"))
                        {
                            __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "Morale", StatCollection.StatOperation.Int_Add, settings.pilot_honest_MoraleBonus, -1, true);
                        }

                        if (pilot.pilotDef.PilotTags.Contains("pilot_dishonest"))
                        {
                            __instance.CompanyStats.ModifyStat<int>("SimGame", 0, "Morale", StatCollection.StatOperation.Int_Add, settings.pilot_dishonest_MoralePenalty, -1, true);
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(SimGameState), "GetReputationShopAdjustment", new Type[] { typeof(Faction) })]
        public static class Merchant_Bonus
        {
            public static void Postfix(SimGameState __instance, ref float __result)
            {
                float MerchantCount = 0;
                foreach(Pilot pilot in __instance.PilotRoster)
                {
                    if (pilot.pilotDef.PilotTags.Contains("pilot_merchant"))
                    {
                        MerchantCount = MerchantCount + 1;
                    }
                }
                __result = __result - settings.pilot_merchant_ShopDiscount * MerchantCount/100;
            }
        }

        [HarmonyPatch(typeof(Shop), "PopulateInventory")]
        public static class Criminal_Shops
        {
            public static void Prefix(Shop __instance, int max)
            {
                SimGameState Sim = Traverse.Create(__instance).Field("Sim").GetValue<SimGameState>();
                if (Sim.CurSystem.Tags.Contains("planet_other_blackmarket"))
                {
                    bool honest = false;
                    foreach (Pilot pilot in Sim.PilotRoster)
                    {
                        if (pilot.pilotDef.PilotTags.Contains("pilot_honest"))
                            honest = true;
                    }
                    foreach (Pilot pilot in Sim.PilotRoster)
                    {
                        if (pilot.pilotDef.PilotTags.Contains("pilot_criminal") && !honest)
                        max = max + 2;
                    }
                }

                foreach (Pilot pilot in Sim.PilotRoster)
                {
                    if (pilot.pilotDef.PilotTags.Contains("pilot_comstar"))
                    {
                        max = max + 3;
                    }
                }
            } 
        }


        [HarmonyPatch(typeof(Pilot), "InjurePilot")]
        public static class Lucky_Pilot
        {
            public static void Prefix(Pilot __instance, int dmg)
            {
                var rng = new System.Random();
                int Roll = rng.Next(1, 101);
                if (Roll <= settings.pilot_lucky_InjuryAvoidance)
                {
                    dmg = 0;
                }
            }
        }

        [HarmonyPatch(typeof(AAR_UnitStatusWidget), "FillInData")]
        public static class Adjust_Pilot_XP
        {
            public static void Prefix(AAR_UnitStatusWidget __instance, int xpEarned)
            {
                UnitResult unit = Traverse.Create(__instance).Field("UnitData").GetValue<UnitResult>();
                if (unit.pilot.pilotDef.PilotTags.Contains("pilot_naive"))
                {
                    float XPModifier = 1 - settings.pilot_naive_LessExperience;
                    xpEarned = (int)(XPModifier * (float)xpEarned);
                }
            }
            
        }
        [HarmonyPatch(typeof(Team), "BaselineMoraleGain")]
        public static class Rebellious_Area_Prefix
        {
            private static bool Prefix(Team __instance)
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(Team), "BaselineMoraleGain")]
        public static class Rebellious_Area_Postfix
        {
            private static void Postfix(Team __instance,ref List<IStackSequence> __result)
            {
                //var attackLogger = Traverse.Create("Logger").GetValue<Logger>();
                List<IStackSequence> list = new List<IStackSequence>();
                bool rebelpilot = false;
                bool officer = false;
                int edgecase = 0;
                if (__instance.Combat.TurnDirector.IsInterleaved)
                {
                    MoraleConstantsDef activeMoraleDef = __instance.Combat.Constants.GetActiveMoraleDef(__instance.Combat);
                    int num = activeMoraleDef.MoraleBaselineGainPerRound;
                    if (activeMoraleDef.MoraleBaselineMultiplyByActive)
                    {
                        num *= __instance.NumLivingUnits;
                    }
                    if (num > 0)
                    {
                        foreach (AbstractActor actor in __instance.units)
                        {
                            Pilot pilot = actor.GetPilot();
                            if (pilot.pilotDef.PilotTags.Contains("pilot_rebellious"))
                            {
                                rebelpilot = true;
                            }
                            if (pilot.pilotDef.PilotTags.Contains("pilot_officer") || pilot.pilotDef.PilotTags.Contains("commander_player"))
                            {
                                officer = true;
                            }
                            if (rebelpilot && officer)
                            {
                                edgecase = edgecase + 1;
                            }
                        }
                        if (rebelpilot && officer && edgecase != 1)
                        {
                            //attackLogger.Log(string.Format("MORALE: team {0} gains {1} baseline morale", __instance.DisplayName, 0));
                            __instance.ModifyMorale(0);
                        }
                        else
                        {
                            //attackLogger.Log(string.Format("MORALE: team {0} gains {1} baseline morale", __instance.DisplayName, num));
                            __instance.ModifyMorale(num);
                        }
                        if (__instance == __instance.Combat.LocalPlayerTeam)
                        {
                            list.Add(new DelaySequence(__instance.Combat, 1f));
                        }
                    }
                    else
                    {
                        //attackLogger.Log(string.Format("MORALE: team {0} gains {0} baseline morale", __instance.DisplayName));
                    }
                }
                __result = list;
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
                try
                {
                    Pilot TargetPilot = target.GetPilot();
                    if (TargetPilot.pilotDef.PilotTags.Contains("pilot_reckless"))
                    {
                        __result = __result + (float)settings.pilot_reckless_ToBeHitBonus;
                    }
                    if (TargetPilot.pilotDef.PilotTags.Contains("pilot_cautious"))
                    {
                        __result = __result + (float)settings.pilot_reckless_ToBeHitBonus;
                    }
                    if (TargetPilot.pilotDef.PilotTags.Contains("pilot_jinxed"))
                    {
                        __result = __result + (float)settings.pilot_jinxed_ToBeHitBonus;
                    }
                    if (TargetPilot.pilotDef.PilotTags.Contains("pilot_jinxed"))
                    {
                        __result = __result + (float)settings.pilot_reckless_ToBeHitBonus;
                    }
                }
                catch (Exception)
                {
                }
                if (pilot.pilotDef.PilotTags.Contains("pilot_reckless"))
                {
                    __result = __result + (float)settings.pilot_reckless_ToHitBonus;
                }
                if (pilot.pilotDef.PilotTags.Contains("pilot_cautious"))
                {
                    __result = __result + (float)settings.pilot_reckless_ToHitBonus;
                }
                if (pilot.pilotDef.PilotTags.Contains("pilot_drunk") && pilot.pilotDef.TimeoutRemaining > 0)
                {
                    __result = __result + (float)settings.pilot_drunk_ToHitBonus;
                }
                if (pilot.pilotDef.PilotTags.Contains("pilot_lostech") && weapon.componentDef.ComponentTags.Contains("component_type_lostech"))
                {
                    __result = __result + (float)settings.pilot_lostech_ToHitBonus;
                }
            }
        }

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

                if (pilot.pilotDef.PilotTags.Contains("pilot_cautious"))
                {
                    __result = string.Format("{0}CAUTIOUS {1:+#;-#}; ", __result, settings.pilot_cautious_ToHitBonus);
                }

                if (pilot.pilotDef.PilotTags.Contains("pilot_drunk") && pilot.pilotDef.TimeoutRemaining > 0)
                {
                    __result = string.Format("{0}DRUNK {1:+#;-#}; ", __result, settings.pilot_drunk_ToHitBonus);
                }

                if (pilot.pilotDef.PilotTags.Contains("pilot_lostech") && weapon.componentDef.ComponentTags.Contains("component_type_lostech"))
                {
                    __result = string.Format("{0}LOSTECH TECHNICIAN {1:+#;-#}; ", __result, settings.pilot_lostech_ToHitBonus);
                }

                if (pilot.pilotDef.PilotTags.Contains("pilot_jinxed"))
                {
                    __result = string.Format("{0}JINXED {1:+#;-#}; ", __result, settings.pilot_reckless_ToHitBonus);
                }
            }
        }

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

                if (pilot.pilotDef.PilotTags.Contains("pilot_cautious"))
                {
                    _this.Method("AddToolTipDetail", "CAUTIOUS", settings.pilot_cautious_ToHitBonus).GetValue();
                }

                if (pilot.pilotDef.PilotTags.Contains("pilot_drunk") && pilot.pilotDef.TimeoutRemaining > 0)
                {
                    _this.Method("AddToolTipDetail", "DRUNK", settings.pilot_drunk_ToHitBonus).GetValue();
                }

                if (__instance.tag.Contains("component_type_lostech") && pilot.pilotDef.PilotTags.Contains("pilot_lostech"))
                {
                    _this.Method("AddToolTipDetail", "LOSTECH TECH", settings.pilot_lostech_ToHitBonus).GetValue();
                }

                if (pilot.pilotDef.PilotTags.Contains("pilot_jinxed"))
                {
                    _this.Method("AddToolTipDetail", "JINXED", settings.pilot_jinxed_ToHitBonus).GetValue();
                }
            }
        }

        [HarmonyPatch(typeof(Mech), "GetEvasivePipsResult")]
        public static class Drunk_Evasive_Malus
        {
            private static void Postfix(Mech __instance, ref int __result)
            {
                if (__instance.pilot.pilotDef.PilotTags.Contains("pilot_drunk") && __instance.pilot.pilotDef.TimeoutRemaining > 0)
                    __result = __result - 1;
            }
        }

        [HarmonyPatch(typeof(SimGameState), "GetMechWarriorValue")]
        public static class Change_Pilot_Cost
        {
            public static void Postfix(SimGameState __instance, PilotDef def, ref int __result)
            {
                float CostPerMonth = __result;
                if (def.PilotTags.Contains("pilot_tech"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_dependable"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_merchant"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_brave"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_assassin"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_athletic"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_gladiator"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_lucky"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_drunk"))
                {
                    CostPerMonth = CostPerMonth - (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_lostech"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_disgraced"))
                {
                    CostPerMonth = CostPerMonth - (float)0.2 * (__result);
                }
                if (def.PilotTags.Contains("pilot_naive"))
                {
                    CostPerMonth = CostPerMonth - (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_military"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_mechwarrior"))
                {
                    CostPerMonth = CostPerMonth + (float)0.2 * (__result);
                }
                if (def.PilotTags.Contains("pilot_bookish"))
                {
                    CostPerMonth = CostPerMonth - (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_command"))
                {
                    CostPerMonth = CostPerMonth + (float)0.2 * (__result);
                }
                if (def.PilotTags.Contains("pilot_officer"))
                {
                    CostPerMonth = CostPerMonth + (float)0.3 * (__result);
                }
                if (def.PilotTags.Contains("pilot_unstable"))
                {
                    CostPerMonth = CostPerMonth - (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_jinxed"))
                {
                    CostPerMonth = CostPerMonth - (float)0.2 * (__result);
                }
                if (def.PilotTags.Contains("pilot_klutz"))
                {
                    CostPerMonth = CostPerMonth - (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_criminal"))
                {
                    CostPerMonth = CostPerMonth - (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_comstar"))
                {
                    CostPerMonth = CostPerMonth + (float)1.0 * (__result);
                }
                if (def.PilotTags.Contains("pilot_dishonest"))
                {
                    CostPerMonth = CostPerMonth - (float)0.1 * (__result);
                }
                if (CostPerMonth < 0)
                    CostPerMonth = 0;

                __result = (int)CostPerMonth;
                
                if (def.PilotTags.Contains("pilot_wealthy"))
                {
                    __result = 0;
                }

                if (def.PilotTags.Contains("pilot_noble"))
                {
                    __result = (int)(__result + settings.pilot_noble_IncreasedCost * __result);
                }

                if (def.PilotTags.Contains("pilot_spacer"))
                {
                    __result = (int)(__result + settings.pilot_spacer_DecreasedCost * __result);
                }
            }
        }

        [HarmonyPatch(typeof(SimGameState), "GetMechWarriorHiringCost")]
        public static class Change_Pilot_Hiring_Cost
        {
            public static void Postfix(SimGameState __instance, PilotDef def, ref int __result)
            {
                float CostPerMonth = __result;
                if (def.PilotTags.Contains("pilot_tech"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_dependable"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_merchant"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_brave"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_assassin"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_athletic"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_gladiator"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_lucky"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_drunk"))
                {
                    CostPerMonth = CostPerMonth - (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_lostech"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_disgraced"))
                {
                    CostPerMonth = CostPerMonth - (float)0.2 * (__result);
                }
                if (def.PilotTags.Contains("pilot_naive"))
                {
                    CostPerMonth = CostPerMonth - (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_military"))
                {
                    CostPerMonth = CostPerMonth + (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_mechwarrior"))
                {
                    CostPerMonth = CostPerMonth + (float)0.2 * (__result);
                }
                if (def.PilotTags.Contains("pilot_bookish"))
                {
                    CostPerMonth = CostPerMonth - (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_command"))
                {
                    CostPerMonth = CostPerMonth + (float)0.2 * (__result);
                }
                if (def.PilotTags.Contains("pilot_officer"))
                {
                    CostPerMonth = CostPerMonth + (float)0.3 * (__result);
                }
                if (def.PilotTags.Contains("pilot_unstable"))
                {
                    CostPerMonth = CostPerMonth - (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_jinxed"))
                {
                    CostPerMonth = CostPerMonth - (float)0.2 * (__result);
                }
                if (def.PilotTags.Contains("pilot_klutz"))
                {
                    CostPerMonth = CostPerMonth - (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_criminal"))
                {
                    CostPerMonth = CostPerMonth - (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_comstar"))
                {
                    CostPerMonth = CostPerMonth + (float)1.0 * (__result);
                }
                if (def.PilotTags.Contains("pilot_dishonest"))
                {
                    CostPerMonth = CostPerMonth - (float)0.1 * (__result);
                }
                if (def.PilotTags.Contains("pilot_noble"))
                {
                    CostPerMonth = CostPerMonth + (float)0.5 * (__result);
                }
                if (CostPerMonth < 0)
                    CostPerMonth = 0;
                

                __result = (int)CostPerMonth;
            }
        }



        [HarmonyPatch(typeof(Mech), "GetHitLocation", new Type[] { typeof(AbstractActor), typeof(Vector3), typeof(float), typeof(ArmorLocation), typeof(float) })]
        public static class Assassin_Patch
        {
            private static void Prefix(Mech __instance, AbstractActor attacker, float bonusMultiplier)
            {
                Pilot pilot = attacker.GetPilot();
                if (pilot.pilotDef.PilotTags.Contains("pilot_assassin"))
                {
                    bonusMultiplier = bonusMultiplier + settings.pilot_assassin_CalledShotBonus;
                }
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
            public float pilot_assassin_CalledShotBonus = 0.25f;
            public float pilot_merchant_ShopDiscount = 1;
            public int pilot_lucky_InjuryAvoidance = 10;
            public int pilot_disgraced_MoralePenalty = -1;
            public int pilot_jinxed_ToHitBonus = 1;
            public int pilot_jinxed_ToBeHitBonus = -1;

            public int pilot_drunk_ToHitBonus = 1;
            public int pilot_lostech_ToHitBonus = -1;
            public float pilot_naive_LessExperience = 0.1f;
            public float pilot_noble_IncreasedCost = 0.5f;
            public int pilot_criminal_StealPercent = 5;
            public int pilot_criminal_StealAmount = -1000;
            public float pilot_spacer_DecreasedCost = -0.5f;
            public int pilot_comstar_TechBonus = 200;
            public int pilot_comstar_StoreBonus = 3;
            public int pilot_honest_MoraleBonus = 1;
            public int pilot_dishonest_MoralePenalty = -1;


            public bool IsSaveGame = false;

        }
    }
}