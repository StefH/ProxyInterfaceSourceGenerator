using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyInterfaceConsumer.Http;

public static class IHttpClientExtensions
{
    #region PostAsJsonAsync
    public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(
        this IHttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri,
        TValue value,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.PostAsJsonAsync(requestUri, value, options, cancellationToken);
    }

    public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(
        this IHttpClient client,
        Uri? requestUri,
        TValue value,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.PostAsJsonAsync(requestUri, value, options, cancellationToken);
    }

    public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(
        this IHttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri,
        TValue value,
        CancellationToken cancellationToken
    )
    {
        return client._Instance.PostAsJsonAsync(
            requestUri,
            value,
            options: null,
            cancellationToken
        );
    }

    public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(
        this IHttpClient client,
        Uri? requestUri,
        TValue value,
        CancellationToken cancellationToken
    )
    {
        return client._Instance.PostAsJsonAsync(
            requestUri,
            value,
            options: null,
            cancellationToken
        );
    }

    public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(
        this IHttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri,
        TValue value,
        JsonTypeInfo<TValue> jsonTypeInfo,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.PostAsJsonAsync(requestUri, value, jsonTypeInfo, cancellationToken);
    }

    public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(
        this IHttpClient client,
        Uri? requestUri,
        TValue value,
        JsonTypeInfo<TValue> jsonTypeInfo,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.PostAsJsonAsync(requestUri, value, jsonTypeInfo, cancellationToken);
    }
    #endregion

    #region GetFromJsonAsync
    public static Task<object?> GetFromJsonAsync(
        this IHttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri,
        Type type,
        JsonSerializerOptions? options,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.GetFromJsonAsync(requestUri, type, options, cancellationToken);
    }

    public static Task<object?> GetFromJsonAsync(
        this IHttpClient client,
        Uri? requestUri,
        Type type,
        JsonSerializerOptions? options,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.GetFromJsonAsync(requestUri, type, options, cancellationToken);
    }

    public static Task<TValue?> GetFromJsonAsync<TValue>(
        this IHttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri,
        JsonSerializerOptions? options,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.GetFromJsonAsync<TValue>(requestUri, options, cancellationToken);
    }

    public static Task<TValue?> GetFromJsonAsync<TValue>(
        this IHttpClient client,
        Uri? requestUri,
        JsonSerializerOptions? options,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.GetFromJsonAsync<TValue>(requestUri, options, cancellationToken);
    }

    public static Task<object?> GetFromJsonAsync(
        this IHttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri,
        Type type,
        JsonSerializerContext context,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.GetFromJsonAsync(requestUri, type, context, cancellationToken);
    }

    public static Task<object?> GetFromJsonAsync(
        this IHttpClient client,
        Uri? requestUri,
        Type type,
        JsonSerializerContext context,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.GetFromJsonAsync(requestUri, type, context, cancellationToken);
    }

    public static Task<TValue?> GetFromJsonAsync<TValue>(
        this IHttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri,
        JsonTypeInfo<TValue> jsonTypeInfo,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.GetFromJsonAsync(requestUri, jsonTypeInfo, cancellationToken);
    }

    public static Task<TValue?> GetFromJsonAsync<TValue>(
        this IHttpClient client,
        Uri? requestUri,
        JsonTypeInfo<TValue> jsonTypeInfo,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.GetFromJsonAsync(requestUri, jsonTypeInfo, cancellationToken);
    }

    public static Task<object?> GetFromJsonAsync(
        this IHttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri,
        Type type,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.GetFromJsonAsync(
            requestUri,
            type,
            options: null,
            cancellationToken
        );
    }

    public static Task<object?> GetFromJsonAsync(
        this IHttpClient client,
        Uri? requestUri,
        Type type,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.GetFromJsonAsync(
            requestUri,
            type,
            options: null,
            cancellationToken
        );
    }

    public static Task<TValue?> GetFromJsonAsync<TValue>(
        this IHttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.GetFromJsonAsync<TValue>(requestUri, cancellationToken);
    }

    public static Task<TValue?> GetFromJsonAsync<TValue>(
        this IHttpClient client,
        Uri? requestUri,
        CancellationToken cancellationToken = default
    )
    {
        return client._Instance.GetFromJsonAsync<TValue>(requestUri, cancellationToken);
    }
    #endregion
}
