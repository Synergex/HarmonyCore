<CODEGEN_FILENAME><StructureNoplural>Tests.dbl</CODEGEN_FILENAME>
<REQUIRES_USERTOKEN>SERVICES_NAMESPACE</REQUIRES_USERTOKEN>
<REQUIRES_CODEGEN_VERSION>5.3.5</REQUIRES_CODEGEN_VERSION>
;//****************************************************************************
;//
;// Title:       ODataUnitTests.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Generates unit tests for Web API / OData controllers in a
;//              Harmony Core environment.
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
;; Title:       <StructureNoplural>Tests.dbl
;;
;; Type:        Class
;;
;; Description: Unit tests for the operations defined in <StructurePlural>Controller.
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

import Microsoft.VisualStudio.TestTools.UnitTesting
import Newtonsoft.Json
import System.Collections.Generic
import System.Net.Http
import <SERVICES_NAMESPACE>
import <NAMESPACE>.Models

namespace <NAMESPACE>

	{TestClass}
	public partial class <StructureNoplural>Tests

		;;------------------------------------------------------------
		;;Get all <StructurePlural>

		{TestMethod}
		{TestCategory("<StructureNoplural> Tests - Read All")}
		public method Get<StructurePlural>, void
		proc
			disposable data client = UnitTestEnvironment.Server.CreateClient()
			<IF DEFINED_AUTHENTICATION>
			client.SetBearerToken(UnitTestEnvironment.AccessToken)
			</IF DEFINED_AUTHENTICATION>
			disposable data response = client.GetAsync("/odata/<StructurePlural>").Result
			data result = response.Content.ReadAsStringAsync().Result
			response.EnsureSuccessStatusCode()
			data <structurePlural>, @OData<StructurePlural>, JsonConvert.DeserializeObject<OData<StructurePlural>>(result)
		endmethod

<IF STRUCTURE_RELATIONS>
	<RELATION_LOOP>
		;;------------------------------------------------------------
		;;Get all <StructurePlural> and expand relation REL_<RelationFromkey>

		{TestMethod}
		{TestCategory("<StructureNoplural> Tests - Read All")}
		<IF MANY_TO_ONE_TO_MANY>
		public method Get<StructurePlural>_Expand_REL_<RelationFromkey>, void
		proc
			data uri = "/odata/<StructurePlural>?$expand=REL_<RelationFromkey>"
		</IF MANY_TO_ONE_TO_MANY>
		<IF ONE_TO_ONE>
		public method Get<StructurePlural>_Expand_REL_<RelationFromkey>, void
		proc
			data uri = "/odata/<StructurePlural>?$expand=REL_<RelationFromkey>"
		</IF ONE_TO_ONE>
		<IF ONE_TO_MANY_TO_ONE>
		public method Get<StructurePlural>_Expand_REL_<RelationTostructurePlural>, void
		proc
			data uri = "/odata/<StructurePlural>?$expand=REL_<RelationTostructurePlural>"
		</IF ONE_TO_MANY_TO_ONE>
		<IF ONE_TO_MANY>
		public method Get<StructurePlural>_Expand_REL_<RelationTostructurePlural>, void
		proc
			data uri = "/odata/<StructurePlural>?$expand=REL_<RelationTostructurePlural>"
		</IF ONE_TO_MANY>
			disposable data client = UnitTestEnvironment.Server.CreateClient()
			<IF DEFINED_AUTHENTICATION>
			client.SetBearerToken(UnitTestEnvironment.AccessToken)
			</IF DEFINED_AUTHENTICATION>
			disposable data response = client.GetAsync(uri).Result
			data result = response.Content.ReadAsStringAsync().Result
			response.EnsureSuccessStatusCode()
		endmethod

	</RELATION_LOOP>
		;;------------------------------------------------------------
		;;Get all <StructurePlural> and expand all relations

		{TestMethod}
		{TestCategory("<StructureNoplural> Tests - Read All")}
		public method Get<StructurePlural>_Expand_All, void
		proc
			data uri = "/odata/<StructurePlural>?$expand=<RELATION_LOOP><IF MANY_TO_ONE_TO_MANY>REL_<RelationFromkey></IF MANY_TO_ONE_TO_MANY><IF ONE_TO_ONE>REL_<RelationFromkey></IF ONE_TO_ONE><IF ONE_TO_MANY_TO_ONE>REL_<RelationTostructurePlural></IF ONE_TO_MANY_TO_ONE><IF ONE_TO_MANY>REL_<RelationTostructurePlural></IF ONE_TO_MANY><,></RELATION_LOOP>"
			disposable data client = UnitTestEnvironment.Server.CreateClient()
			<IF DEFINED_AUTHENTICATION>
			client.SetBearerToken(UnitTestEnvironment.AccessToken)
			</IF DEFINED_AUTHENTICATION>
			disposable data response = client.GetAsync(uri).Result
			data result = response.Content.ReadAsStringAsync().Result
			response.EnsureSuccessStatusCode()
		endmethod
		
</IF STRUCTURE_RELATIONS>
		;;------------------------------------------------------------
		;;Get a single <StructureNoplural> by primary key

		{TestMethod}
		{TestCategory("<StructureNoplural> Tests - Read by Primary Key")}
		public method Get<StructureNoplural>, void
		proc
			data client = UnitTestEnvironment.Server.CreateClient()
			<IF DEFINED_AUTHENTICATION>
			client.SetBearerToken(UnitTestEnvironment.AccessToken)
			</IF DEFINED_AUTHENTICATION>
			data request = String.Format("/odata/<StructurePlural>(<PRIMARY_KEY><SEGMENT_LOOP><SegmentName>=<IF ALPHA>'</IF ALPHA>{<SEGMENT_NUMBER>}<IF ALPHA>'</IF ALPHA><,></SEGMENT_LOOP>)","",<SEGMENT_LOOP>TestContext.Get<StructureNoplural>_<SegmentName><,></SEGMENT_LOOP></PRIMARY_KEY>)
			data response = client.GetAsync(request).Result
			data result = response.Content.ReadAsStringAsync().Result
			response.EnsureSuccessStatusCode()
			data <structureNoplural>, @OData<StructureNoplural>, JsonConvert.DeserializeObject<OData<StructureNoplural>>(result)
		endmethod

<IF STRUCTURE_RELATIONS>
	<RELATION_LOOP>
		;;------------------------------------------------------------
		;;Get a single <StructureNoplural> by primary key and expand relation <IF MANY_TO_ONE_TO_MANY>REL_<RelationFromkey></IF MANY_TO_ONE_TO_MANY><IF ONE_TO_ONE>REL_<RelationFromkey></IF ONE_TO_ONE><IF ONE_TO_MANY_TO_ONE>REL_<RelationTostructurePlural></IF ONE_TO_MANY_TO_ONE><IF ONE_TO_MANY>REL_<RelationTostructurePlural></IF ONE_TO_MANY>


		{TestMethod}
		{TestCategory("<StructureNoplural> Tests - Read by Primary Key")}
		public method Get<StructureNoplural>_Expand_<IF MANY_TO_ONE_TO_MANY>REL_<RelationFromkey></IF MANY_TO_ONE_TO_MANY><IF ONE_TO_ONE>REL_<RelationFromkey></IF ONE_TO_ONE><IF ONE_TO_MANY_TO_ONE>REL_<RelationTostructurePlural></IF ONE_TO_MANY_TO_ONE><IF ONE_TO_MANY>REL_<RelationTostructurePlural></IF ONE_TO_MANY>, void
		proc
			data client = UnitTestEnvironment.Server.CreateClient()
			<IF DEFINED_AUTHENTICATION>
			client.SetBearerToken(UnitTestEnvironment.AccessToken)
			</IF DEFINED_AUTHENTICATION>
			data request = String.Format("/odata/<StructurePlural>(<PRIMARY_KEY><SEGMENT_LOOP><SegmentName>=<IF ALPHA>'</IF ALPHA>{<SEGMENT_NUMBER>}<IF ALPHA>'</IF ALPHA><,></SEGMENT_LOOP></PRIMARY_KEY>)?$expand=<IF MANY_TO_ONE_TO_MANY>REL_<RelationFromkey></IF MANY_TO_ONE_TO_MANY><IF ONE_TO_ONE>REL_<RelationFromkey></IF ONE_TO_ONE><IF ONE_TO_MANY_TO_ONE>REL_<RelationTostructurePlural></IF ONE_TO_MANY_TO_ONE><IF ONE_TO_MANY>REL_<RelationTostructurePlural></IF ONE_TO_MANY>","",<PRIMARY_KEY><SEGMENT_LOOP>TestContext.Get<StructureNoplural>_Expand_<IF MANY_TO_ONE_TO_MANY>REL_<RelationFromkey></IF MANY_TO_ONE_TO_MANY><IF ONE_TO_ONE>REL_<RelationFromkey></IF ONE_TO_ONE><IF ONE_TO_MANY_TO_ONE>REL_<RelationTostructurePlural></IF ONE_TO_MANY_TO_ONE><IF ONE_TO_MANY>REL_<RelationTostructurePlural></IF ONE_TO_MANY>_<SegmentName><,></SEGMENT_LOOP></PRIMARY_KEY>)
			data response = client.GetAsync(request).Result
			data result = response.Content.ReadAsStringAsync().Result
			response.EnsureSuccessStatusCode()
			data <structureNoplural>, @OData<StructureNoplural>, JsonConvert.DeserializeObject<OData<StructureNoplural>>(result)
		endmethod

	</RELATION_LOOP>
		;;------------------------------------------------------------
		;;Get a single <StructureNoplural> by primary key and expand all relations

		{TestMethod}
		{TestCategory("<StructureNoplural> Tests - Read by Primary Key")}
		public method Get<StructureNoplural>_Expand_All, void
		proc
			data client = UnitTestEnvironment.Server.CreateClient()
			<IF DEFINED_AUTHENTICATION>
			client.SetBearerToken(UnitTestEnvironment.AccessToken)
			</IF DEFINED_AUTHENTICATION>
			data request = String.Format("/odata/<StructurePlural>(<PRIMARY_KEY><SEGMENT_LOOP><SegmentName>=<IF ALPHA>'</IF ALPHA>{<SEGMENT_NUMBER>}<IF ALPHA>'</IF ALPHA><,></SEGMENT_LOOP></PRIMARY_KEY>)?$expand=<RELATION_LOOP><IF MANY_TO_ONE_TO_MANY>REL_<RelationFromkey></IF MANY_TO_ONE_TO_MANY><IF ONE_TO_ONE>REL_<RelationFromkey></IF ONE_TO_ONE><IF ONE_TO_MANY_TO_ONE>REL_<RelationTostructurePlural></IF ONE_TO_MANY_TO_ONE><IF ONE_TO_MANY>REL_<RelationTostructurePlural></IF ONE_TO_MANY><,></RELATION_LOOP>","",<PRIMARY_KEY><SEGMENT_LOOP>TestContext.Get<StructureNoplural>_Expand_All_<SegmentName><,></SEGMENT_LOOP></PRIMARY_KEY>)
			data response = client.GetAsync(request).Result
			data result = response.Content.ReadAsStringAsync().Result
			response.EnsureSuccessStatusCode()
			data <structureNoplural>, @OData<StructureNoplural>, JsonConvert.DeserializeObject<OData<StructureNoplural>>(result)
		endmethod

</IF STRUCTURE_RELATIONS>
<ALTERNATE_KEY_LOOP>
		;;------------------------------------------------------------
		;;Get a single <StructureNoplural> by alternate key <KEY_NUMBER> (<KeyName>)

		{TestMethod}
		{TestCategory("<StructureNoplural> Tests - Read by Alternate Key")}
		public method Get<StructureNoplural>_ByAltKey_<KeyName>, void
		proc
			data client = UnitTestEnvironment.Server.CreateClient()
			<IF DEFINED_AUTHENTICATION>
			client.SetBearerToken(UnitTestEnvironment.AccessToken)
			</IF DEFINED_AUTHENTICATION>
			data request = String.Format("/odata/<StructurePlural>(<SEGMENT_LOOP><SegmentName>=<IF ALPHA>'</IF ALPHA>{<SEGMENT_NUMBER>}<IF ALPHA>'</IF ALPHA><,></SEGMENT_LOOP>)", "", <SEGMENT_LOOP>TestContext.Get<StructureNoplural>_ByAltKey_<KeyName>_<SegmentName><,></SEGMENT_LOOP>)
			data response = client.GetAsync(request).Result
			data result = response.Content.ReadAsStringAsync().Result
			response.EnsureSuccessStatusCode()
			data <structureNoplural>, @OData<StructureNoplural>, JsonConvert.DeserializeObject<OData<StructureNoplural>>(result)
		endmethod

</ALTERNATE_KEY_LOOP>
;//		;;------------------------------------------------------------
;//		;;Create a new <StructureNoplural> (auto assign key)
;//
;//		{TestMethod}
;//		{TestCategory("<StructureNoplural> Tests - Create, Update & Delete")}
;//		public method Create<StructureNoplural>, void
;//		proc
;//			disposable data client = UnitTestEnvironment.Server.CreateClient()
;//			<IF DEFINED_AUTHENTICATION>
;//			client.SetBearerToken(UnitTestEnvironment.AccessToken)
;//			</IF DEFINED_AUTHENTICATION>
;//			disposable data requestBody = new StringContent("")
;//			disposable data response = client.PostAsync("/odata/<StructurePlural>", requestBody).Result
;//			data result = response.Content.ReadAsStringAsync().Result
;//			response.EnsureSuccessStatusCode()
;//		endmethod
;//
		;;------------------------------------------------------------
		;;Create new <StructureNoplural> (client specified key)

		{TestMethod}
		{TestCategory("<StructureNoplural> Tests - Create, Update & Delete")}
		public method Update<StructureNoplural>, void
		proc
			disposable data client = UnitTestEnvironment.Server.CreateClient()
			<IF DEFINED_AUTHENTICATION>
			client.SetBearerToken(UnitTestEnvironment.AccessToken)
			</IF DEFINED_AUTHENTICATION>

			;;Get the first record from the file
			data getRequest = String.Format("/odata/<StructurePlural>(<PRIMARY_KEY><SEGMENT_LOOP><SegmentName>=<IF ALPHA>'</IF ALPHA>{<SEGMENT_NUMBER>}<IF ALPHA>'</IF ALPHA><,></SEGMENT_LOOP>)","",<SEGMENT_LOOP>TestContext.Get<StructureNoplural>_<SegmentName><,></SEGMENT_LOOP></PRIMARY_KEY>)
			data getResponse = client.GetAsync(getRequest).Result
			data getResult = getResponse.Content.ReadAsStringAsync().Result
			getResponse.EnsureSuccessStatusCode()
			data do<StructureNoplural>, @<StructureNoplural>, JsonConvert.DeserializeObject<<StructureNoplural>>(getResult)

			<PRIMARY_KEY>
			<SEGMENT_LOOP>
			do<StructureNoplural>.<FieldSqlName> = TestContext.Update<StructureNoplural>_<SegmentName>
			</SEGMENT_LOOP>
			</PRIMARY_KEY>

			;TODO: Also need to ensure any nodups alternate keys get unique values

			;;Update it
			disposable data requestBody = new StringContent(JsonConvert.SerializeObject(do<StructureNoplural>),System.Text.Encoding.UTF8, "application/json")
			data request = String.Format("/odata/<StructurePlural>(<PRIMARY_KEY><SEGMENT_LOOP><SegmentName>=<IF ALPHA>'</IF ALPHA>{<SEGMENT_NUMBER>}<IF ALPHA>'</IF ALPHA><,></SEGMENT_LOOP>)","",<SEGMENT_LOOP>TestContext.Update<StructureNoplural>_<SegmentName><,></SEGMENT_LOOP></PRIMARY_KEY>)
			disposable data response = client.PutAsync(request, requestBody).Result
			response.EnsureSuccessStatusCode()

			;;Get the inserted record
			getResponse = client.GetAsync(request).Result
			getResult = getResponse.Content.ReadAsStringAsync().Result
			getResponse.EnsureSuccessStatusCode()
			do<StructureNoplural> = JsonConvert.DeserializeObject<<StructureNoplural>>(getResult)

			<PRIMARY_KEY>
			<SEGMENT_LOOP>
			Assert.AreEqual(do<StructureNoplural>.<FieldSqlName>, TestContext.Update<StructureNoplural>_<SegmentName>)
			</SEGMENT_LOOP>
			</PRIMARY_KEY>

		endmethod

;//<PRIMARY_KEY>
;//<IF MULTIPLE_SEGMENTS>
;//		;;------------------------------------------------------------
;//		;;Get multiple <StructureNoplural> by partial primary key
;//
;//		{TestMethod}
;//		{TestCategory("<StructureNoplural> Tests - Read by Primary Key")}
;//		public method Get<StructureNoplural>_ByPartialPrimaryKey, void
;//		proc
;//			data client = UnitTestEnvironment.Server.CreateClient()
;//			<IF DEFINED_AUTHENTICATION>
;//			client.SetBearerToken(UnitTestEnvironment.AccessToken)
;//			</IF DEFINED_AUTHENTICATION>
;//			data request = String.Format("/odata/<StructurePlural>(<SEGMENT_LOOP_FILTER><SegmentName>=<IF ALPHA>'</IF ALPHA>{<SEGMENT_NUMBER>}<IF ALPHA>'</IF ALPHA><,></SEGMENT_LOOP_FILTER>)","",<SEGMENT_LOOP_FILTER>TestContext.Get<StructureNoplural>_ByPartialPrimaryKey_<SegmentName><,></SEGMENT_LOOPFILTER>)
;//			data response = client.GetAsync(request).Result
;//			data result = response.Content.ReadAsStringAsync().Result
;//			response.EnsureSuccessStatusCode()
;//			data <structureNoplural>, @OData<StructureNoplural>, JsonConvert.DeserializeObject<OData<StructureNoplural>>(result)
;//		endmethod
;//
;//</IF MULTIPLE_SEGMENTS>
;//</PRIMARY_KEY>
	endclass

endnamespace
