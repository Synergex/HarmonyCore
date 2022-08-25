<CODEGEN_FILENAME><INTERFACE_NAME>Methods.ts</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.8.5</REQUIRES_CODEGEN_VERSION>
;//
;// This template generates TypeScript interfaces corresponding to the methods
;// that are defined in a method catalog interface.
;//
;// To process this template, you must use the following CodeGen command line options:
;//
;//   codegen -smc <smcXmlFile> -interface <ifname> 
;//
/*
  THIS CODE WAS CREATED BY A TOOL AND SHOULD NOT BE EDITED.
  ANY CHANGES WILL BE OVERWRITTEN THE NEXT TIME THE TOOL IS USED!

  This code defines the request and response messages related
  to the operations in the exposed service "<INTERFACE_NAME>".
*/

import * as <INTERFACE_NAME>Structures from './<INTERFACE_NAME>Structures';

<METHOD_LOOP>
//------------------------------------------------------------------------------
// Operation: <METHOD_NAME>

;//--------------
;// REQUEST MODEL
;//
  <IF IN_OR_INOUT>
export interface <METHOD_NAME>Request {
   <PARAMETER_LOOP>
    <IF IN_OR_INOUT>
    <PARAMETER_NAME>: <IF STRUCTURE><INTERFACE_NAME>Structures.</IF STRUCTURE><PARAMETER_TSTYPE>;
    </IF IN_OR_INOUT>
   </PARAMETER_LOOP>
}
  <ELSE>
// No input parameters
  </IF IN_OR_INOUT>

;//---------------
;// RESPONSE MODEL
;//
  <IF RETURNS_DATA>
export interface <METHOD_NAME>Response {
    <IF FUNCTION>
    <IF TWEAK_SMC_CAMEL_CASE>r<ELSE>R</IF>eturnValue: <METHOD_RETURN_TSTYPE>;
    </IF FUNCTION>
    <IF OUT_OR_INOUT>
      <PARAMETER_LOOP>
        <IF OUT_OR_INOUT>
    <PARAMETER_NAME>: <IF STRUCTURE><INTERFACE_NAME>Structures.</IF STRUCTURE><PARAMETER_TSTYPE>;
        </IF OUT_OR_INOUT>
      </PARAMETER_LOOP>
    </IF OUT_OR_INOUT>
}
  <ELSE>
// No returned data
  </IF RETURNS_DATA>

</METHOD_LOOP>
