<CODEGEN_FILENAME>MethodDispachers.dbl</CODEGEN_FILENAME>
;//****************************************************************************
;//
;// Title:       MethodDispachers.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Generates dispatcher classes for exposed methods
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
;; Title:       DispatcherMethods.dbl
;;
;; Type:        Classes
;;
;; Description: Dispatcher classes for exposed methods
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

import Harmony.TraditionalBridge

namespace <NAMESPACE>
<METHOD_LOOP>

	public class <METHOD_ROUTINE>Dispatch extends RoutineStub

		<PARAMETER_LOOP>
		<IF STRUCTURE>
		private m<ParameterStructureNoplural>Metadata = DataObjectMetadataBase.LookupType("<ParameterStructureNoplural>")
		</IF STRUCTURE>
		</PARAMETER_LOOP>

		public method <METHOD_ROUTINE>Dispatch
		proc
			;;Initialize the meta data for any data objects that are used by parameters to the method
			<PARAMETER_LOOP>
			<IF STRUCTURE>
			m<ParameterStructureNoplural>Metadata = DataObjectMetadataBase.LookupType("<ParameterStructureNoplural>")
			</IF STRUCTURE>
			</PARAMETER_LOOP>
		endmethod

		public override method Dispatch, void
			name, @string
			callFrame, @Json.JsonObject
			serializer, @Json.ChannelSerializer
			dispatcher, @RoutineDispatcher
			record
				arguments, @Json.JsonArray
				<COUNTER_1_RESET>
				<PARAMETER_LOOP>
				<COUNTER_1_INCREMENT>
				<IF STRUCTURE>
				<IF COLLECTION>
				arg<COUNTER_1_VALUE>Array, @Json.JsonArray
				arg<COUNTER_1_VALUE>Handle, D_HANDLE
				arg<COUNTER_1_VALUE>HandlePosition, int
				<ELSE>
				arg<COUNTER_1_VALUE>, @DataObjectBase
				arg<COUNTER_1_VALUE>Data, str<ParameterStructureNoplural>
				</IF COLLECTION>
				</IF STRUCTURE>
				</PARAMETER_LOOP>
				<IF FUNCTION>
				returnValue, <METHOD_RETURN_TYPE>
				</IF FUNCTION>
			endrecord
		proc
			arguments = (@Json.JsonArray)callFrame.GetProperty("Arguments")
			<COUNTER_1_RESET>
			<PARAMETER_LOOP>
			<COUNTER_1_INCREMENT>
			<IF STRUCTURE>
			<IF COLLECTION>
			arg<COUNTER_1_VALUE>Array = (@Json.JsonArray)((@Json.JsonObject)arguments.arrayValues[4]).GetProperty("PassedValue")
			arg<COUNTER_1_VALUE>Handle = %mem_proc(DM_ALLOC,^size(str<ParameterStructureNoplural>)*arg<COUNTER_1_VALUE>Array.arrayValues.Count)
			arg<COUNTER_1_VALUE>HandlePosition = 1
			dispatcher.UnwrapObjectCollection(^m(arg<COUNTER_1_VALUE>Handle),^size(str<ParameterStructureNoplural>),arg<COUNTER_1_VALUE>HandlePosition,arg<COUNTER_1_VALUE>Array)
			<ELSE>
			arg<COUNTER_1_VALUE> = dispatcher.DeserializeObject((@Json.JsonObject)arguments.arrayValues[3],m<ParameterStructureNoplural>Metadata)
			arg<COUNTER_1_VALUE>Data = arg<COUNTER_1_VALUE>.SynergyRecord
			</IF COLLECTION>
			</IF STRUCTURE>
			</PARAMETER_LOOP>

			;;Now call the method

;// Call a subroutine
;//
			<IF SUBROUTINE>
			xcall <METHOD_ROUTINE>(
			</IF SUBROUTINE>
;//
;// Call a function

			<IF FUNCTION>
			returnValue = %<METHOD_ROUTINE>(
			</IF FUNCTION>
;//
<COUNTER_1_RESET>
<COUNTER_2_RESET>
<PARAMETER_LOOP>
<COUNTER_1_INCREMENT>
<IF COLLECTION>
;//
;// Collection parameters (Includes ARRAY, HANDLE collections and ArrayList collections, or any supported type)
;//
			<IF ALPHA>
			&	Collection of alpha not implemented<,>
			</IF ALPHA>
			<IF DECIMAL>
			&	Collection of decimal not implemented<,>
			</IF DECIMAL>
			<IF IMPLIED>
			&	Collection of implied decimal not implemented<,>
			</IF IMPLIED>
			<IF INTEGER>
			&	Collection of integer not implemented<,>
			</IF INTEGER>
			<IF DATE>
			&	Collection of date not implemented<,>
			</IF DATE>
			<IF TIME>
			&	Collection of time not implemented<,>
			</IF TIME>
			<IF STRING>
			&	Collection of string not implemented<,>
			</IF STRING>
			<IF STRUCTURE>
			&	^m(str<ParameterStructureNoplural>,arg<COUNTER_1_VALUE>Handle)<,>
			</IF STRUCTURE>
;//
<ELSE>
;//
;// Non-collection parameters
;//
			<IF ALPHA>
			&	dispatcher.GetText((@Json.JsonObject)arguments.arrayValues[<COUNTER_2_VALUE>])<,>
			</IF ALPHA>
			<IF DECIMAL>
			&	dispatcher.GetDecimal((@Json.JsonObject)arguments.arrayValues[<COUNTER_2_VALUE>])<,>
			</IF DECIMAL>
			<IF IMPLIED>
			&	Implied decimal parameter not implemented<,>
			</IF IMPLIED>
			<IF INTEGER>
			&	dispatcher.GetInt((@Json.JsonObject)arguments.arrayValues[<COUNTER_2_VALUE>])<,>
			</IF INTEGER>
			<IF ENUM>
			&	Enum parameter not implemented<,>
			</IF ENUM>
			<IF DATE>
			&	Date parameter not implemented<,>
			</IF DATE>
			<IF TIME>
			&	Time parameter not implemented<,>
			</IF TIME>
			<IF HANDLE>
			&	Handle parameter not implemented<,>
			</IF HANDLE>
			<IF BINARY_HANDLE>
			&	Binary handle parameter not implemented<,>
			</IF BINARY_HANDLE>
			<IF STRING>
			&	String parameter not implemented<,>
			</IF STRING>
			<IF STRUCTURE>
			&	arg<COUNTER_1_VALUE>Data,
			</IF STRUCTURE>
;//
</IF COLLECTION>
<COUNTER_2_INCREMENT>
</PARAMETER_LOOP>			
			& )

			serializer.MapOpen()
			serializer.String("IsError")
			serializer.Bool(false)
			serializer.String("Result")
			serializer.MapOpen()
			serializer.MapClose()
			serializer.MapClose()

		endmethod

	endclass
	</METHOD_LOOP>

endnamespace