<CODEGEN_FILENAME>TestConstants.Values.dbl</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.5.3</REQUIRES_CODEGEN_VERSION>
;//****************************************************************************
;//
;// Title:       ODataTestConstantsValues.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Generates a test context class with static values that can
;//              be used to feed data into unit tests.
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
;; Title:       TestConstants.Values.dbl
;;
;; Description: Test context class with static values that can be used to feed
;;              data into unit tests.
;;
;;*****************************************************************************
;; WARNING: GENERATED CODE!
;; This file was code generated, but generally is set to be generated only one
;; time and not replaced, so that you can edit the code in the file. It is a
;; good idea to ensure that you have a backup of any changes that you make incase
;; someone accidentally regenerates the file.
;;*****************************************************************************

import Microsoft.VisualStudio.TestTools.UnitTesting
import Newtonsoft.Json
import System.Collections.Generic
import System.Net.Http

namespace <NAMESPACE>

    public static partial class TestConstants

        static method TestConstants
        proc
<STRUCTURE_LOOP>
  <IF STRUCTURE_ISAM>

            ;;------------------------------------------------------------
            ;;Test data for <StructureNoplural>
            ;;
    <IF DEFINED_ENABLE_GET_ALL>
            Get<StructurePlural>_Count = 0
    </IF DEFINED_ENABLE_GET_ALL>
;//
    <PRIMARY_KEY>
      <SEGMENT_LOOP>
            Get<StructureNoplural>_<SegmentName> = <FIELD_CSDEFAULT>
      </SEGMENT_LOOP>
    </PRIMARY_KEY>
;//
;//
;//
    <IF DEFINED_ENABLE_RELATIONS>
      <IF STRUCTURE_RELATIONS>
        <RELATION_LOOP_RESTRICTED>
          <PRIMARY_KEY>
            <SEGMENT_LOOP>
            Get<StructureNoplural>_Expand_<IF MANY_TO_ONE_TO_MANY><HARMONYCORE_RELATION_NAME></IF MANY_TO_ONE_TO_MANY><IF ONE_TO_ONE><HARMONYCORE_RELATION_NAME></IF ONE_TO_ONE><IF ONE_TO_MANY_TO_ONE><HARMONYCORE_RELATION_NAME></IF ONE_TO_MANY_TO_ONE><IF ONE_TO_MANY><HARMONYCORE_RELATION_NAME></IF ONE_TO_MANY>_<SegmentName> = <FIELD_CSDEFAULT>
            </SEGMENT_LOOP>
          </PRIMARY_KEY>
        </RELATION_LOOP_RESTRICTED>
      </IF STRUCTURE_RELATIONS>
    </IF DEFINED_ENABLE_RELATIONS>
;//
;//
;//
    <PRIMARY_KEY>
      <SEGMENT_LOOP>
            Get<StructureNoplural>_Expand_All_<SegmentName> = <FIELD_CSDEFAULT>
      </SEGMENT_LOOP>
    </PRIMARY_KEY>
;//
;//
;//
    <ALTERNATE_KEY_LOOP_UNIQUE>
      <SEGMENT_LOOP>
            Get<StructureNoplural>_ByAltKey_<KeyName>_<SegmentName> = <FIELD_CSDEFAULT>
      </SEGMENT_LOOP>
    </ALTERNATE_KEY_LOOP_UNIQUE>
;//
;//
;//
    <PRIMARY_KEY>
      <SEGMENT_LOOP>
            Update<StructureNoplural>_<SegmentName> = <FIELD_CSDEFAULT>
      </SEGMENT_LOOP>
    </PRIMARY_KEY>
  </IF STRUCTURE_ISAM>
</STRUCTURE_LOOP>

        endmethod

    endclass

endnamespace
