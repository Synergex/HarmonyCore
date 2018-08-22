<CODEGEN_FILENAME>SwaggerFile.json</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.3.5</REQUIRES_CODEGEN_VERSION>
<REQUIRES_USERTOKEN>API_CONTACT_EMAIL</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>API_DESCRIPTION</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>API_LICENSE_NAME</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>API_LICENSE_URL</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>API_TERMS_URL</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>API_TITLE</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>API_VERSION</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>SERVER_BASE_PATH</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>SERVER_HTTPS_PORT</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>SERVER_NAME</REQUIRES_USERTOKEN>
{
;//----------------------------------------------------------------------------
;//
;// Document header
;//
  "swagger": "2.0",
  "info": {
    "description": "<API_DESCRIPTION>",
    "version": "<API_VERSION>",
    "title": "<API_TITLE>",
    "termsOfService": "<API_TERMS_URL>",
    "contact": { "email": "<API_CONTACT_EMAIL>" },
    "license": { "name": "<API_LICENSE_NAME>", "url": "<API_LICENSE_URL>" }
  },
  "host": "<SERVER_NAME>:<SERVER_HTTPS_PORT>",
  "basePath": "<SERVER_BASE_PATH>",
  "schemes": [ "https" ],
  "consumes": [ "application/json" ],
  "produces": [ "application/json" ],
;//
;//----------------------------------------------------------------------------
;//
;// Tags that can be applied to operations to categorize them
;//
  "tags": [
<STRUCTURE_LOOP>
    {
      "name": "<StructureNoplural>",
      "description": "Operations related to <STRUCTURE_DESC>",
    },
</STRUCTURE_LOOP>
    {
      "name": "Create",
      "description": "Create operations",
    },
    {
      "name": "Read",
      "description": "Read operations",
    },
    {
      "name": "Update",
      "description": "Update operations",
    },
    {
      "name": "Delete",
      "description": "Delete operations",
    }
  ],
;//----------------------------------------------------------------------------
;//
;// Operation paths
;//
  "paths": {
<STRUCTURE_LOOP>
;//----------------------------------------------------------------------------
;//
;// Get all records
;//
    "/<StructurePlural>": {
      "get": {
        "description": "Get all <structurePlural>",
        "tags": [
          "<StructureNoplural>",
          "Read"
        ],
        "parameters": [
          {
            "name": "$expand",
            "in": "query",
            "description": "Expand one or more navigation properties.",
            "type": "string"
          },
          {
            "name": "$select",
            "in": "query",
            "description": "List of properties to be returned.",
            "type": "string"
          },
          {
            "name": "$filter",
            "in": "query",
            "description": "Filter expression to restrict returned rows.",
            "type": "string"
          },
          {
            "name": "$orderby",
            "in": "query",
            "description": "Order by some property",
            "type": "string"
          },
          {
            "name": "$top",
            "in": "query",
            "description": "Maximum number of rows to return.",
            "type": "integer"
          },
          {
            "name": "$skip",
            "in": "query",
            "description": "Rows to skip before starting to return data.",
            "type": "integer"
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "schema": {
              "type": "array",
              "items": {
                "$ref": "#/definitions/<StructureNoplural>"
              }
            }
          }
        }
      }
    },
    "/<StructurePlural>/$count": {
      "get": {
        "description": "Get count of <structurePlural>",
        "tags": [
          "<StructureNoplural>",
          "Read"
        ],
        "parameters": [ 
          {
            "name": "$filter",
            "in": "query",
            "description": "Filter expression to restrict returned rows.",
            "type": "string"
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "type": "integer"
          }
        }
      }
    },
;//----------------------------------------------------------------------------
;//
;// Primary key operations
;//
<PRIMARY_KEY>
    "/<StructurePlural>(<SEGMENT_LOOP><SegmentName>=<IF ALPHA>'</IF ALPHA>{a<SegmentName>}<IF ALPHA>'</IF ALPHA><,></SEGMENT_LOOP>)": {
;//   ----------------------------------------------------------------------------
;//   Get by primary key
;//
      "get": {
        "description": "Get a <StructureNoplural> by primary key.",
        "operationId": "Get a <StructureNoplural> by primary key.",
        "tags": [
          "<StructureNoplural>",
          "Read"
        ],
        "parameters": [ 
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
            "name": "$expand",
            "in": "query",
            "description": "Expand one or more navigation properties.",
            "type": "string"
          },
          {
            "name": "$select",
            "in": "query",
            "description": "List of properties to be returned.",
            "type": "string"
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "schema": {
              "type": "object",
              "$ref": "#/definitions/<StructureNoplural>"
            }
          }
        }
      },
;//   ----------------------------------------------------------------------------
;//   Delete by primary key
;//
      "delete": {
        "description": "Delete a <StructureNoplural> by primary key.",
        "operationId": "Delete a <StructureNoplural> by primary key.",
        "tags": [
          "<StructureNoplural>",
          "Delete"
        ],
        "parameters": [ 
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
;//   ----------------------------------------------------------------------------
;//   Create or update by primary key
;//
      "put": {
        "description": "Create or update a <StructureNoplural> by primary key.",
        "operationId": "Create or update a <StructureNoplural> by primary key.",
        "tags": [
          "<StructureNoplural>",
          "Create",
          "Update"
        ],
        "parameters": [ 
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
            "schema": {
              "type": "object",
              "$ref": "#/definitions/<StructureNoplural>"
            }
          }
        ],
        "responses": {
          "204": {
            "description": "OK"
          }
        }
      },
;//   ----------------------------------------------------------------------------
;//   Patch by primary key
;//
      "patch": {
        "description": "Patch a <StructureNoplural> by primary key.",
        "operationId": "Patch a <StructureNoplural> by primary key.",
        "tags": [
          "<StructureNoplural>",
          "Update"
        ],
        "parameters": [ 
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
            "schema": {
              "type": "object",
              "$ref": "#/definitions/<StructureNoplural>"
            }
          }
        ],
        "responses": {
          "204": {
            "description": "OK"
          }
        }
      }
;//   ----------------------------------------------------------------------------
;//   End of final primary key operation
;//   ----------------------------------------------------------------------------
    },
</PRIMARY_KEY>
;//----------------------------------------------------------------------------
;//
;// Full alternate key operations
;//
<ALTERNATE_KEY_LOOP>
    "/<StructurePlural>(<SEGMENT_LOOP><SegmentName>=<IF ALPHA>'</IF ALPHA>{a<SegmentName>}<IF ALPHA>'</IF ALPHA><,></SEGMENT_LOOP>)": {
;//   ----------------------------------------------------------------------------
;//   Get by alternate key
;//
      "get": {
  <IF DUPLICATES>
        "description": "Get <structurePlural> via alternate key <KEY_NAME>.",
        "operationId": "Get <structurePlural> via alternate key <KEY_NAME>.",
  <ELSE>
        "description": "Get a <structureNoplural> via alternate key <KEY_NAME>.",
        "operationId": "Get a <structureNoplural> via alternate key <KEY_NAME>.",
  </IF DUPLICATES>
        "tags": [
          "<StructureNoplural>",
		  "Read"
        ],
        "parameters": [ 
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
            "name": "$expand",
            "in": "query",
            "description": "Expand one or more navigation properties.",
            "type": "string"
          },
          {
            "name": "$select",
            "in": "query",
            "description": "List of properties to be returned.",
            "type": "string"
          }<IF DUPLICATES>,</IF DUPLICATES>
  <IF DUPLICATES>
          {
            "name": "$orderby",
            "in": "query",
            "description": "Order by some property",
            "type": "string"
          },
          {
            "name": "$filter",
            "in": "query",
            "description": "Filter expression to restrict returned rows.",
            "type": "string"
          },
          {
            "name": "$top",
            "in": "query",
            "description": "Maximum number of rows to return.",
            "type": "integer"
          },
          {
            "name": "$skip",
            "in": "query",
            "description": "Rows to skip before starting to return data.",
            "type": "integer"
          }
  </IF DUPLICATES>
        ],
        "responses": {
          "200": {
            "description": "OK",
            "schema": {
  <IF DUPLICATES>
              "type": "array",
  <ELSE>
              "type": "object",
  </IF DUPLICATES>
              "$ref": "#/definitions/<StructureNoplural>"
            }
          }
        }
      }
;//   ----------------------------------------------------------------------------
;//   End of final alternate key operation
;//   ----------------------------------------------------------------------------
    }<IF DUPLICATES>,<ELSE><,></IF DUPLICATES>
  <IF DUPLICATES>
    "/<StructurePlural>(<SEGMENT_LOOP><SegmentName>=<IF ALPHA>'</IF ALPHA>{a<SegmentName>}<IF ALPHA>'</IF ALPHA><,></SEGMENT_LOOP>)/$count": {
;//   ----------------------------------------------------------------------------
;//   Get count by alternate key
;//
      "get": {
        "description": "Get count of <structurePlural> via alternate key <KEY_NAME>.",
        "operationId": "Get count of <structurePlural> via alternate key <KEY_NAME>.",
        "tags": [
          "<StructureNoplural>",
		  "Read"
        ],
        "parameters": [ 
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
            "name": "$filter",
            "in": "query",
            "description": "Filter expression to restrict returned rows.",
            "type": "string"
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "type": "integer"
          }
        }
      }
;//   ----------------------------------------------------------------------------
;//   End of final alternate key operation
;//   ----------------------------------------------------------------------------
    }<,>
  </IF DUPLICATES>
</ALTERNATE_KEY_LOOP>
<,>
</STRUCTURE_LOOP>
  },
;//----------------------------------------------------------------------------
;// Configure an authentication server (Needs more work before implementation)
;//
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
;//----------------------------------------------------------------------------
;// Definitions of data models
;//
  "definitions": {
<STRUCTURE_LOOP>
    "<StructureNoplural>": {
      "required": [<PRIMARY_KEY><SEGMENT_LOOP> "<FieldSqlname>"<,></SEGMENT_LOOP></PRIMARY_KEY> ],
      "properties": {
<FIELD_LOOP>
<IF CUSTOM_NOT_HARMONY_EXCLUDE>
        "<FieldSqlname>": {
 <IF ALPHA>
          "type": "string",
          "example": <FIELD_SAMPLE_DATA>,
          "description": "<FIELD_DESC>"
</IF ALPHA>
<IF DECIMAL>
<IF PRECISION>
          "type": "number",
          "format": "float",
          "example": <FIELD_SAMPLE_DATA>,
          "description": "<FIELD_DESC>"
<ELSE>
<IF CUSTOM_HARMONY_AS_STRING>
          "type": "string",
          "example": "<FIELD_SAMPLE_DATA>",
<ELSE>
          "type": "integer",
          "example": <FIELD_SAMPLE_DATA>,
</IF CUSTOM_HARMONY_AS_STRING>
          "description": "<FIELD_DESC>"
</IF PRECISION>
</IF DECIMAL>
<IF DATE>
          "type": "string",
          "format": "date-time",
          "example": "<FIELD_SAMPLE_DATA>",
          "description": "<FIELD_DESC>"
</IF DATE>
<IF TIME>
          "type": "string",
          "format": "date-time",
          "example": "<FIELD_SAMPLE_DATA>",
          "description": "<FIELD_DESC>"
</IF TIME>
<IF INTEGER>
          "type": "number",
          "format": "<IF I124>int32<ELSE>int64</IF I124>",
          "example": <FIELD_SAMPLE_DATA>,
          "description": "<FIELD_DESC>"
</IF INTEGER>
        }<,>
</IF CUSTOM_NOT_HARMONY_EXCLUDE>
</FIELD_LOOP>
      },
      "example": {
<FIELD_LOOP>
<IF CUSTOM_NOT_HARMONY_EXCLUDE>
 <IF ALPHA>
        "<FieldSqlname>": <FIELD_SAMPLE_DATA><,>
</IF ALPHA>
<IF DECIMAL>
<IF PRECISION>
        "<FieldSqlname>": <FIELD_SAMPLE_DATA><,>
<ELSE>
<IF CUSTOM_HARMONY_AS_STRING>
        "<FieldSqlname>": "<FIELD_SAMPLE_DATA>"<,>
<ELSE>
        "<FieldSqlname>": <FIELD_SAMPLE_DATA><,>
</IF CUSTOM_HARMONY_AS_STRING>
</IF PRECISION>
</IF DECIMAL>
<IF DATE>
        "<FieldSqlname>": "<FIELD_SAMPLE_DATA>"<,>
</IF DATE>
<IF TIME>
        "<FieldSqlname>": "<FIELD_SAMPLE_DATA>"<,>
</IF TIME>
<IF INTEGER>
        "<FieldSqlname>": <FIELD_SAMPLE_DATA><,>
</IF INTEGER>
</IF CUSTOM_NOT_HARMONY_EXCLUDE>
</FIELD_LOOP>
      }
    },
</STRUCTURE_LOOP>
;//----------------------------------------------------------------------------
;// Deta model definitions for PATCH requests
;//
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
;//----------------------------------------------------------------------------
}