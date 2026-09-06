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

public class VassalRecord : IExposable
{
    public Settlement settlement;
    public Faction originalFaction;
    public int occupationStartTick = -1;
    public int lastKnightPresenceTick = -1;
    public Pawn loanedKnight;
    public int loanEndTick = -1;
    public int graceUntilTick = -1;
    public bool converted;
    public Pawn archonGovernor;

    public bool HasActiveLoan
    {
        get
        {
            return loanedKnight != null
                && !loanedKnight.Dead
                && !loanedKnight.Destroyed
                && loanEndTick > Find.TickManager.TicksGame;
        }
    }

    public bool InDeathGrace
    {
        get { return graceUntilTick > Find.TickManager.TicksGame; }
    }

    public bool Occupied
    {
        get { return converted || HasActiveLoan || InDeathGrace; }
    }

    public bool ReadyForArchon
    {
        get
        {
            if (converted || occupationStartTick < 0)
            {
                return false;
            }

            return Occupied
                && Find.TickManager.TicksGame - occupationStartTick >= Calendar.Ticks(Calendar.OccupationDays);
        }
    }

    public int OccupationDaysElapsed
    {
        get
        {
            if (occupationStartTick < 0)
            {
                return 0;
            }

            int ticks = Math.Max(0, Find.TickManager.TicksGame - occupationStartTick);
            return ticks / GenDate.TicksPerDay;
        }
    }

    public void ExposeData()
    {
        Scribe_References.Look(ref settlement, "settlement");
        Scribe_References.Look(ref originalFaction, "originalFaction");
        Scribe_Values.Look(ref occupationStartTick, "occupationStartTick", -1);
        Scribe_Values.Look(ref lastKnightPresenceTick, "lastKnightPresenceTick", -1);
        Scribe_References.Look(ref loanedKnight, "loanedKnight");
        Scribe_Values.Look(ref loanEndTick, "loanEndTick", -1);
        Scribe_Values.Look(ref graceUntilTick, "graceUntilTick", -1);
        Scribe_Values.Look(ref converted, "converted");
        Scribe_References.Look(ref archonGovernor, "archonGovernor");
    }
}
