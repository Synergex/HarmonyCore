<CODEGEN_FILENAME>TestConstants.Properties.dbl</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.5.3</REQUIRES_CODEGEN_VERSION>
;//****************************************************************************
;//
;// Title:       ODataTestConstantsProperties.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Generates a test context class with values that can
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
;; Title:       TestConstants.Properties.dbl
;;
;; Description: Test context class with values that can be used to feed
;;              data into unit tests.
;;
;;*****************************************************************************
;; WARNING: GENERATED CODE!
;; This file was generated by CodeGen. Avoid editing the file if possible.
;; Any changes you make will be lost of the file is re-generated.
;;*****************************************************************************

import Microsoft.VisualStudio.TestTools.UnitTesting
import System.Text.Json
import System.Collections.Generic
import System.Net.Http
import System.Threading
import System.IO

namespace <NAMESPACE>

    public sealed class TestConstants
        private static readonly lockObject, @Object, new Object()

        private static instance, @TestConstants, ^null
        public static property Instance, @TestConstants
            method get
            proc
                try
                begin
                    Monitor.Enter(lockObject)
                    begin
                        if (instance == ^null)
                        begin
                            try
                            begin
                                data filePath = Path.Combine(Environment.GetEnvironmentVariable("SOLUTIONDIR"), "Services.Test", "TestConstants.Values.json")
                                if (File.Exists(filePath)) then
                                    instance = JsonSerializer.Deserialize<TestConstants>(File.ReadAllText(filePath))
                                else
                                begin
                                    Console.WriteLine("No JSON file found here: {0}{1}Creating a new JSON file", filePath, Environment.NewLine)
                                    instance = new TestConstants()
                                end
                            end
                            catch (e, @JsonException)
                            begin
                                Console.WriteLine(e)
                                instance = new TestConstants()
                            end
                            endtry
                        end
                        mreturn instance
                    end
                end
                finally
                begin
                    Monitor.Exit(lockObject)
                end
                endtry
            endmethod
        endproperty

        private method TestConstants
        proc
        endmethod

<STRUCTURE_LOOP>
  <IF STRUCTURE_ISAM>

        ;;------------------------------------------------------------
        ;;Test data for <StructureNoplural>
;//
;// ENABLE_GET_ALL
;//
    <IF DEFINED_ENABLE_GET_ALL>
        ;;
        public readwrite property Get<StructurePlural>_Count, int
;//
;// ENABLE_GET_ONE
;//
    <IF DEFINED_ENABLE_GET_ONE>
      <PRIMARY_KEY>
        <SEGMENT_LOOP>
        public readwrite property Get<StructureNoplural>_<SegmentName>, <SEGMENT_SNTYPE>
        </SEGMENT_LOOP>
      </PRIMARY_KEY>
      <IF DEFINED_ENABLE_RELATIONS>
        <IF STRUCTURE_RELATIONS>
          <RELATION_LOOP_RESTRICTED>
            <PRIMARY_KEY>
              <SEGMENT_LOOP>
        public readwrite property Get<StructureNoplural>_Expand_<HARMONYCORE_RELATION_NAME>_<SegmentName>, <SEGMENT_SNTYPE>
              </SEGMENT_LOOP>
            </PRIMARY_KEY>
          </RELATION_LOOP_RESTRICTED>
;//
          <PRIMARY_KEY>
            <SEGMENT_LOOP>
        public readwrite property Get<StructureNoplural>_Expand_All_<SegmentName>, <SEGMENT_SNTYPE>
            </SEGMENT_LOOP>
          </PRIMARY_KEY>
        </IF STRUCTURE_RELATIONS>
      </IF DEFINED_ENABLE_RELATIONS>
;//
    </IF DEFINED_ENABLE_GET_ALL>
;//
    </IF DEFINED_ENABLE_GET_ONE>
;//
;//
;//
;//
  <ALTERNATE_KEY_LOOP_UNIQUE>
    <SEGMENT_LOOP>
        public readwrite property Get<StructureNoplural>_ByAltKey_<KeyName>_<SegmentName>, <SEGMENT_SNTYPE>
    </SEGMENT_LOOP>
  </ALTERNATE_KEY_LOOP_UNIQUE>
;//
;//
;//
  <PRIMARY_KEY>
    <SEGMENT_LOOP>
        public readwrite property Update<StructureNoplural>_<SegmentName>, <SEGMENT_SNTYPE>
    </SEGMENT_LOOP>
  </PRIMARY_KEY>
  </IF STRUCTURE_ISAM>
</STRUCTURE_LOOP>

    endclass

endnamespace
