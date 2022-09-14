using SharpTileRenderer.RPG.Base.Model;
using System.Collections;
using System.Collections.Generic;

namespace SharpTileRenderer.RPG.Base
{
    public class DungeonGameRules
    {
        public DefinedActors Actors { get; }
        public DefinedItems Items { get; }
        public DefinedTerrains Terrains { get; }

        public DungeonGameRules()
        {
            Actors = new DefinedActors();
            Items = new DefinedItems();
            Terrains = new DefinedTerrains();
        }

        public class DefinedTerrains: IEnumerable<TerrainElement>
        {
            public DefinedTerrains()
            {                
                None = new TerrainElement("none", '+', "None");
                Grass = new TerrainElement("grass", ' ', "Grass", "grass");
                Stone = new TerrainElement("Stone", '_', "Stone", "stone");
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<TerrainElement> GetEnumerator()
            {
                yield return None;
                yield return Grass;
                yield return Stone;
            }

            public TerrainElement Stone { get; }

            public TerrainElement Grass { get; }

            public TerrainElement None { get;  }
        }

        public class DefinedItems: IEnumerable<ItemElement>
        {
            public DefinedItems()
            {                
                None = new ItemElement("none", ' ', "None");
                Wall = new ItemElement("wall", '#', "Wall", "structure.wall");
                Door_Open = new ItemElement("door", '[', "Door", "structure.door-open");
                Door_Closed = new ItemElement("door", ']', "Door", "structure.door-closed");
                Stairs = new ItemElement("stairs", '/', "Stairs", "structure.stairs");
                Chest = new ItemElement("chest", '%', "A Chest", "item.chest-closed");
                Bag = new ItemElement("bag", 'B', "A Bag", "bag");
                Gold = new ItemElement("gold", '$', "Some Gold", "gold");
                Sword = new ItemElement("sword", 'S', "A Sword", "sword");
                PlateArmor = new ItemElement("plate-armor", 'P', "A plate armor", "plate");
                Chainmail = new ItemElement("chainmail", 'C', "A chain mail", "chainmail");
                Food = new ItemElement("food", 'F', "Some food", "food");
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<ItemElement> GetEnumerator()
            {
                yield return None;
                yield return Wall;
                yield return Door_Open;
                yield return Door_Closed;
                yield return Stairs;
                yield return Chest;
                yield return Bag;
                yield return Gold;
                yield return Sword;
                yield return Chainmail;
                yield return PlateArmor;
                yield return Food;
            }

            public ItemElement Door_Open { get; }
            public ItemElement Door_Closed { get; }
            public ItemElement Stairs { get; }
            public ItemElement Bag { get; }
            public ItemElement Gold { get; }
            public ItemElement Sword { get; }
            public ItemElement Chainmail { get; }
            public ItemElement PlateArmor { get; }
            public ItemElement Food { get; }
            public ItemElement Wall { get; }

            public ItemElement Chest { get; }

            public ItemElement None { get;  }
        }

        public class DefinedActors: IEnumerable<NPCRuleElement>
        {
            
            public DefinedActors()
            {
                Player = new NPCRuleElement("player", '@', "You, the player", "player");
                Jester = new NPCRuleElement("jester", 'J', "A fool", "jester");
                Farmer = new NPCRuleElement("farmer", '@', "A farmer", "farmer");
                Drunkard = new NPCRuleElement("drunkard", '@', "A drunken man", "drunkard");
                Peasant = new NPCRuleElement("peasant", '@', "A peasant", "peasant");
                Fighter = new NPCRuleElement("fighter", '@', "A trained swordsman", "fighter");
                Thief = new NPCRuleElement("thief", '@', "A thief", "thief");
            }

            public NPCRuleElement Player { get; }
            public NPCRuleElement Jester { get; }
            public NPCRuleElement Farmer { get; }
            public NPCRuleElement Drunkard { get; }
            public NPCRuleElement Peasant { get; }
            public NPCRuleElement Fighter { get; }
            public NPCRuleElement Thief { get; }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<NPCRuleElement> GetEnumerator()
            {
                yield return Player;
                yield return Jester;
                yield return Farmer;
                yield return Drunkard;
                yield return Peasant;
                yield return Fighter;
                yield return Thief;
            }

        }
    }
}