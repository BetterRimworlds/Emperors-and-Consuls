/*
 * This file is part of Emperors and Consuls, a Better Rimworlds Project.
 *
 * Copyright © 2026 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *
 * This file is licensed under the MIT License.
 */

using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BetterRimworlds.EmperorsAndConsuls;

[HarmonyPatch(typeof(Settlement_TraderTracker), "RegenerateStock")]
internal static class Patch_SettlementTraderStock
{
    private static void Postfix(Settlement_TraderTracker __instance)
    {
        try
        {
            if (!EmpireAlliance.CourtIsActive())
            {
                return;
            }

            Settlement settlement = __instance.settlement;
            if (settlement?.Faction == null || !IsEmpireFaction(settlement.Faction))
            {
                return;
            }

            ThingOwner stock = AccessTools.Field(typeof(Settlement_TraderTracker), "stock")
                .GetValue(__instance) as ThingOwner;
            TradeSurplus.TryAdd(stock);
        }
        catch (Exception ex)
        {
            Log.Warning(EmperorsAndConsulsMod.LogPrefix + "Empire settlement stock surplus skipped: " + ex.Message);
        }
    }

    private static bool IsEmpireFaction(Faction faction)
    {
        return faction.def != null && faction.def.defName == "Empire";
    }
}

[HarmonyPatch(typeof(TradeShip), nameof(TradeShip.GenerateThings))]
internal static class Patch_OrbitalTraderStock
{
    private static void Postfix(TradeShip __instance)
    {
        try
        {
            if (!EmpireAlliance.CourtIsActive())
            {
                return;
            }

            if (__instance.Faction == null || __instance.Faction.def == null || __instance.Faction.def.defName != "Empire")
            {
                return;
            }

            TradeSurplus.TryAdd(__instance.GetDirectlyHeldThings());
        }
        catch (Exception ex)
        {
            Log.Warning(EmperorsAndConsulsMod.LogPrefix + "Empire orbital stock surplus skipped: " + ex.Message);
        }
    }
}

internal static class TradeSurplus
{
    public static void TryAdd(ThingOwner stock)
    {
        if (stock == null)
        {
            return;
        }

        Add(stock, ThingDefOf.Silver, 400);
        Add(stock, ThingDefOf.MealSurvivalPack, 40);
        Add(stock, ThingDefOf.MedicineIndustrial, 12);
        Add(stock, ThingDefOf.ComponentIndustrial, 8);
        Add(stock, DefDatabase<ThingDef>.GetNamedSilentFail("Uranium"), 5);
        Add(stock, DefDatabase<ThingDef>.GetNamedSilentFail("Luciferium"), 2);
    }

    private static void Add(ThingOwner stock, ThingDef def, int count)
    {
        if (def == null || count <= 0)
        {
            return;
        }

        Thing thing = ThingMaker.MakeThing(def);
        thing.stackCount = Math.Min(count, def.stackLimit);
        stock.TryAdd(thing, false);
    }
}
