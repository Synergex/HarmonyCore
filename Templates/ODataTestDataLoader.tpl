<CODEGEN_FILENAME><StructureNoplural>Loader.dbl</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.3.5</REQUIRES_CODEGEN_VERSION>
<REQUIRES_USERTOKEN>MODELS_NAMESPACE</REQUIRES_USERTOKEN>

import System.Collections.Generic
import <MODELS_NAMESPACE>

namespace <NAMESPACE>

    public static partial class <StructureNoplural>Loader
    
        public static method LoadFromFile, @List<<StructureNoplural>>
        proc
            data dataFile = "<FILE_NAME>"
            data textFile = dataFile.ToLower().Replace(".ism",".txt")
            data <structureNoplural>Ch, int, 0
            data <structureNoplural>Rec, str<StructureNoplural>
            data <structurePlural> = new List<<StructureNoplural>>()
            open(<structureNoplural>Ch,i:s,textFile)
            repeat
            begin
                reads(<structureNoplural>Ch,<structureNoplural>Rec,eof)
                <structurePlural>.Add(new <StructureNoplural>(<structureNoplural>Rec))
            end
        eof,
            close <structureNoplural>Ch
            mreturn <structurePlural>
        endmethod

    endclass

endnamespace
