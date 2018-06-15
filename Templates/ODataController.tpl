<CODEGEN_FILENAME><StructurePlural>Controller.dbl</CODEGEN_FILENAME>
<REQUIRES_USERTOKEN>DBCONTEXT_NAMESPACE</REQUIRES_USERTOKEN>
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
;; Type:        Class
;;
;; Description: OData controller for the <STRUCTURE_NOALIAS> structure.
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

import Microsoft.AspNetCore.Mvc
import Microsoft.AspNet.OData
import Microsoft.AspNet.OData.Routing
import Microsoft.EntityFrameworkCore
import Microsoft.EntityFrameworkCore.Infrastructure

namespace <NAMESPACE>

	;;; <summary>
	;;; OData controller for <StructurePlural>
	;;; </summary>
	public class <StructurePlural>Controller extends ODataController
	
		public readwrite property DBContext, @<DBCONTEXT_NAMESPACE>.DBContext

		;;; <summary>
		;;; Constructs a new instance of <StructurePlural>Controller
		;;; </summary>
		;;; <param name="dbContext">Database context</param>
		public method <StructurePlural>Controller
			dbContext, @<DBCONTEXT_NAMESPACE>.DBContext
		proc
			this.DBContext = dbContext
		endmethod

		{ODataRoute("<StructurePlural>")}
		;;; <summary>
		;;; Get all <StructurePlural>
		;;; </summary>
		;{EnableQuery(MaxExpansionDepth=3, MaxSkip=10, MaxTop=5, PageSize=4)}
		public method Get, @IActionResult
		proc
			mreturn Ok(DBContext.<StructurePlural>)
		endmethod

		<PRIMARY_KEY>
		{ODataRoute("<StructurePlural>(<SEGMENT_LOOP>{a<SegmentName>}<,></SEGMENT_LOOP>)")}
		;;; <summary>
		;;; Get a single <StructureNoplural> by primary key.
		;;; </summary>
        <SEGMENT_LOOP>
		;;; <param name="a<SegmentName>"><FIELD_DESC></param>
        </SEGMENT_LOOP>
		;;; <returns></returns>
		public method Get, @IActionResult
            <SEGMENT_LOOP>
			{FromODataUri}
            required in a<SegmentName>, <SEGMENT_SNTYPE>
            </SEGMENT_LOOP>
		proc
			data result = DBContext.<StructurePlural>.Find(<SEGMENT_LOOP>a<SegmentName><,></SEGMENT_LOOP>)
			mreturn Ok(result)
		endmethod
		</PRIMARY_KEY>
		
		<ALTERNATE_KEY_LOOP>
;		{ODataRoute("<StructurePlural>/ByKey/<KeyName>/(<SEGMENT_LOOP>{a<SegmentName>}<,></SEGMENT_LOOP>)")}
;		;;; <summary>
;		;;; Get a single <StructureNoplural> by key <KeyName>.
;		;;; </summary>
;        <SEGMENT_LOOP>
;		;;; <param name="a<SegmentName>"><FIELD_DESC></param>
;        </SEGMENT_LOOP>
;		;;; <returns></returns>
;		public method GetByKey<KeyName>, @IActionResult
;            <SEGMENT_LOOP>
;			{FromODataUri}
;            required in a<SegmentName>, <SEGMENT_SNTYPE>
;            </SEGMENT_LOOP>
;		proc
;			data result = DBContext.<StructurePlural>.Find(<SEGMENT_LOOP>a<SegmentName><,></SEGMENT_LOOP>)
;			mreturn Ok(result)
;		endmethod
;		
		</ALTERNATE_KEY_LOOP>
	endclass

endnamespace