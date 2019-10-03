<CODEGEN_FILENAME>SelfHost.dbl</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.4.4</REQUIRES_CODEGEN_VERSION>
<REQUIRES_USERTOKEN>API_DOCS_PATH</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>SERVICES_NAMESPACE</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>SERVER_PROTOCOL</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>SERVER_NAME</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>SERVER_HTTP_PORT</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>SERVER_HTTPS_PORT</REQUIRES_USERTOKEN>
;//****************************************************************************
;//
;// Title:       ODataStandaloneSelfHost.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Generates a program to self-host Harmony Core services
;//
;// Copyright (c) 2018, Synergex International, Inc. All rights reserved.
;//
;// Redistribution and use in source and binary forms, with or without
;// modification, are permitted provided that the following conditions are met:
;//
;// * Redistributions of source code must retain the above copyright notice,
;//   this list of conditions and the following disclaimer.
;//
;// * Redistributions in binary form must reproduce the above copyright notice,
;//   this list of conditions and the following disclaimer in the documentation
;//   and/or other materials provided with the distribution.
;//
;// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
;// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
;// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
;// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
;// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
;// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
;// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
;// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
;// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
;// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
;// POSSIBILITY OF SUCH DAMAGE.
;//
;;*****************************************************************************
;;
;; Title:       SelfHost.dbl
;;
;; Description: A program to self-host Harmony Core services
;;
;;*****************************************************************************
;; WARNING: GENERATED CODE!
;; This file was generated by CodeGen. Avoid editing the file if possible.
;; Any changes you make will be lost of the file is re-generated.
;;*****************************************************************************

import Microsoft.AspNetCore
import Microsoft.AspNetCore.Hosting
import System.Collections.Generic
import System.IO
import System.Text
import <SERVICES_NAMESPACE>
import <NAMESPACE>

main SelfHost

proc
    ;;-------------------------------------------------------------------------
    ;;Configure the environment
    try
    begin
        SelfHostEnvironment.Initialize()
    end
    catch (ex, @Exception)
    begin
        Console.WriteLine(ex.Message)
        Console.Write("Press a key to terminate: ")
        Console.ReadKey()
        stop
    end
    endtry

<IF DEFINED_ENABLE_SWAGGER_DOCS>
    ;;-------------------------------------------------------------------------
    ;;Report the location of the API documentation

    Console.WriteLine("API documentation is available at <SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>/<API_DOCS_PATH>")

</IF DEFINED_ENABLE_SWAGGER_DOCS>
<IF DEFINED_ENABLE_API_VERSIONING>
    ;;-------------------------------------------------------------------------
    ;;Report the location of the API documentation

    Console.WriteLine("API documentation is available at <SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>/<API_DOCS_PATH>")

</IF DEFINED_ENABLE_API_VERSIONING>
    ;;-------------------------------------------------------------------------
    ;;Define the location that static files are served from and make sure it exists

    data wwwroot = Path.Combine(AppContext.BaseDirectory, "wwwroot")

    if (!Directory.Exists(wwwroot))
        Directory.CreateDirectory(wwwroot)
    ;;-------------------------------------------------------------------------
    ;;Start the self-hosting environment (Kestrel)

    WebHost.CreateDefaultBuilder(Environment.GetCommandLineArgs())
    &    .UseContentRoot(AppContext.BaseDirectory)
    &    .UseWebRoot(wwwroot)
<IF DEFINED_ENABLE_IIS_SUPPORT>
    &    .UseIISIntegration()
</IF DEFINED_ENABLE_IIS_SUPPORT>
    &    .UseStartup<Startup>()
    &    .UseUrls("http://<SERVER_NAME>:<SERVER_HTTP_PORT>", "https://<SERVER_NAME>:<SERVER_HTTPS_PORT>")
    &    .Build()
    &    .Run()

    ;;-------------------------------------------------------------------------
    ;;When the server exist, cleanup the environment

    SelfHostEnvironment.Cleanup()

endmain
