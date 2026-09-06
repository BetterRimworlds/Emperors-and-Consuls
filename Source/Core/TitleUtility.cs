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

public static class TitleUtility
{
    public static Faction EmpireOrNull()
    {
        if (!ModsConfig.RoyaltyActive || Find.FactionManager == null)
        {
            return null;
        }

        try
        {
            if (Faction.OfEmpire != null)
            {
                return Faction.OfEmpire;
            }
        }
        catch (Exception)
        {
            // OfEmpire throws if Royalty is installed but no Empire faction spawned.
        }

        FactionDef def = DefDatabase<FactionDef>.GetNamedSilentFail("Empire");
        return def == null ? null : Find.FactionManager.FirstFactionOfDef(def);
    }

    public static RoyalTitleDef KnightDef()
    {
        return DefDatabase<RoyalTitleDef>.GetNamedSilentFail("Knight");
    }

    /// Archon display name is 1.6; the defName remains Count on every Royalty version CryoRegenesis supports.
    public static RoyalTitleDef ArchonDef()
    {
        return DefDatabase<RoyalTitleDef>.GetNamedSilentFail("Count")
            ?? DefDatabase<RoyalTitleDef>.GetNamedSilentFail("Archon");
    }

    public static RoyalTitleDef EmperorDef()
    {
        return DefDatabase<RoyalTitleDef>.GetNamedSilentFail("Emperor");
    }

    public static RoyalTitleDef SpecialConsulDef()
    {
        return EmperorsAndConsulsDefOf.BR_SpecialConsul
            ?? DefDatabase<RoyalTitleDef>.GetNamedSilentFail("BR_SpecialConsul")
            ?? DefDatabase<RoyalTitleDef>.GetNamedSilentFail("Consul");
    }

    public static RoyalTitleDef CurrentEmpireTitle(Pawn pawn)
    {
        Faction empire = EmpireOrNull();
        if (pawn?.royalty == null || empire == null)
        {
            return null;
        }

        return pawn.royalty.GetCurrentTitle(empire);
    }

    public static bool MeetsOrExceeds(Pawn pawn, RoyalTitleDef minimum)
    {
        if (pawn == null || minimum == null || pawn.Dead || pawn.Destroyed)
        {
            return false;
        }

        RoyalTitleDef title = CurrentEmpireTitle(pawn);
        return title != null && title.seniority >= minimum.seniority;
    }

    public static bool IsKnightOrHigher(Pawn pawn)
    {
        return MeetsOrExceeds(pawn, KnightDef());
    }

    public static bool IsArchonOrHigher(Pawn pawn)
    {
        return MeetsOrExceeds(pawn, ArchonDef());
    }

    public static bool IsPlayerHumanlike(Pawn pawn)
    {
        return pawn != null
            && !pawn.Dead
            && !pawn.Destroyed
            && pawn.Faction == Faction.OfPlayer
            && pawn.RaceProps != null
            && pawn.RaceProps.Humanlike;
    }

    /// Consul (vanilla or Special Consul) or Emperor. CryoRegenesis is what grants these.
    public static bool HoldsRecognizedClaim(Pawn pawn)
    {
        if (!IsPlayerHumanlike(pawn))
        {
            return false;
        }

        RoyalTitleDef title = CurrentEmpireTitle(pawn);
        if (title == null)
        {
            return false;
        }

        if (title.defName == "Emperor" || title.defName == "Consul" || title.defName == "BR_SpecialConsul")
        {
            return true;
        }

        RoyalTitleDef consul = SpecialConsulDef();
        RoyalTitleDef emperor = EmperorDef();
        return (emperor != null && title.seniority >= emperor.seniority)
            || (consul != null && title.defName == consul.defName);
    }

    public static Pawn FirstClaimantOrNull()
    {
        foreach (Pawn pawn in EnumeratePlayerHumanlikes())
        {
            if (HoldsRecognizedClaim(pawn))
            {
                return pawn;
            }
        }

        return null;
    }

    public static bool AnyColonistHoldsRecognizedClaim()
    {
        return FirstClaimantOrNull() != null;
    }

    public static IEnumerable<Pawn> EnumeratePlayerHumanlikes()
    {
        if (Find.Maps != null)
        {
            foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction)
            {
                if (IsPlayerHumanlike(pawn))
                {
                    yield return pawn;
                }
            }
        }

        if (Find.WorldPawns != null)
        {
            foreach (Pawn pawn in Find.WorldPawns.AllPawnsAlive)
            {
                if (IsPlayerHumanlike(pawn))
                {
                    yield return pawn;
                }
            }
        }
    }

    public static void Grant(Pawn pawn, string titleDefName, bool sendLetter = false)
    {
        if (pawn == null || titleDefName.NullOrEmpty())
        {
            return;
        }

        Faction empire = EmpireOrNull();
        RoyalTitleDef title = ResolveGrantableTitle(titleDefName);
        if (empire == null || title == null)
        {
            Log.Warning(EmperorsAndConsulsMod.LogPrefix + "Could not grant title " + titleDefName + ".");
            return;
        }

        if (pawn.royalty == null)
        {
            Log.Warning(EmperorsAndConsulsMod.LogPrefix + pawn.LabelShort + " has no royalty tracker.");
            return;
        }

        pawn.royalty.SetTitle(empire, title, grantRewards: false, rewardsOnlyForNewestTitle: false, sendLetter: sendLetter);
    }

    private static RoyalTitleDef ResolveGrantableTitle(string titleDefName)
    {
        if (titleDefName == "BR_SpecialConsul" || titleDefName == "SpecialConsul" || titleDefName == "Consul")
        {
            return SpecialConsulDef();
        }

        return DefDatabase<RoyalTitleDef>.GetNamedSilentFail(titleDefName);
    }
}
