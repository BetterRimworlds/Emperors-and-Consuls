/*
 * This file is part of Emperors and Consuls, a Better Rimworlds Project.
 *
 * Copyright © 2026 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *
 * This file is licensed under the MIT License.
 */

using Verse;

namespace BetterRimworlds.EmperorsAndConsuls;

public static class ModUtility
{
    public const string CryoRegenesisPackageId = "HopeSeekr.BetterRimworlds.LCryoRegenesis";
    public const string StargatePackageId = "HopeSeekr.BetterRimworlds.Stargate";

    public static bool IsModActive(string packageId)
    {
        if (packageId.NullOrEmpty() || LoadedModManager.RunningModsListForReading == null)
        {
            return false;
        }

        foreach (ModContentPack pack in LoadedModManager.RunningModsListForReading)
        {
            if (pack.PackageId.Equals(packageId, StringComparison.OrdinalIgnoreCase)
                || pack.PackageId.StartsWith(packageId + "_", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public static bool CryoRegenesisLoaded()
    {
        return IsModActive(CryoRegenesisPackageId);
    }

    public static bool StargateLoaded()
    {
        return IsModActive(StargatePackageId)
            || DefDatabase<ThingDef>.GetNamedSilentFail("TransdimensionalStargate") != null;
    }
}
