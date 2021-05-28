<CODEGEN_FILENAME>PostManTests.postman_collection.json</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.5.3</REQUIRES_CODEGEN_VERSION>
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
  <IF DEFINED_ENABLE_CUSTOM_AUTHENTICATION>
        {
            "name": "Get Access Token",
            "request": {
                "auth": {
                    "type": "noauth"
                },
                "method": "POST",
                "header": [
                    {
                        "key": "Content-Type",
                        "value": "application/json"
                    }
                ],
                "body": {
                    "mode": "raw",
                    "raw": "<CUSTOM_AUTH_REQUEST_POSTMAN>"
                },
                "url": {
                    "raw": "{{ServerAuthUri}}/<CUSTOM_AUTH_CONTROLLER_PATH>/<CUSTOM_AUTH_ENDPOINT_PATH>",
                    "protocol": "<SERVER_PROTOCOL>",
                    "host": [
                        "<SERVER_NAME>"
                    ],
                    "port": "<SERVER_HTTPS_PORT>",
                    "path": [
                        "<CUSTOM_AUTH_CONTROLLER_PATH>",
                        "<CUSTOM_AUTH_ENDPOINT_PATH>"
                    ]
                }
            },
            "response": []
        },
  <ELSE>
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
  </IF DEFINED_ENABLE_CUSTOM_AUTHENTICATION>
</IF DEFINED_ENABLE_AUTHENTICATION>
<STRUCTURE_LOOP>
        {
  <COUNTER_1_RESET>
            "_postman_id": "<guid_nobrace>",
            "name": "<StructureNoplural> Tests",
            "item": [
  <IF DEFINED_ENABLE_GET_ALL AND GET_ALL_ENDPOINT>
                {
                    "_postman_id": "<guid_nobrace>",
                    "name": "Read <structurePlural>",
                    "request": {
                        "method": "GET",
                        "header": [
                            {
                                "key": "Accept",
                                "value": "application/json"
                            },
                            {
                                "key": "x-tenant-id",
                                "value": "{{TenantID}}",
                                "type": "text"
                            }
                        ],
                        "body": {
                            "mode": "raw",
                            "raw": ""
                        },
                        "url": {
                            "raw": "{{ServerBaseUri}}/<StructurePlural>",
                            "host": [
                                "{{ServerBaseUri}}"
                            ],
                            "path": [
                                "<StructurePlural>"
                            ]
                        }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
  </IF>
  <IF DEFINED_ENABLE_COUNT AND GET_ALL_ENDPOINT>
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
                            },
                            {
                                "key": "x-tenant-id",
                                "value": "{{TenantID}}",
                                "type": "text"
                            }
                        ],
                        "body": {
                            "mode": "raw",
                            "raw": ""
                        },
                        "url": {
                            "raw": "{{ServerBaseUri}}/<StructurePlural>/$count",
                            "host": [
                                "{{ServerBaseUri}}"
                            ],
                            "path": [
                                "<StructurePlural>",
                                "$count"
                            ]
                        }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
  </IF>
  <IF DEFINED_ENABLE_GET_ONE AND GET_ENDPOINT>
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
                        },
                        {
                            "key": "x-tenant-id",
                            "value": "{{TenantID}}",
                            "type": "text"
                        }
                    ],
                    "body": {
                        "mode": "raw",
                        "raw": ""
                    },
                    "url": {
                        "raw": "{{ServerBaseUri}}/<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)",
                        "host": [
                            "{{ServerBaseUri}}"
                        ],
                        "path": [
                            "<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
  </IF>
;//
;// ALTERNATE KEYS
;//
  <IF STRUCTURE_ISAM AND DEFINED_ENABLE_ALTERNATE_KEYS AND ALTERNATE_KEY_ENDPOINTS>
              <ALTERNATE_KEY_LOOP_UNIQUE>
<IF COUNTER_1>                ,</IF COUNTER_1>
                {
                    "_postman_id": "<guid_nobrace>",
    <IF DUPLICATES>
                    "name": "Read <structurePlural> by <KeyName>",
    <ELSE>
                    "name": "Read <structureNoplural> by <KeyName>",
    </IF>
                    "request": {
                    "method": "GET",
                    "header": [
                        {
                            "key": "Accept",
                            "value": "application/json"
                        },
                        {
                            "key": "x-tenant-id",
                            "value": "{{TenantID}}",
                            "type": "text"
                        }
                    ],
                    "body": {
                        "mode": "raw",
                        "raw": ""
                    },
                    "url": {
                        "raw": "{{ServerBaseUri}}/<StructurePlural>(<SEGMENT_LOOP><IF NOT SEG_TAG_EQUAL><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF></SEGMENT_LOOP>)",
                        "host": [
                            "{{ServerBaseUri}}"
                        ],
                        "path": [
                            "<StructurePlural>(<SEGMENT_LOOP><IF NOT SEG_TAG_EQUAL><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF></SEGMENT_LOOP>)"
                        ]
                    }
                    },
                    "response": []
    <COUNTER_1_INCREMENT>
                }
    <IF DEFINED_ENABLE_COUNT AND DUPLICATES>
<IF COUNTER_1>                ,</IF COUNTER_1>
                {
                    "_postman_id": "<guid_nobrace>",
                    "name": "Count <structurePlural> by <KeyName>",
                    "request": {
                    "method": "GET",
                    "header": [
                        {
                            "key": "Accept",
                            "value": "application/json"
                        },
                        {
                            "key": "x-tenant-id",
                            "value": "{{TenantID}}",
                            "type": "text"
                        }
                    ],
                    "body": {
                        "mode": "raw",
                        "raw": ""
                    },
                    "url": {
                        "raw": "{{ServerBaseUri}}/<StructurePlural>(<SEGMENT_LOOP><IF NOT SEG_TAG_EQUAL><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF></SEGMENT_LOOP>)/$count",
                        "host": [
                            "{{ServerBaseUri}}"
                        ],
                        "path": [
                            "<StructurePlural>(<SEGMENT_LOOP><IF NOT SEG_TAG_EQUAL><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF></SEGMENT_LOOP>)",
                            "$count"
                        ]
                    }
                    },
                    "response": []
      <COUNTER_1_INCREMENT>
                }
    </IF>
  </ALTERNATE_KEY_LOOP>
</IF>
;//
;// PARTIAL KEY ENDPOINTS
;//
<IF STRUCTURE_ISAM AND DEFINED_ENABLE_PARTIAL_KEYS>
  <PARTIAL_KEY_LOOP>
<IF COUNTER_1>                ,</IF COUNTER_1>
                {
                    "_postman_id": "<guid_nobrace>",
                    "name": "Read <structurePlural> by partial key <KeyName>",
                    "request": {
                    "method": "GET",
                    "header": [
                        {
                            "key": "Accept",
                            "value": "application/json"
                        },
                        {
                            "key": "x-tenant-id",
                            "value": "{{TenantID}}",
                            "type": "text"
                        }
                    ],
                    "body": {
                        "mode": "raw",
                        "raw": ""
                    },
                    "url": {
                        "raw": "{{ServerBaseUri}}/<StructurePlural>(<SEGMENT_LOOP><IF NOT SEG_TAG_EQUAL><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF></SEGMENT_LOOP>)",
                        "host": [
                            "{{ServerBaseUri}}"
                        ],
                        "path": [
                            "<StructurePlural>(<SEGMENT_LOOP><IF NOT SEG_TAG_EQUAL><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF></SEGMENT_LOOP>)"
                        ]
                    }
                    },
                    "response": []
    <COUNTER_1_INCREMENT>
                }
    <IF DEFINED_ENABLE_COUNT AND DUPLICATES>
<IF COUNTER_1>                ,</IF COUNTER_1>
                {
                    "_postman_id": "<guid_nobrace>",
                    "name": "Count <structurePlural> by <KeyName>",
                    "request": {
                    "method": "GET",
                    "header": [
                        {
                            "key": "Accept",
                            "value": "application/json"
                        },
                        {
                            "key": "x-tenant-id",
                            "value": "{{TenantID}}",
                            "type": "text"
                        }
                    ],
                    "body": {
                        "mode": "raw",
                        "raw": ""
                    },
                    "url": {
                        "raw": "{{ServerBaseUri}}/<StructurePlural>(<SEGMENT_LOOP><IF NOT SEG_TAG_EQUAL><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF></SEGMENT_LOOP>)/$count",
                        "host": [
                            "{{ServerBaseUri}}"
                        ],
                        "path": [
                            "<StructurePlural>(<SEGMENT_LOOP><IF NOT SEG_TAG_EQUAL><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF></SEGMENT_LOOP>)",
                            "$count"
                        ]
                    }
                    },
                    "response": []
      <COUNTER_1_INCREMENT>
                }
    </IF>
  </PARTIAL_KEY_LOOP>
</IF>
;//
;// INDIVIDUAL PROPERTY ENDPOINTS
;//
<IF DEFINED_ENABLE_PROPERTY_ENDPOINTS AND PROPERTY_ENDPOINTS>
  <FIELD_LOOP>
    <IF NOTPKSEGMENT AND CUSTOM_NOT_HARMONY_EXCLUDE>
<IF COUNTER_1>                ,</IF COUNTER_1>
                {
                    "_postman_id": "<guid_nobrace>",
                    "name": "Read <structureNoplural> <fieldSqlName>",
                    "request": {
                    "method": "GET",
                    "header": [
                        {
                            "key": "Accept",
                            "value": "application/json"
                        },
                        {
                            "key": "x-tenant-id",
                            "value": "{{TenantID}}",
                            "type": "text"
                        }
                    ],
                    "body": {
                        "mode": "raw",
                        "raw": ""
                    },
                    "url": {
                        "raw": "{{ServerBaseUri}}/<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF NOT SEG_TAG_EQUAL><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)/<FieldSqlName>",
                        "host": [
                            "{{ServerBaseUri}}"
                        ],
                        "path": [
                            "<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF NOT SEG_TAG_EQUAL><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)/<FieldSqlName>"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
      <IF DEFINED_ENABLE_PROPERTY_VALUE_DOCS>
<IF COUNTER_1>                ,</IF COUNTER_1>
                {
                    "_postman_id": "<guid_nobrace>",
                    "name": "Read <structureNoplural> <fieldSqlName> raw value",
                    "request": {
                    "method": "GET",
                    "header": [
                        {
                            "key": "Accept",
                            "value": "application/json"
                        },
                        {
                            "key": "x-tenant-id",
                            "value": "{{TenantID}}",
                            "type": "text"
                        }
                    ],
                    "body": {
                        "mode": "raw",
                        "raw": ""
                    },
                    "url": {
                        "raw": "{{ServerBaseUri}}/<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF NOT SEG_TAG_EQUAL><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)/<FieldSqlName>/$value",
                        "host": [
                            "{{ServerBaseUri}}"
                        ],
                        "path": [
                            "<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF NOT SEG_TAG_EQUAL><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)/<FieldSqlName>/$value"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
      </IF DEFINED_ENABLE_PROPERTY_VALUE_DOCS>
    </IF>
  </FIELD_LOOP>
</IF>
;//
;// POST
;//
<IF DEFINED_ENABLE_POST AND POST_ENDPOINT>
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
                        },
                        {
                            "key": "x-tenant-id",
                            "value": "{{TenantID}}",
                            "type": "text"
                        }
                    ],
                    "body": {
                        "mode": "raw",
                         "raw": "{\n<FIELD_LOOP><IF CUSTOM_NOT_HARMONY_EXCLUDE>    \"<FieldSqlName>\": <IF ALPHA>\"</IF ALPHA><IF CUSTOM_HARMONY_AS_STRING>\"</IF CUSTOM_HARMONY_AS_STRING><FIELD_SAMPLE_DATA_NOQUOTES><IF CUSTOM_HARMONY_AS_STRING>\"</IF CUSTOM_HARMONY_AS_STRING><IF ALPHA>\"</IF ALPHA><,>\n</IF CUSTOM_NOT_HARMONY_EXCLUDE></FIELD_LOOP>}"
                    },
                    "url": {
                        "raw": "{{ServerBaseUri}}/<StructurePlural>",
                        "host": [
                            "{{ServerBaseUri}}"
                        ],
                        "path": [
                            "<StructurePlural>"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
</IF>
<IF DEFINED_ENABLE_PUT AND PUT_ENDPOINT>
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
                        },
                        {
                            "key": "x-tenant-id",
                            "value": "{{TenantID}}",
                            "type": "text"
                        }
                    ],
                    "body": {
                        "mode": "raw",
                         "raw": "{\n<FIELD_LOOP><IF CUSTOM_NOT_HARMONY_EXCLUDE>    \"<FieldSqlName>\": <IF ALPHA>\"</IF ALPHA><IF CUSTOM_HARMONY_AS_STRING>\"</IF CUSTOM_HARMONY_AS_STRING><FIELD_SAMPLE_DATA_NOQUOTES><IF CUSTOM_HARMONY_AS_STRING>\"</IF CUSTOM_HARMONY_AS_STRING><IF ALPHA>\"</IF ALPHA><,>\n</IF CUSTOM_NOT_HARMONY_EXCLUDE></FIELD_LOOP>}"
                    },
                    "url": {
                        "raw": "{{ServerBaseUri}}/<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)",
                        "host": [
                            "{{ServerBaseUri}}"
                        ],
                        "path": [
                            "<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
</IF>
<IF DEFINED_ENABLE_PATCH AND PATCH_ENDPOINT>
              <IF COUNTER_1>,</IF COUNTER_1>
                {
                    "name": "Patch <structureNoplural>",
                    "request": {
                        "method": "PATCH",
                        "header": [
                            {
                                "key": "Content-Type",
                                "value": "application/json"
                            },
                            {
                                "key": "x-tenant-id",
                                "value": "{{TenantID}}",
                                "type": "text"
                            }
                        ],
                        "body": {
                            "mode": "raw",
                            "raw": "[\r\n  {\r\n    \"op\": \"replace\",\r\n    \"path\": \"PropertyName\",\r\n    \"value\": \"PropertyValue\"\r\n  }\r\n]"
                        },
                    "url": {
                        "raw": "/<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)",
                        "host": [
                            "{{ServerBaseUri}}"
                        ],
                        "path": [
                            "<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
</IF>
<IF DEFINED_ENABLE_DELETE AND DELETE_ENDPOINT>
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
                        },
                        {
                            "key": "x-tenant-id",
                            "value": "{{TenantID}}",
                            "type": "text"
                        }
                    ],
                    "body": {
                        "mode": "raw",
                        "raw": ""
                    },
                    "url": {
                        "raw": "{{ServerBaseUri}}/<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)",
                        "host": [
                            "{{ServerBaseUri}}"
                        ],
                        "path": [
                            "<StructurePlural>(<IF STRUCTURE_ISAM><PRIMARY_KEY><SEGMENT_LOOP><IF SEG_TAG_EQUAL><ELSE><FieldSqlName>=<IF DATEORTIME><YEAR>-<MONTH>-<DAY><IF TIME>T<TIME>:00<TIMEZONE_OFFSET></IF TIME><ELSE><IF ALPHA>'ABC'<ELSE>123</IF ALPHA></IF DATEORTIME><,></IF SEG_TAG_EQUAL></SEGMENT_LOOP></PRIMARY_KEY></IF STRUCTURE_ISAM><IF STRUCTURE_RELATIVE>InsertRecordNumber</IF STRUCTURE_RELATIVE>)"
                        ]
                    }
                    },
                    "response": []
                    <COUNTER_1_INCREMENT>
                }
</IF>
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
            "id": "<guid_nobrace>",
            "key": "TenantID",
            "value": "FCL",
            "type": "string"
        },
        {
            "id": "<guid_nobrace>",
            "key": "ServerBaseUri",
            "value": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>/<SERVER_BASE_PATH>/v<API_VERSION>",
            "type": "string"
        },
        {
            "id": "<guid_nobrace>",
            "key": "ServerAuthUri",
            "value": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT>",
            "type": "string"
        }
    ]
}