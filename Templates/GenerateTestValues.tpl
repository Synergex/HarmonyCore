<CODEGEN_FILENAME>GenerateTestValues.dbl</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.6.5</REQUIRES_CODEGEN_VERSION>
<REQUIRES_USERTOKEN>UNIT_TEST_NAMESPACE</REQUIRES_USERTOKEN>

import System
import System.Text.Json
import System.Text.Json.Serialization
import System.IO
import <UNIT_TEST_NAMESPACE>

main GenerateTestValues
proc
    <UNIT_TEST_NAMESPACE>.UnitTestEnvironment.AssemblyInitialize(^null)
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
;//RELATION LOGIC MISSING
    </IF DEFINED_ENABLE_GET_ALL>
;//
;// ENABLE_GET_ONE
;//
    <IF DEFINED_ENABLE_GET_ONE>
            ;Get by primary key
            repeat
            begin
                read(chin,<structureNoplural>,^LAST) [ERR=eof<StructureNoplural>2]
              <PRIMARY_KEY>
                <SEGMENT_LOOP>
                TestConstants.Instance.Get<StructureNoplural>_<SegmentName> = <IF SEG_TYPE_FIELD><structureNoplural>.<segment_name><ELSE SEG_TYPE_LITERAL>"<SEGMENT_LITVAL>"</IF SEG_TYPE_FIELD>
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
            TestConstants.Instance.Get<StructureNoplural>_Expand_<HARMONYCORE_RELATION_NAME>_<SegmentName> = <IF SEG_TYPE_FIELD><structureNoplural>.<segment_name><ELSE SEG_TYPE_LITERAL>"<SEGMENT_LITVAL>"</IF SEG_TYPE_FIELD>
              </SEGMENT_LOOP>
            </PRIMARY_KEY>
          </RELATION_LOOP_RESTRICTED>
;//
          <PRIMARY_KEY>
            <SEGMENT_LOOP>
            TestConstants.Instance.Get<StructureNoplural>_Expand_All_<SegmentName> = <IF SEG_TYPE_FIELD><structureNoplural>.<segment_name><ELSE SEG_TYPE_LITERAL>"<SEGMENT_LITVAL>"</IF SEG_TYPE_FIELD>
            </SEGMENT_LOOP>
          </PRIMARY_KEY>
        </IF STRUCTURE_RELATIONS>
      </IF DEFINED_ENABLE_RELATIONS>
;//
;//
    </IF DEFINED_ENABLE_GET_ONE>
;//
;//
;//
;//
  <ALTERNATE_KEY_LOOP_UNIQUE>
    <SEGMENT_LOOP>
            TestConstants.Instance.Get<StructureNoplural>_ByAltKey_<KeyName>_<SegmentName> = <IF DATEORTIME>DateTime.ParseExact((string)(<structureNoplural>.<segment_name>), <IF DATE>"yyyyMMdd"<ELSE>"hhmmss"</IF DATE>, System.Globalization.CultureInfo.InvariantCulture)<ELSE><IF SEG_TYPE_FIELD><structureNoplural>.<segment_name><ELSE SEG_TYPE_LITERAL>"<SEGMENT_LITVAL>"</IF SEG_TYPE_FIELD></IF DATEORTIME>
    </SEGMENT_LOOP>
  </ALTERNATE_KEY_LOOP_UNIQUE>
;//
;//
;//
  <PRIMARY_KEY>
    <SEGMENT_LOOP>
            TestConstants.Instance.Update<StructureNoplural>_<SegmentName> = <IF SEG_TYPE_FIELD><structureNoplural>.<segment_name><ELSE SEG_TYPE_LITERAL>"<SEGMENT_LITVAL>"</IF SEG_TYPE_FIELD> + <IF ALPHA>"A"<ELSE>1</IF ALPHA>
    </SEGMENT_LOOP>
  </PRIMARY_KEY>

            close chin
  </IF STRUCTURE_ISAM>
</STRUCTURE_LOOP>

            data jsonFilePath = <UNIT_TEST_NAMESPACE>.UnitTestEnvironment.FindRelativeFolderForAssembly("<UNIT_TEST_NAMESPACE>")
            File.WriteAllText(Path.Combine(jsonFilePath, "TestConstants.Values.json"), JsonSerializer.Serialize(TestConstants.Instance, new JsonSerializerOptions(){ WriteIndented = true }))
        endmethod
    endclass
endnamespace