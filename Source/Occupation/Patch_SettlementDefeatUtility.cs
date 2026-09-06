/*
 * This file is part of Emperors and Consuls, a Better Rimworlds Project.
 *
 * Copyright © 2026 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *
 * This file is licensed under the MIT License.
 */

using HarmonyLib;
using RimWorld.Planet;

namespace BetterRimworlds.EmperorsAndConsuls;

[HarmonyPatch(typeof(SettlementDefeatUtility), nameof(SettlementDefeatUtility.CheckDefeated))]
internal static class Patch_SettlementDefeatUtility
{
    private static bool Prefix(Settlement factionBase)
    {
        if (OccupationUtility.TryVassalize(factionBase))
        {
            return false;
        }

        return true;
    }
}
