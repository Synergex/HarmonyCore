<CODEGEN_FILENAME>PostManTests.postman_collection.json</CODEGEN_FILENAME>
<OPTIONAL_USERTOKEN>TITLE=Tests</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>SERVER_PROTOCOL=https</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>SERVER_NAME=localhost</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>SERVER_PORT=8081</OPTIONAL_USERTOKEN>
<OPTIONAL_USERTOKEN>BASE_PATH=/odata</OPTIONAL_USERTOKEN>
<REQUIRES_CODEGEN_VERSION>5.3.5</REQUIRES_CODEGEN_VERSION>
{
	"info": {
		"_postman_id": "<guid_nobrace>",
		"name": "<TITLE>",
		"schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
	},
	"item": [
<STRUCTURE_LOOP>
		{
			"_postman_id": "<guid_nobrace>",
			"name": "<StructureNoplural> Tests",
			"item": [
				{
					"_postman_id": "<guid_nobrace>",
					"name": "Get all <structurePlural>",
					"request": {
						"method": "GET",
						"header": [
							{
								"key": "Accept",
								"value": "application/json"
							}
						],
						"body": {
							"mode": "raw",
							"raw": ""
						},
						"url": {
							"raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_PORT><BASE_PATH>/<StructurePlural>",
							"protocol": "<SERVER_PROTOCOL>",
							"host": [
								"<SERVER_NAME>"
							],
							"port": "<SERVER_PORT>",
							"path": [
								"odata",
								"<StructurePlural>"
							]
						}
					},
					"response": []
				},
				{
					"_postman_id": "<guid_nobrace>",
					"name": "Get single <structureNoplural>",
					"request": {
					"method": "GET",
					"header": [
						{
						"key": "Accept",
						"value": "application/json"
						}
					],
					"body": {
						"mode": "raw",
						"raw": ""
					},
<PRIMARY_KEY>
					"url": {
						"raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_PORT><BASE_PATH>/<StructurePlural>(<SEGMENT_LOOP><SegmentName><,></SEGMENT_LOOP>)",
						"protocol": "<SERVER_PROTOCOL>",
						"host": [
							"<SERVER_NAME>"
						],
						"port": "<SERVER_PORT>",
						"path": [
							"odata",
							"<StructurePlural>(<SEGMENT_LOOP><SegmentName><,></SEGMENT_LOOP>)"
						]
					}
</PRIMARY_KEY>
					},
					"response": []
				},
				{
					"_postman_id": "<guid_nobrace>",
					"name": "Create new <structureNoplural> (auto assign key)",
					"request": {
					"method": "POST",
					"header": [],
					"body": {
						"mode": "raw",
						"raw": "Put new <structureNoplural> json here. Do not include the primary key fields."
					},
					"url": {
						"raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_PORT><BASE_PATH>/<StructurePlural>",
						"protocol": "<SERVER_PROTOCOL>",
						"host": [
							"<SERVER_NAME>"
						],
						"port": "<SERVER_PORT>",
						"path": [
							"odata",
							"<StructurePlural>"
						]
					}
					},
					"response": []
				},
				{
					"_postman_id": "<guid_nobrace>",
					"name": "Create new <structureNoplural>",
					"request": {
					"method": "PUT",
					"header": [
						{
						"key": "Content-Type",
						"value": "application/json"
						}
					],
					"body": {
						"mode": "raw",
						"raw": "Put new <structureNoplural> json here. Include primary key fields."
					},
					"url": {
						"raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_PORT><BASE_PATH>/<StructurePlural>(New<StructureNoplural>ID)",
						"protocol": "<SERVER_PROTOCOL>",
						"host": [
							"<SERVER_NAME>"
						],
						"port": "<SERVER_PORT>",
						"path": [
							"odata",
							"<StructurePlural>(New<StructureNoplural>ID)"
						]
					}
					},
					"response": []
				},
				{
					"_postman_id": "<guid_nobrace>",
					"name": "Get created <structureNoplural>",
					"request": {
					"method": "GET",
					"header": [
						{
						"key": "Accept",
						"value": "application/json"
						}
					],
					"body": {
						"mode": "raw",
						"raw": ""
					},
					"url": {
						"raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_PORT><BASE_PATH>/<StructurePlural>(New<StructureNoplural>ID)",
						"protocol": "<SERVER_PROTOCOL>",
						"host": [
							"<SERVER_NAME>"
						],
						"port": "<SERVER_PORT>",
						"path": [
							"odata",
							"<StructurePlural>(New<StructureNoplural>ID)"
						]
					}
					},
					"response": []
				},
				{
					"_postman_id": "<guid_nobrace>",
					"name": "Delete created <structureNoplural>",
					"request": {
					"method": "DELETE",
					"header": [
						{
						"key": "Accept",
						"value": "application/json"
						}
					],
					"body": {
						"mode": "raw",
						"raw": ""
					},
					"url": {
						"raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_PORT><BASE_PATH>/<StructurePlural>(<StructureNoplural>IdToDelete)",
						"protocol": "<SERVER_PROTOCOL>",
						"host": [
							"<SERVER_NAME>"
						],
						"port": "<SERVER_PORT>",
						"path": [
							"odata",
							"<StructurePlural>(<StructureNoplural>IdToDelete)"
						]
					}
					},
					"response": []
				}
			]
		}<,>
</STRUCTURE_LOOP>
	],
	"event": [
		{
			"listen": "prerequest",
			"script": {
				"id": "<guid_nobrace>",
				"type": "text/javascript",
				"exec": [
					""
				]
			}
		},
		{
			"listen": "test",
			"script": {
				"id": "<guid_nobrace>",
				"type": "text/javascript",
				"exec": [
					""
				]
			}
		}
	]
}