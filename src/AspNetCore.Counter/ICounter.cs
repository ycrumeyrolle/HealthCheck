namespace AspNetCore.Counter
{
    public interface ICounter
    {
        string Name { get; }

        long Decrement(long decrementBy);

        long Increment(long incrementBy);

        long Value { get; set; }

        void Reset();
    }
}