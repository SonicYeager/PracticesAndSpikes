using SomeContractsNDataAsNuGet;

namespace TestWebApp
{
    public class Contract : IContract
    {
        public event Action<string, double> onFinished;

        public async void DoSomething()
        {
            await Task.Run(() => onFinished.Invoke("Kicker", 2.99));
        }
    }
}
