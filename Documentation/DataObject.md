# DataObject
DataObjects are the fundamental building block in HarmonyCore. DataObjects are usually CodeGenerated from a structure in a [Synergy Repository](http://docs.synergyde.com/index.htm#rps/rpsChap1Whatisrepository.htm) and they inherit from `Harmony.Core.DataObjectBase`. DataObjects wrap synergy data with the ability to efficiently serialize and deserialize. The Metadata that is associated with them allows us to quickly create them from SDMS. DataObjects keep track of whether they've been changed, and they also keep track of the [GRFA](http://docs.synergyde.com/index.htm#lrm/lrmChap4Recordfileaddressesrfas.htm) for the record they were read from. GRFA's are useful for implementing [Optimistic Concurrency](OptimisticConcurrency.md) and are critical to implementing most operations in Entity Framework Core. 

`DataObjectBase` Implements the IComparable interface methods that allow the DataObject instances to be sorted. Each code generated DataObject has additional implementation to ensure these comparisons are as fast and type accurate as possible. The .NET Framework is unaware of Synergy types such as Alpha, Implied Decimal, etc. so the DataObject provides the ability to sort on these unknown types.

The code generated properties for a DataObject convert to and from the underlying repository structure types using [Converters](Converters.md). When building components of your applications using Synergy .NET you have access to the whole Microsoft .NET Framework. However, this framework is unaware of the code Synergy types like alpha, decimal, and implied decimal. The classes within the Converters namespace provide the ability to convert between the standard Synergy types and the appropriate .NET types. For example, you can easily convert between a Synergy alpha and .NET `String` types. Together with the simple converters, there are also a number of advanced converters that allow you to arrange your Synergy data into the appropriate types expected by modern .NET consuming code. For example, there are converters that will convert between a Synergy decimal field and a `DateTime` type. Legacy systems often contain simple data that need organizing into modern types. For example converting "Y"/"N" fields into a `Boolean` type. 

### DataObjectMetadata
The actual synergy record data is kept separately from the metadata we generated about that record. This reduces the size of each DataObject instance and keeps the cost of construction low. `DataObjectMetadata` contains the necessary lookups so that we can convert EF Core query models into runnable Synergy [Select](http://docs.synergyde.com/index.htm#lrm/lrmChap10SYNERGEXSYNERGYDESELECTSELECT.htm) operations. `DataObjectMetadata` also acts as a factory for `DataObjectBase` derivatives. The CodeGen Template will create a static constructor for each DataObject that registers its metadata against a static type lookup in `DataObjectMetadata`. This allows us to implement generic IO handling classes like [FileIO](FileIO.md) and the [Harmony Core Entity Framework Provider](EntityFramework.md) without using reflection, even though those classes have no idea what types they're working on until runtime.

### Code Generating a DataObject
[DataObject Template](../Templates/DataObject.tpl)

`sample command line for running codegen against a structure`

### Code Generating a DataObject for Traditional Synergy
[TraditionalDataObject Template](../Templates/TraditionalDataObject.tpl)

`sample command line for running codegen against a structure`