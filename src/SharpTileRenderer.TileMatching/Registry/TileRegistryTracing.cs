using Serilog;
using Serilog.Events;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Registry
{
    public class TileRegistryTracing
    {
        TileRegistryTracing()
        { }

        public static readonly ILogger MissingTilesTracer = SLog.ForContext<TileRegistryTracing>();

        public static void EmitMissingTileWarning(string tag)
        {
            MissingTilesTracer.Verbose("Missing tile in registry for tag ({Tag})", tag);
        }

        public static void EmitMissingTileWarning(string tag, IReadOnlyList<string> alts)
        {
            if (alts == null || alts.Count == 0)
            {
                EmitMissingTileWarning(tag);
            }
            else if (MissingTilesTracer.IsEnabled(LogEventLevel.Verbose))
            {
                MissingTilesTracer.Verbose("Missing tile in registry for tag ({Tag}) or alternative tags {Tags}", tag, alts);
            }
        }
    }
}
