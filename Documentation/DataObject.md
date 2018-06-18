# DataObject
DataObject is the fundamental building block in HarmonyCore. DataObjects are usually generated (using CodeGen) from a structure in a [Synergy repository](http://docs.synergyde.com/index.htm#rps/rpsChap1Whatisrepository.htm), and they inherit from `Harmony.Core.DataObjectBase`. DataObjects wrap Synergy data so that it can be efficiently serialized and deserialized, and the metadata associated with DataObject enables instances to be created quickly from Synergy data. A DataObject keeps track of whether it has changed, and it keeps track of the [GRFA](http://docs.synergyde.com/index.htm#lrm/lrmChap4Recordfileaddressesrfas.htm) for the record from which it was read. GRFAs are useful for implementing [optimistic concurrency](OptimisticConcurrency.md) and are critical to implementing most operations in Entity Framework (EF) Core. 

`DataObjectBase` implements the IComparable interface methods that allow DataObject instances to be sorted. Each CodeGen-generated DataObject is optimized to ensure that these comparisons are as fast and type-accurate as possible. The .NET Framework is unaware of Synergy types such as alpha, implied decimal, etc., so DataObject provides the ability to sort on these unknown types.

[Converters](Converters.md) enable properties generated for DataObjects to be converted to and from underlying repository structure types. When building application components using Synergy .NET, you have access to the entire Microsoft .NET Framework. However, this framework is unaware of Synergy types such as alpha, decimal, and implied decimal. The classes within the Converters namespace provide the ability to convert between standard Synergy types and appropriate .NET types. For example, you can easily convert between Synergy alpha and .NET `String` types. Along with the simple converters, there are a number of advanced converters that enable you to arrange your Synergy data into types expected by modern .NET consuming code. For example, there are converters that convert between a Synergy decimal field and a `DateTime` type. Legacy systems often contain simple data that need to be converted to modern types—e.g., "Y"/"N" fields that need to be converted to a `Boolean` type. 

### DataObjectMetadata
Synergy record data is kept separately from the metadata generated about that record. This reduces the size of each DataObject instance and keeps the cost of construction low. `DataObjectMetadata` contains the lookups necessary to enable EF Core query models to be converted into runnable Synergy [Select](http://docs.synergyde.com/index.htm#lrm/lrmChap10SYNERGEXSYNERGYDESELECTSELECT.htm) operations. `DataObjectMetadata` also acts as a factory for `DataObjectBase` derivatives. The CodeGen template creates a static constructor for each DataObject that registers its metadata against a static type lookup in `DataObjectMetadata`. This allows us to implement generic IO handling classes like [FileIO](FileIO.md) and the [Harmony Core Entity FrameworkpProvider](EntityFramework.md) without using reflection, even though those classes have no information on the types they are working with until runtime.

### Generating a DataObject for Synergy .NET
[DataObject Template](../Templates/DataObject.tpl)

`sample command line for running codegen against a structure`

### Generating a DataObject for Traditional Synergy
[TraditionalDataObject Template](../Templates/TraditionalDataObject.tpl)

`sample command line for running codegen against a structure`