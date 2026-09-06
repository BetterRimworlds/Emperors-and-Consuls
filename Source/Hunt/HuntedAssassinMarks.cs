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

public static class HuntedAssassinMarks
{
    public static HediffDef MarkDef =>
        DefDatabase<HediffDef>.GetNamedSilentFail("HuntedAssassin");

    public static bool HasMark(Pawn pawn)
    {
        HediffDef def = MarkDef;
        return def != null
            && pawn?.health?.hediffSet != null
            && pawn.health.hediffSet.HasHediff(def);
    }

    public static void ApplyMark(Pawn pawn, bool freshCountdown = false)
    {
        if (pawn?.health?.hediffSet == null || pawn.Dead || pawn.Destroyed)
        {
            return;
        }

        HediffDef def = MarkDef;
        if (def == null)
        {
            Log.Warning(EmperorsAndConsulsMod.LogPrefix + "HuntedAssassin HediffDef not found.");
            return;
        }

        Hediff existing = pawn.health.hediffSet.GetFirstHediffOfDef(def);
        if (existing == null)
        {
            existing = pawn.health.AddHediff(def);
        }

        if (freshCountdown && existing != null)
        {
            HediffComp_Disappears disappears = existing.TryGetComp<HediffComp_Disappears>();
            if (disappears != null)
            {
                disappears.ticksToDisappear = Calendar.Ticks(Calendar.HuntDays);
            }
        }
    }

    public static bool TryInheritFromKill(Pawn victim, Pawn killer)
    {
        if (victim == null || killer == null || killer == victim)
        {
            return false;
        }

        if (killer.Destroyed || killer.Dead)
        {
            return false;
        }

        if (!HasMark(victim) || !TitleUtility.IsKnightOrHigher(killer))
        {
            return false;
        }

        ApplyMark(killer, freshCountdown: true);

        string victimName = victim.Name?.ToStringShort ?? victim.LabelShort;
        string killerName = killer.Name?.ToStringShort ?? killer.LabelShort;
        Find.LetterStack.ReceiveLetter(
            "BR_EAC_LetterInheritLabel".Translate(),
            "BR_EAC_LetterInheritText".Translate(killerName, victimName),
            LetterDefOf.ThreatSmall,
            new LookTargets(killer));

        GameComponent_EmperorsAndConsuls comp = GameComponent_EmperorsAndConsuls.Current;
        if (comp != null)
        {
            comp.lastMarkedPawn = killer;
            comp.assassinCrowned = false;
        }

        return true;
    }
}
