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

[DefOf]
public static class EmperorsAndConsulsDefOf
{
    public static FactionDef BR_ImperialVassals;
    public static RoyalTitleDef BR_SpecialConsul;

    static EmperorsAndConsulsDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(EmperorsAndConsulsDefOf));
    }
}
