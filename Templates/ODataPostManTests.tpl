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
			"name": "<StructurePlural>",
			"request": {
				"method": "GET",
				"header": [],
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
			"name": "<StructureNoplural> 1",
			"request": {
				"method": "GET",
				"header": [],
				"body": {},
				"url": {
					"raw": "http://localhost:5000/odata/<StructurePlural>(1)",
					"protocol": "http",
					"host": [
						"localhost"
					],
					"port": "5000",
					"path": [
						"odata",
						"<StructurePlural>(1)"
					]
				}
			},
			"response": []
		}<,>
</STRUCTURE_LOOP>
	]
}