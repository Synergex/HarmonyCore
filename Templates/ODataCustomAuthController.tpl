<CODEGEN_FILENAME>AuthenticationController.dbl</CODEGEN_FILENAME>
;;*****************************************************************************
;;
;; Title:       AuthenticationController.dbl
;;
;; Description: This class defines a WebAPI controller that exposes an endpoint
;;              that can be used to obtain a custom JSON Web Token.
;;
;; This code was originally code generated but will not be replaced by the code
;; re-generation once in existence.
;;
;;*****************************************************************************

import Microsoft.AspNetCore.Authorization
import Microsoft.AspNetCore.Mvc
import Newtonsoft.Json
import Services.Models
import System
import System.Collections.Generic
import System.Linq
import System.Text
import System.Threading.Tasks

namespace <NAMESPACE>

    {Route("<CUSTOM_AUTH_CONTROLLER_PATH>")}
    public class AuthenticationController extends ControllerBase

        {AllowAnonymous}
        {HttpPost}
        {Route("<CUSTOM_AUTH_ENDPOINT_PATH>")}
        ;;; <summary>
        ;;; 
        ;;; </summary>
        ;;; <returns></returns>
        public async method GetToken, @Task<ActionResult>
            {FromBody}
            required in aRequest, @GetTokenRequest
        proc
            if (!ModelState.IsValid)
            begin
                mreturn BadRequest()
            end

            ;TODO: Authenticate the user login. For now we'll hard code some valid credentials
            if (aRequest.Username.ToLower().Equals("<CUSTOM_AUTH_USERNAME>") && aRequest.Password.Equals("<CUSTOM_AUTH_PASSWORD>")) then
            begin
                data tokenValidHours, int, 24
                ;;To add custom claims to the JWT, add parameters to GetToken and pass values here!
                data accessToken, string, AuthenticationTools.GetToken(aRequest.username,tokenValidHours)
                mreturn ok(accessToken)
            end
            else
            begin
                mreturn Unauthorized()
            end
        endmethod

    endclass

endnamespace
