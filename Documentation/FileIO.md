# FileIO
If you're reading this, you almost certainly have data stored in a Synergy data file of some kind. And if you expose this data as a [DataObject](DataObject.md), you'll need a way to read, write, update, and delete records in your files. This is where `Harmony.Core.FileIO.DataObjectIOBase<T>` comes in. With this class, we've implemented all the glue that's needed between a Synergy record in memory and a usable DataObject. Using `DataObjectIOBase` as a starting point, we made `Harmony.Core.FileIO.IsamDataObjectIO<T>` with a default implementation that should work for most Synergy data. However, if you already have routines that perform file I/O operations along with validation or other business logic, you can implement your own I/O class. We've included a basic example [here](QuickStart/CustomFileIO.md) to show you how to replace our default file I/O  routines with those you already use in your application.

### Thread Safety
`IsamDataObjectIO` is not inherently thread safe. Internally it uses a single file channel to perform its file I/O operations, so multiple concurrent operations performed on a single `IsamDataObjectIO` will result in exceptions at runtime. And multiple interleaved but non-concurrent requests can ruin the file channel's position and locking status. So in general, we recommend that you create an `IsamDataObjectIO` object when you need it, use it for a short time for a single logical unit of work, and then dispose of it when you're finished. Because the class internally uses [IFileChannelManager](Reference/IFileChannelManager.md), creating and destroying `IsamDataObjectIO` objects is very cheap.

### Error Handling
`IsamDataObjectIO` includes default processing for file I/O errors like [Key Not Same](http://docs.synergyde.com/index.htm#tools/toolsChap5Synergydbmserrors.htm). But if you need to run custom code when a particular error occurs, you can inherit from `IsamDataObjectIO` and overload one of the following handling methods:
* OnEOF - Invoked when the end of the file is reached.
* OnRecordLocked - Invoked when the record your program is trying to lock is already locked.
* OnKeyNotFound - Invoked if your program is trying to look up a record by key but the record doesn't exist or the program has read past it.
* OnDuplicateKey - Invoked if your program attempts to add a record to a file, but the file already contains a record with the same value for a key that is not marked as allowing duplicates.
* OnNoCurrentRecord - Invoked if your program attempts to write a record without locking it first.
* OnRecordNotSame - Invoked if your program attempts to update a record using its [GRFA](OptimisticConcurrency.md) when the record has been changed since the program last read it.
* OnFileIOException - Invoked for general Synergy file I/O exceptions that are not otherwise covered by the other handlers.
* OnException - Invoked for exceptions that aren't related to file I/O.

## Example Usage

### Reading
### Writing
### Updating
### Creating
### Finding
