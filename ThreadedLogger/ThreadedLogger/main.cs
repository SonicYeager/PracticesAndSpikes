using System;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace ThreadedLogger
{
    public static class App
    {
        public static IEnumerable<string> Args { get; set; } = new List<string>();

        public static IMapper Mapper { get; set; }

        private static IServiceProvider ServiceProvider { get; set; }
        public static IThreadedLogger ThreadedLogger { get; set; }
        public static IEnumerable<IDoSomethingWorker> DoSomethingWorkers { get; set; } = new List<IDoSomethingWorker>();

        public static int Main(string[] argv)
        {
            Args = new List<string>(argv);

            ConfigureServices();
            ConfigureAutoMapper();

            DataPack dataPack = new DataPack() 
            { CalledCount = 3, Hash = 123456, Messages = new List<double>() { 2.00, 5.67, 8.432 } };

            MessagePack messagePack = Mapper.Map<MessagePack>(dataPack);

            ThreadedLogger = ServiceProvider.GetService<IThreadedLogger>();

            for (int i = 0; i < 10000; ++i)
            {
                ((List<IDoSomethingWorker>)DoSomethingWorkers).Add(ServiceProvider.GetService<IDoSomethingWorker>());
            }
            foreach (var item in DoSomethingWorkers)
            {
                item.Begin();
            }

            Console.ReadLine();

            foreach (var item in DoSomethingWorkers)
            {
                item.End();
            }
            
            return 0;
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IThreadedLogger, ThreadedLogger>();
            services.AddTransient<IDoSomethingWorker, DoSomethingWorker>();

            ServiceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureAutoMapper()
        {
            var config = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<DataPack, MessagePack>();
                    cfg.CreateMap<MessagePack, DataPack>();
                });

            config.AssertConfigurationIsValid();

            Mapper = config.CreateMapper();
        }
    }
}