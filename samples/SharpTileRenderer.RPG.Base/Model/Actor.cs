using SharpTileRenderer.Navigation;
using SharpTileRenderer.RPG.Base.Util;
using SharpTileRenderer.Util;
using System;
using System.Numerics;

namespace SharpTileRenderer.RPG.Base.Model
{
    /// <summary>
    ///   Something that can move. This demo is extra simple - no movement checks, no collisions, just floating in the direction of the target.
    /// </summary>
    public class Actor
    {
        public readonly NPCRuleElement RuleData;
        readonly Random rnd;
        Vector2 position;
        Optional<Vector2> movementTarget;

        public Actor(NPCRuleElement ruleData, Vector2 position, Random rnd)
        {
            RuleData = ruleData ?? throw new ArgumentNullException(nameof(ruleData));
            this.position = position;
            this.rnd = rnd;
        }

        public ContinuousMapCoordinate Position => new ContinuousMapCoordinate(position.X, position.Y);
        
        public void Update(DungeonGameTime time, DungeonGameData data)
        {
            if (!movementTarget.TryGetValue(out var target))
            {
                movementTarget = new Vector2(rnd.Next(data.TerrainWidth), rnd.Next(data.TerrainHeight));
                return;
            }


            var direction = new Vector2(target.X - position.X, target.Y - position.Y);
            if (direction.Length() < 0.01)
            {
                movementTarget = default;
                return;
            }

            var positionChange = Vector2.Normalize(direction) * time.DeltaTime * RuleData.WalkingSpeed;
            position += positionChange;
        }
    }
}