using SharpTileRenderer.Navigation;

namespace SharpTileRenderer.Strategy.Base
{
    /// <summary>
    ///   This would be the game update handler that handles user input and updates the game data model accordingly.
    ///   In this cut-down demo we simply update the fog map to show where the mouse pointer is pointing at to
    ///   showcase our fancy fog of war system.
    /// </summary>
    public class StrategyGame
    {
        public readonly StrategyGameData GameData;
        public readonly StrategyGameFog FogData;

        public StrategyGame()
        {
            this.GameData = new StrategyGameData();
            this.FogData = new StrategyGameFog(GameData.TerrainWidth, GameData.TerrainHeight);

            // make all settlements visible.
            foreach (var s in GameData.Settlements)
            {
                this.FogData.AddSettlement(s);
            }
        }

        public NavigatorMetaData NavigatorConfig => NavigatorMetaData.FromGridType(GridType.Grid)
//                                                                     .WithHorizontalLimit(GameData.TerrainWidth)
//                                                                     .WithVerticalLimit(GameData.TerrainHeight);
                                                                     .WithHorizontalWrap(GameData.TerrainWidth)
                                                                     .WithVerticalWrap(GameData.TerrainHeight);
        
        public void Update(int mouseTileX, int mouseTileY)
        {
            this.FogData.UpdateMousePosition(mouseTileX, mouseTileY);
        }
    }
}