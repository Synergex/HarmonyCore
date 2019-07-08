<CODEGEN_FILENAME><INTERFACE_NAME>PostmanTests.postman_collection.json</CODEGEN_FILENAME>
{
	"info": {
		"_postman_id": "2648742f-eaf1-4fe1-8a13-52af1cd8534a",
		"name": "<INTERFACE_NAME> Interface Tests",
		"schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
	},
	"item": [
		{
			"name": "Get Synergy Environment",
			"request": {
				"method": "GET",
				"header": [],
				"url": {
					"raw": "https://localhost:8086/<INTERFACE_NAME>/GetEnvironment",
					"protocol": "https",
					"host": [
						"localhost"
					],
					"port": "8086",
					"path": [
						"<INTERFACE_NAME>",
						"GetEnvironment"
					]
				}
			},
			"response": []
		},
		{
			"name": "Get Logical Name",
			"request": {
				"method": "GET",
				"header": [],
				"url": {
					"raw": "https://localhost:8086/<INTERFACE_NAME>/GetLogicalName/DBLDIR",
					"protocol": "https",
					"host": [
						"localhost"
					],
					"port": "8086",
					"path": [
						"<INTERFACE_NAME>",
						"GetLogicalName",
						"DBLDIR"
					]
				}
			},
			"response": []
		},
		{
			"name": "Add Two Numbers",
			"request": {
				"method": "GET",
				"header": [],
				"url": {
					"raw": "https://localhost:8086/<INTERFACE_NAME>/AddTwoNumbers/1/1",
					"protocol": "https",
					"host": [
						"localhost"
					],
					"port": "8086",
					"path": [
						"<INTERFACE_NAME>",
						"AddTwoNumbers",
						"1",
						"1"
					]
				}
			},
			"response": []
		},
<METHOD_LOOP>
  <IF IN_OR_INOUT>
		{
			"name": "<METHOD_NAME>",
			"request": {
				"method": "POST",
				"header": [
					{
						"key": "Content-Type",
						"name": "Content-Type",
						"value": "application/json",
						"type": "text"
					}
				],
				"body": {
					"mode": "raw",
					"raw": "{ <PARAMETER_LOOP><IF IN_OR_INOUT>\n\t\"<PARAMETER_NAME>\": <PARAMETER_SAMPLE_DATA_ESCAPED><IF MORE_IN_OR_INOUT>,</IF MORE_IN_OR_INOUT></IF IN_OR_INOUT></PARAMETER_LOOP> \n}"
				},
				"url": {
					"raw": "https://localhost:8086/<INTERFACE_NAME>/<METHOD_NAME>",
					"protocol": "https",
					"host": [
						"localhost"
					],
					"port": "8086",
					"path": [
						"<INTERFACE_NAME>",
						"<METHOD_NAME>"
					]
				}
			},
			"response": []
		}<,>
  <ELSE>
		{
			"name": "<METHOD_NAME>",
			"request": {
				"method": "GET",
				"header": [],
				"url": {
					"raw": "https://localhost:8086/<INTERFACE_NAME>/<METHOD_NAME>",
					"protocol": "https",
					"host": [
						"localhost"
					],
					"port": "8086",
					"path": [
						"<INTERFACE_NAME>",
						"<METHOD_NAME>"
					]
				}
			},
			"response": []
		}<,>
  </IF IN_OR_INOUT>
</METHOD_LOOP>
	]
}