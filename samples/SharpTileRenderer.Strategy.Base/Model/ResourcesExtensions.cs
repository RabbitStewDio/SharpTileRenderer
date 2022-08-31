namespace SharpTileRenderer.Strategy.Base.Model
{
    public static class ResourcesExtensions
    {
        public static Resources Resource(int food, int production, int trade)
        {
            return new Resources(food, production, trade);
        }

        public static Resources Prod(int production)
        {
            return new Resources(0, production, 0);
        }

        public static Resources Food(int food)
        {
            return new Resources(food, 0, 0);
        }

        public static Resources Trade(int trade)
        {
            return new Resources(0, 0, trade);
        }

        public static ResourcesBoost Boost(float food, float production, float trade)
        {
            return new ResourcesBoost(food, production, trade);
        }

        public static ResourcesBoost ProdBoost(float production)
        {
            return new ResourcesBoost(0, production, 0);
        }

        public static ResourcesBoost FoodBoost(float food)
        {
            return new ResourcesBoost(food, 0, 0);
        }

        public static ResourcesBoost TradeBoost(float trade)
        {
            return new ResourcesBoost(0, 0, trade);
        }
    }
}