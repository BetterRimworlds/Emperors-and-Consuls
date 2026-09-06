# Emperors and Consuls

A Better Rimworlds sequel to [CryoRegenesis](https://github.com/BetterRimworlds/CryoRegenesis) Royalty endings.

This is **not** a new-game scenario pack. Continuity is **Stargate**: CryoRegenesis writes the departing court to `Shuttle.xml`, you recall them on a new rimworld, and this mod wakes up only when a living colonist is **Consul** or **Emperor** — titles CryoRegenesis grants.

Until then, alliance, extra Imperial stock, vassalization, Knight loans, and Archon conversion stay inert.

## How a campaign starts

1. Finish a CryoRegenesis Royalty ending (Imperial Court Consul, usurpation Emperor, or Keep What You Kill assassin).
2. The court leaves through a Stargate (`Shuttle.xml`).
3. Start a normal new game with **Royalty**, **Stargate**, and this mod (CryoRegenesis still recommended).
4. Recall the court. When a Consul or Emperor is among them, every Empire city on this world becomes an ally.

An assassin who still bears **Hunted Assassin** fights a sixty-day succession crisis here. If the mark burns out while they live, they are crowned Emperor in the **same save**.

## Campaign loop (once a Consul or Emperor is present)

1. Instant **alliance** with every Empire city. Extra trade stock if the stock patch applies cleanly.
2. **Conquer** outlander cities, tribes, and pirates (a colony Knight must be present at victory) → **Imperial Vassal**.
3. **Loan a Knight** to that vassal, 60 days at a time. Gaps break occupation and reset the five-year clock. A loaned Knight who dies gets a 2-day grace.
4. After **5 RimWorld years** (300 days) of continuous occupation, send an **Archon** (defName `Count`) to the site to convert it to the Empire faction.
5. **Recall** the Archon to the colony, or leave them as governor.

## Requirements

- RimWorld **1.6**
- [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)
- **Royalty**
- **[Stargate](https://github.com/BetterRimworlds/Stargate)** (hard dependency — game-to-game continuity)
- [CryoRegenesis](https://github.com/BetterRimworlds/CryoRegenesis) (source of Consul / Emperor / Hunted Assassin)

## Build

```bash
./build.sh
```

Compiles `Release v1.6` and syncs the mod into `/rimworld/1.6/Mods/EmperorsAndConsuls`.

See [PLAN.md](PLAN.md) for timers, title gates, and Harmony sketch.
