<CODEGEN_FILENAME><interfaceName>Client.ts</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.8.5</REQUIRES_CODEGEN_VERSION>
;//
;// This template generates TypeScript client exposing the methods
;// that are defined in a method catalog interface.
;//
;// To process this template, you must use the following CodeGen command line options:
;//
;//   codegen -smc <smcXmlFile> -interface <ifname> 
;//
/*
  THIS CODE WAS CREATED BY A TOOL AND SHOULD NOT BE EDITED.
  ANY CHANGES WILL BE OVERWRITTEN THE NEXT TIME THE TOOL IS USED!

  This code defines a client for the operations in the service "<interfaceName>".
*/

import * as axios from './axios.js';
import * as <interfaceName>Methods from './<interfaceName>Methods';

export class <interfaceName> {

    instance: any;

    constructor(baseurl: string, jwt: string, tenant: string)
    {
        this.instance = axios.create({ baseURL: baseurl, timeout: 1000, headers: { 'x-tenant-id': tenant, 'Authorization': 'Bearer ' + jwt } });
    }
<METHOD_LOOP>

    async <methodName>(arg: <interfaceName>Methods.<methodName>Request): Promise<<interfaceName>Methods.<methodName>Response>
    {
        return this.instance.<IF IN_OR_INOUT>post<ELSE>get</IF>('<methodName>'<IF IN_OR_INOUT>, arg</IF>);
    }
</METHOD_LOOP>
}
