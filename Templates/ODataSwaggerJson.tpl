<CODEGEN_FILENAME>SwaggerFile.json</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.3.5</REQUIRES_CODEGEN_VERSION>
<OPTIONAL_USERTOKEN>API_TITLE=Harmony Core Sample API</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>API_VERSION=1.0.0</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>API_DESCRIPTION=This environment presents an example of using Harmony Core to expose a collection of RESTful Web Service endpoints that alklow you to interact with a small sample dataset.</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>API_TERMS_URL=/license.html</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>CONTACT_EMAIL=jodah.veloper@synergexpsg.com</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>LICENSE_NAME=BSD 2-Clause License</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>LICENSE_URL=https://github.com/Synergex/HarmonyCore/blob/master/LICENSE.md</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>SERVER_NAME=localhost</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>SERVER_PORT=8081</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>BASE_PATH=/odata</OPTIONAL_USERTOKEN>
{
  "swagger" : "2.0",
  "info" : {
    "description" : "<API_DESCRIPTION>",
    "version" : "<API_VERSION>",
    "title" : "<API_TITLE>",
    "termsOfService" : "<API_TERMS_URL>",
    "contact": { "email": "<CONTACT_EMAIL>" },
    "license": { "name": "<LICENSE_NAME>", "url": "<LICENSE_URL>" }
  },
  "host": "<SERVER_NAME>:<SERVER_PORT>",
  "basePath": "<BASE_PATH>",
  "schemes" : [ "https" ],
  "consumes" : [ "application/json" ],
  "produces" : [ "application/json" ],
  "tags": [
<STRUCTURE_LOOP>
    {
      "name": "<StructureNoplural>",
      "description": "Operations related to <STRUCTURE_DESC>",
    }<,>
</STRUCTURE_LOOP>
  ],
  "paths" : {
<STRUCTURE_LOOP>
;//
;// Get all
;//
    "/<StructurePlural>" : {
      "get" : {
        "description" : "Get all <StructurePlural>",
        "tags": [
          "<StructureNoplural>"
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
    "/<StructurePlural>(<SEGMENT_LOOP><SegmentName>=<IF ALPHA>'</IF ALPHA>{a<SegmentName>}<IF ALPHA>'</IF ALPHA><,></SEGMENT_LOOP>)" : {
      "get" : {
        "description" : "Get a <StructureNoplural> by primary key.",
        "operationId": "Get a <StructureNoplural> by primary key.",
        "tags": [
          "<StructureNoplural>"
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
          "<StructureNoplural>"
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
          "<StructureNoplural>"
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
          "<StructureNoplural>"
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
    "/<StructurePlural>(<SEGMENT_LOOP><SegmentName>=<IF ALPHA>'</IF ALPHA>{a<SegmentName>}<IF ALPHA>'</IF ALPHA><,></SEGMENT_LOOP>)" : {
      "get" : {
        "description" : "Get a <StructureNoplural> by alternate key <KEY_NAME>.",
        "operationId": "Get a <StructureNoplural> by alternate key <KEY_NAME>.",
        "tags": [
          "<StructureNoplural>"
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
;//  "securityDefinitions": {
;//    "oauth2schema": {
;//      "type": "oauth2",
;//      "tokenUrl": "http://localhost:5000/connect/token",
;//      "flow": "application",
;//      "scopes": {
;//        "global": "global"
;//      }
;//    }
;//  },
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