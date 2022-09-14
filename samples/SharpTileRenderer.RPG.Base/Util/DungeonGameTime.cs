namespace SharpTileRenderer.RPG.Base.Util
{
    public readonly struct DungeonGameTime
    {
        public readonly float TimeElapsed;
        public readonly float DeltaTime;

        public DungeonGameTime(float timeElapsed, float deltaTime)
        {
            TimeElapsed = timeElapsed;
            DeltaTime = deltaTime;
        }
    }
}