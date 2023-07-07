using System.Security.Claims;

namespace Extensions;

public static class ListExtensions
{
    /// <summary>
    /// Checks if the collection contains element satisfying the predicate
    /// </summary>
    /// <param name="list"></param>
    /// <param name="pred"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>true, if collection contains such element, otherwise false</returns>
    public static bool Contains<T>(this IEnumerable<T> list, Func<T, bool> pred)
        => list.FirstOrDefault(pred) is not null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool EqualAsSets<T>(List<T> a, List<T> b)
    {
        if (a.Count != b.Count)
        {
            return false;
        }

        return a.All(b.Contains) && b.All(a.Contains);
    }

    public static bool ContainsClaims(this IEnumerable<Claim> claims, params string[] types)
    {
        var array = claims.ToArray();
        return types.All(t => array.Contains(c => c.Type == t));
    }

    public static bool Remove<T>(this List<T> list, Func<T, bool> predicate)
    {
        return list.Contains(predicate) && list.Remove(list.First(predicate));
    }
}

public static class List
{
    public static List<T> Of<T>(params T[] values)
    {
        return new List<T>(values);
    }
}