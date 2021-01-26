<CODEGEN_FILENAME>GenerateTestValues.dbl</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.6.5</REQUIRES_CODEGEN_VERSION>

import System
import System.Text.Json
import System.Text.Json.Serialization
import System.IO

main
proc
    Services.Test.UnitTestEnvironment.AssemblyInitialize(^null)
    new GenerateTestValues().SerializeValues()
endmain

namespace Services.Test.GenerateValues
    public class GenerateTestValues

    <STRUCTURE_LOOP>
        <IF STRUCTURE_ISAM>
        .include "<STRUCTURE_NOALIAS>" repository, record="<structureNoplural>", end
        </IF STRUCTURE_ISAM>
    </STRUCTURE_LOOP>

        public method SerializeValues, void
            endparams
        proc
            data chin, int
            data count, int
            data pknum, int
<STRUCTURE_LOOP>
  <IF STRUCTURE_ISAM>

            ;;------------------------------------------------------------
            ;;Test data for <StructureNoplural>
            open(chin=0,i:i,"<FILE_NAME>")

;//
;// ENABLE_GET_ALL
;//
    <IF DEFINED_ENABLE_GET_ALL>
            ;Total number of records
            count = 0
            repeat
            begin
                reads(chin,<structureNoplural>,eof<StructureNoplural>1)
                count += 1
                nextloop
            eof<StructureNoplural>1,
                if (count) then
                    TestConstants.Instance.Get<StructurePlural>_Count = count
                else
                    Console.WriteLine("ERROR: Failed to read record from <FILE_NAME>")
                exitloop
            end
            
;//
;// ENABLE_GET_ONE
;//
    <IF DEFINED_ENABLE_GET_ONE>
            ;Get by primary key
            repeat
            begin
                read(chin,<structureNoplural>,^FIRST) [ERR=eof<StructureNoplural>2]
              <PRIMARY_KEY>
                <SEGMENT_LOOP>
                pknum = <IF SEG_TYPE_FIELD><structureNoplural>.<segment_name><ELSE SEG_TYPE_LITERAL>"<SEGMENT_LITVAL>"</IF SEG_TYPE_FIELD>
                TestConstants.Instance.Get<StructureNoplural>_<SegmentName> = pknum
                </SEGMENT_LOOP>
              </PRIMARY_KEY>
                exitloop
            eof<StructureNoplural>2,
                Console.WriteLine("ERROR: Failed to read first record from <FILE_NAME>")
                exitloop
            end

      <IF DEFINED_ENABLE_RELATIONS>
        <IF STRUCTURE_RELATIONS>
          <RELATION_LOOP_RESTRICTED>
            <PRIMARY_KEY>
              <SEGMENT_LOOP>
            TestConstants.Instance.Get<StructureNoplural>_Expand_<HARMONYCORE_RELATION_NAME>_<SegmentName> = pknum
              </SEGMENT_LOOP>
            </PRIMARY_KEY>
          </RELATION_LOOP_RESTRICTED>
;//
          <PRIMARY_KEY>
            <SEGMENT_LOOP>
            TestConstants.Instance.Get<StructureNoplural>_Expand_All_<SegmentName> = pknum
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
            TestConstants.Instance.Get<StructureNoplural>_ByAltKey_<KeyName>_<SegmentName> = <IF DATEORTIME>DateTime.ParseExact((string)(<structureNoplural>.<segment_name>), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)<ELSE><IF SEG_TYPE_FIELD><structureNoplural>.<segment_name><ELSE SEG_TYPE_LITERAL>"<SEGMENT_LITVAL>"</IF SEG_TYPE_FIELD></IF DATEORTIME>
    </SEGMENT_LOOP>
  </ALTERNATE_KEY_LOOP_UNIQUE>
;//
;//
;//
  <PRIMARY_KEY>
    <SEGMENT_LOOP>
            TestConstants.Instance.Update<StructureNoplural>_<SegmentName> = pknum + 1
    </SEGMENT_LOOP>
  </PRIMARY_KEY>

            close chin
  </IF STRUCTURE_ISAM>
</STRUCTURE_LOOP>

            File.WriteAllText(Path.Combine(Environment.GetEnvironmentVariable("SOLUTIONDIR"), "Services.Test", "TestConstants.Values.json"), JsonSerializer.Serialize(TestConstants.Instance, new JsonSerializerOptions(){ WriteIndented = true }))
        endmethod
    endclass
endnamespace
