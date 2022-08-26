using Microsoft.IO;
using Newtonsoft.Json;

namespace WebApi.Middlewares;

public sealed class RequestMiddleware
{
    private readonly ILogger<RequestMiddleware> _logger;
    private readonly RequestDelegate _next;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
    
    public RequestMiddleware(ILogger<RequestMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        await using var requestStream = _recyclableMemoryStreamManager.GetStream();
        await context.Request.Body.CopyToAsync(requestStream);

        var obj = new
        {
            Schema = context.Request.Scheme,
            Host = context.Request.Host.ToString(),
            Path = context.Request.Path.ToString(),
            PathBase = context.Request.PathBase.ToString(),
            QueryString = context.Request.QueryString.ToString(),
            Body = ReadStreamInChunks(requestStream),
        };
        
        _logger.LogInformation("Request: {Data}", JsonConvert.SerializeObject(obj));

        context.Request.Body.Position = 0;
        await _next(context);
    }

    private static string ReadStreamInChunks(Stream stream)
    {
        const int readChunkBufferLength = 4096;
        stream.Seek(0, SeekOrigin.Begin);
        using var textWriter = new StringWriter();
        using var reader = new StreamReader(stream);
        var readChunk = new char[readChunkBufferLength];
        int readChunkLength;
        do
        {
            readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
            textWriter.Write(readChunk, 0, readChunkLength);
        } while (readChunkLength > 0);
        return textWriter.ToString();
    }
}