namespace SharpTileRenderer.TileMatching
{
    public delegate TSourceTile MapQuery<out TSourceTile>(int x, int y);
}
