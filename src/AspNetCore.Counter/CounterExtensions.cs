namespace AspNetCore.Counter
{

    public static class CounterExtensions
    {
        public static long Increment(this ICounter counter)
        {
            return counter.Increment(1);
        }

        public static long Decrement(this ICounter counter)
        {
            return counter.Decrement(1);
        }
    }
}