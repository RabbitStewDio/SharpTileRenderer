using SharpTileRenderer.Drawing.ViewPorts;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing.Queries
{
    /// <summary>
    ///   A query planer computes the relationship between the visible area and the displayed map area.
    ///   For maps that have wrap around on any axis, more than one query plan can be returned at any time.
    ///   If multiple query plans are returned, the query should be treated as union of these separate areas.
    /// </summary>
    /// <para>
    ///   The resulting query 
    /// </para>
    public interface IQueryPlaner
    {
        List<QueryPlan> Plan(IViewPort v, List<QueryPlan>? results = null);
    }
}