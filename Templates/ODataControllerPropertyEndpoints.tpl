<CODEGEN_FILENAME><StructurePlural>ControllerPropertyEndpoints.dbl</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.5.2</REQUIRES_CODEGEN_VERSION>
;//****************************************************************************
;//
;// Title:       ODataControllerPropertyEndpoints.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Used to create a partial class that adds endpoints for
;//              properties to an OData controller.
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
;; Title:       <StructurePlural>ControllerPropertyEndpoints.dbl
;;
;; Description: Adds individual property endpoints to <StructurePlural>Controller.
;;
;;*****************************************************************************
;; WARNING: GENERATED CODE!
;; This file was generated by CodeGen. Avoid editing the file if possible.
;; Any changes you make will be lost of the file is re-generated.
;;*****************************************************************************

import Microsoft.AspNet.OData
import Microsoft.AspNet.OData.Routing
import Microsoft.AspNetCore.Http
import Microsoft.AspNetCore.Mvc

namespace <NAMESPACE>

    public partial class <StructurePlural>Controller

<IF PROPERTY_ENDPOINTS>
;//
;// In order for the $value function to work in conjunction with these properties,
;// the name of a single key segment MUST be "key"!!! Likely doesn't work with segmented keys.
;//
  <FIELD_LOOP>
    <IF NOT USER AND CUSTOM_NOT_HARMONY_EXCLUDE>
;//
;// ISAM - onli if the structure has a unique primary key, generate properties for all fields that are NOT primary key segments.
;//
        <IF STRUCTURE_ISAM AND STRUCTURE_HAS_UNIQUE_PK AND NOTPKSEGMENT>
          <PRIMARY_KEY>
        {ODataRoute("(<IF SINGLE_SEGMENT>{key}<ELSE><SEGMENT_LOOP><IF NOT SEG_TAG_EQUAL><FieldSqlName>={a<FieldSqlName>}<SEGMENT_COMMA_NOT_LAST_NORMAL_FIELD></IF SEG_TAG_EQUAL></SEGMENT_LOOP></IF SINGLE_SEGMENT>)/<FieldSqlName>")}
        {Produces("application/json")}
        {ProducesResponseType(StatusCodes.Status200OK)}
            <IF DEFINED_ENABLE_AUTHENTICATION>
        {ProducesResponseType(StatusCodes.Status401Unauthorized)}
            </IF DEFINED_ENABLE_AUTHENTICATION>
        {ProducesResponseType(StatusCodes.Status404NotFound)}
            <IF DEFINED_ENABLE_AUTHENTICATION AND USERTOKEN_ROLES_GET>
        {Authorize(Roles="<ROLES_GET>")}
            </IF DEFINED_ENABLE_AUTHENTICATION>
        ;;; <summary>
        ;;; Get the <FieldSqlName> property of a single <StructureNoplural>, by primary key.
        ;;; </summary>
            <IF SINGLE_SEGMENT>
        ;;; <param name="key"><FIELD_DESC></param>
            <ELSE>
              <SEGMENT_LOOP>
                <IF NOT SEG_TAG_EQUAL>
        ;;; <param name="a<FieldSqlName>"><FIELD_DESC></param>
                </IF SEG_TAG_EQUAL>
              </SEGMENT_LOOP>
            </IF SINGLE_SEGMENT>
        ;;; <returns>
        ;;; Returns <IF ALPHA>a string</IF ALPHA><IF DECIMAL><IF PRECISION>a decimal<ELSE><IF CUSTOM_HARMONY_AS_STRING>a string<ELSE>an int</IF CUSTOM_HARMONY_AS_STRING></IF PRECISION></IF DECIMAL><IF DATE>a DateTime</IF DATE><IF TIME>a DateTime</IF TIME><IF INTEGER>an int</IF INTEGER> containing the value of the requested property.
        ;;;</returns>
        public method Get<FieldSqlName>, @IActionResult
            <SEGMENT_LOOP>
              <IF SINGLE_SEGMENT>
            {FromODataUri}
                <IF CUSTOM_HARMONY_AS_STRING>
            required in key, string
                 <ELSE>
            required in key, <HARMONYCORE_SEGMENT_DATATYPE>
                </IF CUSTOM_HARMONY_AS_STRING>
              <ELSE>
                <IF NOT SEG_TAG_EQUAL>
            {FromODataUri}
                  <IF CUSTOM_HARMONY_AS_STRING>
            required in a<FieldSqlName>, string
                  <ELSE>
            required in a<FieldSqlName>, <HARMONYCORE_SEGMENT_DATATYPE>
                  </IF CUSTOM_HARMONY_AS_STRING>
                </IF SEG_TAG_EQUAL>
              </IF SINGLE_SEGMENT>
            </SEGMENT_LOOP>
        proc
            data result = _DbContext.<StructurePlural>.Find(<IF SINGLE_SEGMENT>key<ELSE><SEGMENT_LOOP><IF SEG_TAG_EQUAL><SEGMENT_TAG_VALUE><ELSE>a<FieldSqlName><IF HARMONYCORE_CUSTOM_SEGMENT_DATATYPE><ELSE><IF ALPHA>.PadRight(<FIELD_SIZE>)</IF ALPHA></IF HARMONYCORE_CUSTOM_SEGMENT_DATATYPE></IF SEG_TAG_EQUAL><,></SEGMENT_LOOP></IF SINGLE_SEGMENT>)
            if (result==^null)
                mreturn NotFound()
            mreturn OK(result.<FieldSqlName>)
        endmethod
          </PRIMARY_KEY>
      </IF STRUCTURE_ISAM>
;//
;// RELATIVE
;//
        <IF STRUCTURE_RELATIVE>
        {ODataRoute("({key})}
        {Produces("application/json")}
        {ProducesResponseType(StatusCodes.Status200OK)}
          <IF DEFINED_ENABLE_AUTHENTICATION>
        {ProducesResponseType(StatusCodes.Status401Unauthorized)}
          </IF DEFINED_ENABLE_AUTHENTICATION>
        {ProducesResponseType(StatusCodes.Status404NotFound)}
          <IF DEFINED_ENABLE_AUTHENTICATION AND USERTOKEN_ROLES_GET>
        {Authorize(Roles="<ROLES_GET>")}
          </IF DEFINED_ENABLE_AUTHENTICATION>
        ;;; <summary>
        ;;; Get the <FieldSqlName> property of a single <StructureNoplural>, by record number.
        ;;; </summary>
        ;;; <param name="key">Record number</param>
        ;;; <returns>
        ;;; Returns <IF ALPHA>a string</IF ALPHA><IF DECIMAL><IF PRECISION>a decimal<ELSE><IF CUSTOM_HARMONY_AS_STRING>a string<ELSE>an int</IF CUSTOM_HARMONY_AS_STRING></IF PRECISION></IF DECIMAL><IF DATE>a DateTime</IF DATE><IF TIME>a DateTime</IF TIME><IF INTEGER>an int</IF INTEGER> containing the value of the requested property.
        ;;;</returns>
        public method Get<FieldSqlName>, @IActionResult
            {FromODataUri}
            required in key, int
        proc
            data result = _DbContext.<StructurePlural>.Find(key)
            if (result==^null)
                mreturn NotFound()
            mreturn OK(result.<FieldSqlName>)
        endmethod
        </IF STRUCTURE_RELATIVE>

    </IF USER>
  </FIELD_LOOP>
</IF PROPERTY_ENDPOINTS>

    endclass

endnamespace