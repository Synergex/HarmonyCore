<CODEGEN_FILENAME>MetadataController.dbl</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.3.5</REQUIRES_CODEGEN_VERSION>
;//****************************************************************************
;//
;// Title:       ODataMetadataController.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Used to create an OData Controller that exposes a collection
;//              of available endpoints.
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
;; Title:       MetadataController.dbl
;;
;; Type:        Class
;;
;; Description: OData controller that exposes available endpoints.
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
import Harmony.Core.EF.Extensions
<IF DEFINED_AUTHENTICATION>
import Microsoft.AspNetCore.Authorization
</IF DEFINED_AUTHENTICATION>
import System.Collections.Generic

namespace <NAMESPACE>

<IF DEFINED_AUTHENTICATION>
	{Authorize}
</IF DEFINED_AUTHENTICATION>
	;;; <summary>
	;;; OData controller to expose metadata about available endpoints
	;;; </summary>
	public class MetadataController extends ODataController
	
		private static mControllers, @List<string>
		private static mEndpoints,  @List<string>

		;;; <summary>
		;;; Constructs a new instance of <StructurePlural>Controller
		;;; </summary>
		;;; <param name="dbContext">Database context</param>
		public method MetadataController
		proc
			if (mControllers == ^null)
			begin
				mControllers = new List<string>()
<STRUCTURE_LOOP>
				mControllers.Add("<StructurePlural>")
</STRUCTURE_LOOP>
			end

			if (mEndpoints == ^null)
			begin
				mEndpoints = new List<string>()
<STRUCTURE_LOOP>

				;;Operations for <StructurePlural>Controller
				mEndpoints.Add("GET     /<StructurePlural>")
	<PRIMARY_KEY>
				mEndpoints.Add("GET     /<StructurePlural>(<SEGMENT_LOOP><SegmentName>={a<SegmentName>}<,></SEGMENT_LOOP>)")
	</PRIMARY_KEY>
	<ALTERNATE_KEY_LOOP>
				mEndpoints.Add("GET     /<StructurePlural>(<SEGMENT_LOOP><SegmentName>={a<SegmentName>}<,></SEGMENT_LOOP>)")
	</ALTERNATE_KEY_LOOP>
				mEndpoints.Add("POST    /<StructurePlural>")
	<PRIMARY_KEY>
				mEndpoints.Add("PUT     /<StructurePlural>(<SEGMENT_LOOP><SegmentName>={a<SegmentName>}<,></SEGMENT_LOOP>)")
	</PRIMARY_KEY>
	<PRIMARY_KEY>
				mEndpoints.Add("PATCH   /<StructurePlural>(<SEGMENT_LOOP><SegmentName>={a<SegmentName>}<,></SEGMENT_LOOP>)")
	</PRIMARY_KEY>
	<PRIMARY_KEY>
				mEndpoints.Add("DELETE  /<StructurePlural>(<SEGMENT_LOOP><SegmentName>={a<SegmentName>}<,></SEGMENT_LOOP>)")
	</PRIMARY_KEY>
</STRUCTURE_LOOP>
			end
		endmethod

		{ODataRoute("Controllers")}
		{EnableQuery}
		;;; <summary>
		;;; Get a list of available controllers
		;;; </summary>
		public method GetControllers, @IActionResult
		proc
			mreturn Ok(mControllers)
		endmethod

		{ODataRoute("Endpoints")}
		{EnableQuery}
		;;; <summary>
		;;; Get a list of available endpoints
		;;; </summary>
		public method GetEndpoints, @IActionResult
		proc
			mreturn Ok(mEndpoints)
		endmethod
	endclass

endnamespace