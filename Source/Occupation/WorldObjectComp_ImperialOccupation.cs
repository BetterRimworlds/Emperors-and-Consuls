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

public class WorldObjectComp_ImperialOccupation : WorldObjectComp
{
    private Settlement Settlement => parent as Settlement;

    private VassalRecord Record =>
        GameComponent_EmperorsAndConsuls.Current?.RecordFor(Settlement);

    public override string CompInspectStringExtra()
    {
        VassalRecord record = Record;
        if (record == null)
        {
            return null;
        }

        if (record.converted)
        {
            if (record.archonGovernor != null)
            {
                return "BR_EAC_InspectConverted".Translate(record.archonGovernor.LabelShort);
            }

            return "BR_EAC_InspectConvertedNoGovernor".Translate();
        }

        string text = "BR_EAC_InspectVassal".Translate(
            record.OccupationDaysElapsed,
            Calendar.OccupationDays);

        if (record.HasActiveLoan)
        {
            text += "\n" + "BR_EAC_InspectLoan".Translate(
                record.loanedKnight.LabelShort,
                Calendar.DaysLeft(record.loanEndTick - Find.TickManager.TicksGame));
        }
        else if (record.InDeathGrace)
        {
            text += "\n" + "BR_EAC_InspectGrace".Translate(
                Calendar.DaysLeft(record.graceUntilTick - Find.TickManager.TicksGame));
        }

        if (record.ReadyForArchon)
        {
            text += "\n" + "BR_EAC_InspectReady".Translate();
        }

        return text;
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        VassalRecord record = Record;
        if (record == null || !EmpireAlliance.CourtIsActive())
        {
            yield break;
        }

        if (record.converted)
        {
            if (record.archonGovernor != null)
            {
                yield return new Command_Action
                {
                    defaultLabel = "BR_EAC_GizmoRecallArchon".Translate(),
                    defaultDesc = "BR_EAC_GizmoRecallArchonDesc".Translate(),
                    icon = TexCommand.ForbidOff,
                    action = () => OccupationUtility.RecallArchon(record)
                };
            }

            yield break;
        }

        if (!record.HasActiveLoan)
        {
            yield return new Command_Action
            {
                defaultLabel = "BR_EAC_GizmoLoan".Translate(),
                defaultDesc = "BR_EAC_GizmoLoanDesc".Translate(),
                icon = TexCommand.Install,
                action = () => OccupationUtility.OfferLoan(Settlement)
            };
        }
        else
        {
            yield return new Command_Action
            {
                defaultLabel = "BR_EAC_GizmoRenew".Translate(),
                defaultDesc = "BR_EAC_GizmoRenewDesc".Translate(),
                icon = TexCommand.Install,
                action = () => OccupationUtility.RenewLoan(record)
            };
            yield return new Command_Action
            {
                defaultLabel = "BR_EAC_GizmoRecallKnight".Translate(),
                defaultDesc = "BR_EAC_GizmoRecallKnightDesc".Translate(),
                icon = TexCommand.ForbidOff,
                action = () => OccupationUtility.RecallKnight(record)
            };
        }
    }

    public override IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
    {
        VassalRecord record = Record;
        if (record == null || record.converted || !record.ReadyForArchon || caravan == null)
        {
            yield break;
        }

        if (!EmpireAlliance.CourtIsActive())
        {
            yield break;
        }

        bool hasArchon = false;
        foreach (Pawn pawn in caravan.PawnsListForReading)
        {
            if (TitleUtility.IsArchonOrHigher(pawn))
            {
                hasArchon = true;
                break;
            }
        }

        if (!hasArchon)
        {
            yield break;
        }

        yield return new Command_Action
        {
            defaultLabel = "BR_EAC_GizmoConvert".Translate(),
            defaultDesc = "BR_EAC_GizmoConvertDesc".Translate(),
            icon = TexCommand.Install,
            action = () => OccupationUtility.ConvertAtSite(record, caravan)
        };
    }
}
