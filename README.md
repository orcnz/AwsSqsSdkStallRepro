# Reproducing the AWSSDK (dotnet) SQS Stall Issue

The stalling issue occurs when _only_ the HTTP header bytes are received by the SDK and _none_ of the HTTP content bytes.

> This repository is based on the Flakey-Bit's initial reproduction of the issue here https://github.com/flakey-bit/aws-sqs-sdk-blocking-repro.

## General Example

### Server

This stalling behaviour is reproduced in the [SqsController Code](Server/Controllers/SqsController.cs).

The gist of it is outlined below.
```csharp
// Set the header information (ie expect 240 bytes in the content)
Response.Headers.ContentLength = bytes.Length;

// Send the header bytes
await Response.Body.FlushAsync();

// Never send the content bytes
await Task.Delay(Timeout.Infinite);
```

The issue occurs in the SDK because the code only awaits for the [response headers](https://github.com/aws/aws-sdk-net/blob/475822dec5e87954b7a47ac65995714ae1f1b115/sdk/src/Core/Amazon.Runtime/Pipeline/HttpHandler/_netstandard/HttpRequestMessageFactory.cs#L520) by passing in `HttpCompletionOption.ResponseHeadersRead`.

After that it gets the response content stream and passes it to [`XmlReader.Create()`](https://github.com/aws/aws-sdk-net/blob/6ca13905276f70299f9b17aac7a78b4dfe6653cb/sdk/src/Core/Amazon.Runtime/Internal/Transform/UnmarshallerContext.cs#L320) method which tries to read from the stream.
Since there are no received bytes for the content the method never returns.
Presumably this holds the HTTP connection open indefinitely.

### Client

The stalling behaviour can be reproduced in a general way with the following code.

```csharp
// Make a request and pass in the HttpCompletionOption.ResponseHeadersRead option to only wait for the headers
var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/");
var response = await new HttpClient().SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

// When we have headers then we can process the content. 
// In our example the following line never returns the content, it just waits indefinitely
// as there is no timeout mechanism in place.
var content = await response.Content.ReadAsStringAsync();
```


## Specific Example

A specific example of the stall using the `AWSSDK.SQS` can been seen in this repository.

To reproduce;
1. Start the Server project.
2. Start the Client project.

You will observe that the client call to `ReceiveMessageAsync()` never returns.
