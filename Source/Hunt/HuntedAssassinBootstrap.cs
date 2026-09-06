/*
 * This file is part of Emperors and Consuls, a Better Rimworlds Project.
 *
 * Copyright © 2026 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *
 * This file is licensed under the MIT License.
 */

using UnityEngine;
using Verse;

namespace BetterRimworlds.EmperorsAndConsuls;

/// Provide HuntedAssassin when CryoRegenesis is not loaded. Same defName so saves stay compatible.
[StaticConstructorOnStartup]
internal static class HuntedAssassinBootstrap
{
    static HuntedAssassinBootstrap()
    {
        if (DefDatabase<HediffDef>.GetNamedSilentFail("HuntedAssassin") != null)
        {
            return;
        }

        int ticks = Calendar.Ticks(Calendar.HuntDays);
        HediffDef def = new HediffDef
        {
            defName = "HuntedAssassin",
            label = "hunted assassin",
            description =
                "Subspace tracker nanites implanted by the Empire under its \"Keep What You Kill\" succession law. "
                + "The body clears them over about sixty days. Survive that hunt, and Imperial custom recognizes the claim on the throne.\n\n"
                + "If a noble of knight rank or higher kills the Marked One, the nanites leap to the killer and begin a fresh sixty-day countdown.",
            hediffClass = typeof(HediffWithComps),
            isBad = false,
            displayWound = false,
            makesSickThought = false,
            everCurableByItem = false,
            tendable = false,
            defaultLabelColor = new Color(0.72f, 0.28f, 0.82f),
            lethalSeverity = -1f,
            comps = new List<HediffCompProperties>
            {
                new HediffCompProperties_Disappears
                {
                    disappearsAfterTicks = new IntRange(ticks, ticks),
                    showRemainingTime = true
                }
            }
        };

        DefDatabase<HediffDef>.Add(def);
        Log.Message(EmperorsAndConsulsMod.LogPrefix + "Provided HuntedAssassin (CryoRegenesis not loaded).");
    }
}
