using System;
using Vintagestory.API.Common;

namespace IceCellar
{
    // Tunables for the ice-cooled cellar mechanic, stored as a single JSON file
    // in the ModConfig folder. In singleplayer the client and the integrated
    // server share that folder, so one file drives both; on a dedicated server
    // the server's copy is authoritative for perishing and melting.
    //
    // Because the mechanic is applied with Harmony patches wired up at load
    // time, changes only take effect after a restart.
    public class IceCellarConfig
    {
        // Master switch, mainly for servers that want the mod installed (so
        // clients can keep it) with the mechanic off. Off means fully vanilla:
        // cellars floor at 5°C again and cellar ice melts by the outdoor
        // climate, flooding the room with water sources - so flipping this on
        // an existing world costs it its stocked ice.
        public bool Enabled = true;

        // MinBlocks is where the effect starts, FullBlocks where it maxes out,
        // Temp the effective cellar temperature at full effect (clamped to
        // -5..5; below -5 the game's own perish-rate floor applies anyway).
        // At full effect food keeps roughly twice as long as in a plain cellar.
        public int MinBlocks = 4;
        public int FullBlocks = 10;
        public float Temp = -3f;

        // Floor on the per-random-tick melt roll for ice in a cellar (a roll
        // lands on a given block about every ~10 real minutes), so even a fully
        // iced cellar slowly loses blocks - at the default about one block per
        // ~17 hours of real play time. 0 makes fully-chilled cellar ice last
        // forever.
        public float MinMeltChance = 0.01f;

        public const string FileName = "icecellar.json";

        // Loads the config, writing out a default file the first time so the
        // player has something to find on disk. Never returns null.
        public static IceCellarConfig Load(ICoreAPI api)
        {
            IceCellarConfig cfg = null;
            try
            {
                cfg = api.LoadModConfig<IceCellarConfig>(FileName);
            }
            catch (Exception e)
            {
                api.Logger.Warning("[icecellar] config load failed, using defaults: {0}", e.Message);
            }

            if (cfg == null)
            {
                cfg = new IceCellarConfig();
                cfg.AdoptSeraphsLedgerSettings(api);
                cfg.Save(api);
            }
            return cfg;
        }

        // This mechanic shipped first inside The Seraph's Ledger (1.13.0-1.15.0),
        // where it lives in that mod's shared features.json under IceCellar*
        // keys. On our very first run, carry over any values a player tuned
        // there instead of silently resetting them to defaults. Missing file or
        // missing keys just leave the defaults in place.
        //
        // Deliberately NOT carried over: that mod's IceCooledCellars on/off
        // switch. Installing this mod is itself the opt-in, and a player who
        // had the feature switched off in the bundle - or who never turned it
        // on back when it shipped off by default - would otherwise install a
        // mod that silently does nothing.
        private void AdoptSeraphsLedgerSettings(ICoreAPI api)
        {
            try
            {
                var old = api.LoadModConfig<SeraphsLedgerIceSettings>("seraphsledger/features.json");
                if (old == null) return;

                MinBlocks = old.IceCellarMinBlocks;
                FullBlocks = old.IceCellarFullBlocks;
                Temp = old.IceCellarTemp;
                MinMeltChance = old.IceCellarMinMeltChance;
                api.Logger.Notification("[icecellar] adopted ice-cellar tuning values from seraphsledger/features.json");
            }
            catch (Exception e)
            {
                api.Logger.Warning("[icecellar] could not read seraphsledger/features.json: {0}", e.Message);
            }
        }

        // Just the ice-cellar tuning slice of Seraph's Ledger's config; every
        // other key in that file is ignored by the deserializer. Defaults match
        // ours, so a file predating any given key leaves that setting alone.
        private class SeraphsLedgerIceSettings
        {
            public int IceCellarMinBlocks = 4;
            public int IceCellarFullBlocks = 10;
            public float IceCellarTemp = -3f;
            public float IceCellarMinMeltChance = 0.01f;
        }

        public void Save(ICoreAPI api)
        {
            try
            {
                api.StoreModConfig(this, FileName);
            }
            catch (Exception e)
            {
                api.Logger.Warning("[icecellar] config save failed: {0}", e.Message);
            }
        }
    }
}
