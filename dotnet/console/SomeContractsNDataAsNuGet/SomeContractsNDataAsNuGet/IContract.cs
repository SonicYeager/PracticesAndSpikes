namespace SomeContractsNDataAsNuGet
{
    public interface IContract
    {
        public event Action<string, double> onFinished;

        void DoSomething();
    }
}