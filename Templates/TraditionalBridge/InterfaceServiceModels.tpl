<CODEGEN_FILENAME><INTERFACE_NAME>ServiceModels.dbl</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.8.7</REQUIRES_CODEGEN_VERSION>
<REQUIRES_USERTOKEN>MODELS_NAMESPACE</REQUIRES_USERTOKEN>
;//****************************************************************************
;//
;// Title:       InterfaceServiceModels.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Creates request and response models for a service class that
;//              exposes former xfServerPlus methods in an interface via Harmony
;//              Core Traditional Bridge
;//
;// Copyright (c) 2019, Synergex International, Inc. All rights reserved.
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
;; Title:       <INTERFACE_NAME>ServiceModels.dbl
;;
;; Description: Request and response models for methods that are part of the
;;              former xfServerPlus / xfNetLink "<INTERFACE_NAME>" interface.
;;
;;*****************************************************************************
;; WARNING: GENERATED CODE!
;; This file was generated by CodeGen. Avoid editing the file if possible.
;; Any changes you make will be lost of the file is re-generated.
;;*****************************************************************************

<IF DEFINED_ENABLE_NEWTONSOFT>
import Newtonsoft.Json
</IF DEFINED_ENABLE_NEWTONSOFT>
import System
import System.ComponentModel.DataAnnotations

import <MODELS_NAMESPACE>

namespace <NAMESPACE>
<ENUM_LOOP>
.ifndef <ENUM_NAME>
.include "<ENUM_NAME>" repository, enum
.endc
</ENUM_LOOP>
<METHOD_LOOP>

;;--------------------------------------------------------------------------------
;; <METHOD_NAME>
;//
;// REQUEST MODEL
;//
  <IF IN_OR_INOUT>

    <IF DEFINED_ENABLE_NEWTONSOFT>
    {JsonObject(MemberSerialization.OptIn)}
    </IF DEFINED_ENABLE_NEWTONSOFT>
    ;;; <summary>
    ;;; Represents IN parameters for method <INTERFACE_NAME>.<METHOD_NAME>.
    ;;; </summary>
    public class <METHOD_NAME>_Request
      <PARAMETER_LOOP>
        <IF IN_OR_INOUT>

        <IF DEFINED_ENABLE_NEWTONSOFT>
        {JsonProperty}
        </IF DEFINED_ENABLE_NEWTONSOFT>
        <IF REQUIRED>
        {Required(ErrorMessage="<PARAMETER_NAME> is required")}
        </IF REQUIRED>
        <IF ALPHA>
        {StringLength(<PARAMETER_SIZE>,ErrorMessage="<PARAMETER_NAME> is limited to <PARAMETER_SIZE> characters")}
        </IF ALPHA>
        ;;; <summary>
        ;;; Parameter <PARAMETER_NUMBER> (<PARAMETER_REQUIRED> <PARAMETER_DIRECTION> <PARAMETER_DEFINITION>)
        <IF COMMENT>
        ;;; <PARAMETER_COMMENT>
        <ELSE>
        ;;; No description found in method catalog
        </IF COMMENT>
        ;;; </summary>
        public <PARAMETER_NAME>, <IF COLLECTION>[#]</IF><HARMONYCORE_BRIDGE_PARAMETER_TYPE><IF HANDLE>, String.Empty</IF>
        </IF IN_OR_INOUT>
      </PARAMETER_LOOP>

    endclass
  <ELSE>
    ;; This method has no in parameters
  </IF IN_OR_INOUT>
;//
;// RESPONSE MODEL
;//
  <IF RETURNS_DATA>

    <IF DEFINED_ENABLE_NEWTONSOFT>
    {JsonObject(MemberSerialization.OptIn)}
    </IF DEFINED_ENABLE_NEWTONSOFT>
    ;;; <summary>
    ;;; Represents OUT parameters<IF FUNCTION> and return value</IF FUNCTION> for method <INTERFACE_NAME>.<METHOD_NAME>.
    ;;; </summary>
    public class <METHOD_NAME>_Response
    <IF FUNCTION>

        <IF DEFINED_ENABLE_NEWTONSOFT>
        {JsonProperty}
        </IF DEFINED_ENABLE_NEWTONSOFT>
        ;;; <summary>
        ;;; Return value
        ;;; </summary>
        public <IF TWEAK_SMC_CAMEL_CASE>returnValue<ELSE>ReturnValue</IF>, <HARMONYCORE_BRIDGE_RETURN_TYPE>
    </IF FUNCTION>
    <IF OUT_OR_INOUT>
      <PARAMETER_LOOP>
        <IF OUT_OR_INOUT>

        <IF DEFINED_ENABLE_NEWTONSOFT>
        {JsonProperty}
        </IF DEFINED_ENABLE_NEWTONSOFT>
        ;;; <summary>
        ;;; Parameter <PARAMETER_NUMBER> (<PARAMETER_REQUIRED> <PARAMETER_DIRECTION> <PARAMETER_DEFINITION>)
        <IF COMMENT>
        ;;; <PARAMETER_COMMENT>
        <ELSE>
        ;;; No description found in method catalog
        </IF COMMENT>
        ;;; </summary>
        public <PARAMETER_NAME>, <IF COLLECTION>[#]</IF><HARMONYCORE_BRIDGE_PARAMETER_TYPE><IF OUT AND ALPHA>, <IF COLLECTION>new string[0]<ELSE>String.Empty</IF></IF><IF HANDLE>, String.Empty</IF>
        </IF OUT_OR_INOUT>
      </PARAMETER_LOOP>
    </IF OUT_OR_INOUT>

    endclass

  <ELSE>
    ;; This method does not return any data!
  </IF RETURNS_DATA>
</METHOD_LOOP>
endnamespace
