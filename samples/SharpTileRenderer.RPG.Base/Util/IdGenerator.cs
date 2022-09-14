namespace SharpTileRenderer.RPG.Base.Util
{
    public class IdGenerator
    {
        int id;

        public IdGenerator(int id = 0)
        {
            this.id = id;
        }

        public int Next()
        {
            var retval = id;
            id += 1;
            return retval;
        }
    }
}
