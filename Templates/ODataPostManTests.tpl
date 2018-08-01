<CODEGEN_FILENAME>HarmonyCoreSampleTests.postman_collection.json</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.3.4</REQUIRES_CODEGEN_VERSION>
{
	"info": {
		"_postman_id": "<GUID1>",
		"name": "Harmony Core Sample Tests",
		"schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
	},
	"item": [
<STRUCTURE_LOOP>
		{
			"name": "Get all <structurePlural>",
			"request": {
				"method": "GET",
				"header": [
					{
						"key": "Accept",
						"value": "application/json"
					}
				],
				"body": {},
				"url": {
					"raw": "http://localhost:5000/odata/<StructurePlural>",
					"protocol": "http",
					"host": [
						"localhost"
					],
					"port": "5000",
					"path": [
						"odata",
						"<StructurePlural>"
					]
				}
			},
			"response": []
		},
		{
			"name": "Get single <structureNoplural>",
			"request": {
			"method": "GET",
			"header": [
				{
				"key": "Accept",
				"value": "application/json"
				}
			],
			"body": {},
			"url": {
				"raw": "http://localhost:5000/odata/<StructurePlural>({{Existing<StructureNoplural>ID}})",
				"protocol": "http",
				"host": [
					"localhost"
				],
				"port": "5000",
				"path": [
					"odata",
					"<StructurePlural>({{Existing<StructureNoplural>ID}})"
				]
			}
			},
			"response": []
		},
		{
			"name": "Create new <structureNoplural> (auto assign key)",
			"request": {
			"method": "POST",
			"header": [],
			"body": {
				"mode": "raw",
				"raw": "Put new <structureNoplural> json here. Do not include the primary key fields."
			},
			"url": {
				"raw": "http://localhost:5000/odata/<StructurePlural>",
				"protocol": "http",
				"host": [
					"localhost"
				],
				"port": "5000",
				"path": [
					"odata",
					"<StructurePlural>"
				]
			}
			},
			"response": []
		},
		{
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
				"raw": "http://localhost:5000/odata/<StructurePlural>({{New<StructureNoplural>ID}})",
				"protocol": "http",
				"host": [
					"localhost"
				],
				"port": "5000",
				"path": [
					"odata",
					"<StructurePlural>({{New<StructureNoplural>ID}})"
				]
			}
			},
			"response": []
		},
		{
			"name": "Get created <structureNoplural>",
			"request": {
			"method": "GET",
			"header": [
				{
				"key": "Accept",
				"value": "application/json"
				}
			],
			"body": {},
			"url": {
				"raw": "http://localhost:5000/odata/<StructurePlural>({{New<StructureNoplural>ID}})",
				"protocol": "http",
				"host": [
					"localhost"
				],
				"port": "5000",
				"path": [
					"odata",
					"<StructurePlural>({{New<StructureNoplural>ID}})"
				]
			}
			},
			"response": []
		},
		{
			"name": "Delete created <structureNoplural>",
			"request": {
			"method": "DELETE",
			"header": [
				{
				"key": "Accept",
				"value": "application/json"
				}
			],
			"body": {},
			"url": {
				"raw": "http://localhost:5000/odata/<StructurePlural>({{New<StructureNoplural>ID}})",
				"protocol": "http",
				"host": [
					"localhost"
				],
				"port": "5000",
				"path": [
					"odata",
					"<StructurePlural>({{New<StructureNoplural>ID}})"
				]
			}
			},
			"response": []
		}<,>
</STRUCTURE_LOOP>
  ],
	"event": [
		{
			"listen": "prerequest",
			"script": {
				"id": "<GUID>",
				"type": "text/javascript",
				"exec": [
					""
				]
			}
		},
		{
			"listen": "test",
			"script": {
				"id": "<GUID>",
				"type": "text/javascript",
				"exec": [
					""
				]
			}
		}
	],
	"variable": [
<STRUCTURE_LOOP>
		{
			"id": "GUID",
			"key": "Existing<StructureNoplural>ID",
			"value": "?",
			"type": "string",
			"description": "The ID of an existing <structureNoplural>."
		},
		{
			"id": "GUID",
			"key": "New<StructureNoplural>ID",
			"value": "?",
			"type": "string",
			"description": "The ID of a new <structureNoplural>."
		}<,>
</STRUCTURE_LOOP>
	]
}