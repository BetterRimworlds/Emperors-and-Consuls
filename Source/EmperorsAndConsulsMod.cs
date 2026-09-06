/*
 * This file is part of Emperors and Consuls, a Better Rimworlds Project.
 *
 * Copyright © 2026 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *
 * This file is licensed under the MIT License.
 */

using System.Reflection;
using HarmonyLib;
using Verse;

namespace BetterRimworlds.EmperorsAndConsuls;

public class EmperorsAndConsulsMod : Mod
{
    public const string HarmonyId = "HopeSeekr.BetterRimworlds.EmperorsAndConsuls";
    public const string LogPrefix = "[Emperors and Consuls] ";

    public EmperorsAndConsulsMod(ModContentPack content) : base(content)
    {
        Harmony harmony = new Harmony(HarmonyId);

        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            try
            {
                if (!type.IsDefined(typeof(HarmonyPatch), inherit: true))
                {
                    continue;
                }

                harmony.CreateClassProcessor(type).Patch();
            }
            catch (Exception ex)
            {
                Log.Error(LogPrefix + "Harmony patch failed on " + type.FullName + ": " + ex);
            }
        }
    }
}
