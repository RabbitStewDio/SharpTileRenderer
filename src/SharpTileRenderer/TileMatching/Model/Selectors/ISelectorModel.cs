using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SharpTileRenderer.TileMatching.Model.Selectors
{
    public interface ISelectorModel: IEquatable<ISelectorModel>, INotifyPropertyChanged
    {
        public string Kind { get; }
        public bool IsQuantifiedSelector { get; }
        public IReadOnlyList<ISelectorModel> ChildSelectors { get; }
    }
}