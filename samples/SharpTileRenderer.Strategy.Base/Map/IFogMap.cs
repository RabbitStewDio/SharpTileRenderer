namespace SharpTileRenderer.Strategy.Base.Map
{
    public interface IFogMap: IMap2D<FogState>
    {
        void MarkExplored(int x, int y);
        void MarkUnexplored(int x, int y);
        void MarkRangeExplored(int x, int y, int radius);
        void MarkRangeUnexplored(int x, int y, int radius);
        void UpdateExplored(int x, int y, bool val);

        void MarkVisible(int x, int y);
        void MarkInvisible(int x, int y);
        void MarkRangeVisible(int x, int y, int radius);
        void MarkRangeInvisible(int x, int y, int radius);
        void UpdateVisible(int x, int y, short val);
    }
}