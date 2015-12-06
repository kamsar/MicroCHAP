# MicroCHAP

A tiny tool to allow authentication between servers or automated deployment tools using HMACSHA512 and CHAP protocols.

This is advantageous because the share secret never travels over the wire, and the MAC enables the target server to be sure that the message is genuine and signed using the shared secret.

The use of CHAP also makes replaying requests mostly useless as challenges are only usable once and the HMAC is never the same due to the use of the challenge in the reply.

IoC is recommended when using this tool. The `IChapServer` instance should be a singleton scoped instance so that challenges can be kept track of.

## How does MicroCHAP work?

MicroCHAP is an interaction between a client and a server to authenticate an action. The client and server have a shared secret value (e.g. long random string) that they both know.

1. The client requests a 'challenge' from the server. This is a unique value used to authenticate the request.
2. The client creates an authentication signature based on SHA512(challenge|sharedSecret|requestUrl). Because of the unique challenge value this signature changes for every request even to the same URL.
3. The client makes a HTTP request to the server that requires authentication, passing the signature in the `X-MC-MAC` HTTP header and the challenge in the `X-MC-Nonce` HTTP header.
4. The server receives the request and calculates the expected signature for the request based on the incoming URL and nonce header. If the expected signature matches the received signature, the request is authenticated.

MicroCHAP does not do any sort of session management or token lifetime. Each challenge is valid for only a single request and multiple authenticated requests require multiple handshakes. In other words this is designed for low volume scenarios like authenticating deployment tools as opposed to authenticating multi-user human requests.