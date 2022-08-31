namespace SharpTileRenderer.TileMatching.Model.DataSets
{
    public interface IQuantifiedDataSetModel: IDataSetModel
    {
        public int DefaultQuantity { get; set; }
    }
}