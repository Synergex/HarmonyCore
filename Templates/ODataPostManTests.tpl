<CODEGEN_FILENAME>PostManTests.postman_collection.json</CODEGEN_FILENAME>
<REQUIRES_CODEGEN_VERSION>5.3.5</REQUIRES_CODEGEN_VERSION>
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
                            "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT><SERVER_BASE_PATH>/<StructurePlural>",
                            "protocol": "<SERVER_PROTOCOL>",
                            "host": [
                                "<SERVER_NAME>"
                            ],
                            "port": "<SERVER_HTTPS_PORT>",
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
                        "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT><SERVER_BASE_PATH>/<StructurePlural>(<SEGMENT_LOOP><SegmentName><,></SEGMENT_LOOP>)",
                        "protocol": "<SERVER_PROTOCOL>",
                        "host": [
                            "<SERVER_NAME>"
                        ],
                        "port": "<SERVER_HTTPS_PORT>",
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
                        "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT><SERVER_BASE_PATH>/<StructurePlural>",
                        "protocol": "<SERVER_PROTOCOL>",
                        "host": [
                            "<SERVER_NAME>"
                        ],
                        "port": "<SERVER_HTTPS_PORT>",
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
                        "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT><SERVER_BASE_PATH>/<StructurePlural>(New<StructureNoplural>ID)",
                        "protocol": "<SERVER_PROTOCOL>",
                        "host": [
                            "<SERVER_NAME>"
                        ],
                        "port": "<SERVER_HTTPS_PORT>",
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
                        "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT><SERVER_BASE_PATH>/<StructurePlural>(New<StructureNoplural>ID)",
                        "protocol": "<SERVER_PROTOCOL>",
                        "host": [
                            "<SERVER_NAME>"
                        ],
                        "port": "<SERVER_HTTPS_PORT>",
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
                        "raw": "<SERVER_PROTOCOL>://<SERVER_NAME>:<SERVER_HTTPS_PORT><SERVER_BASE_PATH>/<StructurePlural>(<StructureNoplural>IdToDelete)",
                        "protocol": "<SERVER_PROTOCOL>",
                        "host": [
                            "<SERVER_NAME>"
                        ],
                        "port": "<SERVER_HTTPS_PORT>",
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