<CODEGEN_FILENAME><StructureNoplural>MetaData.dbl</CODEGEN_FILENAME>
;//****************************************************************************
;//
;// Title:       DataObjectMetaData.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Template to define meta data associated with a data object
;//
;// Copyright (c) 2012, Synergex International, Inc. All rights reserved.
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
;; Title:       <StructureNoplural>MetaData.dbl
;;
;; Type:        Class
;;
;; Description: Defines meta data associated with a data object <StructureNoplural>.
;;
;;*****************************************************************************
;; WARNING
;;
;; This file was code generated. Avoid editing this file, as any changes that
;; you make will be lost of the file is re-generated.
;;
;;*****************************************************************************
;;
;; Copyright (c) 2012, Synergex International, Inc.
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

import System
import System.Collections.Generic
import System.Text
import Harmony.Core
import Harmony.Core.Converters

namespace <NAMESPACE>

	;;; <summary>
	;;; 
	;;; </summary>
	public partial class <StructureNoplural>Metadata extends DataObjectMetadataBase
		
		;;; <summary>
		;;; Constructs an new <StructureNoplural>Metadata object.
		;;; </summary>
		public method <StructureNoplural>Metadata
		proc
			RPSStructureName = "<STRUCTURE_NOALIAS>"
			RPSStructureSize = ^size(str<StructureNoplural>)
			<FIELD_LOOP>
			<IF CUSTOM_NOT_SYMPHONY_ARRAY_FIELD>
			AddFieldInfo("<FieldSqlname>", "<FIELD_TYPE_NAME>", <FIELD_SIZE>, <FIELD_POSITION>, 0<FIELD_PRECISION>, false)
			</IF>
            </FIELD_LOOP>
		endmethod
	
		;;; <summary>
		;;; Returns a new <StructureNoplural> object containing data from a record and a GRFA.
		<IF STRUCTURE_RELATIONS>
		;;; The related data properties (<RELATION_LOOP><IF TWO_WAY_ONE_TO_ONE>REL_<RelationFromkey></IF TWO_WAY_ONE_TO_ONE><IF ONE_WAY_ONE_TO_ONE>REL_<RelationFromkey></IF ONE_WAY_ONE_TO_ONE><IF TWO_WAY_ONE_TO_MANY>REL_<RelationTostructurePlural></IF TWO_WAY_ONE_TO_MANY><IF ONE_WAY_ONE_TO_MANY>REL_<RelationTostructurePlural></IF ONE_WAY_ONE_TO_MANY><,></RELATION_LOOP>) will not be populated.
		</IF STRUCTURE_RELATIONS>
		;;; </summary>
		;;; <param name="dataArea">The record containing the data for the new <StructureNoplural> object.</param>
		;;; <param name="grfa">The GRFA associated with the current state of the data.</param>
		;;; <returns></returns>
		public override method MakeNew, @DataObjectBase
			required in dataArea, a
			required in grfa, a
		proc
			mreturn new <StructureNoplural>((str<StructureNoplural>)dataArea) { GlobalRFA = grfa }
		endmethod

		;;; <summary>
		;;; Returns a new <StructureNoplural> object containing data from a record and a GRFA.
		<IF STRUCTURE_RELATIONS>
		;;; The related data properties (<RELATION_LOOP><IF TWO_WAY_ONE_TO_ONE>REL_<RelationFromkey></IF TWO_WAY_ONE_TO_ONE><IF ONE_WAY_ONE_TO_ONE>REL_<RelationFromkey></IF ONE_WAY_ONE_TO_ONE><IF TWO_WAY_ONE_TO_MANY>REL_<RelationTostructurePlural></IF TWO_WAY_ONE_TO_MANY><IF ONE_WAY_ONE_TO_MANY>REL_<RelationTostructurePlural></IF ONE_WAY_ONE_TO_MANY><,></RELATION_LOOP>) will be populated.
		</IF STRUCTURE_RELATIONS>
		;;; </summary>
		;;; <param name="dataArea">The record containing the data for the new <StructureNoplural> object.</param>
		;;; <param name="grfa">The GRFA associated with the current state of the data.</param>
		;;; <param name="joinedObjects">Data to allow the related data properties (<RELATION_LOOP><IF TWO_WAY_ONE_TO_ONE>REL_<RelationFromkey></IF TWO_WAY_ONE_TO_ONE><IF ONE_WAY_ONE_TO_ONE>REL_<RelationFromkey></IF ONE_WAY_ONE_TO_ONE><IF TWO_WAY_ONE_TO_MANY>REL_<RelationTostructurePlural></IF TWO_WAY_ONE_TO_MANY><IF ONE_WAY_ONE_TO_MANY>REL_<RelationTostructurePlural></IF ONE_WAY_ONE_TO_MANY><,></RELATION_LOOP>) to be populated.</param>
		;;; <returns></returns>
		public override method MakeNew, @DataObjectBase
			required in dataArea, a
			required in grfa, a
			required in joinedObjects, [#]KeyValuePair<String, Object>
		proc
			data new<StructureNoplural> = new <StructureNoplural>((str<StructureNoplural>)dataArea) { GlobalRFA = grfa }
			<IF STRUCTURE_RELATIONS>
			data joinedObject, KeyValuePair<String, Object>
			foreach joinedObject in joinedObjects
			begin
				using joinedObject.Key select
				<RELATION_LOOP>
				<IF TWO_WAY_ONE_TO_ONE>
				("REL_<RelationFromkey>"), 
					new<StructureNoplural>.REL_<RelationFromkey> = (@<RelationTostructureNoplural>)joinedObject.Value
				</IF TWO_WAY_ONE_TO_ONE>
				<IF ONE_WAY_ONE_TO_ONE>
				("REL_<RelationFromkey>"),
					new<StructureNoplural>.REL_<RelationFromkey> = (@<RelationTostructureNoplural>)joinedObject.Value
				</IF ONE_WAY_ONE_TO_ONE>
				<IF TWO_WAY_ONE_TO_MANY>
				("REL_<RelationTostructurePlural>"), 
					new<StructureNoplural>.REL_<RelationTostructurePlural> = (@ICollection<<RelationTostructureNoplural>>)joinedObject.Value
				</IF TWO_WAY_ONE_TO_MANY>
				<IF ONE_WAY_ONE_TO_MANY>
				("REL_<RelationTostructurePlural>"),
					new<StructureNoplural>.REL_<RelationTostructurePlural> = (@ICollection<<RelationTostructureNoplural>>)joinedObject.Value
				</IF ONE_WAY_ONE_TO_MANY>
				</RELATION_LOOP>
				endusing
			end
			</IF STRUCTURE_RELATIONS>

			mreturn new<StructureNoplural>

		endmethod

	endclass

endnamespace

