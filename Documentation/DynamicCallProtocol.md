# Traditional Bridge Protocol

Traditional bridge uses a Length Prefixed JSON object protocol, this means that any time you send or receive a top level message, it will be prefixed by zero filled 10 digits of length. 

## Initial Handshake
In order to signal that the server process is ready and has opened the standard in/out channel, the server process writes `READY` followed by CRLF on all platforms. Once the client receives `READY\r\n` It sends a protocol challenge message in the form of a JSON object with a single member property `ProtocolVersion` containing the highest protocol the client can support. The Server will respond with a JSON object containing the member property `ProtocolSupported` with its value set to true if the protocol is supported. If the protocol is supported and has additional extended functionality, member properties can be returned here to indicate that. Currently there are no protocol extensions. If the Client supports additional lower protocol versions It can continue this negotiation process until it receives a `ProtocolSupported` member property that equals true.

## System Messages
### IsPingRequest
A message sent with an `IsPingRequest` member property set to true will cause the server to response with a JSON message with the `IsAlive` member property set to true. The intent of this message is to verify that the dispatcher is functioning and that nothing has gotten stuck or out of step. When regular requests have failed for some reason or we're returning a connection to a pool we may pass one of these messages to verify that things are still in working order.
### IsShutdownRequest
A message sent with it's `IsShutdownRequest` member property set to true will cause the server to gracefully shut down. Channels will be closed and destructors will be fired.

## User Messages
A message sent with a member property `Name` set to a routine name will either be handled by the dynamic dispatch or by code generated dispatchers. The path it takes is determined by the value of `Name` and the list of dispatchers that have been registered. 

## Dynamic Dispatch
Dynamic dispatch, meaning routine dispatch that has not been pre generated requires that passed in metadata is sufficient to populate an arguments list and make an RCB API call with it. This precludes dispatching of structures and synergy real/pseudo arrays. If you need to pass synergy structures to a routine for dynamic dispatch, the routine needs to be declared as taking a DataObject. For collections, an ArrayList should be used.

## Argument Encoding

## Return Parameter Encoding
