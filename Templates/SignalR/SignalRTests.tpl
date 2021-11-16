<CODEGEN_FILENAME><INTERFACE_NAME>HubTests.dbl</CODEGEN_FILENAME>
<REQUIRES_USERTOKEN>SERVICES_NAMESPACE</REQUIRES_USERTOKEN>
<REQUIRES_USERTOKEN>MODELS_NAMESPACE</REQUIRES_USERTOKEN>

import Harmony.Core.Converters
import Harmony.Core.EF.Extensions
import Microsoft.AspNetCore.JsonPatch
import Microsoft.AspNetCore.SignalR.Client
import Microsoft.Extensions.DependencyInjection
import Microsoft.VisualStudio.TestTools.UnitTesting
import Newtonsoft.Json
import System.Collections.Generic
import System.IO
import System.Linq
import System.Net.Http
import System.Reflection
import System.Text
import System.Threading.Tasks

import <SERVICES_NAMESPACE>
import <MODELS_NAMESPACE>

namespace <NAMESPACE>

    {TestClass}
    public partial class <INTERFACE_NAME>HubTests

        private method getToken, @Task<string>
        proc
            mreturn Task.FromResult(UnitTestEnvironment.AccessToken)
        endmethod

        private method getHandler, @HttpMessageHandler
            handler, @HttpMessageHandler
        proc
            mreturn UnitTestEnvironment.Server.CreateHandler()
        endmethod

<METHOD_LOOP>
        ;;========================================================================================================================
        ;;<METHOD_NAME>

        {TestMethod}
        public async method <METHOD_NAME>, @Task
        proc
            ;;Do we have request data for asp_login?
            data aspLoginRequestFile, string, Path.Combine(UnitTestEnvironment.TestRequestsFolder,"asp.asp_login.request.json")
            if (!File.Exists(aspLoginRequestFile))
                Assert.Inconclusive("No input data provided for test asp.asp_login!")

  <IF IN_OR_INOUT>
            ;;Do we have request data for this operation?
            data requestFile, string, Path.Combine(UnitTestEnvironment.TestRequestsFolder,"<INTERFACE_NAME>.<METHOD_NAME>.request.json")
            if (!File.Exists(requestFile))
                Assert.Inconclusive("No input data provided for test <INTERFACE_NAME>.<METHOD_NAME>!")

  </IF>
  <IF RETURNS_DATA>
            ;;Do we have a partial method implementation to process response data for this operation?
            if (Type.GetType("<NAMESPACE>.<INTERFACE_NAME>HubTests").GetMethod("<METHOD_NAME>_Validate",BindingFlags.NonPublic|BindingFlags.Instance) == ^null)
                Assert.Inconclusive("No response validation method provided for test <INTERFACE_NAME>.<METHOD_NAME>!")

  </IF>
            lambda hubConfig(opts)
            begin
                opts.AccessTokenProvider = getToken
                opts.HttpMessageHandlerFactory = getHandler
            end

            data connection = new HubConnectionBuilder()
            &    .WithUrl("https://localhost:<SERVER_HTTPS_PORT>/hub/<INTERFACE_NAME>", hubConfig )
            &    .AddNewtonsoftJsonProtocol()
            &    .Build()

            ;;ASP_LOGIN
            data asp_login_tcs = new TaskCompletionSource<asp.asp_login_Response>()
            data asp_login_resultHandler, @Action<asp.asp_login_Response>
            asp_login_resultHandler = lambda(msg) { asp_login_tcs.TrySetResult(msg) }
            connection.On<asp.asp_login_Response>("asp_login_Result", asp_login_resultHandler)

            ;;Setup up for a method call
            data tcs = new TaskCompletionSource<IF RETURNS_DATA><<INTERFACE_NAME>.<METHOD_NAME>_Response><ELSE><boolean></IF>()
            data resultHandler, @Action<IF RETURNS_DATA><<INTERFACE_NAME>.<METHOD_NAME>_Response></IF>

  <IF RETURNS_DATA>
            resultHandler = lambda(msg) { tcs.TrySetResult(msg) }
  <ELSE>
            resultHandler = lambda() { tcs.TrySetResult(true) }
  </IF>

            ;;Define the client endpoint to be called by the server when the operation completes
            ;;Name matches name declared in hub (method + "_Result")
            connection.On<IF RETURNS_DATA><<INTERFACE_NAME>.<METHOD_NAME>_Response></IF>("<METHOD_NAME>_Result", resultHandler)

            ;;Start a connection to the server
            await connection.StartAsync()

            ;;-------------------------------------------------------------------------------------
            ;;Call ASP_LOGIN
            begin
                ;;Define the data to be sent to the request
                data aspLoginRequest = JsonConvert.DeserializeObject<asp.asp_login_Request>(File.ReadAllText(aspLoginRequestFile))

                ;;Send the request
                await connection.InvokeAsync("asp_login", aspLoginRequest)

                ;;Wait for the response
                data aspLoginresponse = await asp_login_tcs.Task

                ;;Write the response data to a JSON file for easy viewing
                data responseFile = Path.Combine(UnitTestEnvironment.TestResponsesFolder,"asp.asp_login.response.json")
                File.WriteAllText(responseFile,JsonConvert.SerializeObject(aspLoginresponse,Formatting.Indented))

                ;;Are we logged in?
                data result = false
                data message = String.Empty
                asp_login_Validate(aspLoginRequest,aspLoginresponse,result,message)

                if (!result)
                    Assert.Fail(message)
            end

            ;;-------------------------------------------------------------------------------------
            ;;If we get here, ASP_LOGIN worked so we can call the actual method to be tested

  <IF IN_OR_INOUT>
            ;;Define the data to be sent to the request
            data <METHOD_NAME>Request = JsonConvert.DeserializeObject<<INTERFACE_NAME>.<METHOD_NAME>_Request>(File.ReadAllText(requestFile))

  </IF>
            ;;Send the request
            await connection.InvokeAsync("<METHOD_NAME>"<IF IN_OR_INOUT>, <METHOD_NAME>Request</IF>)
            
            ;;Wait for the response
            data <METHOD_NAME>Response = await tcs.Task

            ;;Clean up the connection
            connection.DisposeAsync()

  <IF RETURNS_DATA>
            ;;Write the response data to a JSON file for easy viewing
            File.WriteAllText(Path.Combine(UnitTestEnvironment.TestResponsesFolder,"<INTERFACE_NAME>.<METHOD_NAME>.response.json"),JsonConvert.SerializeObject(<METHOD_NAME>Response,Formatting.Indented))

            ;;Call the validation method to evaluate the response
            data result = false
            data message = String.Empty
            <METHOD_NAME>_Validate(<IF IN_OR_INOUT><METHOD_NAME>Request,</IF><METHOD_NAME>Response,result,message)

            Assert.IsTrue(result,message)
  <ELSE>
            ;;There is no returned data, so the boolean response indicates whether the call completed
            Assert.IsTrue(<METHOD_NAME>Response)
  </IF>

        endmethod

  <IF RETURNS_DATA>
        ;;--------------------------------------------------
        ;;Response validation for <METHOD_NAME>

        ;;; <summary>
        ;;; Partial method to allow validation of response from <INTERFACE_NAME>.<METHOD_NAME>
        ;;; </summary>
    <IF IN_OR_INOUT>
        ;;; <param name="request">Request sent to method.</param>
    </IF>
        ;;; <param name="response">Response from method.</param>
        ;;; <param name="result">Boolean result indicating whether the method call was successful.</param>
        ;;; <param name="message">Message indicating the naure of a failed method call.</param>
         partial method <METHOD_NAME>_Validate, void
    <IF IN_OR_INOUT>
            required in  <METHOD_NAME>Request,  @<INTERFACE_NAME>.<METHOD_NAME>_Request
    </IF>
            required in  <METHOD_NAME>Response, @<INTERFACE_NAME>.<METHOD_NAME>_Response
            required out result,   boolean
            required out message,  string
        endmethod

  </IF>
</METHOD_LOOP>
    endclass

endnamespace
