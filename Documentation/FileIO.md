# FileIO
If you're reading this, you have data stored in an SDBMS file of some kind. In order to expose that data as a DataObject we need a way to read, write, update and delete records inside your files. This is where `Harmony.Core.FileIO.DataObjectIOBase<T>` comes in we've implemented all of the glue between a synergy record in memory and a usable DataObject. Using `DataObjectIOBase` as a base to work on we've made `Harmony.Core.FileIO.IsamDataObjectIO<T>`, a default implementation that we think will work for most users of SDBMS files. If however you have already got routines that do your file io operations and they perform validation or other buisness logic, you can implement your own DataObjectIO class. We've made a quick start [here](QuickStart/CustomFileIO.md) to show you how to replace our default file io routines with the ones you already use in the rest of your application.

### Thread safety
`IsamDataObjectIO` is not inherently thread safe. Internally it uses a single file channel to perform its file io operations. Multiple concurrent operations performed on a single `IsamDataObjectIO` will result in exceptions at runtime. Multiple interleaved but non concurrent requests can ruin the file channels position and locking status. In general it is recommended to create an `IsamDataObjectIO` object when you need it, use it for a short time for a single logical unit of work and dispose of it when you're finished. Because it internally uses [IFileChannelManager](Reference/IFileChannelManager.md), creating and destroying `IsamDataObjectIO` objects is very cheap.

### Error handling
If you're using `IsamDataObjectIO` we've implementing what we think are sane defaults for what to do when you get a file io error like [Key Not Same](http://docs.synergyde.com/index.htm#tools/toolsChap5Synergydbmserrors.htm). But If you need to run custom code in the event that a particular error occurs you can inherit from `IsamDataObjectIO` and overload one of our error handling methods. We support overriding the error handling behavior using the following methods on `IsamDataObjectIO`
* OnEOF - when we reach the end of a file
* OnRecordLocked - if the record you are trying to lock is already locked
* OnKeyNotFound - if you are trying to look up a record by key but the record doesn't exist or you have read past it
* OnDuplicateKey - if you attempt to add a record to a file, but the file already contains a record with the same value for a key that is not marked as allowing duplicates
* OnNoCurrentRecord - if you attempt to write a record but haven't locked it first
* OnRecordNotSame - if you attempt to update a record using its [GRFA](OptimisticConcurrency.md) but the record has been changed since you read it
* OnFileIOException - this occurs on a general synergy file io exception that is not otherwise covered by the other handlers
* OnException - this occurs for non file io related exceptions

## Example usage

### Reading
### Writing
### Updating
### Creating
### Finding
