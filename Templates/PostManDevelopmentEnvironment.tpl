<CODEGEN_FILENAME>LocalDevelopmentService.postman_environment.json</CODEGEN_FILENAME>
{
	"id": "021c358b-fa1d-45d0-a901-4e15931800b0",
	"name": "LocalDevelopmentService",
	"values": [
		{
			"key": "ServerBaseUri",
			"value": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>",
			"enabled": true
		},
		{
			"key": "ODataPath",
			"value": "<SERVER_BASE_PATH>",
			"enabled": true
		},
		{
			"key": "ApiVersion",
			"value": "<API_VERSION>",
			"enabled": true
		},
		{
			"key": "ServerAuthUri",
			"value": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>/<CUSTOM_AUTH_CONTROLLER_PATH>/<CUSTOM_AUTH_ENDPOINT_PATH>",
			"enabled": true
		},
		{
			"key": "JWT",
			"value": "PUT_JWT_HERE",
			"enabled": true
		},
		{
			"key": "TenantID",
			"value": "PUT_TENANT_ID_HERE",
			"enabled": true
		}
	],
	"_postman_variable_scope": "environment",
	"_postman_exported_at": "2022-02-05T05:51:53.314Z",
	"_postman_exported_using": "Postman/9.12.2"
}