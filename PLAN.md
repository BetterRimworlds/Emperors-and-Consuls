# PLAN.md — Emperors and Consuls

## Summary

Create **Emperors and Consuls** as an independent RimWorld mod and repository. It is the playable sequel to CryoRegenesis Royalty endings. There are **no new scenarios**. Continuity is **Stargate** (`Shuttle.xml`). Court, alliance, and conquest stay inert until a living colonist holds **Consul** or **Emperor** — titles CryoRegenesis grants. Then the Empire on this world treats the colony as legitimate, and the player expands by conquering settlements into vassals — after five years of Knight occupation, converting them into Empire cities with an Archon.

This file is the project plan. It is not implementation.

## Standalone Mod Structure

- Sibling repository: `/code/Rimworld/Emperors-and-Consuls`
- Mod name: Emperors and Consuls
- Assembly: `EmperorsAndConsuls`
- Package ID: `HopeSeekr.BetterRimworlds.EmperorsAndConsuls`
- Harmony ID: `HopeSeekr.BetterRimworlds.EmperorsAndConsuls`
- Author: hopeseekr (Better Rimworlds)

Hard dependencies: Harmony, RimWorld **Royalty**, **Stargate** (game-to-game continuity).

Soft dependencies: CryoRegenesis (grants Consul / Emperor / Hunted Assassin; share the hediff defName when loaded).

Do **not** live inside the CryoRegenesis tree. Do **not** edit CryoRegenesis `CHANGELOG.md`.

Language: match CryoRegenesis. Never use “latch” as a verb. Prefer *records*, *keeps*, *remains*.

---

## Pitch

CryoRegenesis currently ends the story at the credits:

- **Imperial Court** — colonists leave with the rejuvenated Emperor. The highest-ranked passenger is named **Special Consul** in the letter. That title is flavor only; nothing playable follows.
- **You Keep What You Kill (usurpation)** — a colony Archon/Count seals the Emperor and is crowned. Credits. Game over.
- **You Keep What You Kill (assassination)** — a Knight+ kills the Emperor, takes the **Hunted Assassin** mark, flees a Planetkiller, and the credits say: survive sixty days and the throne is yours. The hunt is not a game.

This mod is that next game. You establish a court on a rimworld, the Empire already treats you as legitimate, and you expand by putting Knights and Archons on conquered ground — not by clicking a faction into existence.

---

## Calendar (locked)

RimWorld years are **60 days**.

| Timer | Duration |
|---|---|
| Assassin hunt | 60 days (1 year) |
| Knight loan term | 60 days (1 year) |
| Continuous occupation before Archon conversion | 300 days (5 years) |

Five years of occupation is five consecutive year-long Knight loans with **no gap**.

---

## No scenarios of our own

This mod does **not** add new-game scenarios. A normal rimworld start (Crashlanded, whatever the player picks) is fine.

The court arrives the same way other Better Rimworlds games continue: **Stargate recall** of `Shuttle.xml` written by CryoRegenesis when the Imperial shuttle / endgame party leaves.

Mechanisms (Empire alliance, extra stock, vassalize, Knight loans, Archon conversion) **only click** when a living member of the colony holds **Consul** or **Emperor**. Those titles are granted by CryoRegenesis, not by this mod.

### Keep What You Kill (assassin), if the mark is still on a colonist

If a recalled pawn still bears **Hunted Assassin**, this world runs the hunt: frequent raids for the remaining sixty days. If a Knight+ kills the marked pawn, the nanites jump and the **full 60 days restart**. If the mark burns out while the host lives, they are crowned Emperor **in the same save**, and the court mechanisms click.

---

## Establish on a planet

The player founds a colony however they like. When a Consul or Emperor is among the colonists (typically after Stargate recall):

- Immediate need: shelter, food, power, a throne room that satisfies the ruler’s title.
- The Empire on this world already exists as the stock Royalty faction. You do not replace it; you are its recognized head (Emperor) or its planetary plenipotentiary (Consul).

---

## Empire alliance and trade

When a living colonist holds Consul or Emperor (and again if the assassin is crowned):

1. Find the local Empire faction (`FactionDef` Empire).
2. Set player goodwill to ally (100). Stop ordinary goodwill decay toward the Empire while a living colonist holds Emperor, Special Consul, or (after the hunt) the recognized claimant.
3. All Empire **settlements** on the world map are allied cities. No war with the Empire from the player side unless the player attacks them (forbidden by default / warned).
4. **Trade surplus (if the API allows):** Harmony postfix on Empire settlement and orbital-trader stock generation: extra food, medicine, components, silver, maybe a small uranium/Luciferium trickle. Fail closed: if stock generation cannot be patched cleanly on a version, skip extra stock and keep the alliance.

Do **not** vassalize Empire cities. They are already Imperial.

---

## Conquest, vassals, Knights, Archons

Vanilla RimWorld cannot annex a settlement. This mod adds an occupation layer on existing `Settlement` world objects.

### Who can be conquered

- Outlander cities (civil outlander settlements).
- Tribal settlements.
- Pirates: **allowed by default** (open question below).
- **Not** Empire settlements. **Not** player colonies. **Not** temporary camps / work sites unless they are real `Settlement`s.

### How a settlement becomes a vassal

1. A player caravan (or drop) **attacks** the settlement.
2. At least one **colony Knight or higher** (Empire title Knight/Dame+) must be **present on the map** when the last defenders break.
3. Instead of the usual “destroyed settlement” outcome, the site remains. It is marked **Imperial Vassal**:
   - Original faction is displaced or subordinated.
   - Settlement joins a player-allied **Imperial Vassals** faction (new faction def, never player-controlled maps).
   - Goodwill with the player is locked allied.
   - The site keeps producing caravans / remaining a trade point if possible.

If no Knight is present at victory, the raid is a normal raid. No annexation.

### Knight on loan (60 days)

A vassal **does not stay occupied** without a Knight.

- From the world map (or a gizmo on the vassal settlement), the player **loans** a colony Knight to that settlement for **60 days**.
- The pawn leaves the colony (world pawn stationed at the settlement, not a second player map). They cannot work the home base during the term.
- One Knight cannot occupy two vassals at once.
- The loan can be **renewed** before expiry (another 60 days). Renewal is how you get continuous occupation.
- If the term ends with no Knight on site: occupation breaks. The vassal rebels (reverts to hostile original faction or a new rebel faction). The five-year clock **resets**.
- If the loaned Knight dies: same as a gap, unless another Knight is already there or is sent within a **2-day grace** (default).

This caps expansion at “how many Knights can we spare.”

### Five years, then an Archon

After **300 days of continuous occupation** (no gap in Knight presence):

- The player may **send an Archon** (Empire title Archon, or Count on older Royalty versions) to that vassal.
- The Archon travels there (caravan / shuttle) and performs a **conversion**: the settlement’s faction becomes the **Empire** faction. It is now an Imperial city, same as the ones you allied at start.
- Conversion is a one-time ritual/quest **at the site**, not instant from the world map with the Archon still at home.

### Recall the Archon

After conversion succeeds, the Archon is no longer required on site.

- **Recall** — the Archon returns (shuttle or caravan) and **rejoins** the colony as a colonist, title intact.
- Or leave them as the city’s Imperial governor (world pawn). Recall remains available later.

Knights used for the five-year occupation can be recalled when their current 60-day term ends, or immediately once an Archon has converted the city.

---

## Title gates (Royalty)

Use stock Empire titles. Do not invent a parallel nobility unless vanilla cannot grant Emperor / Consul.

| Act | Required title on the pawn who does it |
|---|---|
| Annex a settlement | Knight / Dame or higher, present at victory |
| Hold a vassal | Knight on loan at that settlement |
| Convert vassal → Empire city | Archon (Count on 1.2–1.4) sent to the site after 5 years |
| Start as Consul | Special Consul (custom) or vanilla Consul |
| Start as / become Emperor | Emperor (custom if needed) |
| Inherit the assassin mark | Knight or higher (existing CryoRegenesis rule) |

1.5/1.6 renamed Count → Archon. Version-gate the def name the same way CryoRegenesis already does.

Consul and Emperor have the **same** conquest rights by default.

---

## Technical sketch

```
Emperors-and-Consuls/
  About/About.xml
  Defs/
    Factions/           Imperial Vassals
    Hediffs/            HuntedAssassin (duplicate or patch-if-missing)
    Titles/             Emperor, Special Consul if vanilla cannot grant them
  Languages/English/...
  Source/
    Core/               Mod, Harmony, GameComponent
    Court/              detect Consul/Emperor, alliance
    Empire/             goodwill lock, trade stock postfix
    Hunt/               60-day crisis storyteller / incident queue
    Occupation/         Settlement component, loan, conversion, recall
  PLAN.md
  README.md
```

**GameComponent** stores: vassal settlement IDs, occupation start tick, last Knight presence tick, loaned pawn IDs and term end ticks, conversion state, whether the assassin has been crowned.

**Settlement component** (or world-object comp) on occupied sites: occupation clock, stationed pawn refs, “needs Knight” alert.

**Harmony (minimum):**

- Settlement raid victory → offer vassalize if Knight present.
- Empire trader stock (optional extra supplies).
- Prevent player vs Empire hostility from random events while the alliance lock is on.
- `Pawn.Kill` → mark inheritance if CryoRegenesis is not loaded.

**Do not depend on** Vanilla Factions Expanded, RimCities, or VE Empire. Detect and disable annexation with a log warning if a known annexation mod is already doing it.

**Pawn on loan:** world pawn stationed at the settlement so they are not garbage-collected. Recall rejoins the player faction without a prisoner flow.

If CryoRegenesis is loaded, **share** the `HuntedAssassin` def. If not, this mod provides a duplicate with the same defName so saves stay compatible either way.

---

## What this is not (v1)

- Not a CryoRegenesis feature pack. No caskets required.
- Not a full new Royalty tree.
- Not player-controlled extra maps for every vassal.
- Not Stellarch / whole-system government.
- Not Stargate gameplay (import only).
- Not a Planetkiller on the new world (they already fled one).
- Not a CryoRegenesis changelog entry.

---

## Implementation slices

Ship in this order so each slice is playable.

1. **Repo + court gate** — no scenarios. Alliance at 100 only while a colonist is Consul or Emperor.
2. **Assassin year** — Hunted Assassin hediff if present, raid storm, mark inheritance, crown on expiry.
3. **Trade surplus** — extra Empire stock if the patch is clean; otherwise skip.
4. **Vassalize** — Knight-present raid converts settlement to Imperial Vassals (requires court).
5. **Knight loans** — 60-day stationing, alerts, rebellion on gap.
6. **Archon conversion + recall** — 300-day clock, send Archon, faction → Empire, recall to colony.
7. **Stargate continuity** — hard dependency; court arrives via existing `Shuttle.xml` recall, not a scenario part.
8. **English keyed letters** first; other languages later.

---

## Implementation status

Recorded 2026-09-06. **About 75%** of this v1 plan is in the Emperors-and-Consuls repo. The remaining quarter is thinner design (quest/ritual, hunt flavor, attack warning) plus the CryoRegenesis handshake that actually turns the court on.

### By implementation slice

| Slice | Plan | Done | Notes |
|---|---|---|---|
| 1. Repo + court gate | no scenarios; alliance only with Consul/Emperor | **90%** | Gate and alliance exist. CryoRegenesis still only *names* Special Consul in credits — it does not `SetTitle`, so a real court recall may never wake the mod. |
| 2. Assassin year | mark, raid storm, inherit, crown in-save | **70%** | Mark, inheritance (if CR absent), expiry crowning, generic enemy raids every 2–4 days. Not a distinct hunter/rival-Knight crisis. |
| 3. Trade surplus | extra Empire stock, fail closed | **85%** | Settlement + orbital postfix, wrapped in try/catch. |
| 4. Vassalize | Knight at victory → Imperial Vassals | **80%** | Auto-annex (no “offer”), skips Empire/player/hidden/mechs/insects, pirates allowed. |
| 5. Knight loans | 60 days, one site, renew, 2-day death grace, rebellion | **90%** | World-pawn stationing, gizmos, alert. |
| 6. Archon conversion + recall | 300 days, rite **at the site**, recall/governor | **70%** | Caravan gizmo when an Archon is on the tile — not a quest/ritual. Recall drop-pods home. |
| 7. Stargate continuity | hard dep; `Shuttle.xml` recall | **50%** | Stargate is a hard dependency. This mod does not read `Shuttle.xml`; it waits for titles on pawns Stargate already spawned. |
| 8. English letters | English first | **95%** | Keyed English is in. Other languages were explicitly later. |

### What is not done (the other ~25%)

- **CryoRegenesis still does not grant Consul/Emperor on the pawn**, so the success playthrough cannot start without debug actions.
- No warning / “you are attacking your own Empire” break of the alliance (`playerBrokeAlliance` is saved, never set). Hostility is just blocked.
- Conversion is a world gizmo, not a one-time site quest.
- Hunt is repeated `RaidEnemy`, not mercenary hunters / rival Knights.
- No extra languages (called out as later).

### Success criteria vs code

Coded end-to-end **if** a Consul/Emperor (or marked assassin) is already on the map. Not closed as a campaign from a CryoRegenesis credits roll, because that ending still does not put the title on the colonist.

---

## Key decisions

1. **Standalone mod**, not a CryoRegenesis module — different save component, different players, Royalty-only installs.
2. **RimWorld years** for every timer (60 / 300 days) — matches Hunted Assassin and “loan for 60 days.”
3. **Vassal is a new allied faction**, then **Empire faction after Archon** — two-step legitimacy, not instant recolor.
4. **Knights as the occupation cap** — expansion costs a noble’s absence from the home colony.
5. **Assassin crowning continues the same save** — no forced new game after 60 days.
6. **Empire cities are allies, not conquest targets.**
7. **Stargate is required** for court continuity. No generated court, no scenarios.
8. **1.6 first**; older Royalty versions only if CryoRegenesis-style multi-target builds are wanted later.
9. **Share `HuntedAssassin`** when CryoRegenesis is present; otherwise provide the same defName.

---

## Open questions

Defaults in **bold** if unanswered:

1. Starting party size — **whatever the player’s scenario is**; court comes through the Stargate.
2. Grace period after a loaned Knight dies — **2 days**.
3. Can pirates be vassalized? — **yes**.
4. Does the Consul get the same conquest rights as the Emperor? — **yes**.
5. Minimum RimWorld version — **1.6 first**.
6. Hunted Assassin def sharing — **same defName, provide if missing**.

---

## Success

A player can finish CryoRegenesis as Special Consul, recall the court through a Stargate, ally every Empire city once that Consul is on the map, buy extra stock from them, take a tribe with a Knight in the caravan, loan that Knight for a year, renew for five years, send an Archon, watch the settlement flip to Empire, and call the Archon home.

A second player can recall a **Keep What You Kill** assassin, fight for 60 days, inherit the mark if they are stupid, and sit the throne without a credits roll.
