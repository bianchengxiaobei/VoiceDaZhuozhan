using System.Collections.Generic;

namespace Tangzx.ABSystem
{
    /// <summary>
    /// ��UGUIԴ����Ų������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ListPool<T>
    {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, l => l.Clear());

        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }

    public static class HashSetPool<T>
    {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<HashSet<T>> s_ListPool = new ObjectPool<HashSet<T>>(null, l => l.Clear());

        public static HashSet<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(HashSet<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }
}
