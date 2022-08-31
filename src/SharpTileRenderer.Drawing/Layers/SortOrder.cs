using System.Collections.Generic;
using SharpTileRenderer.TileMatching.Model.EntitySources;
using System;

namespace SharpTileRenderer.Drawing.Layers
{
    public static class SortOrder
    {
        public static IComparer<ScreenRenderInstruction<TEntity>> AsComparer<TEntity>(this RenderingSortOrder renderSortOrder) =>
            renderSortOrder switch
            {
                RenderingSortOrder.TopDownLeftRight => SortOrder<TEntity>.TopDownLeftRight,
                RenderingSortOrder.TopDownRightLeft => SortOrder<TEntity>.TopDownRightLeft,
                RenderingSortOrder.BottomUpLeftRight => SortOrder<TEntity>.BottomUpLeftRight,
                RenderingSortOrder.BottomUpRightLeft => SortOrder<TEntity>.BottomUpRightLeft,
                _ => throw new ArgumentException()
            };

    }
    
    public static class SortOrder<TEntity>
    {
        public static readonly IComparer<ScreenRenderInstruction<TEntity>> TopDownLeftRight = new TopDownLeftRightComparer();
        public static readonly IComparer<ScreenRenderInstruction<TEntity>> TopDownRightLeft = new TopDownRightLeftComparer();
        public static readonly IComparer<ScreenRenderInstruction<TEntity>> BottomUpLeftRight = new BottomUpLeftRightComparer();
        public static readonly IComparer<ScreenRenderInstruction<TEntity>> BottomUpRightLeft = new BottomUpRightLeftComparer();

        class TopDownLeftRightComparer : IComparer<ScreenRenderInstruction<TEntity>>
        {
            public int Compare(ScreenRenderInstruction<TEntity> a, ScreenRenderInstruction<TEntity> b)
            {
                return a.RenderPosition.Y.CompareTo(b.RenderPosition.Y) switch
                {
                    < 0 => -1,
                    > 0 => +1,
                    _ => a.RenderPosition.X.CompareTo(b.RenderPosition.X) switch
                    {
                        < 0 => -1,
                        > 0 => +1,
                        _ => a.RenderOrder.CompareTo(b.RenderOrder)
                    }
                };
            }
        }

        class TopDownRightLeftComparer : IComparer<ScreenRenderInstruction<TEntity>>
        {
            public int Compare(ScreenRenderInstruction<TEntity> a, ScreenRenderInstruction<TEntity> b)
            {
                return a.RenderPosition.Y.CompareTo(b.RenderPosition.Y) switch
                {
                    < 0 => -1,
                    > 0 => +1,
                    _ => a.RenderPosition.X.CompareTo(b.RenderPosition.X) switch
                    {
                        < 0 => +1,
                        > 0 => -1,
                        _ => a.RenderOrder.CompareTo(b.RenderOrder)
                    }
                };
            }
        }
        
        class BottomUpLeftRightComparer : IComparer<ScreenRenderInstruction<TEntity>>
        {
            public int Compare(ScreenRenderInstruction<TEntity> a, ScreenRenderInstruction<TEntity> b)
            {
                return a.RenderPosition.Y.CompareTo(b.RenderPosition.Y) switch
                {
                    < 0 => +1,
                    > 0 => -1,
                    _ => a.RenderPosition.X.CompareTo(b.RenderPosition.X) switch
                    {
                        < 0 => -1,
                        > 0 => +1,
                        _ => a.RenderOrder.CompareTo(b.RenderOrder)
                    }
                };
            }
        }

        class BottomUpRightLeftComparer : IComparer<ScreenRenderInstruction<TEntity>>
        {
            public int Compare(ScreenRenderInstruction<TEntity> a, ScreenRenderInstruction<TEntity> b)
            {
                return a.RenderPosition.Y.CompareTo(b.RenderPosition.Y) switch
                {
                    < 0 => +1,
                    > 0 => -1,
                    _ => a.RenderPosition.X.CompareTo(b.RenderPosition.X) switch
                    {
                        < 0 => +1,
                        > 0 => -1,
                        _ => a.RenderOrder.CompareTo(b.RenderOrder)
                    }
                };
            }
        }
    }
}