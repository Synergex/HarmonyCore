<CODEGEN_FILENAME>PostManTests.postman_collection.json</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.3.15</REQUIRES_CODEGEN_VERSION>
<REQUIRES_USERTOKEN>API_TITLE</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>SERVER_BASE_PATH</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>SERVER_HTTPS_PORT</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>SERVER_NAME</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>SERVER_PROTOCOL</REQUIRES_USERTOKEN>
{
    "info": {
        "_postman_id": "<guid_nobrace>",
        "name": "<API_TITLE>",
        "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
    },
    "item": [
<IF DEFINED_ENABLE_AUTHENTICATION>
		{
			"name": "Get Access Token (Jodah)",
			"request": {
				"auth": {
					"type": "noauth"
				},
				"method": "POST",
				"header": [
					{
						"key": "Content-Type",
						"value": "application/x-www-form-urlencoded"
					}
				],
				"body": {
					"mode": "raw",
					"raw": "grant_type=password&username=jodah&password=P@ssw0rd&client_id=ro.client&client_secret=CBF7EBE6-D46E-41A7-903B-766A280616C3"
				},
				"url": {
					"raw": "http://localhost:5000/connect/token",
					"protocol": "http",
					"host": [
						"localhost"
					],
					"port": "5000",
					"path": [
						"connect",
						"token"
					]
				}
			},
			"response": []
		},
		{
			"name": "Get Access Token (Manny)",
			"request": {
				"auth": {
					"type": "noauth"
				},
				"method": "POST",
				"header": [
					{
						"key": "Content-Type",
						"value": "application/x-www-form-urlencoded"
					}
				],
				"body": {
					"mode": "raw",
					"raw": "grant_type=password&username=manny&password=P@ssw0rd&client_id=ro.client&client_secret=CBF7EBE6-D46E-41A7-903B-766A280616C3"
				},
				"url": {
					"raw": "http://localhost:5000/connect/token",
					"protocol": "http",
					"host": [
						"localhost"
					],
					"port": "5000",
					"path": [
						"connect",
						"token"
					]
				}
			},
			"response": []
		},
</IF DEFINED_ENABLE_AUTHENTICATION>
		<STRUCTURE_LOOP>
        {
            <COUNTER_1_RESET>
            "_postman_id": "<guid_nobrace>",
            "name": "<StructureNoplural> Tests",
            "item": [
<IF DEFINED_ENABLE_GET_ALL>
<IF GET_ALL_ENDPOINT>
                {
                    "_postman_id": "<guid_nobrace>",
                    "name": "Read <structurePlural>",
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
                            "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>/<SERVER_BASE_PATH>/<StructurePlural>",
                            "protocol": "<SERVER_PROTOCOL>",
                            "host": [
                                "<SERVER_NAME>"
                            ],
                            "port": "<SERVER_HTTPS_PORT>",
                            "path": [
                                "<SERVER_BASE_PATH>",
                                "<StructurePlural>"
                            ]
                        }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
</IF GET_ALL_ENDPOINT>
</IF DEFINED_ENABLE_GET_ALL>
<IF DEFINED_ENABLE_COUNT>
<IF GET_ALL_ENDPOINT>
                <IF COUNTER_1>,</IF COUNTER_1>
                {
                    "_postman_id": "<guid_nobrace>",
                    "name": "Count <structurePlural>",
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
                            "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>/<SERVER_BASE_PATH>/<StructurePlural>/$count",
                            "protocol": "<SERVER_PROTOCOL>",
                            "host": [
                                "<SERVER_NAME>"
                            ],
                            "port": "<SERVER_HTTPS_PORT>",
                            "path": [
                                "<SERVER_BASE_PATH>",
                                "<StructurePlural>",
                                "$count"
                            ]
                        }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
</IF GET_ALL_ENDPOINT>
</IF DEFINED_ENABLE_COUNT>
<IF DEFINED_ENABLE_GET_ONE>
<IF GET_ENDPOINT>
                <IF COUNTER_1>,</IF COUNTER_1>
                {
                    "_postman_id": "<guid_nobrace>",
                    "name": "Read <structureNoplural>",
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
                        "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>/<SERVER_BASE_PATH>/<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)",
                        "protocol": "<SERVER_PROTOCOL>",
                        "host": [
                            "<SERVER_NAME>"
                        ],
                        "port": "<SERVER_HTTPS_PORT>",
                        "path": [
                            "<SERVER_BASE_PATH>",
                            "<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
</IF GET_ENDPOINT>
</IF DEFINED_ENABLE_GET_ONE>
<IF STRUCTURE_ISAM>
<IF DEFINED_ENABLE_ALTERNATE_KEYS>
<IF ALTERNATE_KEY_ENDPOINTS>
              <ALTERNATE_KEY_LOOP>
                <IF COUNTER_1>,</IF COUNTER_1>
                {
                    "_postman_id": "<guid_nobrace>",
                    <IF DUPLICATES>
                    "name": "Read <structurePlural> by <KeyName>",
                    <ELSE>
                    "name": "Read <structureNoplural> by <KeyName>",
                    </IF DUPLICATES>
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
                        "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>/<SERVER_BASE_PATH>/<StructurePlural>(<SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP>)",
                        "protocol": "<SERVER_PROTOCOL>",
                        "host": [
                            "<SERVER_NAME>"
                        ],
                        "port": "<SERVER_HTTPS_PORT>",
                        "path": [
                            "<SERVER_BASE_PATH>",
                            "<StructurePlural>(<SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP>)"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
                <IF DEFINED_ENABLE_COUNT>
                <IF DUPLICATES>
                <IF COUNTER_1>,</IF COUNTER_1>
                {
                    "_postman_id": "<guid_nobrace>",
                    "name": "Count <structurePlural> by <KeyName>",
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
                        "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>/<SERVER_BASE_PATH>/<StructurePlural>(<SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP>)/$count",
                        "protocol": "<SERVER_PROTOCOL>",
                        "host": [
                            "<SERVER_NAME>"
                        ],
                        "port": "<SERVER_HTTPS_PORT>",
                        "path": [
                            "<SERVER_BASE_PATH>",
                            "<StructurePlural>(<SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP>)",
                            "$count"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
                </IF DUPLICATES>
                </IF DEFINED_ENABLE_COUNT>
                </ALTERNATE_KEY_LOOP>
</IF ALTERNATE_KEY_ENDPOINTS>
</IF DEFINED_ENABLE_ALTERNATE_KEYS>
</IF STRUCTURE_ISAM>
<IF DEFINED_ENABLE_PROPERTY_ENDPOINTS>
<IF PROPERTY_ENDPOINTS>
                <FIELD_LOOP>
                <IF NOTPKSEGMENT>
                <IF CUSTOM_NOT_HARMONY_EXCLUDE>
                <IF COUNTER_1>,</IF COUNTER_1>
                {
                    "_postman_id": "<guid_nobrace>",
                    "name": "Read <structureNoplural> <fieldSqlName>",
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
                        "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>/<SERVER_BASE_PATH>/<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)/<FieldSqlName>",
                        "protocol": "<SERVER_PROTOCOL>",
                        "host": [
                            "<SERVER_NAME>"
                        ],
                        "port": "<SERVER_HTTPS_PORT>",
                        "path": [
                            "<SERVER_BASE_PATH>",
                            "<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)/<FieldSqlName>"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
<IF DEFINED_ENABLE_PROPERTY_VALUE_DOCS>
                <IF COUNTER_1>,</IF COUNTER_1>
                {
                    "_postman_id": "<guid_nobrace>",
                    "name": "Read <structureNoplural> <fieldSqlName> raw value",
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
                        "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>/<SERVER_BASE_PATH>/<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)/<FieldSqlName>/$value",
                        "protocol": "<SERVER_PROTOCOL>",
                        "host": [
                            "<SERVER_NAME>"
                        ],
                        "port": "<SERVER_HTTPS_PORT>",
                        "path": [
                            "<SERVER_BASE_PATH>",
                            "<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)/<FieldSqlName>/$value"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
</IF DEFINED_ENABLE_PROPERTY_VALUE_DOCS>
                </IF CUSTOM_NOT_HARMONY_EXCLUDE>
                </IF NOTPKSEGMENT>
                </FIELD_LOOP>
</IF PROPERTY_ENDPOINTS>
</IF DEFINED_ENABLE_PROPERTY_ENDPOINTS>
<IF DEFINED_ENABLE_POST>
<IF POST_ENDPOINT>
                <IF COUNTER_1>,</IF COUNTER_1>
                {
                    "_postman_id": "<guid_nobrace>",
                    "name": "Create <structureNoplural> (auto assign key)",
                    "request": {
                    "method": "POST",
                    "header": [
                        {
                        "key": "Content-Type",
                        "value": "application/json"
                        }
                    ],
                    "body": {
                        "mode": "raw",
                         "raw": "{\n<FIELD_LOOP><IF CUSTOM_NOT_HARMONY_EXCLUDE>    \"<FieldSqlName>\": <IF ALPHA>\"</IF ALPHA><IF CUSTOM_HARMONY_AS_STRING>\"</IF CUSTOM_HARMONY_AS_STRING><FIELD_SAMPLE_DATA_NOQUOTES><IF CUSTOM_HARMONY_AS_STRING>\"</IF CUSTOM_HARMONY_AS_STRING><IF ALPHA>\"</IF ALPHA><,>\n</IF CUSTOM_NOT_HARMONY_EXCLUDE></FIELD_LOOP>}"
                    },
                    "url": {
                        "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>/<SERVER_BASE_PATH>/<StructurePlural>",
                        "protocol": "<SERVER_PROTOCOL>",
                        "host": [
                            "<SERVER_NAME>"
                        ],
                        "port": "<SERVER_HTTPS_PORT>",
                        "path": [
                            "<SERVER_BASE_PATH>",
                            "<StructurePlural>"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
</IF POST_ENDPOINT>
</IF DEFINED_ENABLE_POST>
<IF DEFINED_ENABLE_PUT>
<IF PUT_ENDPOINT>
                <IF COUNTER_1>,</IF COUNTER_1>
                {
                    "_postman_id": "<guid_nobrace>",
                    "name": "Create or update <structureNoplural>",
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
                         "raw": "{\n<FIELD_LOOP><IF CUSTOM_NOT_HARMONY_EXCLUDE>    \"<FieldSqlName>\": <IF ALPHA>\"</IF ALPHA><IF CUSTOM_HARMONY_AS_STRING>\"</IF CUSTOM_HARMONY_AS_STRING><FIELD_SAMPLE_DATA_NOQUOTES><IF CUSTOM_HARMONY_AS_STRING>\"</IF CUSTOM_HARMONY_AS_STRING><IF ALPHA>\"</IF ALPHA><,>\n</IF CUSTOM_NOT_HARMONY_EXCLUDE></FIELD_LOOP>}"
                    },
                    "url": {
                        "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>/<SERVER_BASE_PATH>/<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)",
                        "protocol": "<SERVER_PROTOCOL>",
                        "host": [
                            "<SERVER_NAME>"
                        ],
                        "port": "<SERVER_HTTPS_PORT>",
                        "path": [
                            "<SERVER_BASE_PATH>",
                            "<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
</IF PUT_ENDPOINT>
</IF DEFINED_ENABLE_PUT>
<IF DEFINED_ENABLE_PATCH>
<IF PATCH_ENDPOINT>
              <IF COUNTER_1>,</IF COUNTER_1>
                {
                    "name": "Patch <structureNoplural>",
                    "request": {
                        "method": "PATCH",
                        "header": [
                            {
                                "key": "Content-Type",
                                "value": "application/json"
                            }
                        ],
                        "body": {
                            "mode": "raw",
                            "raw": "[\r\n  {\r\n    \"op\": \"replace\",\r\n    \"path\": \"PropertyName\",\r\n    \"value\": \"PropertyValue\"\r\n  }\r\n]"
                        },
                    "url": {
                        "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>/<SERVER_BASE_PATH>/<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)",
                        "protocol": "<SERVER_PROTOCOL>",
                        "host": [
                            "<SERVER_NAME>"
                        ],
                        "port": "<SERVER_HTTPS_PORT>",
                        "path": [
                            "<SERVER_BASE_PATH>",
                            "<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
</IF PATCH_ENDPOINT>
</IF DEFINED_ENABLE_PATCH>
<IF DEFINED_ENABLE_DELETE>
<IF DELETE_ENDPOINT>
                <IF COUNTER_1>,</IF COUNTER_1>
                {
                    "_postman_id": "<guid_nobrace>",
                    "name": "Delete <structureNoplural>",
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
                        "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>/<SERVER_BASE_PATH>/<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)",
                        "protocol": "<SERVER_PROTOCOL>",
                        "host": [
                            "<SERVER_NAME>"
                        ],
                        "port": "<SERVER_HTTPS_PORT>",
                        "path": [
                            "<SERVER_BASE_PATH>",
                            "<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
</IF DELETE_ENDPOINT>
</IF DEFINED_ENABLE_DELETE>
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
    ],
	"variable": [
		{
			"id": "bd6f096f-5211-4e1d-ba95-e944e7e7b89a",
			"key": "AccessToken",
			"value": "",
			"type": "string"
		}
	]
}