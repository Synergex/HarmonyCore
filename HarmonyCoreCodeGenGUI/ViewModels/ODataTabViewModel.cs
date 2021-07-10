using Microsoft.Toolkit.Mvvm;
using Microsoft.Toolkit.Mvvm.Messaging;
using HarmonyCoreCodeGenGUI.Classes;
using HarmonyCoreCodeGenGUI.Views;
using HarmonyCoreGenerator.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HarmonyCoreCodeGenGUI.ViewModels
{
    public class ODataTabViewModel : ObservableObject
    {
        public ODataTabViewModel()
        {
            // Initial state
            StrongReferenceMessenger.Default.Register<Solution>(this, (obj, sender) => {
                OAuthApi = sender.OAuthApi;
                OAuthClient = sender.OAuthClient;
                OAuthSecret = sender.OAuthSecret;
                OAuthServer = sender.OAuthServer;
                OAuthTestUser = sender.OAuthTestUser;
                OAuthTestPassword = sender.OAuthTestPassword;

                CustomAuthController = sender.CustomAuthController;
                CustomAuthEndpointPath = sender.CustomAuthEndpointPath;
                CustomAuthUserName = sender.CustomAuthUserName;
                CustomAuthPassword = sender.CustomAuthPassword;

                APIContactEmail = sender.APIContactEmail;
                APIContactName = sender.APIContactName;
                APIDescription = sender.APIDescription;
                APIDocsPath = sender.APIDocsPath;
                APIEnableQueryParams = sender.APIEnableQueryParams;
                APILicenseName = sender.APILicenseName;
                if (sender.APILicenseUrl != null)
                    APILicenseUrl = new Uri(sender.APILicenseUrl);
                APITerms = sender.APITerms;
                APITitle = sender.APITitle;
                APIVersion = sender.APIVersion;

                ServerBasePath = sender.ServerBasePath;
                ServerName = sender.ServerName;
                ServerHttpPort = sender.ServerHttpPort;
                ServerHttpsPort = sender.ServerHttpsPort;
                ServerProtocol = sender.ServerProtocol;

                AlternateKeyEndpoints = sender.AlternateKeyEndpoints;
                CollectionCountEndpoints = sender.CollectionCountEndpoints;
                DeleteEndpoints = sender.DeleteEndpoints;
                DocumentPropertyEndpoints = sender.DocumentPropertyEndpoints;
                FullCollectionEndpoints = sender.FullCollectionEndpoints;
                IndividualPropertyEndpoints = sender.IndividualPropertyEndpoints;
                PatchEndpoints = sender.PatchEndpoints;
                PostEndpoints = sender.PostEndpoints;
                PrimaryKeyEndpoints = sender.PrimaryKeyEndpoints;
                PutEndpoints = sender.PutEndpoints;

                GenerateOData = sender.GenerateOData;
                GeneratePostmanTests = sender.GeneratePostmanTests;
                GenerateSelfHost = sender.GenerateSelfHost;
                GenerateUnitTests = sender.GenerateUnitTests;

                ODataFilter = sender.ODataFilter;
                ODataOrderBy = sender.ODataOrderBy;
                ODataRelations = sender.ODataRelations;
                ODataRelationValidation = sender.ODataRelationValidation;
                ODataSelect = sender.ODataSelect;
                ODataSkip = sender.ODataSkip;
                ODataTop = sender.ODataTop;

                AdapterRouting = sender.AdapterRouting;
                AlternateFieldNames = sender.AlternateFieldNames;
                Authentication = sender.Authentication;
                CaseSensitiveUrls = sender.CaseSensitiveUrls;
                CreateTestFiles = sender.CreateTestFiles;

                CrossDomainBrowsing = sender.CrossDomainBrowsing;
                CustomAuthentication = sender.CustomAuthentication;
                DisableFileLogicals = sender.DisableFileLogicals;
                FieldOverlays = sender.FieldOverlays;
                FieldSecurity = sender.FieldSecurity;

                IISSupport = sender.IISSupport;
                ReadOnlyProperties = sender.ReadOnlyProperties;
                SmcPostmanTests = sender.SmcPostmanTests;
                SmcSignalRHubs = sender.SmcSignalRHubs;
                StoredProcedureRouting = sender.StoredProcedureRouting;

                if (sender.VersioningOrSwagger != null)
                    VersioningOrSwagger = (VersioningOrSwaggerModeEnum)(int)sender.VersioningOrSwagger;
            });

            // Send updated state
            StrongReferenceMessenger.Default.Register<NotificationMessageAction<ODataTabViewModel>>(this, (obj, sender) => sender.callback(this));
        }

        #region OAuthApi
        private string _oauthApi;
        public string OAuthApi
        {
            get
            {
                return _oauthApi;
            }
            set
            {
                SetProperty(ref _oauthApi, value);
            }
        }
        #endregion
        #region OAuthClient
        private string _oauthClient;
        public string OAuthClient
        {
            get
            {
                return _oauthClient;
            }
            set
            {
                SetProperty(ref _oauthClient, value);
            }
        }
        #endregion
        #region OAuthSecret
        private string _oauthSecret;
        public string OAuthSecret
        {
            get
            {
                return _oauthSecret;
            }
            set
            {
                SetProperty(ref _oauthSecret, value);
            }
        }
        #endregion
        #region OAuthServer
        private string _oauthServer;
        public string OAuthServer
        {
            get
            {
                return _oauthServer;
            }
            set
            {
                SetProperty(ref _oauthServer, value);
            }
        }
        #endregion
        #region OAuthTestUser
        private string _oauthTestUser;
        public string OAuthTestUser
        {
            get
            {
                return _oauthTestUser;
            }
            set
            {
                SetProperty(ref _oauthTestUser, value);
            }
        }
        #endregion
        #region OAuthTestPassword
        private string _oauthTestPassword;
        public string OAuthTestPassword
        {
            get
            {
                return _oauthTestPassword;
            }
            set
            {
                SetProperty(ref _oauthTestPassword, value);
            }
        }
        #endregion

        #region CustomAuthController
        private string _customAuthController;
        public string CustomAuthController
        {
            get
            {
                return _customAuthController;
            }
            set
            {
                SetProperty(ref _customAuthController, value);
            }
        }
        #endregion
        #region CustomAuthEndpointPath
        private string _customAuthEndpointPath;
        public string CustomAuthEndpointPath
        {
            get
            {
                return _customAuthEndpointPath;
            }
            set
            {
                SetProperty(ref _customAuthEndpointPath, value);
            }
        }
        #endregion
        #region CustomAuthUserName
        private string _customAuthUserName;
        public string CustomAuthUserName
        {
            get
            {
                return _customAuthUserName;
            }
            set
            {
                SetProperty(ref _customAuthUserName, value);
            }
        }
        #endregion
        #region CustomAuthPassword
        private string _customAuthPassword;
        public string CustomAuthPassword
        {
            get
            {
                return _customAuthPassword;
            }
            set
            {
                SetProperty(ref _customAuthPassword, value);
            }
        }
        #endregion

        #region APIContactEmail
        private string _apiContactEmail;
        public string APIContactEmail
        {
            get
            {
                return _apiContactEmail;
            }
            set
            {
                SetProperty(ref _apiContactEmail, value);
            }
        }
        #endregion
        #region APIContactName
        private string _apiContactName;
        public string APIContactName
        {
            get
            {
                return _apiContactName;
            }
            set
            {
                SetProperty(ref _apiContactName, value);
            }
        }
        #endregion
        #region APIDescription
        private string _apiDescription;
        public string APIDescription
        {
            get
            {
                return _apiDescription;
            }
            set
            {
                SetProperty(ref _apiDescription, value);
            }
        }
        #endregion
        #region APIDocsPath
        private string _apiDocsPath;
        public string APIDocsPath
        {
            get
            {
                return _apiDocsPath;
            }
            set
            {
                SetProperty(ref _apiDocsPath, value);
            }
        }
        #endregion
        #region APIEnableQueryParams
        private string _apiEnableQueryParams;
        public string APIEnableQueryParams
        {
            get
            {
                return _apiEnableQueryParams;
            }
            set
            {
                SetProperty(ref _apiEnableQueryParams, value);
            }
        }
        #endregion
        #region APILicenseName
        private string _apiLicenseName;
        public string APILicenseName
        {
            get
            {
                return _apiLicenseName;
            }
            set
            {
                SetProperty(ref _apiLicenseName, value);
            }
        }
        #endregion
        #region APILicenseUrl
        private Uri _apiLicenseUrl;
        public Uri APILicenseUrl
        {
            get
            {
                return _apiLicenseUrl;
            }
            set
            {
                SetProperty(ref _apiLicenseUrl, value);
            }
        }
        #endregion
        #region APITerms
        private string _apiTerms;
        public string APITerms
        {
            get
            {
                return _apiTerms;
            }
            set
            {
                SetProperty(ref _apiTerms, value);
            }
        }
        #endregion
        #region APITitle
        private string _apiTitle;
        public string APITitle
        {
            get
            {
                return _apiTitle;
            }
            set
            {
                SetProperty(ref _apiTitle, value);
            }
        }
        #endregion
        #region APIVersion
        private string _apiVersion;
        public string APIVersion
        {
            get
            {
                return _apiVersion;
            }
            set
            {
                SetProperty(ref _apiVersion, value);
            }
        }
        #endregion

        #region ServerBasePath
        private string _serverBasePath;
        public string ServerBasePath
        {
            get
            {
                return _serverBasePath;
            }
            set
            {
                SetProperty(ref _serverBasePath, value);
            }
        }
        #endregion
        #region ServerHttpPort
        private string _serverHttpPort;
        public string ServerHttpPort
        {
            get
            {
                return _serverHttpPort;
            }
            set
            {
                SetProperty(ref _serverHttpPort, value);
            }
        }
        #endregion
        #region ServerHttpsPort
        private string _serverHttpsPort;
        public string ServerHttpsPort
        {
            get
            {
                return _serverHttpsPort;
            }
            set
            {
                SetProperty(ref _serverHttpsPort, value);
            }
        }
        #endregion
        #region ServerName
        private string _serverName;
        public string ServerName
        {
            get
            {
                return _serverName;
            }
            set
            {
                SetProperty(ref _serverName, value);
            }
        }
        #endregion
        #region ServerProtocol
        private string _serverProtocol;
        public string ServerProtocol
        {
            get
            {
                return _serverProtocol;
            }
            set
            {
                SetProperty(ref _serverProtocol, value);
            }
        }
        #endregion

        #region AlternateKeyEndpoints
        private bool? _alternateKeyEndpoints;
        public bool? AlternateKeyEndpoints
        {
            get
            {
                return _alternateKeyEndpoints;
            }
            set
            {
                SetProperty(ref _alternateKeyEndpoints, value);
            }
        }
        #endregion
        #region CollectionCountEndpoints
        private bool? _collectionCountEndpoints;
        public bool? CollectionCountEndpoints
        {
            get
            {
                return _collectionCountEndpoints;
            }
            set
            {
                SetProperty(ref _collectionCountEndpoints, value);
            }
        }
        #endregion
        #region DeleteEndpoints
        private bool? _deleteEndpoints;
        public bool? DeleteEndpoints
        {
            get
            {
                return _deleteEndpoints;
            }
            set
            {
                SetProperty(ref _deleteEndpoints, value);
            }
        }
        #endregion
        #region DocumentPropertyEndpoints
        private bool? _documentPropertyEndpoints;
        public bool? DocumentPropertyEndpoints
        {
            get
            {
                return _documentPropertyEndpoints;
            }
            set
            {
                SetProperty(ref _documentPropertyEndpoints, value);
            }
        }
        #endregion
        #region FullCollectionEndpoints
        private bool? _fullCollectionEndpoints;
        public bool? FullCollectionEndpoints
        {
            get
            {
                return _fullCollectionEndpoints;
            }
            set
            {
                SetProperty(ref _fullCollectionEndpoints, value);
            }
        }
        #endregion
        #region IndividualPropertyEndpoints
        private bool? _individualPropertyEndpoints;
        public bool? IndividualPropertyEndpoints
        {
            get
            {
                return _individualPropertyEndpoints;
            }
            set
            {
                SetProperty(ref _individualPropertyEndpoints, value);
            }
        }
        #endregion
        #region PatchEndpoints
        private bool? _patchEndpoints;
        public bool? PatchEndpoints
        {
            get
            {
                return _patchEndpoints;
            }
            set
            {
                SetProperty(ref _patchEndpoints, value);
            }
        }
        #endregion
        #region PostEndpoints
        private bool? _postEndpoints;
        public bool? PostEndpoints
        {
            get
            {
                return _postEndpoints;
            }
            set
            {
                SetProperty(ref _postEndpoints, value);
            }
        }
        #endregion
        #region PrimaryKeyEndpoints
        private bool? _primaryKeyEndpoints;
        public bool? PrimaryKeyEndpoints
        {
            get
            {
                return _primaryKeyEndpoints;
            }
            set
            {
                SetProperty(ref _primaryKeyEndpoints, value);
            }
        }
        #endregion
        #region PutEndpoints
        private bool? _putEndpoints;
        public bool? PutEndpoints
        {
            get
            {
                return _putEndpoints;
            }
            set
            {
                SetProperty(ref _putEndpoints, value);
            }
        }
        #endregion

        #region GenerateOData
        private bool? _generateOData;
        public bool? GenerateOData
        {
            get
            {
                return _generateOData;
            }
            set
            {
                SetProperty(ref _generateOData, value);
            }
        }
        #endregion
        #region GeneratePostmanTests
        private bool? _generatePostmanTests;
        public bool? GeneratePostmanTests
        {
            get
            {
                return _generatePostmanTests;
            }
            set
            {
                SetProperty(ref _generatePostmanTests, value);
            }
        }
        #endregion
        #region GenerateSelfHost
        private bool? _generateSelfHost;
        public bool? GenerateSelfHost
        {
            get
            {
                return _generateSelfHost;
            }
            set
            {
                SetProperty(ref _generateSelfHost, value);
            }
        }
        #endregion
        #region GenerateUnitTests
        private bool? _generateUnitTests;
        public bool? GenerateUnitTests
        {
            get
            {
                return _generateUnitTests;
            }
            set
            {
                SetProperty(ref _generateUnitTests, value);
            }
        }
        #endregion

        #region ODataFilter
        private bool? _odataFilter;
        public bool? ODataFilter
        {
            get
            {
                return _odataFilter;
            }
            set
            {
                SetProperty(ref _odataFilter, value);
            }
        }
        #endregion
        #region ODataOrderBy
        private bool? _odataOrderBy;
        public bool? ODataOrderBy
        {
            get
            {
                return _odataOrderBy;
            }
            set
            {
                SetProperty(ref _odataOrderBy, value);
            }
        }
        #endregion
        #region ODataRelations
        private bool? _odataRelations;
        public bool? ODataRelations
        {
            get
            {
                return _odataRelations;
            }
            set
            {
                SetProperty(ref _odataRelations, value);
            }
        }
        #endregion
        #region ODataRelationValidation
        private bool? _odataRelationValidation;
        public bool? ODataRelationValidation
        {
            get
            {
                return _odataRelationValidation;
            }
            set
            {
                SetProperty(ref _odataRelationValidation, value);
            }
        }
        #endregion
        #region ODataSelect
        private bool? _odataSelect;
        public bool? ODataSelect
        {
            get
            {
                return _odataSelect;
            }
            set
            {
                SetProperty(ref _odataSelect, value);
            }
        }
        #endregion
        #region ODataSkip
        private bool? _odataSkip;
        public bool? ODataSkip
        {
            get
            {
                return _odataSkip;
            }
            set
            {
                SetProperty(ref _odataSkip, value);
            }
        }
        #endregion
        #region ODataTop
        private bool? _odataTop;
        public bool? ODataTop
        {
            get
            {
                return _odataTop;
            }
            set
            {
                SetProperty(ref _odataTop, value);
            }
        }
        #endregion

        #region AdapterRouting
        private bool? _adapterRouting;
        public bool? AdapterRouting
        {
            get
            {
                return _adapterRouting;
            }
            set
            {
                SetProperty(ref _adapterRouting, value);
            }
        }
        #endregion
        #region AlternateFieldNames
        private bool? _alternateFieldNames;
        public bool? AlternateFieldNames
        {
            get
            {
                return _alternateFieldNames;
            }
            set
            {
                SetProperty(ref _alternateFieldNames, value);
            }
        }
        #endregion
        #region Authentication
        private bool? _authentication;
        public bool? Authentication
        {
            get
            {
                return _authentication;
            }
            set
            {
                SetProperty(ref _authentication, value);
            }
        }
        #endregion
        #region CaseSensitiveUrls
        private bool? _caseSensitiveUrls;
        public bool? CaseSensitiveUrls
        {
            get
            {
                return _caseSensitiveUrls;
            }
            set
            {
                SetProperty(ref _caseSensitiveUrls, value);
            }
        }
        #endregion
        #region CreateTestFiles
        private bool? _createTestFiles;
        public bool? CreateTestFiles
        {
            get
            {
                return _createTestFiles;
            }
            set
            {
                SetProperty(ref _createTestFiles, value);
            }
        }
        #endregion

        #region CrossDomainBrowsing
        private bool? _crossDomainBrowsing;
        public bool? CrossDomainBrowsing
        {
            get
            {
                return _crossDomainBrowsing;
            }
            set
            {
                SetProperty(ref _crossDomainBrowsing, value);
            }
        }
        #endregion
        #region CustomAuthentication
        private bool? _customAuthentication;
        public bool? CustomAuthentication
        {
            get
            {
                return _customAuthentication;
            }
            set
            {
                SetProperty(ref _customAuthentication, value);
            }
        }
        #endregion
        #region DisableFileLogicals
        private bool? _disableFileLogicals;
        public bool? DisableFileLogicals
        {
            get
            {
                return _disableFileLogicals;
            }
            set
            {
                SetProperty(ref _disableFileLogicals, value);
            }
        }
        #endregion
        #region FieldOverlays
        private bool? _fieldOverlays;
        public bool? FieldOverlays
        {
            get
            {
                return _fieldOverlays;
            }
            set
            {
                SetProperty(ref _fieldOverlays, value);
            }
        }
        #endregion
        #region FieldSecurity
        private bool? _fieldSecurity;
        public bool? FieldSecurity
        {
            get
            {
                return _fieldSecurity;
            }
            set
            {
                SetProperty(ref _fieldSecurity, value);
            }
        }
        #endregion

        #region IISSupport
        private bool? _iisSupport;
        public bool? IISSupport
        {
            get
            {
                return _iisSupport;
            }
            set
            {
                SetProperty(ref _iisSupport, value);
            }
        }
        #endregion
        #region ReadOnlyProperties
        private bool? _readOnlyProperties;
        public bool? ReadOnlyProperties
        {
            get
            {
                return _readOnlyProperties;
            }
            set
            {
                SetProperty(ref _readOnlyProperties, value);
            }
        }
        #endregion
        #region SmcPostmanTests
        private bool? _smcPostmanTests;
        public bool? SmcPostmanTests
        {
            get
            {
                return _smcPostmanTests;
            }
            set
            {
                SetProperty(ref _smcPostmanTests, value);
            }
        }
        #endregion
        #region SmcSignalRHubs
        private bool? _smcSignalRHubs;
        public bool? SmcSignalRHubs
        {
            get
            {
                return _smcSignalRHubs;
            }
            set
            {
                SetProperty(ref _smcSignalRHubs, value);
            }
        }
        #endregion
        #region StoredProcedureRouting
        private bool? _storedProcedureRouting;
        public bool? StoredProcedureRouting
        {
            get
            {
                return _storedProcedureRouting;
            }
            set
            {
                SetProperty(ref _storedProcedureRouting, value);
            }
        }
        #endregion

        [TypeConverter(typeof(EnumDescriptionTypeConverter))]
        public enum VersioningOrSwaggerModeEnum
        {
            None = 0,
            [Description("API Versioning")]
            ApiVersioning = 1,
            [Description("Generate Swagger Docs")]
            GenerateSwaggerDoc = 2
        }
        public IEnumerable<VersioningOrSwaggerModeEnum> VersioningOrSwaggerMode { get; } = Enum.GetValues(typeof(VersioningOrSwaggerModeEnum)).Cast<VersioningOrSwaggerModeEnum>();
        #region VersioningOrSwagger
        private VersioningOrSwaggerModeEnum? _versioningOrSwagger;
        public VersioningOrSwaggerModeEnum? VersioningOrSwagger
        {
            get
            {
                return _versioningOrSwagger;
            }
            set
            {
                SetProperty(ref _versioningOrSwagger, value);
            }
        }
        #endregion
    }
}
