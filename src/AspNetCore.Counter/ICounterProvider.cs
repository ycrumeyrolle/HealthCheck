namespace AspNetCore.Counter
{
    public interface ICounterProvider
    {
        ICounter GetCounter(string name, bool distributed);
    }
}