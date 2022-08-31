using System;
using System.ComponentModel;

namespace SharpTileRenderer.TileMatching.Model.Selectors
{
    public interface ISelectorModel: IEquatable<ISelectorModel>, INotifyPropertyChanged
    {
        public string Kind { get; }
        public bool IsQuantifiedSelector { get; }
    }
}