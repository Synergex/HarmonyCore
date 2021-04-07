<CODEGEN_FILENAME><INTERFACE_NAME>ResponseValidation.dbl</CODEGEN_FILENAME>
<REQUIRES_USERTOKEN>MODELS_NAMESPACE</REQUIRES_USERTOKEN>

import <MODELS_NAMESPACE>

namespace <NAMESPACE>

    {TestClass}
    public static class <INTERFACE_NAME>ResponseValidation

<METHOD_LOOP>
  <IF RETURNS_DATA>
        public static method Validate_<METHOD_NAME>_Response, boolean
            required in  response, @<INTERFACE_NAME>.<METHOD_NAME>_Request
            required out message, string
        proc
            data responseIsValid = false
            message = String.Empty

            ;TODO Add code to determine whether the response is valid
            responseIsValid = false
            message = "Validate_<METHOD_NAME>_Response has not been implemented!"

            mreturn responseIsValid
        endmethod

  </IF RETURNS_DATA>>
</METHOD_LOOP>
    endclass

endnamespace
