using SharpTileRenderer.Strategy.Base.Map;
using SharpTileRenderer.Strategy.Base.Model;
using SharpTileRenderer.Util;

namespace SharpTileRenderer.Strategy.Base
{
    public class StrategyGameFog
    {
        public int TerrainWidth { get; }
        public int TerrainHeight { get; }
        public IFogMap Fog { get; }

        public StrategyGameFog(int terrainWidth, int terrainHeight)
        {
            TerrainWidth = terrainWidth;
            TerrainHeight = terrainHeight;
            Fog = new FogMap(TerrainWidth, TerrainHeight);

            // Explicitly set the initial visible tracker to the location of one of the cities 
            // here so that the visibility marking works correctly.
            Fog.MarkRangeVisible(0, 0, 2);
            
        }

        Optional<(int x, int y)> cachedPos;
        
        public void UpdateMousePosition(int mousePositionX, int mousePositionY)
        {
            if (cachedPos.TryGetValue(out var oldPos))
            {
                Fog.MarkRangeInvisible(oldPos.x, oldPos.y, 2);
            }

            cachedPos = (mousePositionX, mousePositionY);
            Fog.MarkRangeVisible(mousePositionX, mousePositionY, 2);
            Fog.MarkRangeExplored(mousePositionX, mousePositionY, 2);
        }

        public void AddSettlement(Settlement s)
        {
            Fog.MarkRangeExplored(s.Location.X, s.Location.Y, 2);
            Fog.MarkRangeVisible(s.Location.X, s.Location.Y, 2);
        }
        
    }
}