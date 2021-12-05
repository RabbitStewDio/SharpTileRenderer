﻿namespace SharpTileRenderer.TexturePack.Grids
{
    public readonly struct GridTileDefinition
    {
        public GridTileDefinition(int gridX,
                                  int gridY,
                                  params string[] tags) :
            this($"{gridX}-{gridY}-{string.Join(",", tags)}", gridX, gridY, null, null, tags)
        { }

        public GridTileDefinition(string name,
                                  int gridX,
                                  int gridY,
                                  int? anchorX,
                                  int? anchorY,
                                  params string[] tags)
        {
            Name = name;
            GridX = gridX;
            GridY = gridY;
            AnchorX = anchorX;
            AnchorY = anchorY;
            Tags = new ReadOnlyListWrapper<string>(tags);
        }

        public GridTileDefinition(string tag) : this(tag, 0, 0, null, null, tag)
        { }

        public ReadOnlyListWrapper<string> Tags { get; }
        public string Name { get; }
        public int GridX { get; }
        public int GridY { get; }
        public int? AnchorX { get; }
        public int? AnchorY { get; }
    }
}
