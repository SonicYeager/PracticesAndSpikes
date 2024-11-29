using Microsoft.Extensions.Hosting;

namespace LibraryPlayground.Pdf.Worker;

public sealed class Worker : IHostedService
{
    private readonly PdfJuggler _pdfJuggler;
    public Worker(PdfJuggler pdfJuggler)
    {
        _pdfJuggler = pdfJuggler;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _pdfJuggler.Generate();
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // No-op
    }
}