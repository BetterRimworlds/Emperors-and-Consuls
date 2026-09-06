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

public class Alert_VassalNeedsKnight : Alert
{
    public Alert_VassalNeedsKnight()
    {
        defaultLabel = "BR_EAC_AlertNeedsKnight".Translate();
        defaultPriority = AlertPriority.High;
    }

    public override AlertReport GetReport()
    {
        GameComponent_EmperorsAndConsuls comp = GameComponent_EmperorsAndConsuls.Current;
        if (comp?.vassals == null || !EmpireAlliance.CourtIsActive())
        {
            return false;
        }

        List<GlobalTargetInfo> targets = new List<GlobalTargetInfo>();
        foreach (VassalRecord record in comp.vassals)
        {
            if (record?.settlement == null || record.converted || record.HasActiveLoan)
            {
                continue;
            }

            targets.Add(record.settlement);
        }

        if (targets.Count == 0)
        {
            return false;
        }

        return AlertReport.CulpritsAre(targets);
    }

    public override TaggedString GetExplanation()
    {
        GameComponent_EmperorsAndConsuls comp = GameComponent_EmperorsAndConsuls.Current;
        string names = "";
        if (comp?.vassals != null)
        {
            foreach (VassalRecord record in comp.vassals)
            {
                if (record?.settlement != null && !record.converted && !record.HasActiveLoan)
                {
                    names += "  " + record.settlement.Label + "\n";
                }
            }
        }

        return "BR_EAC_AlertNeedsKnightDesc".Translate(names.TrimEnd());
    }
}
