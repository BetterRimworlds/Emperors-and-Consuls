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
using Verse;

namespace BetterRimworlds.EmperorsAndConsuls;

[HarmonyPatch(typeof(Faction), nameof(Faction.TryAffectGoodwillWith))]
internal static class Patch_TryAffectGoodwillWith
{
    private static bool Prefix(Faction __instance, Faction other, int goodwillChange, ref bool __result)
    {
        if (goodwillChange >= 0)
        {
            return true;
        }

        if (!EmpireAlliance.ShouldBlockNegativeGoodwill(__instance, other))
        {
            return true;
        }

        __result = false;
        return false;
    }
}

[HarmonyPatch(typeof(Faction), nameof(Faction.SetRelationDirect))]
internal static class Patch_SetRelationDirect
{
    private static bool Prefix(Faction __instance, Faction other, FactionRelationKind kind)
    {
        if (kind != FactionRelationKind.Hostile)
        {
            return true;
        }

        if (!EmpireAlliance.ShouldBlockNegativeGoodwill(__instance, other))
        {
            return true;
        }

        return false;
    }
}
