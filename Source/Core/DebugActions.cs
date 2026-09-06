/*
 * This file is part of Emperors and Consuls, a Better Rimworlds Project.
 *
 * Copyright © 2026 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *
 * This file is licensed under the MIT License.
 */

using LudeonTK;
using RimWorld;
using Verse;

namespace BetterRimworlds.EmperorsAndConsuls;

public static class DebugActions
{
    [DebugAction("Emperors and Consuls", "Grant Special Consul to first colonist", allowedGameStates = AllowedGameStates.PlayingOnMap)]
    private static void GrantConsul()
    {
        Pawn pawn = Find.CurrentMap?.mapPawns?.FreeColonists?.FirstOrDefault();
        if (pawn == null)
        {
            return;
        }

        TitleUtility.Grant(pawn, "BR_SpecialConsul");
        EmpireAlliance.TryRecognizeCourt();
    }

    [DebugAction("Emperors and Consuls", "Grant Emperor to first colonist", allowedGameStates = AllowedGameStates.PlayingOnMap)]
    private static void GrantEmperor()
    {
        Pawn pawn = Find.CurrentMap?.mapPawns?.FreeColonists?.FirstOrDefault();
        if (pawn == null)
        {
            return;
        }

        TitleUtility.Grant(pawn, "Emperor");
        EmpireAlliance.TryRecognizeCourt();
    }

    [DebugAction("Emperors and Consuls", "Apply Hunted Assassin to first colonist", allowedGameStates = AllowedGameStates.PlayingOnMap)]
    private static void ApplyMark()
    {
        Pawn pawn = Find.CurrentMap?.mapPawns?.FreeColonists?.FirstOrDefault();
        if (pawn == null)
        {
            return;
        }

        HuntedAssassinMarks.ApplyMark(pawn, freshCountdown: true);
        GameComponent_EmperorsAndConsuls comp = GameComponent_EmperorsAndConsuls.Current;
        if (comp != null)
        {
            comp.lastMarkedPawn = pawn;
            comp.assassinCrowned = false;
            comp.huntLetterSent = false;
        }
    }
}
