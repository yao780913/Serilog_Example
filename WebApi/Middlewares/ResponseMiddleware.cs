namespace WebApi.Middlewares;

public sealed class ResponseMiddleware
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;

    public ResponseMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _logger = serviceProvider.GetRequiredService<ILogger<ResponseMiddleware>>();
    }

    public async Task Invoke(HttpContext context)
    {
        var bodyStream = context.Response.Body;
        var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        await _next(context);

        if (context.Request.Path.HasValue && context.Request.Path.Value.Contains("api"))
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var recordContent = await new StreamReader(responseBodyStream).ReadToEndAsync();
            _logger.LogInformation("Response: {RecordContent}", recordContent);
        }

        responseBodyStream.Seek(0, SeekOrigin.Begin);
        await responseBodyStream.CopyToAsync(bodyStream);
    }
}