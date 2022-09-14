using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpTileRenderer.RPG.Base.Util
{
    public static class ListExtensions
    {
        public static T ChooseRandomElement<T>(this IEnumerable<T> t, Random rnd)
        {
            var r = t.ToArray();
            return r[rnd.Next(r.Length)];
        }
    }
}