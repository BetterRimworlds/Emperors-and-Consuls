/*
 * This file is part of Emperors and Consuls, a Better Rimworlds Project.
 *
 * Copyright © 2026 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *
 * This file is licensed under the MIT License.
 */

using RimWorld;
using Verse;

namespace BetterRimworlds.EmperorsAndConsuls;

public static class HuntDirector
{
    private const int MinRaidIntervalDays = 2;
    private const int MaxRaidIntervalDays = 4;

    public static void Tick()
    {
        GameComponent_EmperorsAndConsuls comp = GameComponent_EmperorsAndConsuls.Current;
        if (comp == null || comp.assassinCrowned)
        {
            return;
        }

        Pawn marked = FindMarkedPawn();
        if (marked != null)
        {
            comp.lastMarkedPawn = marked;
            NotifyHuntIfNeeded(comp, marked);
            MaybeFireRaid(comp, marked);
            return;
        }

        Pawn last = comp.lastMarkedPawn;
        if (last != null && !last.Dead && !last.Destroyed && TitleUtility.IsPlayerHumanlike(last))
        {
            Crown(comp, last);
        }
    }

    private static Pawn FindMarkedPawn()
    {
        foreach (Pawn pawn in TitleUtility.EnumeratePlayerHumanlikes())
        {
            if (HuntedAssassinMarks.HasMark(pawn))
            {
                return pawn;
            }
        }

        return null;
    }

    private static void NotifyHuntIfNeeded(GameComponent_EmperorsAndConsuls comp, Pawn marked)
    {
        if (comp.huntLetterSent)
        {
            return;
        }

        Find.LetterStack.ReceiveLetter(
            "BR_EAC_LetterHuntStartLabel".Translate(),
            "BR_EAC_LetterHuntStartText".Translate(marked.Name?.ToStringShort ?? marked.LabelShort),
            LetterDefOf.ThreatBig,
            marked);
        comp.huntLetterSent = true;
        comp.lastHuntRaidTick = Find.TickManager.TicksGame;
    }

    private static void MaybeFireRaid(GameComponent_EmperorsAndConsuls comp, Pawn marked)
    {
        Map map = marked.MapHeld ?? Find.Maps?.FirstOrDefault(m => m.IsPlayerHome);
        if (map == null)
        {
            return;
        }

        int now = Find.TickManager.TicksGame;
        if (comp.lastHuntRaidTick < 0)
        {
            comp.lastHuntRaidTick = now;
            return;
        }

        int interval = Calendar.Ticks(Rand.RangeInclusive(MinRaidIntervalDays, MaxRaidIntervalDays));
        if (now - comp.lastHuntRaidTick < interval)
        {
            return;
        }

        try
        {
            IncidentDef raid = IncidentDefOf.RaidEnemy;
            IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
            parms.forced = true;
            parms.points = StorytellerUtility.DefaultThreatPointsNow(map) * 1.6f;
            parms.customLetterLabel = "BR_EAC_HuntRaidLetterLabel".Translate();
            parms.customLetterText = "BR_EAC_HuntRaidLetterText".Translate();
            if (raid != null && raid.Worker.TryExecute(parms))
            {
                comp.lastHuntRaidTick = now;
            }
        }
        catch (Exception ex)
        {
            Log.Warning(EmperorsAndConsulsMod.LogPrefix + "Hunt raid failed: " + ex.Message);
            comp.lastHuntRaidTick = now;
        }
    }

    private static void Crown(GameComponent_EmperorsAndConsuls comp, Pawn pawn)
    {
        TitleUtility.Grant(pawn, "Emperor", sendLetter: false);
        comp.assassinCrowned = true;
        Find.LetterStack.ReceiveLetter(
            "BR_EAC_LetterCrownLabel".Translate(),
            "BR_EAC_LetterCrownText".Translate(pawn.Name?.ToStringShort ?? pawn.LabelShort),
            LetterDefOf.PositiveEvent,
            pawn);
        EmpireAlliance.TryRecognizeCourt();
    }
}
