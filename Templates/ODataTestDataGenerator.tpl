<CODEGEN_FILENAME><StructureNoplural>Generator.dbl</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.3.5</REQUIRES_CODEGEN_VERSION>
<REQUIRES_OPTION>TF</REQUIRES_OPTION>
<CODEGEN_FOLDER>DataGenerators</CODEGEN_FOLDER>
<REQUIRES_USERTOKEN>MODELS_NAMESPACE</REQUIRES_USERTOKEN>
;//****************************************************************************
;//
;// Title:       ODataTestDataGenerator.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Generates a placeholder where developers can implement the
;//              generation of test data for the environment
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
;; Title:       <StructureNoplural>Generator.dbl
;;
;; Description: A placeholder where you can implement the generation of test data
;;              for the environment <FILE_NAME>.
;;
;;*****************************************************************************
;; WARNING: GENERATED CODE!
;; This file was code generated, but generally is set to be generated only one
;; time and not replaced, so that you can edit the code in the file. It is a
;; good idea to ensure that you have a backup of any changes that you make incase
;; someone accidentally regenerates the file.
;;*****************************************************************************

import System.Collections.Generic
import <MODELS_NAMESPACE>

namespace <NAMESPACE>

    public static partial class <StructureNoplural>Generator
    
        public static method Generate, @List<<StructureNoplural>>
        proc
            throw new NotImplementedException()
            mreturn new List<<StructureNoplural>>()
        endmethod

    endclass

endnamespace
