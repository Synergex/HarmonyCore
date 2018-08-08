<CODEGEN_FILENAME>HarmonyCoreSwaggerFile.yaml</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.3.5</REQUIRES_CODEGEN_VERSION>
<REQUIRES_USERTOKEN>API_DESCRIPTION</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>API_VERSION</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>API_TITLE</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>API_TERMS_URL</REQUIRES_USERTOKEN>
---
swagger: "2.0"
info:
  description: <API_DESCRIPTION>
  version: <API_VERSION>
  title: <API_TITLE>
  termsOfService: <API_TERMS_URL>
schemes:
- https
consumes:
- application/json
produces:
- application/json
paths:
<STRUCTURE_LOOP>
  /<StructurePlural>:
    get:
      description: Get all <StructurePlural>
      parameters: []
      responses:
        200:
          description: OK
          schema:
            type: array
            items:
              $ref: '#/definitions/<StructureNoplural>'
</STRUCTURE_LOOP>
definitions:
<STRUCTURE_LOOP>
  <StructureNoplural>:
    required:
<PRIMARY_KEY>
<SEGMENT_LOOP>
    - <FieldSqlname>
    properties:
</SEGMENT_LOOP>
</PRIMARY_KEY>
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
        example : <FIELD_SAMPLE_DATA>
        description: <FIELD_DESC>
	  <ELSE>
        type: integer
        example: <FIELD_SAMPLE_DATA>
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
