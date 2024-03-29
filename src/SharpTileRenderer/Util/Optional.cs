﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SharpTileRenderer.Util
{ 
    public readonly struct OptionalEmptyPlaceholder {}
    
    public static class Optional
    {
        public static Optional<T> Empty<T>()
        {
            return new Optional<T>(false, default);
        }

        public static OptionalEmptyPlaceholder Empty()
        {
            return new OptionalEmptyPlaceholder();
        }

        public static Optional<T> ValueOf<T>(T value)
        {
            return new Optional<T>(true, value);
        }

        public static Optional<T> OfNullable<T>(T? value) where T: struct
        {
            if (!value.HasValue)
            {
                return Empty();
            }

            return new Optional<T>(true, value.Value);
        }

        public static Optional<T> OfNullable<T>(T? value) where T: class
        {
            if (value == null)
            {
                return Empty();
            }

            return new Optional<T>(true, value);
        }
    }

    
    [DataContract]
    public readonly struct Optional<T> : IEquatable<Optional<T>>, IEnumerable<T>
    {
        [DataMember(Order=1)]
        readonly T? value;

        internal Optional(bool hasValue, T? value)
        {
            this.HasValue = hasValue;
            this.value = value;
        }

        public bool TryGetValue([MaybeNullWhen(false)] out T v)
        {
            v = this.value;
            return HasValue;
        }

        [DataMember(Order = 0)]
        public bool HasValue { get; }

        [IgnoreDataMember]
        public T Value => HasValue ? value! : throw new InvalidOperationException(); 
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (HasValue)
            {
                yield return value!;
            }
        }

        public override string ToString()
        {
            if (HasValue)
            {
                return $"Some({value})";
            }
            else
            {
                return "None";
            }
        }

        public Optional<TOther> Cast<TOther>()
        {
            if (!HasValue) return default;
            
            object ret = value!;
            return Optional.ValueOf<TOther>((TOther)ret);
        }
        
        public static implicit operator Optional<T>(T data)
        {
            return Optional.ValueOf(data);
        }

        public bool Equals(Optional<T> other)
        {
            return HasValue == other.HasValue && EqualityComparer<T>.Default.Equals(value!, other.value!);
        }

        public bool Equals(T other)
        {
            return HasValue && EqualityComparer<T>.Default.Equals(value!, other);
        }

        public override bool Equals(object? obj)
        {
            return obj is Optional<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                if (HasValue)
                {
                    return (EqualityComparer<T>.Default.GetHashCode(value!) * 397) ^ HasValue.GetHashCode();
                }

                return HasValue.GetHashCode();
            }
        }

        public T GetOrElse(T t) => HasValue ? value! : t;
        public T GetOrElse(Func<T> ft) => HasValue ? value! : ft();

        public static bool operator ==(Optional<T> left, T right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Optional<T> left, T right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(Optional<T> left, Optional<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Optional<T> left, Optional<T> right)
        {
            return !left.Equals(right);
        }

        public static implicit operator Optional<T>(OptionalEmptyPlaceholder p)
        {
            return default;
        }
    }
}