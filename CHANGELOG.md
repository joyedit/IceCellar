# Changelog

All notable changes to Ice Cellar are documented here.

## 1.0.0 — 2026-08-07

Initial standalone release. Ice Cellar began life inside The Seraph's Ledger
(1.13.0–1.15.0) and was extracted into its own mod by request; version 1.0.0 is
feature-identical to the ice-cooled cellars feature in Seraph's Ledger 1.15.0.

### Features
- **Ice-chilled cellars.** Ice blocks (lake, glacier, packed glacier — anything
  with the Ice block material) inside a proper cellar pull it below vanilla's
  hardcoded 5°C floor, down to -3°C at ten blocks, roughly doubling food shelf
  life. The chill ramps from 4 blocks to 10 and scales with the room's own
  cellar rating, so skylights and non-cooling walls still cost you. Ice within
  a one-block shell of the room counts, so wall-, floor- and ceiling-lining
  builds work as well as free-standing stacks.
- **Ice counts as a cooling wall.** Vanilla treats only Ore, Stone, Soil and
  Ceramic as cooling cellar materials — Ice counts against the room — so
  without this, lining a cellar with ice would degrade its rating.
- **Cellar-aware melting.** Vanilla rolls melt chance off the outdoor climate
  and replaces melted ice with a full water source, flooding stocked cellars.
  The roll now runs against the chilled room temperature, with a floor chance
  so ice still wastes away (about one block per ~17 hours of real play in a
  fully chilled cellar). Ice melting inside an enclosed room vanishes instead
  of flooding it, and ice buried in a wall doesn't melt at all. Vanilla skips
  the roll entirely below 2°C outdoors, so winter preserves cellar ice for
  free.
- **Discoverability.** Food containers report the ice count and current chill
  percentage in the block info panel, above the perish-speed line vanilla
  already shows.
- **Configurable** through `ModConfig/icecellar.json`: master switch, the two
  block thresholds, the target temperature, and the melt-chance floor.
- **Universal, but declared `requiredOnClient: false`.** Perishing and melting
  are computed server-side, so a server running this covers everyone on it and
  clients without the mod can still join — they just see vanilla's uncooled
  perish speed and no ice count in the block info panel.

### Coexistence with The Seraph's Ledger
- Both mods patch the same vanilla methods, so stacking them would chill a
  cellar twice. Seraph's Ledger 1.15.1 stands down when it sees `icecellar`
  installed; running both is safe from that version on.
- On first run, Ice Cellar adopts any ice-cellar *tuning* values already set in
  `ModConfig/seraphsledger/features.json` (block thresholds, temperature, melt
  floor) instead of resetting them to defaults. That mod's on/off switch is
  deliberately not carried over — installing this mod is the opt-in, and
  players whose bundle config predates Seraph's Ledger 1.15.0 have the feature
  stored as off.
