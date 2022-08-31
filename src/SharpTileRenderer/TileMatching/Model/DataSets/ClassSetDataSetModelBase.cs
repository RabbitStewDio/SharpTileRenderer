using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model.DataSets
{
    public abstract class ClassSetDataSetModelBase : DataSetModelBase, IClassSetDataSetModel
    {
        [DataMember]
        string? defaultClass;
        
        [DataMember]
        public ObservableCollection<string> Classes { get; }

        public string? DefaultClass
        {
            get
            {
                return defaultClass;
            }
            set
            {
                if (value == defaultClass) return;
                defaultClass = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public abstract DataSetType Kind { get; }

        protected ClassSetDataSetModelBase()
        {
            Classes = new ObservableCollection<string>();
            var np = (INotifyPropertyChanged)Classes;
            np.PropertyChanged += OnClassDataSetChanged;
        }

        ~ClassSetDataSetModelBase()
        {
            var np = Classes as INotifyPropertyChanged;
            np.PropertyChanged -= OnClassDataSetChanged;
        }

        void OnClassDataSetChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Classes));
        }
    }
}