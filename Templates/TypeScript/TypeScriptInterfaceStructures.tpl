<CODEGEN_FILENAME><INTERFACE_NAME>Structures.ts</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.7.1</REQUIRES_CODEGEN_VERSION>
;//
;// This template generates TypeScript interfaces corresponding to the data model classes
;// that are used for method parameter definitions in a method catalog.
;//
;// All structures that are defined in the method calatalog will be processed, regardless
;// of which interfaces they are referenced by.
;//
;// To process this template, you must use the following CodeGen command line options:
;//
;//   codegen -smcstrs <smcXmlFile> -ms
;//
;//
/*
  THIS CODE WAS CREATED BY A TOOL AND SHOULD NOT BE EDITED.
  ANY CHANGES WILL BE OVERWRITTEN THE NEXT TIME THE TOOL IS USED!

  This code defines the complex types related to the operations
  in the exposed service "<INTERFACE_NAME>".
*/
<STRUCTURE_LOOP>
/*
  Stucture:    <STRUCTURE_NAME>
  Description: <STRUCTURE_DESC>
*/
export interface <StructureNoplural> {
  <FIELD_LOOP>
    <FieldSqlname>: <FIELD_TSTYPE>;
  </FIELD_LOOP>
}

</STRUCTURE_LOOP>
