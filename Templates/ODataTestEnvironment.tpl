<CODEGEN_FILENAME>TestEnvironment.dbl</CODEGEN_FILENAME>
;//****************************************************************************
;//
;// Title:       ODataTestEnvironment.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Generates utilities for configuting a hosting environment.
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
;; Description: Utilities for configuting a hosting environment.
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

import System.Collections.Generic
import System.IO
import System.Text

.array 0

namespace <NAMESPACE>

	public static class TestEnvironment

		public static method SetLogicals, void
		proc

			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)

			data sampleDataFolder = findRelativeFolderForAssembly("SampleData")
			data logicals = new List<string>()
			data logical = String.Empty
			data fileSpec = String.Empty
			<STRUCTURE_LOOP>

			fileSpec = "<FILE_NAME>"
			if (fileSpec.Contains(":"))
			begin
				logical = fileSpec.Split(":")[0].ToUpper()
				if (!logicals.Contains(logical))
					logicals.Add(logical)
			end
			</STRUCTURE_LOOP>

			foreach logical in logicals
			begin
				data sts, int
				xcall setlog(logical,sampleDataFolder,sts)
			end

		endmethod

		public static method CreateFiles, void
			<STRUCTURE_LOOP>
			.include "<STRUCTURE_NOALIAS>" repository, stack record="<structureNoplural>", nofields, end
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

			try
			begin
				open(chout=0,o:i,dataFile,FDL:xdlFile)
				open(chin,i,textFile)
				repeat
				begin
					reads(chin,<structureNoplural>)
					store(chout,<structureNoplural>)
				end
			end
			catch (ex, @EndOfFileException)
			begin
				close chin
				close chout
			end
			endtry

			</STRUCTURE_LOOP>
		endmethod

		public static method DeleteFiles, void
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

		private static method findRelativeFolderForAssembly, string
			folderName, string
		proc
			data assemblyLocation = ^typeof(TestEnvironment).Assembly.Location
			data currentFolder = Path.GetDirectoryName(assemblyLocation)
			data rootPath = Path.GetPathRoot(currentFolder)
			while(currentFolder != rootPath)
			begin
				if(Directory.Exists(Path.Combine(currentFolder, folderName))) then
					mreturn Path.Combine(currentFolder, folderName)
				else
					currentFolder = Path.GetFullPath(currentFolder + "..\")
			end
			mreturn ^null
		endmethod

	endclass

endnamespace
