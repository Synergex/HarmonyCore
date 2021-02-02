<CODEGEN_FILENAME><INTERFACE_NAME>MethodDispachers.dbl</CODEGEN_FILENAME>
<REQUIRES_USERTOKEN>MODELS_NAMESPACE</REQUIRES_USERTOKEN>
<REQUIRES_CODEGEN_VERSION>5.4.6</REQUIRES_CODEGEN_VERSION>
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
;; Title:       <INTERFACE_NAME>MethodDispachers.dbl
;;
;; Description: Dispatcher classes for exposed methods
;;
;;*****************************************************************************
;; WARNING: GENERATED CODE!
;; This file was generated by CodeGen. Avoid editing the file if possible.
;; Any changes you make will be lost of the file is re-generated.
;;*****************************************************************************

import Json
import Harmony.TraditionalBridge
import System.Collections
import <MODELS_NAMESPACE>

.ifdef DBLV11
import System.Text.Json
.define JSON_ELEMENT @JsonElement
.else
.define JSON_ELEMENT @JsonValue
.endc

namespace <NAMESPACE>.<INTERFACE_NAME>

<METHOD_LOOP>

    ;;-------------------------------------------------------------------------
    ;;; <summary>
    ;;; Dispatcher for method <INTERFACE_NAME>.<METHOD_NAME>
    ;;; </summary>
    public class <METHOD_NAME>_Dispatcher extends RoutineStub

        <PARAMETER_LOOP>
        <IF STRUCTURE>
        <IF FIRST_INSTANCE_OF_STRUCTURE>
        private m<ParameterStructureNoplural>Metadata, @DataObjectMetadataBase
        </IF FIRST_INSTANCE_OF_STRUCTURE>
        </IF STRUCTURE>
        </PARAMETER_LOOP>

        public method <METHOD_NAME>_Dispatcher
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

        protected override method DispatchInternal, void
            required in name,       string
            required in callFrame,  JSON_ELEMENT
            required in serializer, @DispatchSerializer
            required in dispatcher, @RoutineDispatcher
            record
                requestId,          int
                arguments,          JSON_ELEMENT
                argumentDefinition, @ArgumentDataDefinition

<COUNTER_1_RESET>
<PARAMETER_LOOP>
    <COUNTER_1_INCREMENT>
;//
;//=========================================================================================================================
;// Declare variables for arguments
;//
                ;;Argument <COUNTER_1_VALUE> (<PARAMETER_REQUIRED> <PARAMETER_DIRECTION> <PARAMETER_NAME> <IF COLLECTION_ARRAY>[*]</IF COLLECTION_ARRAY><IF COLLECTION_HANDLE>memory handle collection of </IF COLLECTION_HANDLE><IF COLLECTION_ARRAYLIST>ArrayList collection of </IF COLLECTION_ARRAYLIST><IF STRUCTURE>structure </IF STRUCTURE><IF ENUM>enum </IF ENUM><IF STRUCTURE>@<ParameterStructureNoplural><ELSE><PARAMETER_DEFINITION></IF STRUCTURE><IF DATE> <PARAMETER_DATE_FORMAT> date</IF DATE><IF TIME> <PARAMETER_DATE_FORMAT> time</IF TIME><IF REFERENCE> passed by REFERENCE</IF REFERENCE><IF VALUE> passed by VALUE</IF VALUE><IF DATATABLE> returned as DataTable</IF DATATABLE>)
    <IF COLLECTION>
                arg<COUNTER_1_VALUE>Array,          JSON_ELEMENT
        <IF COLLECTION_ARRAY>
                arg<COUNTER_1_VALUE>Handle,         D_HANDLE
                arg<COUNTER_1_VALUE>HandlePos,      int
        </IF COLLECTION_ARRAY>
        <IF COLLECTION_HANDLE>
                arg<COUNTER_1_VALUE>Handle,         D_HANDLE
                arg<COUNTER_1_VALUE>HandlePos,      int
        </IF COLLECTION_HANDLE>
        <IF COLLECTION_ARRAYLIST>
                arg<COUNTER_1_VALUE>,               @ArrayList
        </IF COLLECTION_ARRAYLIST>
    <ELSE>
        <IF STRUCTURE>
                arg<COUNTER_1_VALUE>DataObject,     @DataObjectBase
                arg<COUNTER_1_VALUE>,               str<ParameterStructureNoplural>
        <ELSE HANDLE OR BINARY_HANDLE>
                arg<COUNTER_1_VALUE>Array,          JSON_ELEMENT
                arg<COUNTER_1_VALUE>Handle,         <parameter_definition>
        <ELSE>
                arg<COUNTER_1_VALUE>,               <HARMONYCORE_BRIDGE_PARAMETER_DEFINITION>
        </IF>
    </IF COLLECTION>
</PARAMETER_LOOP>
;//
;//=========================================================================================================================
;// Declare variable for function return value
;//
<IF FUNCTION>
                returnValue,         <IF HATVAL>i4<ELSE><METHOD_RETURN_TYPE></IF HATVAL>
</IF FUNCTION>
;//=========================================================================================================================
            endrecord

        <COUNTER_1_RESET>
        <PARAMETER_LOOP>
            <COUNTER_1_INCREMENT>
            <IF COLLECTION_ARRAY>
            ;;Temp structure tempstr<COUNTER_1_VALUE>
            structure tempstr<COUNTER_1_VALUE>
                arry, <IF STRUCTURE>@<ParameterStructureNoplural><ELSE>[1]<PARAMETER_DEFINITION></IF STRUCTURE>
            endstructure

            </IF COLLECTION_ARRAY>
        </PARAMETER_LOOP>

        proc
;//
;//=========================================================================================================================
;// Assign values to argument variables
;//
<COUNTER_1_RESET>
<PARAMETER_LOOP>
    <COUNTER_1_INCREMENT> 
</PARAMETER_LOOP>
;//

            ;;------------------------------------------------------------
            ;;Process inbound arguments

<IF COUNTER_1>
            arguments = callFrame.GetProperty("params")
<ELSE>
            ;;There are no inbound arguments to process
</IF COUNTER_1>
;//
<COUNTER_1_RESET>
<PARAMETER_LOOP>
    <COUNTER_1_INCREMENT>

            ;;Argument <COUNTER_1_VALUE> (<PARAMETER_REQUIRED> <PARAMETER_DIRECTION> <PARAMETER_NAME> <IF COLLECTION_ARRAY>[*]</IF COLLECTION_ARRAY><IF COLLECTION_HANDLE>memory handle collection of </IF COLLECTION_HANDLE><IF COLLECTION_ARRAYLIST>ArrayList collection of </IF COLLECTION_ARRAYLIST><IF STRUCTURE>structure </IF STRUCTURE><IF ENUM>enum </IF ENUM><IF STRUCTURE>@<ParameterStructureNoplural><ELSE><PARAMETER_DEFINITION></IF STRUCTURE><IF DATE> <PARAMETER_DATE_FORMAT> date</IF DATE><IF TIME> <PARAMETER_DATE_FORMAT> time</IF TIME><IF REFERENCE> passed by REFERENCE</IF REFERENCE><IF VALUE> passed by VALUE</IF VALUE><IF DATATABLE> returned as DataTable</IF DATATABLE>)

    <IF IN_OR_INOUT>
    <IF COLLECTION>
;//
            argumentDefinition = dispatcher.GetArgumentDataDefForCollection(arguments[<COUNTER_1_VALUE>],<PARAMETER_SIZE><IF IMPLIED>,<PARAMETER_PRECISION></IF>)
            arg<COUNTER_1_VALUE>Array = arguments[<COUNTER_1_VALUE>].GetProperty("PassedValue")
;//
        <IF COLLECTION_ARRAY>
            arg<COUNTER_1_VALUE>Handle = %mem_proc(DM_ALLOC,argumentDefinition.ElementSize*arg<COUNTER_1_VALUE>Array.GetArrayLength())
            arg<COUNTER_1_VALUE>HandlePos = 1
            dispatcher.UnwrapObjectCollection(^m(arg<COUNTER_1_VALUE>Handle),argumentDefinition,arg<COUNTER_1_VALUE>HandlePos,arg<COUNTER_1_VALUE>Array)
        </IF COLLECTION_ARRAY>
;//
        <IF COLLECTION_HANDLE>
            arg<COUNTER_1_VALUE>Handle = %mem_proc(DM_ALLOC,argumentDefinition.ElementSize*arg<COUNTER_1_VALUE>Array.GetArrayLength())
            arg<COUNTER_1_VALUE>HandlePos = 1
            dispatcher.UnwrapObjectCollection(^m(arg<COUNTER_1_VALUE>Handle),argumentDefinition,arg<COUNTER_1_VALUE>HandlePos,arg<COUNTER_1_VALUE>Array)
        </IF COLLECTION_HANDLE>
;//
        <IF COLLECTION_ARRAYLIST>
            arg<COUNTER_1_VALUE> = new ArrayList()
            dispatcher.UnwrapObjectCollection(argumentDefinition,arg<COUNTER_1_VALUE>Array,arg<COUNTER_1_VALUE>,<IF ALPHA>FieldDataType.AlphaField<ELSE DECIMAL>FieldDataType.DecimalField<ELSE IMPLIED>FieldDataType.ImpliedDecimal<ELSE INTEGER>FieldDataType.IntegerField<ELSE STRING>FieldDataType.StringField<ELSE ENUM>FieldDataType.IntegerField<ELSE DATE>FieldDataType.DecimalField<ELSE TIME>FieldDataType.DecimalField<ELSE>FieldDataType.AlphaField</IF>)
        </IF COLLECTION_ARRAYLIST>
    <ELSE>
;//
        <IF ALPHA>
            arg<COUNTER_1_VALUE> = dispatcher.GetText(arguments[<COUNTER_1_VALUE>])
        </IF ALPHA>
;//
        <IF DECIMAL>
            arg<COUNTER_1_VALUE> = dispatcher.GetDecimal(arguments[<COUNTER_1_VALUE>])
        </IF DECIMAL>
;//
        <IF IMPLIED>
            arg<COUNTER_1_VALUE> = dispatcher.GetImplied(arguments[<COUNTER_1_VALUE>])
        </IF IMPLIED>
;//
        <IF INTEGER>
            arg<COUNTER_1_VALUE> = dispatcher.GetInt(arguments[<COUNTER_1_VALUE>])
        </IF INTEGER>
;//
        <IF ENUM>
            arg<COUNTER_1_VALUE> = (<PARAMETER_ENUM>)dispatcher.GetInt(arguments[<COUNTER_1_VALUE>])
        </IF ENUM>
;//
        <IF DATE>
            arg<COUNTER_1_VALUE> = dispatcher.GetDecimal(arguments[<COUNTER_1_VALUE>])
        </IF DATE>
;//
        <IF TIME>
            arg<COUNTER_1_VALUE> = dispatcher.GetDecimal(arguments[<COUNTER_1_VALUE>])
        </IF TIME>
;//
        <IF HANDLE OR BINARY_HANDLE>
            argumentDefinition = dispatcher.GetArgumentDataDefForCollection(arguments[<COUNTER_1_VALUE>])
            arg<COUNTER_1_VALUE>Array = arguments[<COUNTER_1_VALUE>].GetProperty("PassedValue")
            arg<COUNTER_1_VALUE>Handle = %mem_proc(DM_ALLOC,arg<COUNTER_1_VALUE>Array.GetArrayLength())
        </IF>
;//
        <IF STRING>
            arg<COUNTER_1_VALUE> = dispatcher.GetText(arguments[<COUNTER_1_VALUE>])
        </IF STRING>
;//
        <IF STRUCTURE>
            ;;Structure argument. Get the data object then get the record from it
            arg<COUNTER_1_VALUE>DataObject = dispatcher.DeserializeObject(arguments[<COUNTER_1_VALUE>],m<ParameterStructureNoplural>Metadata)
            arg<COUNTER_1_VALUE> = arg<COUNTER_1_VALUE>DataObject.SynergyRecord
        </IF STRUCTURE>
;//
    </IF COLLECTION>
    <ELSE OUT>
        <IF HANDLE OR BINARY_HANDLE>
            arg<COUNTER_1_VALUE>Handle = %mem_proc(DM_ALLOC,4)
        <ELSE COLLECTION>
            <IF COLLECTION_HANDLE OR COLLECTION_ARRAY>
            <IF STRUCTURE>
            arg<COUNTER_1_VALUE>Handle = %mem_proc(DM_ALLOC,<PARAMETER_SIZE>)
            <ELSE>
            arg<COUNTER_1_VALUE>Handle = %mem_proc(DM_ALLOC,1)
            </IF>
            </IF>
        </IF>
    </IF IN_OR_INOUT>
</PARAMETER_LOOP>
;//
;//=========================================================================================================================
;// Make the method call
;//

            ;;------------------------------------------------------------
            ;; Call the underlying routine

            <IF SUBROUTINE>xcall <ELSE>returnValue = %</IF SUBROUTINE><METHOD_ROUTINE>(<COUNTER_1_RESET><PARAMETER_LOOP><COUNTER_1_INCREMENT><IF HANDLE OR BINARY_HANDLE>arg<COUNTER_1_VALUE>Handle<,><ELSE><IF COLLECTION><IF COLLECTION_ARRAY>^m(<IF STRUCTURE>str<ParameterStructureNoplural><ELSE>tempstr<COUNTER_1_VALUE>.arry</IF STRUCTURE>,arg<COUNTER_1_VALUE>Handle)<,></IF COLLECTION_ARRAY><IF COLLECTION_HANDLE>arg<COUNTER_1_VALUE>Handle<,></IF COLLECTION_HANDLE><IF COLLECTION_ARRAYLIST>arg<COUNTER_1_VALUE><,></IF COLLECTION_ARRAYLIST><ELSE>arg<COUNTER_1_VALUE><,></IF COLLECTION></IF></PARAMETER_LOOP>)
;//
;//=========================================================================================================================
;// Build the JSON response
;//
<IF FUNCTION>

            ;;Function return value
  <IF ALPHA>
            serializer.ArgumentData(0,%atrim(returnValue),FieldDataType.AlphaField,<METHOD_RETURN_SIZE>,0,true)
  <ELSE DECIMAL>
            serializer.ArgumentData(0,returnValue,FieldDataType.DecimalField,<METHOD_RETURN_SIZE>,0,false)
  <ELSE IMPLIED>
            serializer.ArgumentData(0,returnValue,FieldDataType.ImpliedDecimalField,<METHOD_RETURN_SIZE>,<METHOD_RETURN_PRECISION>,false)
  <ELSE INTEGER OR HATVAL>
            serializer.ArgumentData(0,returnValue,FieldDataType.IntegerField,<METHOD_RETURN_SIZE>,0,false)
  <ELSE STRING>
            serializer.ArgumentData(0,%atrim(returnValue),FieldDataType.StringField,<METHOD_RETURN_SIZE>,0,false)
  <ELSE ENUM>
            serializer.ArgumentData(0,(int)returnValue,FieldDataType.EnumField,<METHOD_RETURN_SIZE>,0,false)
  </IF>
</IF FUNCTION>
;//
;//Argument processing
;//
<COUNTER_1_RESET>
<PARAMETER_LOOP>
<COUNTER_1_INCREMENT>
<IF OUT_OR_INOUT>

            ;;--------------------------------------------------------------------------------
            ;;Argument <COUNTER_1_VALUE> (<PARAMETER_REQUIRED> <PARAMETER_DIRECTION> <PARAMETER_NAME> <IF COLLECTION_ARRAY>[*]</IF COLLECTION_ARRAY><IF COLLECTION_HANDLE>memory handle collection of </IF COLLECTION_HANDLE><IF COLLECTION_ARRAYLIST>ArrayList collection of </IF COLLECTION_ARRAYLIST><IF STRUCTURE>structure </IF STRUCTURE><IF ENUM>enum </IF ENUM><IF STRUCTURE>@<ParameterStructureNoplural><ELSE><PARAMETER_DEFINITION></IF STRUCTURE><IF DATE> <PARAMETER_DATE_FORMAT> date</IF DATE><IF TIME> <PARAMETER_DATE_FORMAT> time</IF TIME><IF REFERENCE> passed by REFERENCE</IF REFERENCE><IF VALUE> passed by VALUE</IF VALUE><IF DATATABLE> returned as DataTable</IF DATATABLE>)
            
;//
    <IF ALPHA>
        <IF COLLECTION>
            <IF COLLECTION_ARRAY>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.AlphaArrayField,<PARAMETER_SIZE>,0<PARAMETER_PRECISION>,%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,false)
            </IF COLLECTION_ARRAY>
            <IF COLLECTION_ARRAYLIST>
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>,FieldDataType.AlphaArrayField,<PARAMETER_SIZE>,0,false)
            </IF COLLECTION_ARRAYLIST>
            <IF COLLECTION_HANDLE>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.AlphaArrayField,<PARAMETER_SIZE>,0,%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,false)
            </IF COLLECTION_HANDLE>
        <ELSE>
            serializer.ArgumentData(<COUNTER_1_VALUE>,%atrim(arg<COUNTER_1_VALUE>),FieldDataType.AlphaField,<PARAMETER_SIZE>,0,false)
        </IF COLLECTION>
    </IF ALPHA>
;//
    <IF DECIMAL>
        <IF COLLECTION>
            <IF COLLECTION_ARRAY>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.DecimalArrayField,<PARAMETER_SIZE>,0,%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,false)
            </IF COLLECTION_ARRAY>
            <IF COLLECTION_ARRAYLIST>
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>,FieldDataType.DecimalArrayField,<PARAMETER_SIZE>,0,false)
            </IF COLLECTION_ARRAYLIST>
            <IF COLLECTION_HANDLE>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.DecimalArrayField,<PARAMETER_SIZE>,0,%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,false)
            </IF COLLECTION_HANDLE>
        <ELSE>
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>,FieldDataType.DecimalField,<PARAMETER_SIZE>,0,false)
        </IF COLLECTION>
    </IF DECIMAL>
;//
    <IF IMPLIED>
        <IF COLLECTION>
            <IF COLLECTION_ARRAY>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.ImpliedDecimalArrayField,<PARAMETER_SIZE>,<PARAMETER_PRECISION>,%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,false)
            </IF COLLECTION_ARRAY>
            <IF COLLECTION_ARRAYLIST>
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>,FieldDataType.ImpliedDecimalArrayField,<PARAMETER_SIZE>,<PARAMETER_PRECISION>,false)
            </IF COLLECTION_ARRAYLIST>
            <IF COLLECTION_HANDLE>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.ImpliedDecimalArrayField,<PARAMETER_SIZE>,<PARAMETER_PRECISION>,%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,false)
            </IF COLLECTION_HANDLE>
        <ELSE>
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>,FieldDataType.ImpliedDecimalField,<PARAMETER_SIZE>,<PARAMETER_PRECISION>,false)
        </IF COLLECTION>
    </IF IMPLIED>
;//
    <IF INTEGER>
        <IF COLLECTION>
            <IF COLLECTION_ARRAY>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.IntegerArrayField,<PARAMETER_SIZE>,0,%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,false)
            </IF COLLECTION_ARRAY>
            <IF COLLECTION_ARRAYLIST>
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>,FieldDataType.IntegerArrayField,<PARAMETER_SIZE>,0,false)
            </IF COLLECTION_ARRAYLIST>
            <IF COLLECTION_HANDLE>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.IntegerArrayField,<PARAMETER_SIZE>,0,%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,false)
            </IF COLLECTION_HANDLE>
        <ELSE>
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>,FieldDataType.IntegerField,<PARAMETER_SIZE>,0,false)
        </IF COLLECTION>
    </IF INTEGER>
;//
    <IF ENUM>
            ;TODO: Do we need custom processing for enum fields beyond the integer value?
            serializer.ArgumentData(<COUNTER_1_VALUE>,(int)arg<COUNTER_1_VALUE>,FieldDataType.IntegerField,<PARAMETER_SIZE>,0,false)
    </IF ENUM>
;//
    <IF DATE>
        <IF COLLECTION>
            <IF COLLECTION_ARRAY>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.DecimalArrayField,<PARAMETER_SIZE>,0,%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,false)
            </IF COLLECTION_ARRAY>
            <IF COLLECTION_ARRAYLIST>
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>,FieldDataType.DecimalArrayField,<PARAMETER_SIZE>,0,false)
            </IF COLLECTION_ARRAYLIST>
            <IF COLLECTION_HANDLE>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.DecimalArrayField,<PARAMETER_SIZE>,0,%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,false)
            </IF COLLECTION_HANDLE>
        <ELSE>
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>,FieldDataType.DecimalField,<PARAMETER_SIZE>,0,false)
        </IF COLLECTION>
    </IF DATE>
;//
    <IF TIME>
        <IF COLLECTION>
            <IF COLLECTION_ARRAY>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.DecimalArrayField,<PARAMETER_SIZE>,0,%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,false)
            </IF COLLECTION_ARRAY>
            <IF COLLECTION_ARRAYLIST>
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>,FieldDataType.DecimalArrayField,<PARAMETER_SIZE>,0,false)
            </IF COLLECTION_ARRAYLIST>
            <IF COLLECTION_HANDLE>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.DecimalArrayField,<PARAMETER_SIZE>,0,%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,false)
            </IF COLLECTION_HANDLE>
        <ELSE>
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>,FieldDataType.DecimalField,<PARAMETER_SIZE>,0,false)
        </IF COLLECTION>
    </IF TIME>
;//
    <IF HANDLE OR BINARY_HANDLE>
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.IntegerField,4,0,false)
    </IF>
;//
    <IF STRING>
        <IF COLLECTION>
            <IF COLLECTION_ARRAY>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.StringArrayField,<PARAMETER_SIZE>,0,%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,false)
            </IF COLLECTION_ARRAY>
            <IF COLLECTION_ARRAYLIST>
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>,FieldDataType.StringArrayField,<PARAMETER_SIZE>,0,false)
            </IF COLLECTION_ARRAYLIST>
            <IF COLLECTION_HANDLE>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.StringArrayField,<PARAMETER_SIZE>,0,%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,false)
            </IF COLLECTION_HANDLE>
        <ELSE>
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>,FieldDataType.StringField)
        </IF COLLECTION>
    </IF STRING>
;//
;//Start of structure parameter processing
;//
    <IF STRUCTURE>
        <IF COLLECTION>
;//
;//Structure collection processing
;//
;//
;//Structure array processing
;//
        <IF COLLECTION_ARRAY>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.DataObjectCollectionField,<PARAMETER_SIZE>,"<PARAMETER_STRUCTURE>",%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,true)
        </IF COLLECTION_ARRAY>
;//
;//Structure memory handle collection processing
;//
        <IF COLLECTION_HANDLE>
            serializer.ArgumentHandleData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>Handle,FieldDataType.DataObjectCollectionField,<PARAMETER_SIZE>,"<PARAMETER_STRUCTURE>",%mem_proc(DM_GETSIZE,arg<COUNTER_1_VALUE>Handle)/<PARAMETER_SIZE>,true)
        </IF COLLECTION_HANDLE>
;//
;//Structure ArrayList processing
;//
        <IF COLLECTION_ARRAYLIST>
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>,FieldDataType.DataObjectCollectionField,<PARAMETER_SIZE>,"<PARAMETER_STRUCTURE>",true)
        </IF COLLECTION_ARRAYLIST>
;//
;//End of structure collection processing
;//
        <ELSE>
;//
;//Single structure processing
;//
            ;;Argument <COUNTER_1_VALUE>: Single <ParameterStructureNoplural> record
            serializer.ArgumentData(<COUNTER_1_VALUE>,arg<COUNTER_1_VALUE>,FieldDataType.DataObjectField,<PARAMETER_SIZE>,"<PARAMETER_STRUCTURE>",true)
        </IF COLLECTION>
;//
    </IF STRUCTURE>
</IF OUT_OR_INOUT>
</PARAMETER_LOOP>
        endmethod

    endclass
</METHOD_LOOP>

endnamespace

;; This is here to ensure that the <MODELS_NAMESPACE> namespace exists.
;; If the Synergy methods don't expsoe any structure or collection of structure
;; parameters then there won't be anything in the Models folder, and the import above will fail.
namespace <MODELS_NAMESPACE>
    public class DummyModel
    endclass
endnamespace

