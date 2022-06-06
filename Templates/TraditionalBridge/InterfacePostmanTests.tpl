<CODEGEN_FILENAME><INTERFACE_NAME>PostmanTests.postman_collection.json</CODEGEN_FILENAME>
<REQUIRES_USERTOKEN>API_TITLE</REQUIRES_USERTOKEN>
<REQUIRES_CODEGEN_VERSION>5.4.6</REQUIRES_CODEGEN_VERSION>
{
	"info": {
		"_postman_id": "2648742f-eaf1-4fe1-8a13-52af1cd8534a",
		"name": "<API_TITLE> (<INTERFACE_NAME>)",
		"schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
	},
	"item": [
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
					"raw": "{{BridgeBaseUri}}/{{ControllerPath}}/<METHOD_NAME>",
					"host": [
						"{{BridgeBaseUri}}"
					],
					"path": [
						"{{ControllerPath}}",
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
					"raw": "{{BridgeBaseUri}}/{{ControllerPath}}/<METHOD_NAME>",
					"host": [
						"{{BridgeBaseUri}}"
					],
					"path": [
						"{{ControllerPath}}",
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
			"key": "BridgeBaseUri",
			"value": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>",
			"type": "string"
		},
		{
			"id": "a3b6a948-ca0e-4b59-958f-560f2eaa2205",
			"key": "ControllerPath",
			"value": "<INTERFACE_NAME>",
			"type": "string"
		}
	]
}