using LibraryPlayground.Pdf;
using LibraryPlayground.Pdf.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(static s =>
    {
        s.AddSingleton<PdfJuggler>();
        s.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();