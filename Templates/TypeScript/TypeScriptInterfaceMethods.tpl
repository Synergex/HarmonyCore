<CODEGEN_FILENAME><interfaceName>Methods.ts</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.8.1</REQUIRES_CODEGEN_VERSION>
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
  to the operations in the exposed service "<interfaceName>".
*/

import * as <interfaceName>Structures from './<interfaceName>Structures';

<METHOD_LOOP>
//------------------------------------------------------------------------------
// Operation: <methodName>

;//--------------
;// REQUEST MODEL
;//
  <IF IN_OR_INOUT>
export interface <methodName>Request
{
   <PARAMETER_LOOP>
    <IF IN_OR_INOUT>
    <parameterName>: <IF STRUCTURE><interfaceName>Structures.</IF STRUCTURE><PARAMETER_TSTYPE>;
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
export interface <methodName>Response
{
    <IF FUNCTION>
    returnValue: <METHOD_RETURN_TSTYPE>;
    </IF FUNCTION>
    <IF OUT_OR_INOUT>
      <PARAMETER_LOOP>
        <IF OUT_OR_INOUT>
    <parameterName>: <IF STRUCTURE><interfaceName>Structures.</IF STRUCTURE><PARAMETER_TSTYPE>;
        </IF OUT_OR_INOUT>
      </PARAMETER_LOOP>
    </IF OUT_OR_INOUT>
}
  <ELSE>
// No returned data
  </IF RETURNS_DATA>

</METHOD_LOOP>
