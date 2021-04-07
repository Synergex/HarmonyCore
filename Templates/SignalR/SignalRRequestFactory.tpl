<CODEGEN_FILENAME><INTERFACE_NAME>RequestFactory.dbl</CODEGEN_FILENAME>
<REQUIRES_USERTOKEN>MODELS_NAMESPACE</REQUIRES_USERTOKEN>

import <MODELS_NAMESPACE>

namespace <NAMESPACE>

    {TestClass}
    public static class <INTERFACE_NAME>RequestFactory

<METHOD_LOOP>
  <IF IN_OR_IOUT>
        public static method <METHOD_NAME>_Request, @<INTERFACE_NAME>.<METHOD_NAME>_Request
        proc
;// new AspLogin() {PCompanyCode="FCL",PLogonId="KEN",PUserId="SYN",PPassword="DIUQSOBHBAMRRAYMLITR",RFclaspApp1="1",RFclaspApp2="1",RFclaspApp3="1",RFclaspApp4="1",RFclaspApp5="1",RFclaspApp6="1",RFclaspApp7="1",RFclaspApp8="1",RFclaspApp9="1",RFclaspApp10="1",RFclaspUi1="1",RFclaspUi2="1",RFclaspUi3="1",RFclaspUi4="1",RFclaspUi5="1",RFclaspUi6="1",RFclaspUi7="1",RFclaspUi8="1",RFclaspUi9="1",RFclaspUi10="1",RPartyType=1}
            data request = new <INTERFACE_NAME>.<METHOD_NAME>_Request() {
    <PARAMETER_LOOP>
      <IF IN_OR_IOUT>
            &   <PARAMETER_NAME> = <PARAMETER_SAMPLE_DATA><IF MORE_IN_OR_INOUT>,</IF>
      </IF IN_OR_IOUT>
    </PARAMETER_LOOP>
            & }

            mreturn request

        endmethod

  </IF IN_OR_IOUT>
</METHOD_LOOP>
    endclass

endnamespace
