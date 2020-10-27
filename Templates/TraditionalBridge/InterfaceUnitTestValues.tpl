<CODEGEN_FILENAME><INTERFACE_NAME>TestValues.dbl</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.5.3</REQUIRES_CODEGEN_VERSION>
<REQUIRES_OPTION>TF</REQUIRES_OPTION>
<CODEGEN_FOLDER>UnitTests</CODEGEN_FOLDER>
<REQUIRES_USERTOKEN>CLIENT_MODELS_NAMESPACE</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>DTOS_NAMESPACE</REQUIRES_USERTOKEN>
;//****************************************************************************
;//
;// Title:       InterfaceUnitTests.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Generates a partial class containing values to be used when
;//              unit testing controllers based on methods in an xfServerPlus
;//              interface.
;//
;// Copyright (c) 2020, Synergex International, Inc. All rights reserved.
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
;; Title:       <INTERFACE_NAME>TestValues.dbl
;;
;; Description: Test context class with static values that can be used to feed
;;              data into unit tests.
;;
;;*****************************************************************************
;; WARNING: GENERATED CODE!
;; This file was code generated, but generally is set to be generated only one
;; time and not replaced, so that you can edit the code in the file. It is a
;; good idea to ensure that you have a backup of any changes that you make incase
;; someone accidentally regenerates the file.
;;*****************************************************************************

import Microsoft.VisualStudio.TestTools.UnitTesting
import Newtonsoft.Json
import System.Collections.Generic
import System.Net.Http
import <CLIENT_MODELS_NAMESPACE>
import <DTOS_NAMESPACE>

namespace <NAMESPACE>

    public partial class <INTERFACE_NAME>Tests

        public method <INTERFACE_NAME>Tests
        proc
<METHOD_LOOP>
  <IF IN_OR_INOUT>
            m<METHOD_NAME>_Request = new <METHOD_NAME>_Request()
  <PARAMETER_LOOP>
    <IF IN_OR_INOUT>
            m<METHOD_NAME>_Request.<PARAMETER_NAME> = 
    </IF IN_OR_INOUT>
  </PARAMETER_LOOP>

  </IF IN_OR_INOUT>
</METHOD_LOOP>
        endmethod

    endclass

endnamespace
