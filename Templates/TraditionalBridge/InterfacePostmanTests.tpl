<CODEGEN_FILENAME><INTERFACE_NAME>PostmanTests.postman_collection.json</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.4.2</REQUIRES_CODEGEN_VERSION>
{
	"info": {
		"_postman_id": "2648742f-eaf1-4fe1-8a13-52af1cd8534a",
		"name": "<INTERFACE_NAME> Interface Tests",
		"schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
	},
	"item": [
		{
			"name": "Add Two Numbers",
			"request": {
				"method": "GET",
				"header": [],
				"url": {
					"raw": "{{server}}/<INTERFACE_NAME>/AddTwoNumbers/1/1",
					"host": [
						"{{server}}"
					],
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
					"raw": "{{server}}/<INTERFACE_NAME>/<METHOD_NAME>",
					"host": [
						"{{server}}"
					],
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
					"raw": "{{server}}/<INTERFACE_NAME>/<METHOD_NAME>",
					"host": [
						"{{server}}"
					],
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
	],
	"auth": {
		"type": "bearer",
		"bearer": [
			{
				"key": "token",
				"value": "JWT_HERE",
				"type": "string"
			}
		]
	},
	"event": [
		{
			"listen": "prerequest",
			"script": {
				"id": "3f8076a8-3672-4d3d-bf27-6a7fa8e1d73a",
				"type": "text/javascript",
				"exec": [
					""
				]
			}
		},
		{
			"listen": "test",
			"script": {
				"id": "64969d3e-8215-4845-9deb-77f993fb049a",
				"type": "text/javascript",
				"exec": [
					""
				]
			}
		}
	],
	"variable": [
		{
			"id": "348faa4b-487a-4d1e-9c94-9880cb11f521",
			"key": "server",
			"value": "https://localhost:8086",
			"type": "string"
		}
	]
}