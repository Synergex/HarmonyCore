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
Scalar Primatives, meaning a, d, i, id, are passed as a Json object with the following format. `PassedValue` is a quoted string for a, d, id and a number for i, d, id. If a numeric value is encoded as a quoted string it should not contain a decimal point. `DataType` is based on the synergy defined data type values. Alpha is 1, Decimal is 2, Implied Decimal is 4 Integer is 8. We've added addtional data types to the DataType enum in `TraditionalBridge\FieldDataDefinition.dbl`
```
{
    "PassedValue":"123456789123456789",
    "DataType":4,
    "ElementSize":10,
    "DecimalPrecision":10
}
```

Primative Collections are encoded similarly, but instead of being a scalar value, `PassedValue` should be an array. It is permissable to put the quoted string or number directly in this array. This is the advised pattern, however you can also put a JSON Object with an unencoded `Value` Property or a Base64 Encoded `Base64Value`.

```
{
    "PassedValue":["12345", "12345", "12345"],
    "DataType":2,
    "ElementSize":5
}
```

Scalar Complex types, meaining synergy structures and DataObjects are passed similarly to primatives with one significant change, the value passed for `PassedValue` fits the following format. `Value` is the contents of a non-binary synergy record/structure area. `Base64Value` is a base64 encoded synergy record/structure. `GRFA` is a base64 encoded grfa or rfa depending on the size of it.

```
{
    "DataType":"StructureName",
    "Value": "SynergyRecordData",
    "Base64Value":"EncodedRecordData"
    "GRFA":"Base64EncodedGRFA"
}
```

Collections of Complex types are encoded as follows

```
{
    "PassedValue":[
        {
            "DataType":"StructureName",
            "Value": "SynergyRecordData",
            "Base64Value":"EncodedRecordData"
            "GRFA":"Base64EncodedGRFA"
        },
        {
            "DataType":"StructureName",
            "Value": "SynergyRecordData",
            "Base64Value":"EncodedRecordData"
            "GRFA":"Base64EncodedGRFA"
        }
    ],
    "DataType":16,
    "ElementSize":10
}
```
## Return Parameter Encoding
Below is an example of the return JSON object for a method that returns a structure and the second argument is a collection of structures.

```
{
    "IsError":false,
    "Result": {
        "ReturnParameters":[
            {
                "Position":0,
                "Value":{
                    "DataType":"StructureName",
                    "Value": "SynergyRecordData",
                    "Base64Value":"EncodedRecordData"
                    "GRFA":"Base64EncodedGRFA"
                }
            },
            {
                "Position":2,
                "Value":{
                    "PassedValue":[
                        {
                            "DataType":"StructureName",
                            "Value": "SynergyRecordData",
                            "Base64Value":"EncodedRecordData"
                            "GRFA":"Base64EncodedGRFA"
                        },
                        {
                            "DataType":"StructureName",
                            "Value": "SynergyRecordData",
                            "Base64Value":"EncodedRecordData"
                            "GRFA":"Base64EncodedGRFA"
                        }
                    ]
                }
            }
        ],
        "DataType":16,
        "ElementSize":10
    }
}
```
Return parameter encoding for primatives is the same as it is on the way in. So if we had an alpha out parameter at position 1, the parameter JSON object would look like this.
```
{
   "Position":1,
   "Value":{
        "PassedValue":"this is alpha data",
        "DataType":1,
        "ElementSize":18
    } 
}
```

## Errors
When an unhandled error is encountered it will be trapped by the dispatcher library. If an error is trapped by the dispatcher, the client will recive a JSON object with `IsError` set to true and the `Exception` property set to the string value that comes from calling `Exception.ToString` for the caught exception.