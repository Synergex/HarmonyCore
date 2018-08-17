<CODEGEN_FILENAME>HarmonyCoreSwaggerFile.json</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.3.5</REQUIRES_CODEGEN_VERSION>
<REQUIRES_USERTOKEN>API_DESCRIPTION</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>API_VERSION</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>API_TITLE</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>API_TERMS_URL</REQUIRES_USERTOKEN>
{
  "swagger" : "2.0",
  "info" : {
    "description" : "<API_DESCRIPTION>",
    "version" : "<API_VERSION>",
    "title" : "<API_TITLE>",
    "termsOfService" : "<API_TERMS_URL>"
  },
  "schemes" : [ "https" ],
  "consumes" : [ "application/json" ],
  "produces" : [ "application/json" ],
  "paths" : {
<STRUCTURE_LOOP>
;//
;// Get all
;//
    "/<StructurePlural>" : {
      "get" : {
        "description" : "Get all <StructurePlural>",
        "tags": [
          "<StructureNoplural> Operations"
        ],
        "parameters" : [ ],
        "responses" : {
          "200" : {
            "description" : "OK",
            "schema" : {
              "type" : "array",
              "items" : {
                "$ref" : "#/definitions/<StructureNoplural>"
              }
            }
          }
        }
      }
    },
;//
;// Get single by primary key
;//
<PRIMARY_KEY>
    "/<StructurePlural>(<SEGMENT_LOOP><SegmentName>={a<SegmentName>}<,></SEGMENT_LOOP>)" : {
      "get" : {
        "description" : "Get a <StructureNoplural> by primary key.",
        "operationId": "Get a <StructureNoplural> by primary key.",
        "tags": [
          "<StructureNoplural> Operations"
        ],
        "parameters" : [ 
<SEGMENT_LOOP>
          {
            "name": "a<SegmentName>",
            "in": "path",
            "description": "<SEGMENT_DESC>",
            "required": true,
            "type": "<IF ALPHA>string</IF ALPHA><IF DECIMAL><IF PRECISION>number<ELSE>integer</IF PRECISION></IF DECIMAL><IF DATE>string</IF DATE><IF TIME>string</IF TIME><IF INTEGER>number</IF INTEGER>",
<IF DATE>
            "format": "date-time"
</IF DATE>
<IF TIME>
            "format": "date-time"
</IF TIME>
          }<,>
</SEGMENT_LOOP>
        ],
        "responses" : {
          "200" : {
            "description" : "OK",
            "schema" : {
              "type" : "object",
              "$ref" : "#/definitions/<StructureNoplural>"
            }
          }
        }
      },
      "delete": {
        "description": "Delete a <StructureNoplural> by primary key.",
        "operationId": "Delete a <StructureNoplural> by primary key.",
        "tags": [
          "<StructureNoplural> Operations"
        ],
        "parameters" : [ 
<SEGMENT_LOOP>
          {
            "name": "a<SegmentName>",
            "in": "path",
            "description": "<SEGMENT_DESC>",
            "required": true,
            "type": "<IF ALPHA>string</IF ALPHA><IF DECIMAL><IF PRECISION>number<ELSE>integer</IF PRECISION></IF DECIMAL><IF DATE>string</IF DATE><IF TIME>string</IF TIME><IF INTEGER>number</IF INTEGER>",
<IF DATE>
            "format": "date-time"
</IF DATE>
<IF TIME>
            "format": "date-time"
</IF TIME>
          }<,>
</SEGMENT_LOOP>
        ],
        "responses": {
          "204": {
            "description": "OK"
          }
        }
      },
      "put": {
        "description": "Create or update a <StructureNoplural> by primary key.",
        "operationId": "Create or update a <StructureNoplural> by primary key.",
        "tags": [
          "<StructureNoplural> Operations"
        ],
        "parameters" : [ 
<SEGMENT_LOOP>
          {
            "name": "a<SegmentName>",
            "in": "path",
            "description": "<SEGMENT_DESC>",
            "required": true,
            "type": "<IF ALPHA>string</IF ALPHA><IF DECIMAL><IF PRECISION>number<ELSE>integer</IF PRECISION></IF DECIMAL><IF DATE>string</IF DATE><IF TIME>string</IF TIME><IF INTEGER>number</IF INTEGER>",
<IF DATE>
            "format": "date-time"
</IF DATE>
<IF TIME>
            "format": "date-time"
</IF TIME>
          },
</SEGMENT_LOOP>
          {
            "name": "a<StructureNoplural>",
            "in": "body",
            "description": "Data for <structureNoplural> to create or update.",
            "required": true,
            "schema" : {
              "type" : "object",
              "$ref" : "#/definitions/<StructureNoplural>"
            }
          }
        ],
        "responses": {
          "204": {
            "description": "OK"
          }
        }
      },
      "patch": {
        "description": "Patch a <StructureNoplural> by primary key.",
        "operationId": "Patch a <StructureNoplural> by primary key.",
        "tags": [
          "<StructureNoplural> Operations"
        ],
        "parameters" : [ 
<SEGMENT_LOOP>
          {
            "name": "a<SegmentName>",
            "in": "path",
            "description": "<SEGMENT_DESC>",
            "required": true,
            "type": "<IF ALPHA>string</IF ALPHA><IF DECIMAL><IF PRECISION>number<ELSE>integer</IF PRECISION></IF DECIMAL><IF DATE>string</IF DATE><IF TIME>string</IF TIME><IF INTEGER>number</IF INTEGER>",
<IF DATE>
            "format": "date-time"
</IF DATE>
<IF TIME>
            "format": "date-time"
</IF TIME>
          },
</SEGMENT_LOOP>
          {
            "name": "a<StructureNoplural>",
            "in": "body",
            "description": "Data for <structureNoplural> to create or update.",
            "required": true,
            "schema" : {
              "type" : "object",
              "$ref" : "#/definitions/<StructureNoplural>"
            }
          }
        ],
        "responses": {
          "204": {
            "description": "OK"
          }
        }
      }
    },
</PRIMARY_KEY>
<ALTERNATE_KEY_LOOP>
    "/<StructurePlural>(<SEGMENT_LOOP><SegmentName>={a<SegmentName>}<,></SEGMENT_LOOP>)" : {
      "get" : {
        "description" : "Get a <StructureNoplural> by alternate key <KEY_NAME>.",
        "operationId": "Get a <StructureNoplural> by alternate key <KEY_NAME>.",
        "tags": [
          "<StructureNoplural> Operations"
        ],
        "parameters" : [ 
<SEGMENT_LOOP>
          {
            "name": "a<SegmentName>",
            "in": "path",
            "description": "<SEGMENT_DESC>",
            "required": true,
            "type": "<IF ALPHA>string</IF ALPHA><IF DECIMAL><IF PRECISION>number<ELSE>integer</IF PRECISION></IF DECIMAL><IF DATE>string</IF DATE><IF TIME>string</IF TIME><IF INTEGER>number</IF INTEGER>",
<IF DATE>
            "format": "date-time"
</IF DATE>
<IF TIME>
            "format": "date-time"
</IF TIME>
          }<,>
</SEGMENT_LOOP>
        ],
        "responses" : {
          "200" : {
            "description" : "OK",
            "schema" : {
              "type" : "object",
              "$ref" : "#/definitions/<StructureNoplural>"
            }
          }
        }
      }
  }<,>
</ALTERNATE_KEY_LOOP>
<,>
</STRUCTURE_LOOP>
  },
  "definitions" : {
<STRUCTURE_LOOP>
    "<StructureNoplural>" : {
      "required" : [<PRIMARY_KEY><SEGMENT_LOOP> "<FieldSqlname>"<,></SEGMENT_LOOP></PRIMARY_KEY> ],
      "properties" : {
<FIELD_LOOP>
<IF CUSTOM_NOT_HARMONY_EXCLUDE>
        "<FieldSqlname>": {
 <IF ALPHA>
          "type": "string",
          "example" : <FIELD_SAMPLE_DATA>,
          "description": "<FIELD_DESC>"
</IF ALPHA>
<IF DECIMAL>
<IF PRECISION>
          "type": "number",
          "format": "float",
          "example" : <FIELD_SAMPLE_DATA>,
          "description": "<FIELD_DESC>"
<ELSE>
          "type": "integer",
          "example" : <FIELD_SAMPLE_DATA>,
          "description": "<FIELD_DESC>"
</IF PRECISION>
</IF DECIMAL>
<IF DATE>
          "type": "string",
          "format": "date-time",
          "example" : <FIELD_SAMPLE_DATA>,
          "description": "<FIELD_DESC>"
</IF DATE>
<IF TIME>
          "type": "string",
          "format": "date-time",
          "example" : <FIELD_SAMPLE_DATA>,
          "description": "<FIELD_DESC>"
</IF TIME>
<IF INTEGER>
          "type": "number",
          "format": "<IF I124>int32<ELSE>int64</IF I124>",
          "example" : <FIELD_SAMPLE_DATA>,
          "description": "<FIELD_DESC>"
</IF INTEGER>
        }<,>
</IF CUSTOM_NOT_HARMONY_EXCLUDE>
</FIELD_LOOP>
      },
      "example" : {
<FIELD_LOOP>
<IF CUSTOM_NOT_HARMONY_EXCLUDE>
        "<FieldSqlname>" : <FIELD_SAMPLE_DATA><,>
</IF CUSTOM_NOT_HARMONY_EXCLUDE>
</FIELD_LOOP>
      }
    },
</STRUCTURE_LOOP>
    "PatchRequest": {
      "type": "array",
      "items": {
        "$ref": "#/definitions/PatchDocument"
      }
    },
    "PatchDocument": {
      "description": "A JSONPatch document as defined by RFC 6902",
      "required": [
        "op",
        "path"
      ],
      "properties": {
        "op": {
          "type": "string",
          "description": "The operation to be performed",
          "enum": [
            "add",
            "remove",
            "replace",
            "move",
            "copy",
            "test"
          ]
        },
        "path": {
          "type": "string",
          "description": "A JSON-Pointer"
        },
        "value": {
          "type": "object",
          "description": "The value to be used within the operations."
        },
        "from": {
          "type": "string",
          "description": "A string containing a JSON Pointer value."
        }
      }
    }
  }
}