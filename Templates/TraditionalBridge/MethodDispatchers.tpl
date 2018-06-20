<CODEGEN_FILENAME>MethodDispachers.dbl</CODEGEN_FILENAME>
<REQUIRES_USERTOKEN>MODELS_NAMESPACE</REQUIRES_USERTOKEN>
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

import Json
import Harmony.TraditionalBridge
import System.Collections
import <MODELS_NAMESPACE>

namespace <NAMESPACE>

	structure strFake
		,a1
	endstructure
<METHOD_LOOP>

	;;-------------------------------------------------------------------------
	;;Dispatcher for method <METHOD_NAME>

	public class <METHOD_ROUTINE>Dispatch extends RoutineStub

		<PARAMETER_LOOP>
		<IF STRUCTURE>
		<IF FIRST_INSTANCE_OF_STRUCTURE>
		private m<ParameterStructureNoplural>Metadata, @DataObjectMetadataBase
		</IF FIRST_INSTANCE_OF_STRUCTURE>
		</IF STRUCTURE>
		</PARAMETER_LOOP>

		public method <METHOD_ROUTINE>Dispatch
		proc
			;;Initialize the meta data for any data objects that are used by parameters to the method
			<PARAMETER_LOOP>
			<IF STRUCTURE>
			<IF FIRST_INSTANCE_OF_STRUCTURE>
			m<ParameterStructureNoplural>Metadata = DataObjectMetadataBase.LookupType("<ParameterStructureNoplural>")
			</IF FIRST_INSTANCE_OF_STRUCTURE>
			</IF STRUCTURE>
			</PARAMETER_LOOP>
		endmethod

		public override method Dispatch, void
			required in name,       string
			required in callFrame,  @JsonObject
			required in serializer, @ChannelSerializer
			required in dispatcher, @RoutineDispatcher
			record
				arguments,			@JsonArray
				argumentDefinition, @ArgumentDataDefinition
<COUNTER_1_RESET>
<PARAMETER_LOOP>
	<COUNTER_1_INCREMENT>
;//
;//=========================================================================================================================
;// Declare variables for arguments
;//
	<IF COLLECTION>
				arg<COUNTER_1_VALUE>Array,			@JsonArray
		<IF COLLECTION_ARRAY>
				arg<COUNTER_1_VALUE>Handle, D_HANDLE
				arg<COUNTER_1_VALUE>HandlePos, int
		</IF COLLECTION_ARRAY>
		<IF COLLECTION_HANDLE>
				arg<COUNTER_1_VALUE>Handle, D_HANDLE
				arg<COUNTER_1_VALUE>HandlePos, int
		</IF COLLECTION_HANDLE>
		<IF COLLECTION_ARRAYLIST>
				arg<COUNTER_1_VALUE>,				@ArrayList
		</IF COLLECTION_ARRAYLIST>
	<ELSE>
		<IF STRUCTURE>
				arg<COUNTER_1_VALUE>DataObject, @DataObjectBase
				arg<COUNTER_1_VALUE>, str<ParameterStructureNoplural>
		<ELSE>
				arg<COUNTER_1_VALUE>,				<parameter_definition>
		</IF STRUCTURE>
	</IF COLLECTION>
</PARAMETER_LOOP>
;//
;//=========================================================================================================================
;// Declare variable for function return value
;//
<IF FUNCTION>
				returnValue,		<METHOD_RETURN_TYPE>
</IF FUNCTION>
;//=========================================================================================================================
			endrecord
		proc
			;;Retrieve argument data
			arguments = (@JsonArray)callFrame.GetProperty("Arguments")

;//
;//=========================================================================================================================
;// Assign values to argument variables
;//
			;;Populate variables for IN and INOUT arguments
<COUNTER_1_RESET>
<PARAMETER_LOOP>
	<COUNTER_1_INCREMENT>
	<IF IN_OR_INOUT>
	<IF COLLECTION>

			argumentDefinition = dispatcher.GetArgumentDataDefForCollection((@JsonObject)arguments.arrayValues[<COUNTER_1_VALUE>])
			arg<COUNTER_1_VALUE>Array = (@JsonArray)((@JsonObject)arguments.arrayValues[<COUNTER_1_VALUE>]).GetProperty("PassedValue")
;//
		<IF COLLECTION_ARRAY>
			arg<COUNTER_1_VALUE>Handle = %mem_proc(DM_ALLOC,argumentDefinition.ElementSize*arg<COUNTER_1_VALUE>Array.arrayValues.Count)
			arg<COUNTER_1_VALUE>HandlePos = 1
			dispatcher.UnwrapObjectCollection(^m(arg<COUNTER_1_VALUE>Handle),argumentDefinition,arg<COUNTER_1_VALUE>HandlePos,arg<COUNTER_1_VALUE>Array)
		</IF COLLECTION_ARRAY>
;//
		<IF COLLECTION_HANDLE>
			arg<COUNTER_1_VALUE>Handle = %mem_proc(DM_ALLOC,argumentDefinition.ElementSize*arg<COUNTER_1_VALUE>Array.arrayValues.Count)
			arg<COUNTER_1_VALUE>HandlePos = 1
			dispatcher.UnwrapObjectCollection(^m(arg<COUNTER_1_VALUE>Handle),argumentDefinition,arg<COUNTER_1_VALUE>HandlePos,arg<COUNTER_1_VALUE>Array)
		</IF COLLECTION_HANDLE>
;//
		<IF COLLECTION_ARRAYLIST>
			arg<COUNTER_1_VALUE> = new ArrayList()
			dispatcher.UnwrapObjectCollection(argumentDefinition,arg<COUNTER_1_VALUE>Array,arg<COUNTER_1_VALUE>)
		</IF COLLECTION_ARRAYLIST>
	<ELSE>

		<IF ALPHA>
			arg<COUNTER_1_VALUE> = dispatcher.GetText((@JsonObject)arguments.arrayValues[<COUNTER_1_VALUE>])
		</IF ALPHA>
;//
		<IF DECIMAL>
			arg<COUNTER_1_VALUE> = dispatcher.GetDecimal((@JsonObject)arguments.arrayValues[<COUNTER_1_VALUE>])
		</IF DECIMAL>
;//
		<IF IMPLIED>
			arg<COUNTER_1_VALUE> = dispatcher.GetImplied((@JsonObject)arguments.arrayValues[<COUNTER_1_VALUE>])
		</IF IMPLIED>
;//
		<IF INTEGER>
			arg<COUNTER_1_VALUE> = dispatcher.GetInt((@JsonObject)arguments.arrayValues[<COUNTER_1_VALUE>])
		</IF INTEGER>
;//
		<IF ENUM>
			arg<COUNTER_1_VALUE> = (<PARAMETER_ENUM>)dispatcher.GetInt((@JsonObject)arguments.arrayValues[<COUNTER_1_VALUE>])
		</IF ENUM>
;//
		<IF DATE>
			arg<COUNTER_1_VALUE> = dispatcher.GetDecimal((@JsonObject)arguments.arrayValues[<COUNTER_1_VALUE>])
		</IF DATE>
;//
		<IF TIME>
			arg<COUNTER_1_VALUE> = dispatcher.GetDecimal((@JsonObject)arguments.arrayValues[<COUNTER_1_VALUE>])
		</IF TIME>
;//
		<IF HANDLE>
			;TODO: Template needs code for HANDLE arguments!
			arg<COUNTER_1_VALUE> = 
		</IF HANDLE>
;//
		<IF BINARY_HANDLE>
			;TODO: Template needs code for BINARY HANDLE arguments!
			arg<COUNTER_1_VALUE> =
		</IF BINARY_HANDLE>
;//
		<IF STRING>
			arg<COUNTER_1_VALUE> = dispatcher.GetText((@JsonObject)arguments.arrayValues[<COUNTER_1_VALUE>])
		</IF STRING>
;//
		<IF STRUCTURE>
			;; Structure: get the data object
			arg<COUNTER_1_VALUE>DataObject = dispatcher.DeserializeObject((@JsonObject)arguments.arrayValues[3],m<ParameterStructureNoplural>Metadata)
			;; Then get the record from the data object
			arg<COUNTER_1_VALUE> = arg<COUNTER_1_VALUE>DataObject.SynergyRecord
		</IF STRUCTURE>
;//
	</IF COLLECTION>
	</IF IN_OR_INOUT>
</PARAMETER_LOOP>
;//
;//=========================================================================================================================
;// Make the method call
;//

			;; Call the method

			<IF SUBROUTINE>
			xcall <METHOD_ROUTINE>(
			</IF SUBROUTINE>
;//
			<IF FUNCTION>
			returnValue = %<METHOD_ROUTINE>(
			</IF FUNCTION>
;//
;//=========================================================================================================================
;// Pass the arguments
;//
<COUNTER_1_RESET>
<PARAMETER_LOOP>
	<COUNTER_1_INCREMENT>
	<IF COLLECTION>
		<IF COLLECTION_ARRAY>
			&	^m(<IF STRUCTURE>str<ParameterStructureNoplural><ELSE>strFake(1:<PARAMETER_SIZE>)</IF STRUCTURE>,arg<COUNTER_1_VALUE>Handle)<,>
		</IF COLLECTION_ARRAY>
		<IF COLLECTION_HANDLE>
			&	arg<COUNTER_1_VALUE>Handle<,>
		</IF COLLECTION_HANDLE>
		<IF COLLECTION_ARRAYLIST>
			&	arg<COUNTER_1_VALUE><,>
		</IF COLLECTION_ARRAYLIST>
	<ELSE>
			&	arg<COUNTER_1_VALUE><,>
	</IF COLLECTION>
</PARAMETER_LOOP>			
			& )

;//
;//=========================================================================================================================
;// Process the returned data
;//
			;Process any returned data

<COUNTER_1_RESET>
<PARAMETER_LOOP>
	<COUNTER_1_INCREMENT>
	<IF OUT_OR_INOUT>
			;TODO: Need to return parameter <COUNTER_1_VALUE>
	</IF OUT_OR_INOUT>
</PARAMETER_LOOP>

;//
;//=========================================================================================================================
;// Build the JSON response
;//
			;Build the JSON response

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