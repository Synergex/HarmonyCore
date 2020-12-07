<CODEGEN_FILENAME><StructurePlural>Controller.dbl</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.5.3</REQUIRES_CODEGEN_VERSION>
<REQUIRES_USERTOKEN>MODELS_NAMESPACE</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>SERVICES_NAMESPACE</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>API_ENABLE_QUERY_PARAMS</REQUIRES_USERTOKEN>
;//****************************************************************************
;//
;// Title:       ODataController.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Used to create OData Controllers in a Harmony Core environment
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
;; Title:       <StructurePlural>Controller.dbl
;;
;; Description: OData controller for the <STRUCTURE_NOALIAS> structure.
;;
;;*****************************************************************************
;; WARNING: GENERATED CODE!
;; This file was generated by CodeGen. Avoid editing the file if possible.
;; Any changes you make will be lost of the file is re-generated.
;;*****************************************************************************

<IF DEFINED_ENABLE_AUTHENTICATION>
import Microsoft.AspNetCore.Authorization
</IF DEFINED_ENABLE_AUTHENTICATION>
import Microsoft.AspNetCore.Http
import Microsoft.OData
import Microsoft.AspNetCore.JsonPatch
import Microsoft.AspNetCore.Mvc
import Microsoft.AspNet.OData
import Microsoft.AspNet.OData.Routing
import Microsoft.EntityFrameworkCore
import Microsoft.EntityFrameworkCore.Infrastructure
import Microsoft.Extensions.Options
import System.Collections.Generic
import System.ComponentModel.DataAnnotations
import Harmony.Core.EF.Extensions
import Harmony.Core.Interface
import Harmony.OData
import Harmony.AspNetCore
import Newtonsoft.Json
import <MODELS_NAMESPACE>

namespace <NAMESPACE>

<IF DEFINED_ENABLE_AUTHENTICATION>
    {Authorize}
</IF DEFINED_ENABLE_AUTHENTICATION>
    {ApiVersion("<API_VERSION>")}
    {ODataRoutePrefix("<StructurePlural>")}
    ;;; <summary>
    ;;; OData controller for <StructurePlural>
    ;;; </summary>
    public partial class <StructurePlural>Controller extends ODataController
    
        ;;Services provided via dependency injection
        private _DbContext, @<MODELS_NAMESPACE>.DBContext
        private _ServiceProvider, @IServiceProvider
        private _AppSettings, @IOptions<AppSettings>

        ;;; <summary>
        ;;; Constructs a new instance of <StructurePlural>Controller
        ;;; </summary>
        ;;; <param name="aDbContext">Database context instance (DI)</param>
        ;;; <param name="aServiceProvider">Service provider instance (DI)</param>
        ;;; <param name="aAppSettings">Application settings</param>
        public method <StructurePlural>Controller
            aDbContext, @<MODELS_NAMESPACE>.DBContext
            aServiceProvider, @IServiceProvider
            aAppSettings, @IOptions<AppSettings>
        proc
            this._DbContext = aDbContext
            this._ServiceProvider = aServiceProvider
            this._AppSettings = aAppSettings
        endmethod

;//
;// GET ALL -------------------------------------------------------------------
;//
<IF DEFINED_ENABLE_GET_ALL AND GET_ALL_ENDPOINT>
        {ODataRoute}
        {Produces("application/json")}
        {ProducesResponseType(^typeof(ODataValue<IEnumerable<<StructureNoplural>>>),StatusCodes.Status200OK)}
  <IF DEFINED_ENABLE_AUTHENTICATION AND USERTOKEN_ROLES_GET>
        {Authorize(Roles="<ROLES_GET>")}
  </IF DEFINED_ENABLE_AUTHENTICATION_AND_USERTOKEN_ROLES_GET>
  <IF DEFINED_ENABLE_FIELD_SECURITY>
        {HarmonyFieldSecurity<API_ENABLE_QUERY_PARAMS>}
  <ELSE>
        {EnableQuery<API_ENABLE_QUERY_PARAMS>}
  </IF DEFINED_ENABLE_FIELD_SECURITY>
        ;;; <summary>
        ;;; Get all <StructurePlural>
        ;;; </summary>
        ;;; <returns>Returns an IActionResult indicating the status of the operation and containing any data that was returned.</returns>
        public method Get<StructurePlural>, @IActionResult
        proc
            mreturn Ok(_DbContext.<StructurePlural>.AsNoTracking())
        endmethod

</IF DEFINED_ENABLE_GET_ALL_AND_GET_ALL_ENDPOINT>
;//
;// GET ONE (ISAM, UNIQUE PRIMARY KEY READ) -----------------------------------
;//
<IF STRUCTURE_ISAM AND STRUCTURE_HAS_UNIQUE_PK AND DEFINED_ENABLE_GET_ONE AND GET_ENDPOINT>
        {ODataRoute("(<PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>={a<FieldSqlName>}<,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY>)")}
        {Produces("application/json")}
        {ProducesResponseType(^typeof(<StructureNoplural>),StatusCodes.Status200OK)}
        {ProducesResponseType(StatusCodes.Status404NotFound)}
  <IF DEFINED_ENABLE_AUTHENTICATION AND USERTOKEN_ROLES_GET>
        {Authorize(Roles="<ROLES_GET>")}
  </IF DEFINED_ENABLE_AUTHENTICATION>
  <IF DEFINED_ENABLE_FIELD_SECURITY>
        {HarmonyFieldSecurity<API_ENABLE_QUERY_PARAMS>}
  <ELSE>
        {EnableQuery<API_ENABLE_QUERY_PARAMS>}
  </IF DEFINED_ENABLE_FIELD_SECURITY>
        ;;; <summary>
        ;;; Get a single <StructureNoplural> by primary key.
        ;;; </summary>
  <PRIMARY_KEY>
    <SEGMENT_LOOP>
      <IF NOT SEG_TAG_EQUAL>
        ;;; <param name="a<FieldSqlName>"><FIELD_DESC></param>
      </IF>
    </SEGMENT_LOOP>
  </PRIMARY_KEY>
        ;;; <returns>Returns a SingleResult indicating the status of the operation and containing any data that was returned.</returns>
        public method Get<StructureNoplural>, @SingleResult<<StructureNoplural>>
  <PRIMARY_KEY>
    <SEGMENT_LOOP>
      <IF NOT SEG_TAG_EQUAL>
            {FromODataUri}
        <IF CUSTOM_HARMONY_AS_STRING>
            required in a<FieldSqlName>, string
        <ELSE>
            required in a<FieldSqlName>, <HARMONYCORE_SEGMENT_DATATYPE>
        </IF CUSTOM_HARMONY_AS_STRING>
      </IF>
    </SEGMENT_LOOP>
  </PRIMARY_KEY>
        proc
;//Shouldn't really need the generic type arg on FindQuery. Compiler issue?
            mreturn new SingleResult<<StructureNoplural>>(_DbContext.<StructurePlural>.AsNoTracking().FindQuery<<StructureNoplural>>(_DbContext, <PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><SEGMENT_TAG_VALUE><ELSE>a<FieldSqlName><IF HARMONYCORE_CUSTOM_SEGMENT_DATATYPE><ELSE><IF ALPHA>.PadRight(<FIELD_SIZE>)</IF ALPHA></IF HARMONYCORE_CUSTOM_SEGMENT_DATATYPE></IF SEG_TAG_EQUAL><,></SEGMENT_LOOP></PRIMARY_KEY>))
        endmethod

</IF STRUCTURE_ISAM>
;//
;// GET "ONE" (not in this case!) (ISAM, NON-UNIQUE PRIMARY KEY READ) ---------
;//
<IF STRUCTURE_ISAM AND NOT STRUCTURE_HAS_UNIQUE_PK AND DEFINED_ENABLE_GET_ONE AND GET_ENDPOINT>
        {ODataRoute("(<PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>={a<FieldSqlName>}<,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY>)")}
        {Produces("application/json")}
        {ProducesResponseType(^typeof(ODataValue<IEnumerable<<StructureNoplural>>>),StatusCodes.Status200OK)}
  <IF DEFINED_ENABLE_AUTHENTICATION AND USERTOKEN_ROLES_GET>
        {Authorize(Roles="<ROLES_GET>")}
  </IF DEFINED_ENABLE_AUTHENTICATION>
  <IF DEFINED_ENABLE_FIELD_SECURITY>
        {HarmonyFieldSecurity<API_ENABLE_QUERY_PARAMS>}
  <ELSE>
        {EnableQuery<API_ENABLE_QUERY_PARAMS>}
  </IF DEFINED_ENABLE_FIELD_SECURITY>
        ;;; <summary>
        ;;; Get all <StructurePlural> matching non-unique primary key.
        ;;; </summary>
  <PRIMARY_KEY>
    <SEGMENT_LOOP>
      <IF NOT SEG_TAG_EQUAL>
        ;;; <param name="a<FieldSqlName>"><FIELD_DESC></param>
      </IF>
    </SEGMENT_LOOP>
  </PRIMARY_KEY>
        ;;; <returns>Returns a collection of any <StructurePlural> matching non-unique primary key, or an empty collection if no matching records are found.</returns>
        public method Get<StructureNoplural>, @IActionResult
  <PRIMARY_KEY>
    <SEGMENT_LOOP>
      <IF NOT SEG_TAG_EQUAL>
            {FromODataUri}
        <IF CUSTOM_HARMONY_AS_STRING>
            required in a<FieldSqlName>, string
        <ELSE>
            required in a<FieldSqlName>, <HARMONYCORE_SEGMENT_DATATYPE>
        </IF CUSTOM_HARMONY_AS_STRING>
      </IF>
    </SEGMENT_LOOP>
  </PRIMARY_KEY>
        proc
;//Shouldn't really need the generic type arg on FindQuery. Compiler issue?
            mreturn Ok(_DbContext.<StructurePlural>.AsNoTracking().FindQuery<<StructureNoplural>>(_DbContext, <PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><SEGMENT_TAG_VALUE><ELSE>a<FieldSqlName><IF HARMONYCORE_CUSTOM_SEGMENT_DATATYPE><ELSE><IF ALPHA>.PadRight(<FIELD_SIZE>)</IF ALPHA></IF HARMONYCORE_CUSTOM_SEGMENT_DATATYPE></IF SEG_TAG_EQUAL><,></SEGMENT_LOOP></PRIMARY_KEY>))
        endmethod

</IF STRUCTURE_ISAM>
;//
;// GET ONE (RELATIVE FILE RECORD NUMBER READ) --------------------------------
;//
<IF STRUCTURE_RELATIVE AND DEFINED_ENABLE_GET_ONE AND GET_ENDPOINT>
        {ODataRoute("(aRecordNumber)")}
        {Produces("application/json")}
        {ProducesResponseType(^typeof(<StructureNoplural>),StatusCodes.Status200OK)}
        {ProducesResponseType(StatusCodes.Status404NotFound)}
  <IF DEFINED_ENABLE_AUTHENTICATION AND USERTOKEN_ROLES_GET>
        {Authorize(Roles="<ROLES_GET>")}
  </IF DEFINED_ENABLE_AUTHENTICATION>
  <IF DEFINED_ENABLE_FIELD_SECURITY>
        {HarmonyFieldSecurity<API_ENABLE_QUERY_PARAMS>}
  <ELSE>
        {EnableQuery<API_ENABLE_QUERY_PARAMS>}
  </IF DEFINED_ENABLE_FIELD_SECURITY>
        ;;; <summary>
        ;;; Get a single <StructureNoplural> by relative record number.
        ;;; </summary>
        ;;; <param name="aRecordNumber">Record number</param>
        ;;; <returns>Returns a SingleResult indicating the status of the operation and containing any data that was returned.</returns>
        public method Get<StructureNoplural>, @SingleResult<<StructureNoplural>>
            {FromODataUri}
            required in aRecordNumber, int
        proc
;//Shouldn't really need the generic type arg on FindQuery. Compiler issue?
            mreturn new SingleResult<<StructureNoplural>>(_DbContext.<StructurePlural>.AsNoTracking().FindQuery<<StructureNoplural>>(_DbContext, aRecordNumber))
        endmethod

</IF STRUCTURE_RELATIVE>
;//
;// GET BY ALTERNATE KEY ------------------------------------------------------
;//
<IF STRUCTURE_ISAM AND DEFINED_ENABLE_ALTERNATE_KEYS AND ALTERNATE_KEY_ENDPOINTS> 
  <ALTERNATE_KEY_LOOP_UNIQUE>
    <IF DUPLICATES>
        {ODataRoute("(<SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>={a<FieldSqlName>}<,></IF SEG_TAG_EQUAL></SEGMENT_LOOP>)")}
        {Produces("application/json")}
        {ProducesResponseType(^typeof(ODataValue<IEnumerable<<StructureNoplural>>>),StatusCodes.Status200OK)}
        {ProducesResponseType(StatusCodes.Status404NotFound)}
      <IF DEFINED_ENABLE_AUTHENTICATION AND USERTOKEN_ROLES_GET>
        {Authorize(Roles="<ROLES_GET>")}
      </IF DEFINED_ENABLE_AUTHENTICATION>
      <IF DEFINED_ENABLE_FIELD_SECURITY>
        {HarmonyFieldSecurity<API_ENABLE_QUERY_PARAMS>}
      <ELSE>
        {EnableQuery<API_ENABLE_QUERY_PARAMS>}
      </IF DEFINED_ENABLE_FIELD_SECURITY>
        ;;; <summary>
        ;;; Get <structurePlural> by alternate key key <KeyName>.
        ;;; </summary>
      <SEGMENT_LOOP>
        <IF NOT SEG_TAG_EQUAL>
        ;;; <param name="a<FieldSqlName>"><FIELD_DESC></param>
        </IF>
      </SEGMENT_LOOP>
        ;;; <returns>Returns an IActionResult indicating the status of the operation and containing any data that was returned.</returns>
        public method Get<StructurePlural>By<KeyName>, @IActionResult
      <SEGMENT_LOOP>
        <IF NOT SEG_TAG_EQUAL>
            {FromODataUri}
          <IF CUSTOM_HARMONY_AS_STRING>
            required in a<FieldSqlName>, string
          <ELSE>
            required in a<FieldSqlName>, <HARMONYCORE_SEGMENT_DATATYPE>
          </IF CUSTOM_HARMONY_AS_STRING>
        </IF>
      </SEGMENT_LOOP>
        proc
            data result = _DbContext.<StructurePlural>.AsNoTracking().FindAlternate(<SEGMENT_LOOP>"<FieldSqlName>",<IF SEG_TAG_EQUAL><SEGMENT_TAG_VALUE><ELSE>a<FieldSqlName></IF SEG_TAG_EQUAL><,></SEGMENT_LOOP>)
            if (result == ^null)
                mreturn NotFound()
            mreturn Ok(result)
        endmethod
    <ELSE>
        {ODataRoute("(<SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>={a<FieldSqlName>}<,></IF SEG_TAG_EQUAL></SEGMENT_LOOP>)")}
        {Produces("application/json")}
        {ProducesResponseType(^typeof(<StructureNoplural>),StatusCodes.Status200OK)}
        {ProducesResponseType(StatusCodes.Status404NotFound)}
      <IF DEFINED_ENABLE_AUTHENTICATION AND USERTOKEN_ROLES_GET>
        {Authorize(Roles="<ROLES_GET>")}
      </IF DEFINED_ENABLE_AUTHENTICATION>
      <IF DEFINED_ENABLE_FIELD_SECURITY>
        {HarmonyFieldSecurity<API_ENABLE_QUERY_PARAMS>}
      <ELSE>
        {EnableQuery<API_ENABLE_QUERY_PARAMS>}
      </IF DEFINED_ENABLE_FIELD_SECURITY>
        ;;; <summary>
        ;;; Get <structureNoplural> by alternate key <KeyName>.
        ;;; </summary>
      <SEGMENT_LOOP>
        <IF NOT SEG_TAG_EQUAL>
        ;;; <param name="a<FieldSqlName>"><FIELD_DESC></param>
        </IF>
      </SEGMENT_LOOP>
        ;;; <returns>Returns a SingleResult indicating the status of the operation and containing any data that was returned.</returns>
        public method Get<StructureNoplural>By<KeyName>, @SingleResult<<StructureNoplural>>
      <SEGMENT_LOOP>
        <IF NOT SEG_TAG_EQUAL>
            {FromODataUri}
          <IF CUSTOM_HARMONY_AS_STRING>
            required in a<FieldSqlName>, string
          <ELSE>
            required in a<FieldSqlName>, <HARMONYCORE_SEGMENT_DATATYPE>
          </IF CUSTOM_HARMONY_AS_STRING>
        </IF>
      </SEGMENT_LOOP>
        proc
            mreturn new SingleResult<<StructureNoplural>>(_DbContext.<StructurePlural>.AsNoTracking().FindAlternate(<SEGMENT_LOOP>"<FieldSqlName>",<IF SEG_TAG_EQUAL><SEGMENT_TAG_VALUE><ELSE>a<FieldSqlName><IF HARMONYCORE_CUSTOM_SEGMENT_DATATYPE><ELSE><IF ALPHA>.PadRight(<FIELD_SIZE>)</IF ALPHA></IF HARMONYCORE_CUSTOM_SEGMENT_DATATYPE></IF SEG_TAG_EQUAL><,></SEGMENT_LOOP>))
        endmethod
    </IF DUPLICATES>

  </ALTERNATE_KEY_LOOP_UNIQUE>
</IF STRUCTURE_ISAM>
;//
;// POST ----------------------------------------------------------------------
;//
<IF STRUCTURE_ISAM AND DEFINED_ENABLE_POST AND POST_ENDPOINT AND STRUCTURE_HAS_UNIQUE_PK>
  <IF DEFINED_ENABLE_AUTHENTICATION>
    <IF USERTOKEN_ROLES_POST>
        {Authorize(Roles="<ROLES_POST>")}
    </IF USERTOKEN_ROLES_POST>
  </IF DEFINED_ENABLE_AUTHENTICATION>
        {ODataRoute}
        {Produces("application/json")}
        {ProducesResponseType(^typeof(<StructureNoplural>),StatusCodes.Status200OK)}
        {ProducesResponseType(StatusCodes.Status400BadRequest)}
        {HttpPost}
        ;;; <summary>
        ;;; Create a new <structureNoplural> (automatically assigned primary key).
        ;;; </summary>
        ;;; <returns>Returns an IActionResult indicating the status of the operation and containing any data that was returned.</returns>
        public method Post<StructureNoplural>, @IActionResult
            {FromBody}
            required in a<StructureNoplural>, @<StructureNoplural>
        proc
            ;;Remove the primary key fields from ModelState
;//
;// ISAM
;//
    <PRIMARY_KEY>
      <SEGMENT_LOOP>
            ModelState.Remove("<FieldSqlName>")
      </SEGMENT_LOOP>
    </PRIMARY_KEY>

            ;; Validate inbound data
            if (!ModelState.IsValid)
                mreturn ValidationHelper.ReturnValidationError(ModelState)

            ;;Get the next available primary key value
            disposable data keyFactory = (@IPrimaryKeyFactory)_ServiceProvider.GetService(^typeof(IPrimaryKeyFactory))
            KeyFactory.AssignPrimaryKey(a<StructureNoplural>)

            ;;Add the new <structureNoplural>
            try
            begin
                _DbContext.<StructurePlural>.Add(a<StructureNoplural>)
                _DbContext.SaveChanges(keyFactory)
            end
            catch (e, @ValidationException)
            begin
                ModelState.AddModelError("RelationValidation",e.Message)
                mreturn ValidationHelper.ReturnValidationError(ModelState)
            end
            endtry

            mreturn Created(a<StructureNoplural>)

        endmethod

</IF STRUCTURE_ISAM>
;//
;// PUT (By non-unique primary key, if no unique key exists)-------------------
;//
<IF STRUCTURE_ISAM AND DEFINED_ENABLE_PUT AND PUT_ENDPOINT>
  <PRIMARY_KEY>
      <IF DEFINED_ENABLE_AUTHENTICATION AND USERTOKEN_ROLES_PUT>
        {Authorize(Roles="<ROLES_PUT>")}
      </IF DEFINED_ENABLE_AUTHENTICATION>
        {ODataRoute("(<SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>={a<FieldSqlName>}<,></IF SEG_TAG_EQUAL></SEGMENT_LOOP>)")}
        {Produces("application/json")}
        {ProducesResponseType(StatusCodes.Status201Created)}
        {ProducesResponseType(StatusCodes.Status400BadRequest)}
        {ProducesResponseType(StatusCodes.Status404NotFound)}
        {HttpPut}
        ;;; <summary>
        ;;; Create (with a client-supplied primary key) or replace a <structureNoplural>.
        ;;; </summary>

      <SEGMENT_LOOP>
        <IF NOT SEG_TAG_EQUAL>
        ;;; <param name="a<FieldSqlName>"><FIELD_DESC></param>
        </IF>
      </SEGMENT_LOOP>
        ;;; <returns>Returns an IActionResult indicating the status of the operation and containing any data that was returned.</returns>
        public method Put<StructureNoplural><IF NOT FIRST_UNIQUE_KEY>By<KeyName></IF>, @IActionResult
      <SEGMENT_LOOP>
        <IF NOT SEG_TAG_EQUAL>
            {FromODataUri}
          <IF CUSTOM_HARMONY_AS_STRING>
            required in a<FieldSqlName>, string
          <ELSE>
            required in a<FieldSqlName>, <HARMONYCORE_SEGMENT_DATATYPE>
          </IF CUSTOM_HARMONY_AS_STRING>
        </IF>
      </SEGMENT_LOOP>
            {FromBody}
            required in a<StructureNoplural>, @<StructureNoplural>
        proc
        <IF NOT STRUCTURE_HAS_UNIQUE_KEY>
            ;;Ensure that the key values in the URI win over any data that may be in the model object
      <SEGMENT_LOOP>
        <IF SEG_TAG_EQUAL>
            a<StructureNoplural>.<FieldSqlname> = <SEGMENT_TAG_VALUE>
        <ELSE>
            a<StructureNoplural>.<FieldSqlname> = a<FieldSqlName>
        </IF SEG_TAG_EQUAL>
            ModelState.Remove("<FieldSqlname>")
      </SEGMENT_LOOP>
        </IF>

            ;; Validate inbound data
            if (!ModelState.IsValid)
                mreturn ValidationHelper.ReturnValidationError(ModelState)

        <IF STRUCTURE_HAS_UNIQUE_KEY>
            ;;Ensure that the key values in the URI win over any data that may be in the model object
      <SEGMENT_LOOP>
        <IF SEG_TAG_EQUAL>
            a<StructureNoplural>.<FieldSqlname> = <SEGMENT_TAG_VALUE>
        <ELSE>
            a<StructureNoplural>.<FieldSqlname> = a<FieldSqlName>
        </IF SEG_TAG_EQUAL>
      </SEGMENT_LOOP>
        </IF STRUCTURE_HAS_UNIQUE_KEY>

            <IF STRUCTURE_HAS_UNIQUE_KEY>
            try
            begin
                ;;Add and commit
                data existing = _DbContext.<StructurePlural>.Find(<SEGMENT_LOOP><IF SEG_TAG_EQUAL><SEGMENT_TAG_VALUE><ELSE>a<FieldSqlName><IF HARMONYCORE_CUSTOM_SEGMENT_DATATYPE><ELSE><IF ALPHA>.PadRight(<FIELD_SIZE>)</IF ALPHA></IF HARMONYCORE_CUSTOM_SEGMENT_DATATYPE></IF SEG_TAG_EQUAL><,></SEGMENT_LOOP>)
                if(existing == ^null) then
                begin
                    _DbContext.<StructurePlural>.Add(a<StructureNoplural>)
                    _DbContext.SaveChanges()
                    mreturn Created(a<StructureNoplural>)
                end
                else
                begin
                    a<StructureNoplural>.CopyTo(existing)
                    _DbContext.SaveChanges()
                    mreturn NoContent()
                end
            end
            <ELSE>
            try
            begin
                _DbContext.<StructurePlural>.Add(a<StructureNoplural>)
                _DbContext.SaveChanges()
                mreturn Created(a<StructureNoplural>)
            end
            </IF STRUCTURE_HAS_UNIQUE_KEY>
            catch (e, @InvalidOperationException)
            begin
                mreturn BadRequest(e)
            end
            catch (e, @ValidationException)
            begin
                ModelState.AddModelError("RelationValidation",e.Message)
                mreturn ValidationHelper.ReturnValidationError(ModelState)
            end
            endtry

        endmethod
  </PRIMARY_KEY>
</IF STRUCTURE_ISAM>
;//
;// PATCH ---------------------------------------------------------------------
;//
<IF STRUCTURE_ISAM AND DEFINED_ENABLE_PATCH AND PATCH_ENDPOINT>
  <PRIMARY_KEY>
    <IF DEFINED_ENABLE_AUTHENTICATION AND USERTOKEN_ROLES_PATCH>
        {Authorize(Roles="<ROLES_PATCH>")}
    </IF DEFINED_ENABLE_AUTHENTICATION>
        {ODataRoute("(<SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>={a<FieldSqlName>}<,></IF SEG_TAG_EQUAL></SEGMENT_LOOP>)")}
        {Produces("application/json")}
        {ProducesResponseType(StatusCodes.Status204NoContent)}
        {ProducesResponseType(StatusCodes.Status400BadRequest)}
        {ProducesResponseType(StatusCodes.Status404NotFound)}
        {HttpPatch}
        ;;; <summary>
        ;;; Patch  (partial update) a <structureNoplural>.
        ;;; </summary>
        <SEGMENT_LOOP>
          <IF NOT SEG_TAG_EQUAL>
        ;;; <param name="a<FieldSqlName>"><FIELD_DESC></param>
          </IF>
        </SEGMENT_LOOP>
        ;;; <returns>Returns an IActionResult indicating the status of the operation and containing any data that was returned.</returns>
        public method Patch<StructureNoplural><IF NOT FIRST_UNIQUE_KEY>By<KeyName></IF>, @IActionResult
        <SEGMENT_LOOP>
          <IF NOT SEG_TAG_EQUAL>
            {FromODataUri}
            <IF CUSTOM_HARMONY_AS_STRING>
            required in a<FieldSqlName>, string
            <ELSE>
            required in a<FieldSqlName>, <HARMONYCORE_SEGMENT_DATATYPE>
            </IF CUSTOM_HARMONY_AS_STRING>
          </IF>
        </SEGMENT_LOOP>
            {FromBody}
            required in a<StructureNoplural>, @JsonPatchDocument<<StructureNoplural>>
        proc
            ;; Validate inbound data
            if (!ModelState.IsValid)
                mreturn ValidationHelper.ReturnValidationError(ModelState)

            ;;Patch the existing <structureNoplural>
            try
            begin
                ;;Get the <structureNoplural> to be updated
                data <structureNoplural>ToUpdate = _DbContext.<StructurePlural>.Find<IF NOT STRUCTURE_HAS_UNIQUE_KEY>Query<<StructureNoplural>></IF>(<SEGMENT_LOOP><IF SEG_TAG_EQUAL><SEGMENT_TAG_VALUE><ELSE>a<FieldSqlName><IF HARMONYCORE_CUSTOM_SEGMENT_DATATYPE><ELSE><IF ALPHA>.PadRight(<FIELD_SIZE>)</IF ALPHA></IF HARMONYCORE_CUSTOM_SEGMENT_DATATYPE></IF SEG_TAG_EQUAL><,></SEGMENT_LOOP>)
                data patchError, @JsonPatchError, ^null
                ;;Did we find it?
                if(<structureNoplural>ToUpdate == ^null)
                    mreturn NotFound()

                ;;Apply the changes to the <structureNoplural> we read
                <IF STRUCTURE_HAS_UNIQUE_KEY>
                a<StructureNoplural>.ApplyTo(<structureNoplural>ToUpdate, lambda(error) { patchError = error })
                ;;if the patchdoc was bad return the error info
                if(patchError != ^null)
                    mreturn BadRequest(string.Format("Error applying patch document: error message {0}, caused by {1}", patchError.ErrorMessage, JsonConvert.SerializeObject(patchError.Operation)))

                ;;Update and commit
                _DbContext.<StructurePlural>.Update(<structureNoplural>ToUpdate)
                <ELSE>
                data item, @<StructureNoplural>
                foreach item in <structureNoplural>ToUpdate
                begin
                    a<StructureNoplural>.ApplyTo(item, lambda(error) { patchError = error })
                    ;;if the patchdoc was bad return the error info
                    if(patchError != ^null)
                        mreturn BadRequest(string.Format("Error applying patch document: error message {0}, caused by {1}", patchError.ErrorMessage, JsonConvert.SerializeObject(patchError.Operation)))

                    ;;Update and commit
                    _DbContext.<StructurePlural>.Update(item)
                end
                </IF STRUCTURE_HAS_UNIQUE_KEY>
                _DbContext.SaveChanges()
            end
            catch (e, @InvalidOperationException)
            begin
                mreturn BadRequest(e)
            end
            catch (e, @ValidationException)
            begin
                ModelState.AddModelError("RelationValidation",e.Message)
                mreturn ValidationHelper.ReturnValidationError(ModelState)
            end
            endtry

            mreturn NoContent()

        endmethod
  </PRIMARY_KEY>
</IF STRUCTURE_ISAM>
;//
;// DELETE --------------------------------------------------------------------
;//
<IF STRUCTURE_ISAM AND DEFINED_ENABLE_DELETE AND DELETE_ENDPOINT>
  <PRIMARY_KEY>
    <IF DEFINED_ENABLE_AUTHENTICATION AND USERTOKEN_ROLES_DELETE>
        {Authorize(Roles="<ROLES_DELETE>")}
    </IF DEFINED_ENABLE_AUTHENTICATION>
        {ODataRoute("(<SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>={a<FieldSqlName>}<,></IF SEG_TAG_EQUAL></SEGMENT_LOOP>)")}
        {ProducesResponseType(StatusCodes.Status204NoContent)}
        {ProducesResponseType(StatusCodes.Status404NotFound)}
        {HttpDelete}
        ;;; <summary>
        ;;; Delete a <structureNoplural>.
        ;;; </summary>
        <SEGMENT_LOOP>
          <IF NOT SEG_TAG_EQUAL>
        ;;; <param name="a<FieldSqlName>"><FIELD_DESC></param>
          </IF>
        </SEGMENT_LOOP>
        ;;; <returns>Returns an IActionResult indicating the status of the operation and containing any data that was returned.</returns>
        public method Delete<StructureNoplural><IF NOT FIRST_UNIQUE_KEY>By<KeyName></IF>, @IActionResult
        <SEGMENT_LOOP>
          <IF NOT SEG_TAG_EQUAL>
            {FromODataUri}
            <IF CUSTOM_HARMONY_AS_STRING>
            required in a<FieldSqlName>, string
            <ELSE>
            required in a<FieldSqlName>, <HARMONYCORE_SEGMENT_DATATYPE>
            </IF CUSTOM_HARMONY_AS_STRING>
          </IF>
        </SEGMENT_LOOP>
        proc
            ;;Get the <structureNoplural> to be deleted
            data <structureNoplural>ToRemove = _DbContext.<StructurePlural>.Find<IF NOT STRUCTURE_HAS_UNIQUE_KEY>Query<<StructureNoplural>></IF>(<SEGMENT_LOOP><IF SEG_TAG_EQUAL><SEGMENT_TAG_VALUE><ELSE>a<FieldSqlName><IF HARMONYCORE_CUSTOM_SEGMENT_DATATYPE><ELSE><IF ALPHA>.PadRight(<FIELD_SIZE>)</IF ALPHA></IF HARMONYCORE_CUSTOM_SEGMENT_DATATYPE></IF SEG_TAG_EQUAL><,></SEGMENT_LOOP>)

            ;;Did we find it?
            if (<structureNoplural>ToRemove == ^null)
                mreturn NotFound()

            ;;Delete and commit
            <IF STRUCTURE_HAS_UNIQUE_KEY>
            _DbContext.<StructurePlural>.Remove(<structureNoplural>ToRemove)
            <ELSE>
            data item, @<StructureNoplural>
            foreach item in <structureNoplural>ToRemove
            begin
                _DbContext.<StructurePlural>.Remove(item)
            end
            </IF STRUCTURE_HAS_UNIQUE_KEY>
            _DbContext.SaveChanges()

            mreturn NoContent()

        endmethod
  </PRIMARY_KEY>
</IF STRUCTURE_ISAM>
    endclass

endnamespace