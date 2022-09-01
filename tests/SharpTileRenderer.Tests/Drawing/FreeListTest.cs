using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Drawing.Utils;
using System;

namespace SharpTileRenderer.Tests.Drawing
{
    public class FreeListTest
    {
        [Test]
        public void BasicUseTest()
        {
            var fl = new FreeList<int>();
            var ix0 = fl.Add(0);
            var ix1 = fl.Add(1);
            var ix2 = fl.Add(2);
            fl.TryGetValue(ix0, out var t1).Should().Be(true);
            t1.Should().Be(0);

            fl.TryGetValue(ix1, out var t2).Should().Be(true);
            t2.Should().Be(1);
            
            fl.TryGetValue(ix2, out var t3).Should().Be(true);
            t3.Should().Be(2);

            fl.Range.Should().Be(128);
            fl.Count.Should().Be(3);
            
            fl.Remove(ix0);
            fl.Count.Should().Be(2);

            var ix3 = fl.Add(3);
            ix3.Should().Be(ix0);
            
            fl.Add(4);
            fl.Count.Should().Be(4);
        }
    }
    
    public class SmartFreeListTest
    {
        struct Payload : ISmartFreeListElement<Payload>, IEquatable<Payload>
        {
            public bool Active;
            public int Data { get; private set; }
            public Payload AsFreePointer(FreeListIndex ptr)
            {
                return new Payload()
                {
                    Active = false,
                    Data = ptr.Value + 1
                };
            }

            public FreeListIndex FreePointer => FreeListIndex.Of(Data);

            public Payload(bool active, int freePointer)
            {
                Active = active;
                Data = freePointer;
            }

            public bool Equals(Payload other)
            {
                return Active == other.Active && Data == other.Data;
            }

            public override bool Equals(object? obj)
            {
                return obj is Payload other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Active.GetHashCode() * 397) ^ Data;
                }
            }

            public static bool operator ==(Payload left, Payload right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Payload left, Payload right)
            {
                return !left.Equals(right);
            }
        }

        [Test]
        public void BasicUseTest()
        {
            var fl = new SmartFreeList<Payload>();
            var ix0 = fl.Add(new Payload(true, 0));
            var ix1 = fl.Add(new Payload(true, 1));
            var ix2 = fl.Add(new Payload(true, 2));
            fl.TryGetValue(ix0, out var t1).Should().Be(true);
            t1.Should().Be(new Payload(true, 0));
            fl[ix0].Active.Should().BeTrue();

            fl.TryGetValue(ix1, out var t2).Should().Be(true);
            t2.Should().Be(new Payload(true, 1));
            fl[ix1].Active.Should().BeTrue();
            
            fl.TryGetValue(ix2, out var t3).Should().Be(true);
            t3.Should().Be(new Payload(true, 2));
            fl[ix2].Active.Should().BeTrue();

            fl.Range.Should().Be(128);
            fl.Count.Should().Be(3);
            
            fl.Remove(ix0);
            fl.Count.Should().Be(2);
            fl[ix0].Active.Should().BeFalse();

            var ix3 = fl.Add(new Payload(true, 3));
            ix3.Should().Be(ix0);
            
            fl.Add(new Payload(true, 4));
            fl.Count.Should().Be(4);
        }
    }
}