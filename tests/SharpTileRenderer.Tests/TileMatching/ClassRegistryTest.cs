using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.TileMatching;
using System;

namespace SharpTileRenderer.Tests.TileMatching
{
    [TestFixture]
    public class ClassRegistryTest
    {
        [Test]
        public void ValidateTagDefinitions()
        {
            var classRegistry = new EntityClassificationRegistry<EntityClassification32>();

            var classA = classRegistry.Register("A-Class");
            var classB = classRegistry.Register("B-Class");
            var classC = classRegistry.Register("C-Class");
            classA.Should().Be(new EntityClassification32(1));
            classB.Should().Be(new EntityClassification32(2));
            classC.Should().Be(new EntityClassification32(4));
        }

        [Test]
        public void ValidateDuplicateTagRegistration()
        {
            var classRegistry = new EntityClassificationRegistry<EntityClassification32>();

            var classA = classRegistry.Register("A-Class");
            classA.Should().Be(new EntityClassification32(1));
            classRegistry.Register("A-Class").Should().Be(classA);
        }

        [Test]
        public void ValidateLimits()
        {
            var defaultValue = default(EntityClassification32);
            var classRegistry = new EntityClassificationRegistry<EntityClassification32>();
            for (int i = 0; i < defaultValue.Cardinality; i += 1)
            {
                classRegistry.Register($"Class {i}").Should().NotBe(defaultValue);
            }

            classRegistry.Invoking(c => c.Register("out of limits")).Should().Throw<ArgumentException>();
        }
    }
}