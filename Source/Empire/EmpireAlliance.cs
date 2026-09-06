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

public static class EmpireAlliance
{
    public static bool CourtIsActive()
    {
        GameComponent_EmperorsAndConsuls comp = GameComponent_EmperorsAndConsuls.Current;
        if (comp != null && comp.playerBrokeAlliance)
        {
            return false;
        }

        return TitleUtility.AnyColonistHoldsRecognizedClaim();
    }

    public static bool IsEmpirePlayerPair(Faction a, Faction b)
    {
        if (a == null || b == null)
        {
            return false;
        }

        Faction empire = TitleUtility.EmpireOrNull();
        Faction player = Faction.OfPlayer;
        if (empire == null || player == null)
        {
            return false;
        }

        return (a == empire && b == player) || (a == player && b == empire);
    }

    public static bool IsVassalPlayerPair(Faction a, Faction b)
    {
        if (a == null || b == null)
        {
            return false;
        }

        FactionDef vassalDef = EmperorsAndConsulsDefOf.BR_ImperialVassals;
        if (vassalDef == null)
        {
            return false;
        }

        bool aVassal = a.def == vassalDef;
        bool bVassal = b.def == vassalDef;
        return (aVassal && b == Faction.OfPlayer) || (bVassal && a == Faction.OfPlayer);
    }

    public static bool ShouldBlockNegativeGoodwill(Faction a, Faction b)
    {
        if (!CourtIsActive())
        {
            return false;
        }

        return IsEmpirePlayerPair(a, b) || IsVassalPlayerPair(a, b);
    }

    public static void TryRecognizeCourt()
    {
        if (!CourtIsActive())
        {
            return;
        }

        LockWith(TitleUtility.EmpireOrNull());

        Faction vassals = Find.FactionManager?.FirstFactionOfDef(EmperorsAndConsulsDefOf.BR_ImperialVassals);
        if (vassals != null)
        {
            LockWith(vassals);
        }

        GameComponent_EmperorsAndConsuls comp = GameComponent_EmperorsAndConsuls.Current;
        if (comp == null || comp.allianceLetterSent)
        {
            return;
        }

        Pawn claimant = TitleUtility.FirstClaimantOrNull();
        string name = claimant?.Name?.ToStringShort ?? claimant?.LabelShort ?? "the court";
        Find.LetterStack.ReceiveLetter(
            "BR_EAC_LetterAllianceLabel".Translate(),
            "BR_EAC_LetterAllianceText".Translate(name),
            LetterDefOf.PositiveEvent,
            claimant);
        comp.allianceLetterSent = true;
    }

    public static void LockWith(Faction faction)
    {
        Faction player = Faction.OfPlayer;
        if (faction == null || player == null || faction == player)
        {
            return;
        }

        faction.SetRelationDirect(player, FactionRelationKind.Ally, false);
        FactionRelation a = faction.RelationWith(player, false);
        FactionRelation b = player.RelationWith(faction, false);
        if (a != null)
        {
            a.baseGoodwill = 100;
        }

        if (b != null)
        {
            b.baseGoodwill = 100;
            b.kind = FactionRelationKind.Ally;
        }
    }
}
