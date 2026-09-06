/*
 * This file is part of Emperors and Consuls, a Better Rimworlds Project.
 *
 * Copyright © 2026 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *
 * This file is licensed under the MIT License.
 */

using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BetterRimworlds.EmperorsAndConsuls;

public static class OccupationUtility
{
    private static bool? annexationDisabled;
    private static string annexationModName;

    public static bool AnnexationDisabled
    {
        get
        {
            if (!annexationDisabled.HasValue)
            {
                DetectAnnexationMods();
            }

            return annexationDisabled == true;
        }
    }

    public static string AnnexationModName => annexationModName;

    private static void DetectAnnexationMods()
    {
        string[] known =
        {
            "oskarpotocki.vfe.empire",
            "kikohi.rimcities",
            "vanillaexpanded.vfe.empire"
        };

        foreach (string id in known)
        {
            if (ModUtility.IsModActive(id))
            {
                annexationDisabled = true;
                annexationModName = id;
                Log.Warning("BR_EAC_AnnexationDisabled".Translate(id));
                return;
            }
        }

        annexationDisabled = false;
    }

    public static bool CanAnnex(Settlement settlement)
    {
        if (AnnexationDisabled || settlement?.Faction == null)
        {
            return false;
        }

        if (!EmpireAlliance.CourtIsActive())
        {
            return false;
        }

        Faction faction = settlement.Faction;
        if (faction.IsPlayer)
        {
            return false;
        }

        if (faction.def != null && faction.def.defName == "Empire")
        {
            return false;
        }

        if (faction.def != null && faction.def.hidden)
        {
            return false;
        }

        if (faction.def == FactionDefOf.Mechanoid || faction.def == FactionDefOf.Insect)
        {
            return false;
        }

        return settlement.def == WorldObjectDefOf.Settlement;
    }

    public static bool KnightPresent(Map map)
    {
        if (map?.mapPawns == null)
        {
            return false;
        }

        foreach (Pawn pawn in map.mapPawns.FreeColonistsSpawned)
        {
            if (TitleUtility.IsKnightOrHigher(pawn))
            {
                return true;
            }
        }

        return false;
    }

    public static bool TryVassalize(Settlement settlement)
    {
        if (settlement?.Map == null || !CanAnnex(settlement))
        {
            return false;
        }

        if (!IsDefeated(settlement.Map, settlement.Faction))
        {
            return false;
        }

        if (!KnightPresent(settlement.Map))
        {
            return false;
        }

        Faction vassalFaction = EnsureVassalFaction();
        if (vassalFaction == null)
        {
            Log.Warning(EmperorsAndConsulsMod.LogPrefix + "Imperial Vassals faction is missing.");
            return false;
        }

        GameComponent_EmperorsAndConsuls comp = GameComponent_EmperorsAndConsuls.Current;
        if (comp == null)
        {
            return false;
        }

        VassalRecord record = new VassalRecord
        {
            settlement = settlement,
            originalFaction = settlement.Faction,
            occupationStartTick = -1
        };

        settlement.SetFaction(vassalFaction);
        EmpireAlliance.LockWith(vassalFaction);
        comp.vassals.Add(record);

        Find.LetterStack.ReceiveLetter(
            "BR_EAC_LetterVassalLabel".Translate(),
            "BR_EAC_LetterVassalText".Translate(settlement.Label),
            LetterDefOf.PositiveEvent,
            settlement);

        return true;
    }

    public static bool IsDefeated(Map map, Faction faction)
    {
        if (map == null || faction == null)
        {
            return false;
        }

        List<Pawn> list = map.mapPawns.SpawnedPawnsInFaction(faction);
        for (int i = 0; i < list.Count; i++)
        {
            Pawn pawn = list[i];
            if (pawn.RaceProps.Humanlike && GenHostility.IsActiveThreatToPlayer(pawn))
            {
                return false;
            }
        }

        return true;
    }

    public static Faction EnsureVassalFaction()
    {
        FactionDef def = EmperorsAndConsulsDefOf.BR_ImperialVassals;
        if (def == null || Find.FactionManager == null)
        {
            return null;
        }

        Faction existing = Find.FactionManager.FirstFactionOfDef(def);
        if (existing != null)
        {
            return existing;
        }

        Faction generated = FactionGenerator.NewGeneratedFaction(new FactionGeneratorParms(def));
        Find.FactionManager.Add(generated);
        return generated;
    }

    public static bool IsLoaned(Pawn pawn)
    {
        GameComponent_EmperorsAndConsuls comp = GameComponent_EmperorsAndConsuls.Current;
        if (comp?.vassals == null || pawn == null)
        {
            return false;
        }

        for (int i = 0; i < comp.vassals.Count; i++)
        {
            if (comp.vassals[i]?.loanedKnight == pawn)
            {
                return true;
            }
        }

        return false;
    }

    public static void OfferLoan(Settlement settlement)
    {
        List<FloatMenuOption> options = new List<FloatMenuOption>();
        foreach (Pawn pawn in AvailableKnights())
        {
            Pawn local = pawn;
            options.Add(new FloatMenuOption(
                local.LabelShortCap,
                () => LoanKnight(settlement, local)));
        }

        if (options.Count == 0)
        {
            Messages.Message("BR_EAC_NoKnightToLoan".Translate(), MessageTypeDefOf.RejectInput, false);
            return;
        }

        Find.WindowStack.Add(new FloatMenu(options));
    }

    public static IEnumerable<Pawn> AvailableKnights()
    {
        foreach (Map map in Find.Maps)
        {
            if (!map.IsPlayerHome)
            {
                continue;
            }

            foreach (Pawn pawn in map.mapPawns.FreeColonistsSpawned)
            {
                if (TitleUtility.IsKnightOrHigher(pawn) && !IsLoaned(pawn))
                {
                    yield return pawn;
                }
            }
        }
    }

    public static void LoanKnight(Settlement settlement, Pawn pawn)
    {
        if (settlement == null || pawn == null)
        {
            return;
        }

        if (IsLoaned(pawn))
        {
            Messages.Message("BR_EAC_KnightAlreadyLoaned".Translate(pawn.LabelShort), MessageTypeDefOf.RejectInput, false);
            return;
        }

        GameComponent_EmperorsAndConsuls comp = GameComponent_EmperorsAndConsuls.Current;
        VassalRecord record = comp?.RecordFor(settlement);
        if (record == null || record.converted)
        {
            return;
        }

        StationAtSettlement(pawn, settlement);

        int now = Find.TickManager.TicksGame;
        record.loanedKnight = pawn;
        record.loanEndTick = now + Calendar.Ticks(Calendar.LoanDays);
        record.lastKnightPresenceTick = now;
        record.graceUntilTick = -1;
        if (record.occupationStartTick < 0)
        {
            record.occupationStartTick = now;
        }

        Find.LetterStack.ReceiveLetter(
            "BR_EAC_LetterLoanLabel".Translate(),
            "BR_EAC_LetterLoanText".Translate(pawn.LabelShort, settlement.Label),
            LetterDefOf.NeutralEvent,
            settlement);
    }

    public static void RenewLoan(VassalRecord record)
    {
        if (record == null || !record.HasActiveLoan)
        {
            return;
        }

        record.loanEndTick += Calendar.Ticks(Calendar.LoanDays);
        record.lastKnightPresenceTick = Find.TickManager.TicksGame;
        Messages.Message(
            "BR_EAC_LetterLoanText".Translate(record.loanedKnight.LabelShort, record.settlement.Label),
            record.settlement,
            MessageTypeDefOf.PositiveEvent);
    }

    public static void RecallKnight(VassalRecord record)
    {
        if (record?.loanedKnight == null)
        {
            return;
        }

        Pawn pawn = record.loanedKnight;
        record.loanedKnight = null;
        record.loanEndTick = -1;
        DropHome(pawn);
        if (!record.converted)
        {
            Rebel(record);
        }
    }

    public static void ConvertAtSite(VassalRecord record, Caravan caravan)
    {
        if (record == null || caravan == null || !record.ReadyForArchon)
        {
            Messages.Message("BR_EAC_ConvertNeedArchon".Translate(), MessageTypeDefOf.RejectInput, false);
            return;
        }

        Pawn archon = null;
        foreach (Pawn pawn in caravan.PawnsListForReading)
        {
            if (TitleUtility.IsArchonOrHigher(pawn))
            {
                archon = pawn;
                break;
            }
        }

        if (archon == null)
        {
            Messages.Message("BR_EAC_ConvertNeedArchon".Translate(), MessageTypeDefOf.RejectInput, false);
            return;
        }

        Faction empire = TitleUtility.EmpireOrNull();
        if (empire == null || record.settlement == null)
        {
            return;
        }

        record.settlement.SetFaction(empire);
        record.converted = true;
        if (record.loanedKnight != null)
        {
            Pawn knight = record.loanedKnight;
            record.loanedKnight = null;
            record.loanEndTick = -1;
            DropHome(knight);
        }

        caravan.RemovePawn(archon);
        StationAtSettlement(archon, record.settlement);
        record.archonGovernor = archon;
        EmpireAlliance.LockWith(empire);

        Find.LetterStack.ReceiveLetter(
            "BR_EAC_LetterConvertedLabel".Translate(),
            "BR_EAC_LetterConvertedText".Translate(record.settlement.Label, archon.LabelShort),
            LetterDefOf.PositiveEvent,
            record.settlement);
    }

    public static void RecallArchon(VassalRecord record)
    {
        if (record?.archonGovernor == null)
        {
            return;
        }

        Pawn pawn = record.archonGovernor;
        record.archonGovernor = null;
        DropHome(pawn);
    }

    public static void TickVassals()
    {
        GameComponent_EmperorsAndConsuls comp = GameComponent_EmperorsAndConsuls.Current;
        if (comp?.vassals == null)
        {
            return;
        }

        int now = Find.TickManager.TicksGame;
        for (int i = comp.vassals.Count - 1; i >= 0; i--)
        {
            VassalRecord record = comp.vassals[i];
            if (record == null || record.settlement == null || record.settlement.Destroyed)
            {
                comp.vassals.RemoveAt(i);
                continue;
            }

            if (record.converted)
            {
                continue;
            }

            if (record.HasActiveLoan)
            {
                if (record.loanedKnight.Dead)
                {
                    record.loanedKnight = null;
                    record.loanEndTick = -1;
                    record.graceUntilTick = now + Calendar.Ticks(Calendar.KnightDeathGraceDays);
                    continue;
                }

                record.lastKnightPresenceTick = now;
                continue;
            }

            if (record.loanedKnight != null && record.loanEndTick > 0 && now >= record.loanEndTick)
            {
                Pawn expired = record.loanedKnight;
                record.loanedKnight = null;
                record.loanEndTick = -1;
                DropHome(expired);
                Find.LetterStack.ReceiveLetter(
                    "BR_EAC_LetterLoanEndedLabel".Translate(),
                    "BR_EAC_LetterLoanEndedText".Translate(expired.LabelShort, record.settlement.Label),
                    LetterDefOf.NegativeEvent,
                    record.settlement);
                Rebel(record);
                continue;
            }

            if (record.graceUntilTick > 0 && now >= record.graceUntilTick)
            {
                record.graceUntilTick = -1;
                Rebel(record);
            }
        }
    }

    public static void Rebel(VassalRecord record)
    {
        if (record?.settlement == null || record.converted)
        {
            return;
        }

        Faction original = record.originalFaction;
        if (original == null || original.defeated)
        {
            original = Find.FactionManager.FirstFactionOfDef(
                DefDatabase<FactionDef>.GetNamedSilentFail("Pirate"))
                ?? Find.FactionManager.AllFactionsVisible.FirstOrDefault(f => f.HostileTo(Faction.OfPlayer));
        }

        if (original != null)
        {
            record.settlement.SetFaction(original);
            original.SetRelationDirect(Faction.OfPlayer, FactionRelationKind.Hostile, true);
        }

        Find.LetterStack.ReceiveLetter(
            "BR_EAC_LetterRebellionLabel".Translate(),
            "BR_EAC_LetterRebellionText".Translate(record.settlement.Label),
            LetterDefOf.ThreatBig,
            record.settlement);

        GameComponent_EmperorsAndConsuls.Current?.vassals.Remove(record);
    }

    private static void StationAtSettlement(Pawn pawn, Settlement settlement)
    {
        Caravan caravan = pawn.GetCaravan();
        if (caravan != null)
        {
            caravan.RemovePawn(pawn);
        }
        else if (pawn.Spawned)
        {
            pawn.DeSpawn();
        }

        if (!pawn.IsWorldPawn())
        {
            Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.KeepForever);
        }
    }

    private static void DropHome(Pawn pawn)
    {
        if (pawn == null || pawn.Dead || pawn.Destroyed)
        {
            return;
        }

        Map map = Find.Maps.FirstOrDefault(m => m.IsPlayerHome);
        if (map == null)
        {
            if (!pawn.IsWorldPawn())
            {
                Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.KeepForever);
            }

            return;
        }

        IntVec3 cell = DropCellFinder.TradeDropSpot(map);
        ActiveTransporterInfo info = new ActiveTransporterInfo();
        info.innerContainer.TryAddOrTransfer(pawn);
        DropPodUtility.MakeDropPodAt(cell, map, info);
        Find.LetterStack.ReceiveLetter(
            "BR_EAC_LetterRecallLabel".Translate(),
            "BR_EAC_LetterRecallText".Translate(pawn.LabelShort),
            LetterDefOf.PositiveEvent,
            new TargetInfo(cell, map));
    }
}
