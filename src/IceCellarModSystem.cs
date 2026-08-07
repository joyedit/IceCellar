using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace IceCellar
{
    // Ice-cooled cellars, as a standalone mod. Everything the mechanic needs is
    // four Harmony patches on vanilla methods - there are no blocks, items or
    // recipes of our own, so an existing world can add or drop this mod freely
    // (dropping it just reverts cellars to vanilla, including ice that melts
    // into water sources again).
    //
    // This mechanic also ships inside The Seraph's Ledger 1.13.0+. Both mods
    // patch the same methods, and two stacked postfixes would chill a cellar
    // twice, so Seraph's Ledger 1.15.1+ stands down when it sees this mod
    // installed - this one always owns the feature when present.
    //
    // Note the modid is "icecooledcellars", not "icecellar" - the ModDB had the
    // shorter id reserved by an unlisted mod. The assembly, namespace and repo
    // stay IceCellar; only the modid, the asset domain (assets/icecooledcellars)
    // and the lang key prefix follow the registered id.
    public class IceCellarModSystem : ModSystem
    {
        private const string HarmonyId = "icecooledcellars.patches";
        private Harmony harmony;

        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            // Runs on both sides: the server applies the reduced perish rate to
            // stored food and drives melting, the client shows the ice line in
            // the container tooltip. Start() runs once per side, and in
            // singleplayer both share a process - hence the HasAnyPatches guard,
            // since a Harmony patch is process-wide and one pass covers both.
            var cfg = IceCellarConfig.Load(api);
            if (!cfg.Enabled)
            {
                // Say so out loud: with no blocks or items of our own, a
                // silently disabled mod is indistinguishable from a broken one.
                api.Logger.Notification(
                    "[icecooledcellars] disabled by ModConfig/{0} (\"Enabled\": false), cellars stay vanilla",
                    IceCellarConfig.FileName);
                return;
            }
            if (Harmony.HasAnyPatches(HarmonyId)) return;

            IceCellarPatches.Configure(cfg);
            harmony = new Harmony(HarmonyId);

            // The one method every food container's perish speed (and its
            // tooltip number) funnels through.
            harmony.Patch(
                AccessTools.Method(typeof(Vintagestory.GameContent.InWorldContainer), "GetPerishRate"),
                postfix: new HarmonyMethod(AccessTools.Method(
                    typeof(IceCellarPatches), nameof(IceCellarPatches.GetPerishRatePostfix))));

            // Make Ice count as a cooling cellar-wall material (vanilla counts
            // it as non-cooling, so placed ice would otherwise degrade the
            // room's cellar rating).
            harmony.Patch(
                AccessTools.Method(typeof(Block), nameof(Block.GetRetention),
                    new[] { typeof(BlockPos), typeof(BlockFacing), typeof(EnumRetentionType) }),
                postfix: new HarmonyMethod(AccessTools.Method(
                    typeof(IceCellarPatches), nameof(IceCellarPatches.GetRetentionPostfix))));

            // Cellar-aware ice melting: slower melt driven by the chilled room
            // temperature, and no water-source flooding indoors.
            harmony.Patch(
                AccessTools.Method(typeof(Vintagestory.GameContent.BlockLakeIce), "OnServerGameTick"),
                prefix: new HarmonyMethod(AccessTools.Method(
                    typeof(IceCellarPatches), nameof(IceCellarPatches.IceMeltPrefix))));

            // Ice-count line in container tooltips, for discoverability.
            harmony.Patch(
                AccessTools.Method(typeof(Vintagestory.GameContent.BlockEntityContainer),
                    nameof(Vintagestory.GameContent.BlockEntityContainer.GetBlockInfo)),
                postfix: new HarmonyMethod(AccessTools.Method(
                    typeof(IceCellarPatches), nameof(IceCellarPatches.GetBlockInfoPostfix))));

            api.Logger.Notification(
                "[icecooledcellars] active: {0}+ ice blocks chill a cellar, {1} for full effect ({2}°C)",
                cfg.MinBlocks, cfg.FullBlocks, cfg.Temp);
        }

        public override void Dispose()
        {
            harmony?.UnpatchAll(HarmonyId);
            harmony = null;
            base.Dispose();
        }
    }
}
