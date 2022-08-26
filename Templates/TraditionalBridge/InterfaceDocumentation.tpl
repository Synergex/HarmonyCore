<CODEGEN_FILENAME><INTERFACE_NAME>.html</CODEGEN_FILENAME>
<REQUIRES_USERTOKEN>MODELS_NAMESPACE</REQUIRES_USERTOKEN>
<REQUIRES_CODEGEN_VERSION>5.8.1</REQUIRES_CODEGEN_VERSION>
;//****************************************************************************
;//
;// Title:       InterfaceDocumentation.tpl
;//
;// Type:        CodeGen Template
;//
;// Description: Generates documentation for exposed methods
;//
;// Copyright (c) 2021, Synergex International, Inc. All rights reserved.
;//
;// Redistribution and use in source and binary forms, with or without
;// modification, are permitted provided that the following conditions are met:
;//
;// * Redistributions of source code must retain the above copyright notice,
;//   this list of conditions and the following disclaimer.
;//
;// * Redistributions in binary form must reproduce the above copyright notice,
;//   this list of conditions and the following disclaimer in the documentation
;//   and/or other materials provided with the distribution.
;//
;// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
;// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
;// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
;// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
;// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
;// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
;// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
;// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
;// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
;// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
;// POSSIBILITY OF SUCH DAMAGE.
;//
<html>
<head>
    <title><INTERFACE_NAME></title>

    <link rel="apple-touch-icon" sizes="57x57" href="/apple-icon-57x57.png">
    <link rel="apple-touch-icon" sizes="60x60" href="/apple-icon-60x60.png">
    <link rel="apple-touch-icon" sizes="72x72" href="/apple-icon-72x72.png">
    <link rel="apple-touch-icon" sizes="76x76" href="/apple-icon-76x76.png">
    <link rel="apple-touch-icon" sizes="114x114" href="/apple-icon-114x114.png">
    <link rel="apple-touch-icon" sizes="120x120" href="/apple-icon-120x120.png">
    <link rel="apple-touch-icon" sizes="144x144" href="/apple-icon-144x144.png">
    <link rel="apple-touch-icon" sizes="152x152" href="/apple-icon-152x152.png">
    <link rel="apple-touch-icon" sizes="180x180" href="/apple-icon-180x180.png">
    <link rel="icon" type="image/png" sizes="192x192" href="/android-icon-192x192.png">
    <link rel="icon" type="image/png" sizes="32x32" href="/favicon-32x32.png">
    <link rel="icon" type="image/png" sizes="96x96" href="/favicon-96x96.png">
    <link rel="icon" type="image/png" sizes="16x16" href="/favicon-16x16.png">
    <link rel="manifest" href="/manifest.json">
    <meta name="msapplication-TileColor" content="#ffffff">
    <meta name="msapplication-TileImage" content="/ms-icon-144x144.png">
    <meta name="theme-color" content="#ffffff">

    <style>
        body {
            font-family: "Helvetica";
            background-color: lightsteelblue;
            margin-top: 40px;
            margin-bottom: 40px;
            margin-left: 80px;
            margin-right: 80px;
        }

        h1 {
            color: navy;
        }

        h2 {
            color: navy;
        }

        a:link {
            color: blue;
            text-decoration: none;
        }

        a:visited {
            color: blue;
            text-decoration: none;
        }

        a:hover {
            color: red;
            text-decoration: none;
        }

        a:active {
            color: red;
            text-decoration: none;
        }
    </style>
</head>
<body>

    <a id="top"><h1>Traditional Bridge Documentation for <INTERFACE_NAME></h1></a>

    <UL>
<METHOD_LOOP>
      <LI><a href="#<METHOD_NAME>"><METHOD_NAME></a></LI>
</METHOD_LOOP>
      <LI><a href="/">Back to Service Home Page</a></LI>
    </UL>

<METHOD_LOOP>
    <hr noshade>
    <a id="<METHOD_NAME>"><h2><METHOD_NAME></h2></a>

    <table border="0">
    <tr>
        <td><b>Description</b></td>
        <td><IF COMMENT><METHOD_COMMENT><ELSE>Not provided</IF COMMENT></td>
    </tr>
    <tr>
        <td><b>HTTP&nbsp;method</b></td>
        <td><IF IN_OR_INOUT>POST<ELSE>GET</IF></td>
    </tr>
    <tr>
        <td><b>Method&nbsp;URI</b></td>
        <td><IF NOT IN_OR_INOUT><a href="/<INTERFACE_NAME>/<METHOD_NAME>"></IF>/<INTERFACE_NAME>/<METHOD_NAME><IF NOT IN_OR_INOUT></a></IF></td>
    </tr>
    <tr>
        <td><b>Request&nbsp;headers</b></td>
        <td>
            <COUNTER_1_RESET>
            <IF IN_OR_INOUT>Content-Type: application/json<br><COUNTER_1_INCREMENT></IF>
            <IF DEFINED_ENABLE_AUTHENTICATION>Authorization: Bearer &lt;JWT&gt;<br><COUNTER_1_INCREMENT></IF>
            <IF COUNTER_1_EQ_0>None</IF>
        </td>
    </tr>
    <tr>
        <td><b>Request&nbsp;body</b></td>
        <td><IF IN_OR_INOUT><pre>{<PARAMETER_LOOP><IF IN_OR_INOUT>"<PARAMETER_NAME>": <PARAMETER_SAMPLE_DATA><IF MORE_IN_OR_INOUT>,</IF MORE_IN_OR_INOUT></IF IN_OR_INOUT></PARAMETER_LOOP>}</pre></td><ELSE>None</IF IN_OR_INOUT>
    </tr>
    <tr>
        <td><b>Response&nbsp;body</b></td>
        <td><IF RETURNS_DATA><COUNTER_1_RESET><PARAMETER_LOOP><IF OUT_OR_INOUT><COUNTER_1_INCREMENT></IF OUT_OR_INOUT></PARAMETER_LOOP><pre>{<IF FUNCTION>"ReturnValue": <METHOD_RETURN_SAMPLE_DATA><IF COUNTER_1>, </IF COUNTER_1></IF FUNCTION><PARAMETER_LOOP><IF OUT_OR_INOUT>"<PARAMETER_NAME>": <PARAMETER_SAMPLE_DATA><IF MORE_IN_OR_INOUT>, </IF MORE_IN_OR_INOUT></IF OUT_OR_INOUT></PARAMETER_LOOP>}</pre><ELSE>None</IF RETURNS_DATA></td>
    </tr>
    </table>

<p><a href="#top">Back to Top</a></p>

</METHOD_LOOP>
</body>
</html>
