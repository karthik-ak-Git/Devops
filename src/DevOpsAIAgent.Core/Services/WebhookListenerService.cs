using System.Net;
using System.Text;
using System.Text.Json;
using CommunityToolkit.Mvvm.Messaging;
using DevOpsAIAgent.Core.Messages;
using DevOpsAIAgent.Core.Models;
using Microsoft.Extensions.Logging;

namespace DevOpsAIAgent.Core.Services;

public class WebhookListenerService : IWebhookListenerService
{
    private readonly ILogger<WebhookListenerService> _logger;
    private HttpListener? _listener;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _listenerTask;
    private bool _isRunning;

    private const string ListenerPrefix = "http://localhost:5000/cicd-webhook/";

    public bool IsRunning => _isRunning;

    public WebhookListenerService(ILogger<WebhookListenerService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_isRunning)
        {
            _logger.LogWarning("Webhook listener is already running");
            return Task.CompletedTask;
        }

        try
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(ListenerPrefix);
            _listener.Start();

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _isRunning = true;

            _listenerTask = Task.Run(() => ListenForRequestsAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);

            _logger.LogInformation("Webhook listener started on {Url}", ListenerPrefix);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start webhook listener");
            throw;
        }

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!_isRunning)
        {
            return;
        }

        _logger.LogInformation("Stopping webhook listener");

        _isRunning = false;
        _cancellationTokenSource?.Cancel();

        if (_listener != null)
        {
            _listener.Stop();
            _listener.Close();
        }

        if (_listenerTask != null)
        {
            try
            {
                await _listenerTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelling
            }
        }

        _logger.LogInformation("Webhook listener stopped");
    }

    private async Task ListenForRequestsAsync(CancellationToken cancellationToken)
    {
        while (_isRunning && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (_listener == null)
                {
                    break;
                }

                var context = await _listener.GetContextAsync().ConfigureAwait(false);
                _ = Task.Run(() => HandleRequestAsync(context, cancellationToken), cancellationToken);
            }
            catch (HttpListenerException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in webhook listener loop");
                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        try
        {
            if (context.Request.HttpMethod != "POST")
            {
                await SendResponseAsync(context.Response, HttpStatusCode.MethodNotAllowed, "Only POST requests are allowed").ConfigureAwait(false);
                return;
            }

            using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
            var json = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogDebug("Received webhook payload: {Json}", json);

            var payload = JsonSerializer.Deserialize<WebhookPayload>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (payload == null)
            {
                await SendResponseAsync(context.Response, HttpStatusCode.BadRequest, "Invalid payload").ConfigureAwait(false);
                return;
            }

            var cicdEvent = CiCdEvent.FromWebhookPayload(payload);

            if (cicdEvent.IsFailure)
            {
                _logger.LogInformation(
                    "Pipeline failure detected: Repository={Repository}, Workflow={Workflow}, Commit={Commit}",
                    cicdEvent.RepositoryName,
                    cicdEvent.WorkflowName,
                    cicdEvent.CommitHash);

                WeakReferenceMessenger.Default.Send(new PipelineFailedMessage(cicdEvent));
            }
            else
            {
                _logger.LogDebug("Received non-failure event: Status={Status}, Conclusion={Conclusion}",
                    cicdEvent.Status, cicdEvent.Conclusion);
            }

            await SendResponseAsync(context.Response, HttpStatusCode.OK, "Webhook received").ConfigureAwait(false);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize webhook payload");
            await SendResponseAsync(context.Response, HttpStatusCode.BadRequest, "Invalid JSON").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling webhook request");
            await SendResponseAsync(context.Response, HttpStatusCode.InternalServerError, "Internal error").ConfigureAwait(false);
        }
    }

    private static async Task SendResponseAsync(HttpListenerResponse response, HttpStatusCode statusCode, string message)
    {
        response.StatusCode = (int)statusCode;
        response.ContentType = "text/plain";

        var buffer = Encoding.UTF8.GetBytes(message);
        response.ContentLength64 = buffer.Length;

        await response.OutputStream.WriteAsync(buffer).ConfigureAwait(false);
        response.Close();
    }

    public void Dispose()
    {
        if (_isRunning)
        {
            StopAsync().GetAwaiter().GetResult();
        }

        _cancellationTokenSource?.Dispose();
        _listener?.Close();
    }
}
