# MicroCHAP

A tiny tool to allow authentication between servers or automated deployment tools using HMACSHA512 and CHAP protocols.

This is advantageous because the share secret never travels over the wire, and the MAC enables the target server to be sure that the message is genuine and signed using the shared secret.

The use of CHAP also makes replaying requests mostly useless as challenges are only usable once and the HMAC is never the same due to the use of the challenge in the reply.

IoC is recommended when using this tool. The `IChapServer` instance should be a singleton scoped instance so that challenges can be kept track of.