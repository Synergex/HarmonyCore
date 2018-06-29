<CODEGEN_FILENAME>HarmonyCoreSwaggerFile.yaml</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.3.3</REQUIRES_CODEGEN_VERSION>
swagger: "2.0"
info:
  description: This is a description of the API.
  version: 1.0.0
  title: This is the Title of the API
  termsOfService: http://www.synergexpsg.com/terms
schemes:
- https
consumes:
- application/json
produces:
- application/json
# host: http://localhost:5000
# basePath: /odata
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
    </SEGMENT_LOOP>
    </PRIMARY_KEY>
    properties:
    <FIELD_LOOP>
	  <FieldSqlname>:
<IF ALPHA>
        type: string
        description: <FIELD_DESC>
        example: "ABC"
</IF ALPHA>
<IF DECIMAL>
  <IF PRECISION>
        type: number
        format: float
        description: <FIELD_DESC>
        example: 1.23
  <ELSE>
        type: integer
        description: <FIELD_DESC>
        example: 123
  </IF PRECISION>
</IF DECIMAL>
<IF DATE>
        type: string
        format: date-time
        description: <FIELD_DESC>
</IF DATE>
<IF TIME>
        type: string
        format: date-time
        description: <FIELD_DESC>
</IF TIME>
<IF INTEGER>
        type: integer
		format: <IF I124>int32<ELSE>int64</IF I124>
        description: <FIELD_DESC>
        example: 123
</IF INTEGER>
    </FIELD_LOOP>
  </STRUCTURE_LOOP>