<CODEGEN_FILENAME><INTERFACE_NAME>ActionModels.dbl</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.3.6</REQUIRES_CODEGEN_VERSION>
;//****************************************************************************
;//
;// Title:       ODataActionModels.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Generates model classes for methods exposed as OData Actions
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
;; Title:       <INTERFACE_NAME>ActionModels.dbl
;;
;; Description: Model classes for methods in the <INTERFACE_NAME> interface
;;              that are exposed as OData Actions
;;
;;*****************************************************************************
;; WARNING: GENERATED CODE!
;; This file was generated by CodeGen. Avoid editing the file if possible.
;; Any changes you make will be lost of the file is re-generated.
;;*****************************************************************************

import System.Collections.Generic

namespace <NAMESPACE>
;//
;//
;//
<METHOD_LOOP>
<COUNTER_1_RESET>
<PARAMETER_LOOP>
<IF OUT_OR_INOUT>
<COUNTER_1_INCREMENT>
</IF OUT_OR_INOUT>
</PARAMETER_LOOP>
;//
;//
;//
<IF COUNTER_1>
	;;-------------------------------------------------------------------------
	;;; <summary>
	;;; Return data model for <INTERFACE_NAME>.<METHOD_NAME>
	;;; </summary>
	public class <INTERFACE_NAME>_<METHOD_NAME>

<PARAMETER_LOOP>
<IF OUT_OR_INOUT>
<IF COLLECTION>
		<IF STRUCTURE>
		public readwrite property <PARAMETER_NAME>, @List<<ParameterStructureNoplural>>
		</IF STRUCTURE>
		<IF ALPHA>
		public readwrite property <PARAMETER_NAME>, @List<string>
		</IF ALPHA>
		<IF DECIMAL>
		public readwrite property <PARAMETER_NAME>, @List<int>
		</IF DECIMAL>
		<IF IMPLIED>
		public readwrite property <PARAMETER_NAME>, @List<decimal>
		</IF IMPLIED>
		<IF INTEGER>
		public readwrite property <PARAMETER_NAME>, @List<int>
		</IF INTEGER>
<ELSE>
		<IF STRUCTURE>
		public readwrite property <PARAMETER_NAME>, @<ParameterStructureNoplural>
		</IF STRUCTURE>
		<IF ALPHA>
		public readwrite property <PARAMETER_NAME>, string
		</IF ALPHA>
		<IF DECIMAL>
		public readwrite property <PARAMETER_NAME>, int
		</IF DECIMAL>
		<IF IMPLIED>
		public readwrite property <PARAMETER_NAME>, decimal
		</IF IMPLIED>
		<IF INTEGER>
		public readwrite property <PARAMETER_NAME>, int
		</IF INTEGER>
</IF COLLECTION>
</IF OUT_OR_INOUT>
</PARAMETER_LOOP>
<IF FUNCTION>

		;Function return value
		<IF ALPHA>
		public readwrite property ReturnValue, string
		</IF ALPHA>
		<IF DECIMAL>
		public readwrite property ReturnValue, int
		</IF DECIMAL>
		<IF IMPLIED>
		public readwrite property ReturnValue, decimal
		</IF IMPLIED>
		<IF INTEGER>
		public readwrite property ReturnValue, int
		</IF INTEGER>
		<IF HATVAL>
		public readwrite property ReturnValue, int
		</IF HATVAL>
		<IF ENUM>
		public readwrite property ReturnValue, <METHOD_RETURN_ENUM>
		</IF ENUM>
		<IF STRING>
		public readwrite property ReturnValue, string
		</IF STRING>

</IF FUNCTION>
	endclass

</IF COUNTER_1>
</METHOD_LOOP>
endnamespace