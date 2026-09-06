/*
 * This file is part of Emperors and Consuls, a Better Rimworlds Project.
 *
 * Copyright © 2026 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *
 * This file is licensed under the MIT License.
 */

using RimWorld;

namespace BetterRimworlds.EmperorsAndConsuls;

public static class Calendar
{
    public const int HuntDays = 60;
    public const int LoanDays = 60;
    public const int OccupationDays = 300;
    public const int KnightDeathGraceDays = 2;

    public static int Ticks(int days)
    {
        return days * GenDate.TicksPerDay;
    }

    public static int DaysLeft(int ticksRemaining)
    {
        if (ticksRemaining <= 0)
        {
            return 0;
        }

        return (ticksRemaining + GenDate.TicksPerDay - 1) / GenDate.TicksPerDay;
    }
}
