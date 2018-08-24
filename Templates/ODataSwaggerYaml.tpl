<CODEGEN_FILENAME>SwaggerFile.yaml</CODEGEN_FILENAME>
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
;//
;//----------------------------------------------------------------------------
;//
;// Document header
;//
swagger: '2.0'
info:
  description: <API_DESCRIPTION>
  version: <API_VERSION>
  title: <API_TITLE>
  termsOfService: <API_TERMS_URL>
  contact:
    email: <API_CONTACT_EMAIL>
  license:
    name: <API_LICENSE_NAME>
    url: '<API_LICENSE_URL>'
host: '<SERVER_NAME>:<SERVER_HTTPS_PORT>'
basePath: <SERVER_BASE_PATH>
schemes:
  - https
consumes:
  - application/json
produces:
  - application/json
;//
;//----------------------------------------------------------------------------
;//
;// Tags that can be applied to operations to categorize them
;//
tags:
<STRUCTURE_LOOP>
  - name: <StructureNoplural>
    description: Operations related to <STRUCTURE_DESC>
  - name: <StructureNoplural>Properties
    description: Operations related to individual properties of <STRUCTURE_DESC>
</STRUCTURE_LOOP>
  - name: Count
    description: All count operations
  - name: Create
    description: All create operations
  - name: Read
    description: All read operations
  - name: Update
    description: All update operations
  - name: Delete
    description: All delete operations
;//
;//----------------------------------------------------------------------------
;//
;// Operation paths
;//
paths:
<STRUCTURE_LOOP>
;//
;//----------------------------------------------------------------------------
;//
;// Get all records
;//
  /<StructurePlural>:
    get:
      summary: Get <structurePlural>
      description: Get all or a filtered collection of <structurePlural>.
      operationId: Get<StructurePlural>
      deprecated: false
      tags:
        - <StructureNoplural>
        - Read
      parameters:
        - name: $expand
          in: query
          description: Expand one or more navigation properties.
          type: string
        - name: $select
          in: query
          description: List of properties to be returned.
          type: string
        - name: $filter
          in: query
          description: Filter expression to restrict returned rows.
          type: string
        - name: $orderby
          in: query
          description: Order by some property
          type: string
        - name: $top
          in: query
          description: Maximum number of rows to return.
          type: integer
        - name: $skip
          in: query
          description: Rows to skip before starting to return data.
          type: integer
      responses:
        '200':
          description: OK
          schema:
            type: array
            items:
              $ref: '#/definitions/<StructureNoplural>'
  /<StructurePlural>/$count:
    get:
      summary: Count <structurePlural>
      description: Count all or a filtered collection of <structurePlural>.
      operationId: Count<StructurePlural>
      deprecated: false
      tags:
        - <StructureNoplural>
        - Count
      parameters:
        - name: $filter
          in: query
          description: Filter expression to restrict returned rows.
          type: string
      responses:
        '200':
          description: OK
          schema:
            type: integer
;//
;//----------------------------------------------------------------------------
;//
;// Primary key operations
;//
<PRIMARY_KEY>
  '/<StructurePlural>(<SEGMENT_LOOP><SegmentName>=<IF ALPHA>''</IF ALPHA>{a<SegmentName>}<IF ALPHA>''</IF ALPHA><,></SEGMENT_LOOP>)':
;//
;// ----------------------------------------------------------------------------
;// Get via primary key
;//
    get:
      summary: Get <structureNoplural>
      description: Get a <structureNoplural> via a complete primary key.
      operationId: Get<StructureNoplural>
      deprecated: false
      tags:
        - <StructureNoplural>
        - Read
      parameters:
<SEGMENT_LOOP>
        - name: a<SegmentName>
          in: path
          description: <SEGMENT_DESC>
          required: true
          type: <IF ALPHA>string</IF ALPHA><IF DECIMAL><IF PRECISION>number<ELSE>integer</IF PRECISION></IF DECIMAL><IF DATE>string</IF DATE><IF TIME>string</IF TIME><IF INTEGER>number</IF INTEGER>
<IF DATE>
          format: date-time
</IF DATE>
<IF TIME>
          format: date-time
</IF TIME>
</SEGMENT_LOOP>
        - name: $expand
          in: query
          description: Expand one or more navigation properties.
          type: string
        - name: $select
          in: query
          description: List of properties to be returned.
          type: string
      responses:
        '200':
          description: OK
          schema:
            $ref: '#/definitions/<StructureNoplural>'
;//
;// ----------------------------------------------------------------------------
;// Delete via primary key
;//
<IF DEFINED_ENABLE_DELETE>
    delete:
      summary: Delete <structureNoplural>
      description: Delete a <structureNoplural> via a complete primary key.
      operationId: Delete<StructureNoplural>
      deprecated: false
      tags:
        - <StructureNoplural>
        - Delete
      parameters:
        <SEGMENT_LOOP>
        - name: a<SegmentName>
          in: path
          description: <SEGMENT_DESC>
          required: true
          type: <IF ALPHA>string</IF ALPHA><IF DECIMAL><IF PRECISION>number<ELSE>integer</IF PRECISION></IF DECIMAL><IF DATE>string</IF DATE><IF TIME>string</IF TIME><IF INTEGER>number</IF INTEGER>
          <IF DATE>
          format: date-time
          </IF DATE>
          <IF TIME>
          format: date-time
          </IF TIME>
        </SEGMENT_LOOP>
      responses:
        '204':
          description: OK
</IF DEFINED_ENABLE_DELETE>
;//
;// ----------------------------------------------------------------------------
;// Create or update via primary key
;//
<IF DEFINED_ENABLE_PUT>
    put:
      summary: Create or update <structureNoplural>
      description: Create or update a <structureNoplural> via a complete primary key.
      operationId: CreateOrUpdate<StructureNoplural>
      deprecated: false
      tags:
        - <StructureNoplural>
        - Create
        - Update
      parameters:
<SEGMENT_LOOP>
        - name: a<SegmentName>
          in: path
          description: <SEGMENT_DESC>
          required: true
          type: <IF ALPHA>string</IF ALPHA><IF DECIMAL><IF PRECISION>number<ELSE>integer</IF PRECISION></IF DECIMAL><IF DATE>string</IF DATE><IF TIME>string</IF TIME><IF INTEGER>number</IF INTEGER>
<IF DATE>
          format: date-time
</IF DATE>
<IF TIME>
          format: date-time
</IF TIME>
</SEGMENT_LOOP>
        - name: a<StructureNoplural>
          in: body
          description: Data for <structureNoplural> to create or update.
          required: true
          schema:
            $ref: '#/definitions/<StructureNoplural>'
      responses:
        '204':
          description: OK
</IF DEFINED_ENABLE_PUT>
;//
;// ----------------------------------------------------------------------------
;// Patch via primary key
;//
<IF DEFINED_ENABLE_PATCH>
    patch:
      summary: Patch <structureNoplural>
      description: Patch a <structureNoplural> via complete primary key.
      operationId: Patch<StructureNoplural>
      deprecated: false
      tags:
        - <StructureNoplural>
        - Update
      parameters:
<SEGMENT_LOOP>
        - name: a<SegmentName>
          in: path
          description: <SEGMENT_DESC>
          required: true
          type: <IF ALPHA>string</IF ALPHA><IF DECIMAL><IF PRECISION>number<ELSE>integer</IF PRECISION></IF DECIMAL><IF DATE>string</IF DATE><IF TIME>string</IF TIME><IF INTEGER>number</IF INTEGER>
<IF DATE>
          format: date-time
</IF DATE>
<IF TIME>
          format: date-time
</IF TIME>
</SEGMENT_LOOP>
        - name: a<StructureNoplural>
          in: body
          description: Data for <structureNoplural> to create or update.
          required: true
          schema:
            $ref: '#/definitions/<StructureNoplural>'
      responses:
        '204':
          description: OK
</IF DEFINED_ENABLE_PUT>
</PRIMARY_KEY>
;//
;//----------------------------------------------------------------------------
;//
;// Alternate key operations
;//
<IF DEFINED_ENABLE_ALTERNATE_KEYS>
<ALTERNATE_KEY_LOOP>
  '/<StructurePlural>(<SEGMENT_LOOP><SegmentName>=<IF ALPHA>''</IF ALPHA>{a<SegmentName>}<IF ALPHA>''</IF ALPHA><,></SEGMENT_LOOP>)':
;//
;// ----------------------------------------------------------------------------
;// Get via alternate key
;//
    get:
<IF DUPLICATES>
      summary: Get <structurePlural>
      description: Get a <structurePlural> via complete alternate key <KeyName>.
      operationId: Get<StructurePlural>ByKey<KeyName>
<ELSE>
      summary: Get <structureNoplural>
      description: Get a <structureNoplural> via complete alternate key <KeyName>.
      operationId: <StructureNoplural>ByKey<KeyName>
</IF DUPLICATES>
      deprecated: false
      tags:
        - <StructureNoplural>
        - Read
      parameters:
<SEGMENT_LOOP>
        - name: a<SegmentName>
          in: path
          description: <SEGMENT_DESC>
          required: true
          type: <IF ALPHA>string</IF ALPHA><IF DECIMAL><IF PRECISION>number<ELSE>integer</IF PRECISION></IF DECIMAL><IF DATE>string</IF DATE><IF TIME>string</IF TIME><IF INTEGER>number</IF INTEGER>
<IF DATE>
          format: date-time
</IF DATE>
<IF TIME>
          format: date-time
</IF TIME>
</SEGMENT_LOOP>
        - name: $expand
          in: query
          description: Expand one or more navigation properties.
          type: string
        - name: $select
          in: query
          description: List of properties to be returned.
          type: string
  <IF DUPLICATES>
        - name: $orderby
          in: query
          description: Order by some property
          type: string
        - name: $filter
          in: query
          description: Filter expression to restrict returned rows.
          type: string
        - name: $top
          in: query
          description: Maximum number of rows to return.
          type: integer
        - name: $skip
          in: query
          description: Rows to skip before starting to return data.
          type: integer
</IF DUPLICATES>
      responses:
        '200':
          description: OK
          schema:
  <IF DUPLICATES>
            type: array
            items:
              $ref: '#/definitions/<StructureNoplural>'
  <ELSE>
            $ref: '#/definitions/<StructureNoplural>'
  </IF DUPLICATES>
;//
;// ----------------------------------------------------------------------------
;// Get count via alternate key
;//
  <IF DUPLICATES>
  '/<StructurePlural>(<SEGMENT_LOOP><SegmentName>=<IF ALPHA>''</IF ALPHA>{a<SegmentName>}<IF ALPHA>''</IF ALPHA><,></SEGMENT_LOOP>)/$count':
    get:
      summary: Count <structurePlural>
      description: Count <structurePlural> via complete alternate key <KeyName>.
      operationId: Count<StructurePlural>ByKey<KeyName>
      deprecated: false
      tags:
        - <StructureNoplural>
        - Count
      parameters:
<SEGMENT_LOOP>
        - name: a<SegmentName>
          in: path
          description: <SEGMENT_DESC>
          required: true
          type: <IF ALPHA>string</IF ALPHA><IF DECIMAL><IF PRECISION>number<ELSE>integer</IF PRECISION></IF DECIMAL><IF DATE>string</IF DATE><IF TIME>string</IF TIME><IF INTEGER>number</IF INTEGER>
<IF DATE>
          format: date-time
</IF DATE>
<IF TIME>
          format: date-time
</IF TIME>
</SEGMENT_LOOP>
        - name: $filter
          in: query
          description: Filter expression to restrict returned rows.
          type: string
      responses:
        '200':
          description: OK
          schema:
            type: integer
</IF DUPLICATES>
</ALTERNATE_KEY_LOOP>
</IF DEFINED_ENABLE_ALTERNATE_KEYS>
<IF DEFINED_ENABLE_PROPERTY_ENDPOINTS>
;//
;// ----------------------------------------------------------------------------
;// Single property operations
;//
<FIELD_LOOP>
<PRIMARY_KEY>
;//
;// Invividual property
;//
  '/<StructurePlural>(<IF SINGLE_SEGMENT>{key}<ELSE><SEGMENT_LOOP><SegmentName>=<IF ALPHA>''</IF ALPHA>{a<SegmentName>}<IF ALPHA>''</IF ALPHA><,></SEGMENT_LOOP></IF SINGLE_SEGMENT>)/<FieldSqlName>':
    get:
      summary: Get <structureNoplural> property <FieldSqlName>
      description: Get <structureNoplural> property <FieldSqlName> via complete primary key.
      operationId: <StructureNoplural><FieldSqlName>
      deprecated: false
      tags:
        - <StructureNoplural>Properties
        - Read
      parameters:
        <SEGMENT_LOOP>
        <IF SINGLE_SEGMENT>
        - name: key
        <ELSE>
        - name: a<SegmentName>
        </IF SINGLE_SEGMENT>
          in: path
          description: <SEGMENT_DESC>
          required: true
          type: <IF ALPHA>string</IF ALPHA><IF DECIMAL><IF PRECISION>number<ELSE>integer</IF PRECISION></IF DECIMAL><IF DATE>string</IF DATE><IF TIME>string</IF TIME><IF INTEGER>number</IF INTEGER>
          <IF DATE>
          format: date-time
          </IF DATE>
          <IF TIME>
          format: date-time
          </IF TIME>
          </SEGMENT_LOOP>
      responses:
        '200':
          description: OK
          schema:
            type: <IF ALPHA>string</IF ALPHA><IF DECIMAL><IF PRECISION>number<ELSE>integer</IF PRECISION></IF DECIMAL><IF DATE>string</IF DATE><IF TIME>string</IF TIME><IF INTEGER>number</IF INTEGER>
;//
;// Invividual property - value only
;//
  '/<StructurePlural>(<IF SINGLE_SEGMENT>{key}<ELSE><SEGMENT_LOOP><SegmentName>=<IF ALPHA>''</IF ALPHA>{a<SegmentName>}<IF ALPHA>''</IF ALPHA><,></SEGMENT_LOOP></IF SINGLE_SEGMENT>)/<FieldSqlName>/$value':
    get:
      summary: Get <structureNoplural> property <FieldSqlName>
      description: Get <structureNoplural> property <FieldSqlName> via complete primary key, returning the raw value.
      operationId: <StructureNoplural><FieldSqlName>value
      deprecated: false
      tags:
        - <StructureNoplural>Properties
        - Read
      parameters:
        <SEGMENT_LOOP>
        <IF SINGLE_SEGMENT>
        - name: key
        <ELSE>
        - name: a<SegmentName>
        </IF SINGLE_SEGMENT>
          in: path
          description: <SEGMENT_DESC>
          required: true
          type: <IF ALPHA>string</IF ALPHA><IF DECIMAL><IF PRECISION>number<ELSE>integer</IF PRECISION></IF DECIMAL><IF DATE>string</IF DATE><IF TIME>string</IF TIME><IF INTEGER>number</IF INTEGER>
          <IF DATE>
          format: date-time
          </IF DATE>
          <IF TIME>
          format: date-time
          </IF TIME>
          </SEGMENT_LOOP>
      responses:
        '200':
          description: OK
          schema:
            type: <IF ALPHA>string</IF ALPHA><IF DECIMAL><IF PRECISION>number<ELSE>integer</IF PRECISION></IF DECIMAL><IF DATE>string</IF DATE><IF TIME>string</IF TIME><IF INTEGER>number</IF INTEGER>
</PRIMARY_KEY>
</FIELD_LOOP>
</IF DEFINED_ENABLE_PROPERTY_ENDPOINTS>
</STRUCTURE_LOOP>
;//
;//----------------------------------------------------------------------------
;//
;// Configure an authentication server (Needs more work before implementation)
;//
;//<IF DEFINED_ENABLE_AUTHENTICATION>
;//securityDefinitions": {
;//  oauth2schema:
;//    type: oauth2
;//    tokenUrl: http://localhost:5000/connect/token
;//    flow: application
;//    scopes:
;//      global: global
;//</IF DEFINED_ENABLE_AUTHENTICATION>
;//----------------------------------------------------------------------------
;//
;// Definitions of data models 
;//
definitions:
<STRUCTURE_LOOP>
  <StructureNoplural>:
    required:
<PRIMARY_KEY>
<SEGMENT_LOOP>
      - <FieldSqlname>
</SEGMENT_LOOP>
</PRIMARY_KEY>
    properties:
<FIELD_LOOP>
    <IF CUSTOM_NOT_HARMONY_EXCLUDE>
      <FieldSqlname>:
        <IF ALPHA>
        type: string
        example: <FIELD_SAMPLE_DATA>
        description: <FIELD_DESC>
        </IF ALPHA>
        <IF DECIMAL>
        <IF PRECISION>
        type: number
        format: float
        example: <FIELD_SAMPLE_DATA>
        description: <FIELD_DESC>
        <ELSE>
        <IF CUSTOM_HARMONY_AS_STRING>
        type: string
        example: <FIELD_SAMPLE_DATA>
        <ELSE>
        type: integer
        example: <FIELD_SAMPLE_DATA>
        </IF CUSTOM_HARMONY_AS_STRING>
        description: <FIELD_DESC>
        </IF PRECISION>
        </IF DECIMAL>
        <IF DATE>
        type: string
        format: date-time
        example: <FIELD_SAMPLE_DATA>
        description: <FIELD_DESC>
        </IF DATE>
        <IF TIME>
        type: string
        format: date-time
        example: <FIELD_SAMPLE_DATA>
        description: <FIELD_DESC>
        </IF TIME>
        <IF INTEGER>
        type: number
        format: <IF I124>int32<ELSE>int64</IF I124>
        example: <FIELD_SAMPLE_DATA>
        description: <FIELD_DESC>
        </IF INTEGER>
    </IF CUSTOM_NOT_HARMONY_EXCLUDE>
</FIELD_LOOP>
    example:
<FIELD_LOOP>
    <IF CUSTOM_NOT_HARMONY_EXCLUDE>
      <FieldSqlname>: <FIELD_SAMPLE_DATA>
    </IF CUSTOM_NOT_HARMONY_EXCLUDE>
</FIELD_LOOP>
</STRUCTURE_LOOP>
;//
;//----------------------------------------------------------------------------
;// Data model definitions for PATCH requests
;//
  PatchRequest:
    type: array
    items:
      $ref: '#/definitions/PatchDocument'
  PatchDocument:
    description: A JSONPatch document as defined by RFC 6902
    required:
      - op
      - path
    properties:
      op:
        type: string
        description: The operation to be performed
        enum:
          - add
          - remove
          - replace
          - move
          - copy
          - test
      path:
        type: string
        description: A JSON-Pointer
      value:
        type: object
        description: The value to be used within the operations.
      from:
        type: string
        description: A string containing a JSON Pointer value.