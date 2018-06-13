<CODEGEN_FILENAME>TestEnvironment.dbl</CODEGEN_FILENAME>
<REQUIRES_USERTOKEN>SERVICES_NAMESPACE</REQUIRES_USERTOKEN>
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
;; Title:       TestEnvironment.dbl
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

import Microsoft.AspNetCore.Hosting
import Microsoft.AspNetCore.TestHost
import Microsoft.VisualStudio.TestTools.UnitTesting
import System.Text
import <SERVICES_NAMESPACE>

main TestEnvironment
proc
	;For debugging!

	TestEnvironment.AssemblyInitialize(^null)

	data tester = new CustomerTests()
	tester.GetAllCustomers()

	TestEnvironment.AssemblyCleanup()

endmain

namespace <NAMESPACE>

	{TestClass}
	public class TestEnvironment

		public static Server, @TestServer 

		{AssemblyInitialize}
		public static method AssemblyInitialize, void
			required in context, @TestContext
		proc
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)

			;;Set the logical names that will be used to access the data files
			data status, i4
			xcall setlog("ICSTUT", EnvironmentRootBuilder.FindRelativeFolderForAssembly("SampleData"), status)

			;;Make sure the files don't already exist
			deleteFiles()

			;;Create the data files
			createFiles()

			;;Create a TestServer to host the Web API services
			Server = new TestServer(new WebHostBuilder().UseStartup<Startup>())

		endmethod

		{AssemblyCleanup}
		public static method AssemblyCleanup, void
		proc
			;;Clean up the test host
			Server.Dispose()
			Server = ^null

			;;Delete the data files
			deleteFiles()

		endmethod

		private static method createFiles, void
			<STRUCTURE_LOOP>
			.include "<STRUCTURE_NOALIAS>" repository, record="<structureNoplural>"
			</STRUCTURE_LOOP>
		proc
			data chout, int
			data chin, int
			data dataFile, string
			data xdlFile, string
			data textFile, string

			<STRUCTURE_LOOP>
			;;Create and load the <structurePlural> file

			dataFile = "<FILE_NAME>"
			xdlFile = "@" + dataFile.ToLower().Replace(".ism",".xdl")
			textFile = dataFile.ToLower().Replace(".ism",".txt")

			open(chout=0,o:i,dataFile,FDL:xdlFile)
			open(chin,i,textFile)
			repeat
			begin
				reads(chin,<structureNoplural>,end<structurePlural>)
				store(chout,<structureNoplural>)
			end
		end<structurePlural>,
			close chin
			close chout

			</STRUCTURE_LOOP>
		endmethod

		private static method deleteFiles, void
		proc
			<STRUCTURE_LOOP>
			;;Delete the <structurePlural> file
			try
			begin
				xcall delet("<FILE_NAME>")
			end
			catch (e, @NoFileFoundException)
			begin
				nop
			end
			endtry

			</STRUCTURE_LOOP>
		endmethod

	endclass

endnamespace
