/*
 * This file is part of Emperors and Consuls, a Better Rimworlds Project.
 *
 * Copyright © 2026 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *
 * This file is licensed under the MIT License.
 */

using HarmonyLib;
using Verse;

namespace BetterRimworlds.EmperorsAndConsuls;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill))]
internal static class Patch_Pawn_Kill_HuntedAssassin
{
    private static void Prefix(Pawn __instance, DamageInfo? dinfo)
    {
        if (ModUtility.CryoRegenesisLoaded())
        {
            return;
        }

        if (__instance == null || __instance.Dead)
        {
            return;
        }

        if (!HuntedAssassinMarks.HasMark(__instance))
        {
            return;
        }

        Pawn killer = dinfo?.Instigator as Pawn;
        if (killer == null)
        {
            return;
        }

        HuntedAssassinMarks.TryInheritFromKill(__instance, killer);
    }
}
