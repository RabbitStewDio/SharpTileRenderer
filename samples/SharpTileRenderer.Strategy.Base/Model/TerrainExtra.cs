using SharpTileRenderer.Strategy.Base.Util;

namespace SharpTileRenderer.Strategy.Base.Model
{
    /*
    public static class TerrainExtra
    {
        public static bool HasExtra(this ushort flags, ITerrainExtra? extra)
        {
            if (extra == null || extra.DataId == -1)
            {
                return false;
            }

            return flags.Read(extra.DataId);
        }

        public static ushort AddExtra(this ushort flags, ITerrainExtra extra)
        {
            if (extra.Requires.TryGetValue(out var requirement))
            {
                if (!flags.HasExtra(requirement))
                {
                    flags = flags.AddExtra(requirement);
                }
            }

            if (extra.DataId != -1)
            {
                flags = flags.Write(extra.DataId, true);
            }

            return flags;
        }
    }
    */
}