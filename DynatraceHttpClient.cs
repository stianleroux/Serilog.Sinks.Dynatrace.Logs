namespace Serilog.Sinks.Dynatrace;

using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.Http;

/// <summary>
/// A Serilog HTTP sink client for sending logs to Dynatrace.
/// </summary>
public sealed class DynatraceHttpClient : IHttpClient, IDisposable
{
    private readonly HttpClient client;

    private bool disposed;

    /// <summary>
    /// Creates a new instance of <see cref="DynatraceHttpClient"/> with the given Dynatrace API token.
    /// </summary>
    /// <param name="accessToken">Dynatrace API token.</param>
    /// <exception cref="ArgumentException">Thrown if the access token is null or empty.</exception>
    public DynatraceHttpClient(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("Access token cannot be null or empty.", nameof(accessToken));

        this.client = new HttpClient();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Api-Token", accessToken);
    }

    /// <inheritdoc />
    public void Configure(IConfiguration configuration)
    {
        // Reserved for future use.
    }

    /// <inheritdoc />
    public async Task<HttpResponseMessage> PostAsync(string requestUri, Stream contentStream, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(requestUri))
            throw new ArgumentException("Request URI must not be null or empty.", nameof(requestUri));
        if (contentStream == null)
            throw new ArgumentNullException(nameof(contentStream));

        using var content = new StreamContent(contentStream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json")
        {
            CharSet = Encoding.UTF8.WebName
        };

        return await this.client.PostAsync(requestUri, content, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (this.disposed) return;
        this.client.Dispose();
        this.disposed = true;
        GC.SuppressFinalize(this);
    }
}
