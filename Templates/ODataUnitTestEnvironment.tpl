<CODEGEN_FILENAME>UnitTestEnvironment.dbl</CODEGEN_FILENAME>
<REQUIRES_USERTOKEN>SERVICES_NAMESPACE</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>MODELS_NAMESPACE</REQUIRES_USERTOKEN>
<OPTIONAL_USERTOKEN>OAUTH_SERVER=http://localhost:5000</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>OAUTH_CLIENT=ro.client</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>OAUTH_SECRET=CBF7EBE6-D46E-41A7-903B-766A280616C3</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>TEST_USER=jodah</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>TEST_PASSWORD=P@ssw0rd</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>TEST_API=api1</OPTIONAL_USERTOKEN>
<REQUIRES_CODEGEN_VERSION>5.3.4</REQUIRES_CODEGEN_VERSION>
;//****************************************************************************
;//
;// Title:       ODataUnitTestEnvironment.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Generates a class that configures an environment in which unit
;//              tests can operate with a known initial data state.
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
;; Title:       UnitTestEnvironment.dbl
;;
;; Type:        Class
;;
;; Description: Configures an environment in which unit tests can operate
;;              with a known initial data state.
;;
;;*****************************************************************************
;; WARNING
;;
;; This file was code generated. Avoid editing this file, as any changes that
;; you make will be lost of the file is re-generated.
;;
;;*****************************************************************************
;;
;; Copyright (c) 2018, Synergex International, Inc.
;; All rights reserved.
;;
;; Redistribution and use in source and binary forms, with or without
;; modification, are permitted provided that the following conditions are met:
;;
;; * Redistributions of source code must retain the above copyright notice,
;;   this list of conditions and the following disclaimer.
;;
;; * Redistributions in binary form must reproduce the above copyright notice,
;;   this list of conditions and the following disclaimer in the documentation
;;   and/or other materials provided with the distribution.
;;
;; THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
;; AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
;; IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
;; ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
;; LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
;; CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
;; SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
;; INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
;; CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
;; ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
;; POSSIBILITY OF SUCH DAMAGE.
;;
;;*****************************************************************************

<IF DEFINED_AUTHENTICATION>
import IdentityModel.Client
</IF DEFINED_AUTHENTICATION>
import Microsoft.AspNetCore
import Microsoft.AspNetCore.Hosting
import Microsoft.AspNetCore.TestHost
import Microsoft.VisualStudio.TestTools.UnitTesting
import System.Collections.Generic
import System.IO
import System.Linq
import System.Text

import Harmony.Core.EF.Extensions
import <SERVICES_NAMESPACE>
import <MODELS_NAMESPACE>

main UnitTestEnvironment

proc
	;;Configure the environment
	UnitTestEnvironment.AssemblyInitialize(^null)

	;Leave this here for Jeff 
	;data tester = new OrderTests()  
	;tester.GetOrders_Expand_REL_TagCustomer() 
	;tester.GetOrders_Expand_REL_TagVendor()
	;tester.GetOrders_Expand_All()
	;tester.GetCustomer()      

	;;Start self-hosting (Kestrel)
	WebHost.CreateDefaultBuilder(new string[0]).UseStartup<Startup>().Build().Run()

	;;Cleanup the environment
	UnitTestEnvironment.AssemblyCleanup()

endmain

namespace <NAMESPACE>

	{TestClass}
	public class UnitTestEnvironment

		public static Server, @TestServer
<IF DEFINED_AUTHENTICATION>
		public static AccessToken, string
</IF DEFINED_AUTHENTICATION>

		{AssemblyInitialize}
		public static method AssemblyInitialize, void
			required in context, @Microsoft.VisualStudio.TestTools.UnitTesting.TestContext
		proc
			;;Configure the test environment (set logicals, create files in a known state, etc.)
			TestEnvironment.Configure()

			;;Define the content root and web root folders (so we can pick up the Swagger file for API documentation)
			data wwwroot = Environment.GetEnvironmentVariable("WWWROOT")

			if(string.IsNullOrEmpty(wwwroot)) then
			begin
				;;Create a TestServer to host the Web API services
				Server = new TestServer(new WebHostBuilder().UseStartup<Startup>())
			end
			else
			begin
				;;Create a TestServer to host the Web API services
				Server = new TestServer(new WebHostBuilder().UseContentRoot(wwwroot).UseWebRoot(wwwroot).UseStartup<Startup>())
			end

<IF DEFINED_AUTHENTICATION>
			;;Get the access token from the OAuth Server
			data disco = DiscoveryClient.GetAsync("<OAUTH_SERVER>").GetAwaiter().GetResult()

			if (disco.IsError) then
			begin
				throw new Exception("OAuth endpoint discovery failed. Error was: " + disco.Error)
			end
			else
			begin
                data tokenClient = new TokenClient(disco.TokenEndpoint, "<OAUTH_CLIENT>", "<OAUTH_SECRET>");
                data tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync("<TEST_USER>","<TEST_PASSWORD>","<TEST_API>").GetAwaiter().GetResult()

                if (tokenResponse.IsError) then
                begin
                    ;;Failed to get an access token from the OAuth server
                    throw new Exception(tokenResponse.Error);
                end
                else
                begin
                    ;;Now we have an access token that we can use to call our protected API
					AccessToken = tokenResponse.AccessToken
                end
			end

</IF DEFINED_AUTHENTICATION>
		endmethod

		{AssemblyCleanup}
		public static method AssemblyCleanup, void
		proc
			;;Clean up the test host
			Server.Dispose()
			Server = ^null

			;;Delete the data files
			TestEnvironment.Cleanup()

		endmethod

	endclass

endnamespace
