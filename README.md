# Ice Cellar

A Vintage Story mod: **store ice, keep food fresh.**

- **Mod ID:** `icecellar`
- **Author:** Feyd

Vanilla cellars bottom out at 5°C no matter how deep you dig. Stack blocks of
ice in one and it gets genuinely cold — down to **-3°C** with ten blocks, which
roughly **doubles how long food keeps**. And cellar ice finally behaves like ice
in a cellar rather than ice in the summer sun: it melts slowly, and it never
floods the room.

No new blocks, items, or recipes. Just ice and a good cellar.

## How it works

- **Any ice counts** — lake ice you harvested, glacier ice, packed glacier ice.
- **Where you put it doesn't matter** — ice lining the walls, floor, or ceiling
  counts the same as ice stacked in the middle of the room.
- **4 blocks** to start chilling, **10 blocks** for the full effect. The chill
  scales with how good the cellar already is: a leaky, skylit room gets less of
  it, exactly the way vanilla's own cellar rating works.
- **Look at any food container** to see where you stand — the block info panel
  reports how many ice blocks it sees and what percentage of the chill you've
  earned, above the usual perish-speed line.
- **Winter is free** — vanilla already lets ambient cold through, so a cellar
  colder than the ice target is left alone.

### Melting

Vanilla melts ice purely by the outdoor climate and replaces each melted block
with a full water source, which quietly floods any cellar you stock. Here:

- Melt chance is rolled against the **chilled room temperature** instead, so a
  well-stocked cellar preserves its own ice.
- A small floor chance means ice still wastes away eventually — about **one
  block per ~17 hours of real play time** in a fully chilled cellar. Restock it.
- Inside an enclosed room, melting ice **vanishes instead of flooding**.
- Ice buried inside a wall is insulated: it doesn't melt at all.
- Below 2°C outdoors, vanilla never rolls at all — winter preserves cellar ice
  completely.

## Configuration

`ModConfig/icecellar.json`, created on first run. A restart is needed for
changes to take effect (the mechanic is wired up at load time).

| Key | Default | Meaning |
| --- | --- | --- |
| `Enabled` | `true` | Master switch. Off is fully vanilla — including ice that melts into water sources again. |
| `MinBlocks` | `4` | Ice blocks needed before any chill applies. |
| `FullBlocks` | `10` | Ice blocks for the full effect. |
| `Temp` | `-3` | Effective cellar temperature at full effect (clamped to -5..5). |
| `MinMeltChance` | `0.01` | Floor on the per-tick melt roll. `0` makes fully chilled cellar ice last forever. |

## Multiplayer

Universal mod, but only genuinely needed on the **server** — that's where food
perishing and ice melting are computed, so a server running this delivers the
whole mechanic to every player on it. It's therefore declared
`requiredOnClient: false`, so players without it can still join.

Installing it client-side only affects what you're told: the ice count and
chill percentage in the block info panel, and the perish-speed number above
them, are computed client-side. Without the mod a client sees vanilla's
uncooled estimate while their food is in fact keeping twice as long.

## Relationship to The Seraph's Ledger

This mechanic shipped first inside [The Seraph's
Ledger](https://github.com/joyedit/SeraphsLedger) (1.13.0–1.15.0) and still
does, for players who want the whole bundle. Ice Cellar is the same feature on
its own, for players who want only this.

**Running both is safe.** Seraph's Ledger 1.15.1+ detects this mod and stands
down, so the mechanic is applied exactly once. On its first run, Ice Cellar also
carries over any ice-cellar *tuning* values you had set in
`ModConfig/seraphsledger/features.json` — the thresholds, temperature and melt
floor, but not that mod's on/off switch, since installing this one is the
opt-in.

If cellars don't seem to be chilling, check the log: Ice Cellar reports either
`[icecellar] active: …` or `[icecellar] disabled by ModConfig/icecellar.json`
at startup.

If you run Seraph's Ledger older than 1.15.1 alongside this mod, turn its
ice-cooled cellars feature off (Ctrl+Shift+L or `.slconfig`) to avoid stacking
the two.

## Building

`dotnet build -c Debug IceCellar.csproj`, or `./deploy.sh` to build, zip, and
drop `IceCellar_<version>.zip` into the local Mods folder.
