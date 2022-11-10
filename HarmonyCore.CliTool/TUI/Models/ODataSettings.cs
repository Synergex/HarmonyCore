using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Models
{
    internal class ODataSettings :  SingleItemSettingsBase
    {
        public ODataSettings(SolutionInfo context) : base(context)
        {
            var solution = context.CodeGenSolution;
            BaseInterface.LoadSameProperties(solution);
            //OAuthApi = solution.OAuthApi;
            //OAuthClient = solution.OAuthClient;
            //OAuthSecret = solution.OAuthSecret;
            //OAuthServer = solution.OAuthServer;
            //OAuthTestUser = solution.OAuthTestUser;
            //OAuthTestPassword = solution.OAuthTestPassword;

            //CustomAuthController = solution.CustomAuthController;
            //CustomAuthEndpointPath = solution.CustomAuthEndpointPath;
            //CustomAuthUserName = solution.CustomAuthUserName;
            //CustomAuthPassword = solution.CustomAuthPassword;

            //APIContactEmail = solution.APIContactEmail;
            //APIContactName = solution.APIContactName;
            //APIDescription = solution.APIDescription;
            //APIDocsPath = solution.APIDocsPath;
            //APIEnableQueryParams = solution.APIEnableQueryParams;
            //APILicenseName = solution.APILicenseName;
            if (solution.APILicenseUrl != null)
                APILicenseUrl = new Uri(solution.APILicenseUrl);

            //APITerms = solution.APITerms;
            //APITitle = solution.APITitle;
            //APIVersion = solution.APIVersion;

            //ServerBasePath = solution.ServerBasePath;
            //ServerName = solution.ServerName;
            //ServerHttpPort = solution.ServerHttpPort;
            //ServerHttpsPort = solution.ServerHttpsPort;
            //ServerProtocol = solution.ServerProtocol;

            //AlternateKeyEndpoints = solution.AlternateKeyEndpoints;
            //CollectionCountEndpoints = solution.CollectionCountEndpoints;
            //DeleteEndpoints = solution.DeleteEndpoints;
            //DocumentPropertyEndpoints = solution.DocumentPropertyEndpoints;
            //FullCollectionEndpoints = solution.FullCollectionEndpoints;
            //IndividualPropertyEndpoints = solution.IndividualPropertyEndpoints;
            //PatchEndpoints = solution.PatchEndpoints;
            //PostEndpoints = solution.PostEndpoints;
            //PrimaryKeyEndpoints = solution.PrimaryKeyEndpoints;
            //PutEndpoints = solution.PutEndpoints;

            //GenerateOData = solution.GenerateOData;
            //GeneratePostmanTests = solution.GeneratePostmanTests;
            //GenerateSelfHost = solution.GenerateSelfHost;
            //GenerateUnitTests = solution.GenerateUnitTests;

            //ODataFilter = solution.ODataFilter;
            //ODataOrderBy = solution.ODataOrderBy;
            //ODataRelations = solution.ODataRelations;
            //ODataRelationValidation = solution.ODataRelationValidation;
            //ODataSelect = solution.ODataSelect;
            //ODataSkip = solution.ODataSkip;
            //ODataTop = solution.ODataTop;

            //AdapterRouting = solution.AdapterRouting;
            //AlternateFieldNames = solution.AlternateFieldNames;
            //Authentication = solution.Authentication;
            //CaseSensitiveUrls = solution.CaseSensitiveUrls;
            //CreateTestFiles = solution.CreateTestFiles;

            //CrossDomainBrowsing = solution.CrossDomainBrowsing;
            //CustomAuthentication = solution.CustomAuthentication;
            //DisableFileLogicals = solution.DisableFileLogicals;
            //FieldOverlays = solution.FieldOverlays;
            //FieldSecurity = solution.FieldSecurity;

            //IISSupport = solution.IISSupport;
            //ReadOnlyProperties = solution.ReadOnlyProperties;
            //SmcPostmanTests = solution.SmcPostmanTests;
            //SmcSignalRHubs = solution.SmcSignalRHubs;
            //StoredProcedureRouting = solution.StoredProcedureRouting;

            Name = "OData";
        }

        public override void Save(SolutionInfo context)
        {
            BaseInterface.SaveSameProperties(context.CodeGenSolution);
            if (APILicenseUrl != null)
                context.CodeGenSolution.APILicenseUrl = APILicenseUrl.ToString();
        }

        [Prompt("OAuth API")]
        public string OAuthApi { get; private set; }
        [Prompt("OAuth Client")]
        public string OAuthClient { get; private set; }
        [Prompt("OAuth Secret")]
        public string OAuthSecret { get; private set; }
        [Prompt("OAuth Server")]
        public string OAuthServer { get; private set; }
        [Prompt("OAuth Test User")]
        public string OAuthTestUser { get; private set; }
        [Prompt("OAuth Test Password")]
        public string OAuthTestPassword { get; private set; }
        [Prompt("Custom Auth Controller")]
        public string CustomAuthController { get; private set; }
        [Prompt("Custom Auth Path")]
        public string CustomAuthEndpointPath { get; private set; }
        [Prompt("Custom Auth User Name")]
        public string CustomAuthUserName { get; private set; }
        [Prompt("Custom Auth Password")]
        public string CustomAuthPassword { get; private set; }
        [Prompt("API contact email")]
        public string APIContactEmail { get; private set; }
        [Prompt("API contact name")]
        public string APIContactName { get; private set; }
        [Prompt("API description")]
        public string APIDescription { get; private set; }
        [Prompt("API docs path")]
        public string APIDocsPath { get; private set; }
        [Prompt("API enable query params")]
        public string APIEnableQueryParams { get; private set; }
        [Prompt("API license name")]
        public string APILicenseName { get; private set; }
        [Prompt("API license url")]
        public Uri APILicenseUrl { get; private set; }
        [Prompt("API terms")]
        public string APITerms { get; private set; }
        [Prompt("API title")]
        public string APITitle { get; private set; }
        [Prompt("API version")]
        public string APIVersion { get; private set; }
        [Prompt("Server base path")]
        public string ServerBasePath { get; private set; }
        public string ServerName { get; private set; }
        [Prompt("Server http port")]
        public string ServerHttpPort { get; private set; }
        [Prompt("Server https port")]
        public string ServerHttpsPort { get; private set; }
        [Prompt("Server protocol")]
        public string ServerProtocol { get; private set; }
        [Prompt("Enable alt endpoints")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? AlternateKeyEndpoints { get; private set; }
        [Prompt("Enable collection count endpoints")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? CollectionCountEndpoints { get; private set; }
        [Prompt("Enable DELETE endpoints")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? DeleteEndpoints { get; private set; }
        [Prompt("Enable property endpoint docs")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? DocumentPropertyEndpoints { get; private set; }
        [Prompt("Enable collection endpoints")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? FullCollectionEndpoints { get; private set; }
        [Prompt("Enable property endpoints")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? IndividualPropertyEndpoints { get; private set; }
        [Prompt("Enable PATCH endpoints")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? PatchEndpoints { get; private set; }
        [Prompt("Enable POST endpoints")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? PostEndpoints { get; private set; }
        [Prompt("Enable primary key endpoints")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? PrimaryKeyEndpoints { get; private set; }
        [Prompt("Enable PUT endpoints")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? PutEndpoints { get; private set; }
        [Prompt("Enable OData")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? GenerateOData { get; private set; }
        [Prompt("Enable Postman test generation")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? GeneratePostmanTests { get; private set; }
        [Prompt("Enable self host")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? GenerateSelfHost { get; private set; }
        [Prompt("Enable unit tests")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? GenerateUnitTests { get; private set; }
        [Prompt("Enable OData filters")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? ODataFilter { get; private set; }
        [Prompt("Enable OData orderby")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? ODataOrderBy { get; private set; }
        [Prompt("Enable OData relations")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? ODataRelations { get; private set; }
        [Prompt("Enable relation validation")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? ODataRelationValidation { get; private set; }
        [Prompt("Enable OData select")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? ODataSelect { get; private set; }
        [Prompt("Enable OData skip")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? ODataSkip { get; private set; }
        [Prompt("Enable OData top")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? ODataTop { get; private set; }
        [Prompt("Enable adapter routing")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? AdapterRouting { get; private set; }
        [Prompt("Enable alternate field names")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? AlternateFieldNames { get; private set; }
        [Prompt("Enable authentication")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? Authentication { get; private set; }
        [Prompt("Enable case sensitive urls")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? CaseSensitiveUrls { get; private set; }
        [Prompt("Enable test file creation")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? CreateTestFiles { get; private set; }
        [Prompt("Enable Cors")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? CrossDomainBrowsing { get; private set; }
        [Prompt("Enable Custom Auth")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? CustomAuthentication { get; private set; }
        [Prompt("Disable file logicals")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? DisableFileLogicals { get; private set; }
        [Prompt("Enable field overlays")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? FieldOverlays { get; private set; }
        [Prompt("Enable field security")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? FieldSecurity { get; private set; }
        [Prompt("Enable IIS support")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? IISSupport { get; private set; }
        [Prompt("Enable readonly properties")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? ReadOnlyProperties { get; private set; }
        [Prompt("Enable SMC postman tests")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? SmcPostmanTests { get; private set; }
        [Prompt("Enable SMC signalR hubs")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? SmcSignalRHubs { get; private set; }
        [Prompt("Enable sproc routing")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? StoredProcedureRouting { get; private set; }
    }
}
