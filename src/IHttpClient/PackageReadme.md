## Info

This project uses source generation to generate an `IHttpClient` interface and `HttpClientProxy` from the `HttpClient` to make it injectable and unit-testable.

All the methods and properties from the `HttpClient` are replicated to `IHttpClient`.

## Usage
``` c#
HttpClient httpClient = new HttpClient();
IHttpClient httpClientProxy = new HttpClientProxy(httpClient); 

var result = await httpClientProxy.GetAsync("https://www.google.nl");
var todo = await httpClientProxy.GetFromJsonAsync<Todo>("https://jsonplaceholder.typicode.com/todos/1");
var postResult = await httpClientProxy.PostAsJsonAsync("https://jsonplaceholder.typicode.com/todos", new Todo { Id = 123 });
var patchResult = await httpClientProxy.PatchAsJsonAsync("https://jsonplaceholder.typicode.com/todos/1", new Todo { Id = 400 });
var putResult = await httpClientProxy.PutAsJsonAsync("https://jsonplaceholder.typicode.com/todos/1", new Todo { Id = 444 });
```