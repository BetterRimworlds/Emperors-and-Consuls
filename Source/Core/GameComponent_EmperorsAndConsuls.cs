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

public class GameComponent_EmperorsAndConsuls : GameComponent
{
    public const int CheckIntervalTicks = 250;

    public bool allianceLetterSent;
    public bool assassinCrowned;
    public bool huntLetterSent;
    public bool playerBrokeAlliance;
    public int lastHuntRaidTick = -1;
    public Pawn lastMarkedPawn;
    public List<VassalRecord> vassals = new List<VassalRecord>();

    public static GameComponent_EmperorsAndConsuls Current =>
        Verse.Current.Game?.GetComponent<GameComponent_EmperorsAndConsuls>();

    public GameComponent_EmperorsAndConsuls(Game game)
    {
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref allianceLetterSent, "allianceLetterSent");
        Scribe_Values.Look(ref assassinCrowned, "assassinCrowned");
        Scribe_Values.Look(ref huntLetterSent, "huntLetterSent");
        Scribe_Values.Look(ref playerBrokeAlliance, "playerBrokeAlliance");
        Scribe_Values.Look(ref lastHuntRaidTick, "lastHuntRaidTick", -1);
        Scribe_References.Look(ref lastMarkedPawn, "lastMarkedPawn");
        Scribe_Collections.Look(ref vassals, "vassals", LookMode.Deep);
        if (Scribe.mode == LoadSaveMode.LoadingVars && vassals == null)
        {
            vassals = new List<VassalRecord>();
        }
    }

    public override void GameComponentTick()
    {
        if (Find.TickManager.TicksGame % CheckIntervalTicks != 0)
        {
            return;
        }

        EmpireAlliance.TryRecognizeCourt();
        HuntDirector.Tick();
        OccupationUtility.TickVassals();
    }

    public VassalRecord RecordFor(Settlement settlement)
    {
        if (settlement == null || vassals == null)
        {
            return null;
        }

        for (int i = 0; i < vassals.Count; i++)
        {
            VassalRecord record = vassals[i];
            if (record != null && record.settlement == settlement)
            {
                return record;
            }
        }

        return null;
    }
}
