<CODEGEN_FILENAME><StructureNoplural>Loader.dbl</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.4.6</REQUIRES_CODEGEN_VERSION>
<REQUIRES_OPTION>TF</REQUIRES_OPTION>
<CODEGEN_FOLDER>DataGenerators</CODEGEN_FOLDER>
<REQUIRES_USERTOKEN>MODELS_NAMESPACE</REQUIRES_USERTOKEN>
;//****************************************************************************
;//
;// Title:       ODataTestDataLoader.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Creates a class that loads sample data from a sequential file.
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
;; Title:       <StructureNoplural>Loader.dbl
;;
;; Description: Loads sample <structureNoplural> data from a sequential file.
;;
;;*****************************************************************************
;; WARNING: GENERATED CODE!
;; This file was generated by CodeGen. Avoid editing the file if possible.
;; Any changes you make will be lost of the file is re-generated.
;;*****************************************************************************

import System.Collections.Generic
import <MODELS_NAMESPACE>

namespace <NAMESPACE>

    public static partial class <StructureNoplural>Loader
    
        public static method LoadFromFile, @List<<StructureNoplural>>
        proc
            data dataFile = "<FILE_NAME>"
            data textFile = dataFile.Replace(".ism",".txt", StringComparison.CurrentCultureIgnoreCase)
			UnitTestEnvironment.EnsurePlatformSpecificLineEndings(textFile.Replace(":", System.IO.Path.DirectorySeparatorChar).Replace("dat", Environment.GetEnvironmentVariable("DAT")), <STRUCTURE_SIZE>)
			data <structureNoplural>Ch, int, 0
            data <structureNoplural>Rec, str<StructureNoplural>
            data grfa, a10
            data <structurePlural> = new List<<StructureNoplural>>()
            open(<structureNoplural>Ch,i:s,textFile)
            repeat
            begin
                reads(<structureNoplural>Ch,<structureNoplural>Rec,eof)
                <structurePlural>.Add(new <StructureNoplural>(<structureNoplural>Rec, grfa))
            end
        eof,
            close <structureNoplural>Ch
            mreturn <structurePlural>
        endmethod

    endclass

endnamespace
