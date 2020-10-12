using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HarmonyCoreCodeGenGUI.Classes;
using HarmonyCoreCodeGenGUI.Views;
using HarmonyCoreGenerator.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HarmonyCoreCodeGenGUI.ViewModels
{
    public class ODataTabViewModel : ViewModelBase
    {
        public ODataTabViewModel()
        {
            // Initial state
            Messenger.Default.Register<Solution>(this, sender => {
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
            Messenger.Default.Register<NotificationMessageAction<ODataTabViewModel>>(this, callback => callback.Execute(this));
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
                _oauthApi = value;
                RaisePropertyChanged(() => OAuthApi);
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
                _oauthClient = value;
                RaisePropertyChanged(() => OAuthClient);
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
                _oauthSecret = value;
                RaisePropertyChanged(() => OAuthSecret);
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
                _oauthServer = value;
                RaisePropertyChanged(() => OAuthServer);
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
                _oauthTestUser = value;
                RaisePropertyChanged(() => OAuthTestUser);
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
                _oauthTestPassword = value;
                RaisePropertyChanged(() => OAuthTestPassword);
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
                _customAuthController = value;
                RaisePropertyChanged(() => CustomAuthController);
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
                _customAuthEndpointPath = value;
                RaisePropertyChanged(() => CustomAuthEndpointPath);
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
                _customAuthUserName = value;
                RaisePropertyChanged(() => CustomAuthUserName);
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
                _customAuthPassword = value;
                RaisePropertyChanged(() => CustomAuthPassword);
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
                _apiContactEmail = value;
                RaisePropertyChanged(() => APIContactEmail);
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
                _apiContactName = value;
                RaisePropertyChanged(() => APIContactName);
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
                _apiDescription = value;
                RaisePropertyChanged(() => APIDescription);
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
                _apiDocsPath = value;
                RaisePropertyChanged(() => APIDocsPath);
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
                _apiEnableQueryParams = value;
                RaisePropertyChanged(() => APIEnableQueryParams);
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
                _apiLicenseName = value;
                RaisePropertyChanged(() => APILicenseName);
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
                _apiLicenseUrl = value;
                RaisePropertyChanged(() => APILicenseUrl);
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
                _apiTerms = value;
                RaisePropertyChanged(() => APITerms);
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
                _apiTitle = value;
                RaisePropertyChanged(() => APITitle);
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
                _apiVersion = value;
                RaisePropertyChanged(() => APIVersion);
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
                _serverBasePath = value;
                RaisePropertyChanged(() => ServerBasePath);
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
                _serverHttpPort = value;
                RaisePropertyChanged(() => ServerHttpPort);
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
                _serverHttpsPort = value;
                RaisePropertyChanged(() => ServerHttpsPort);
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
                _serverName = value;
                RaisePropertyChanged(() => ServerName);
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
                _serverProtocol = value;
                RaisePropertyChanged(() => ServerProtocol);
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
                _alternateKeyEndpoints = value;
                RaisePropertyChanged(() => AlternateKeyEndpoints);
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
                _collectionCountEndpoints = value;
                RaisePropertyChanged(() => CollectionCountEndpoints);
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
                _deleteEndpoints = value;
                RaisePropertyChanged(() => DeleteEndpoints);
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
                _documentPropertyEndpoints = value;
                RaisePropertyChanged(() => DocumentPropertyEndpoints);
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
                _fullCollectionEndpoints = value;
                RaisePropertyChanged(() => FullCollectionEndpoints);
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
                _individualPropertyEndpoints = value;
                RaisePropertyChanged(() => IndividualPropertyEndpoints);
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
                _patchEndpoints = value;
                RaisePropertyChanged(() => PatchEndpoints);
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
                _postEndpoints = value;
                RaisePropertyChanged(() => PostEndpoints);
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
                _primaryKeyEndpoints = value;
                RaisePropertyChanged(() => PrimaryKeyEndpoints);
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
                _putEndpoints = value;
                RaisePropertyChanged(() => PutEndpoints);
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
                _generateOData = value;
                RaisePropertyChanged(() => GenerateOData);
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
                _generatePostmanTests = value;
                RaisePropertyChanged(() => GeneratePostmanTests);
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
                _generateSelfHost = value;
                RaisePropertyChanged(() => GenerateSelfHost);
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
                _generateUnitTests = value;
                RaisePropertyChanged(() => GenerateUnitTests);
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
                _odataFilter = value;
                RaisePropertyChanged(() => ODataFilter);
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
                _odataOrderBy = value;
                RaisePropertyChanged(() => ODataOrderBy);
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
                _odataRelations = value;
                RaisePropertyChanged(() => ODataRelations);
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
                _odataRelationValidation = value;
                RaisePropertyChanged(() => ODataRelationValidation);
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
                _odataSelect = value;
                RaisePropertyChanged(() => ODataSelect);
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
                _odataSkip = value;
                RaisePropertyChanged(() => ODataSkip);
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
                _odataTop = value;
                RaisePropertyChanged(() => ODataTop);
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
                _adapterRouting = value;
                RaisePropertyChanged(() => AdapterRouting);
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
                _alternateFieldNames = value;
                RaisePropertyChanged(() => AlternateFieldNames);
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
                _authentication = value;
                RaisePropertyChanged(() => Authentication);
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
                _caseSensitiveUrls = value;
                RaisePropertyChanged(() => CaseSensitiveUrls);
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
                _createTestFiles = value;
                RaisePropertyChanged(() => CreateTestFiles);
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
                _crossDomainBrowsing = value;
                RaisePropertyChanged(() => CrossDomainBrowsing);
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
                _customAuthentication = value;
                RaisePropertyChanged(() => CustomAuthentication);
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
                _disableFileLogicals = value;
                RaisePropertyChanged(() => DisableFileLogicals);
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
                _fieldOverlays = value;
                RaisePropertyChanged(() => FieldOverlays);
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
                _fieldSecurity = value;
                RaisePropertyChanged(() => FieldSecurity);
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
                _iisSupport = value;
                RaisePropertyChanged(() => IISSupport);
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
                _readOnlyProperties = value;
                RaisePropertyChanged(() => ReadOnlyProperties);
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
                _smcPostmanTests = value;
                RaisePropertyChanged(() => SmcPostmanTests);
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
                _smcSignalRHubs = value;
                RaisePropertyChanged(() => SmcSignalRHubs);
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
                _storedProcedureRouting = value;
                RaisePropertyChanged(() => StoredProcedureRouting);
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
                _versioningOrSwagger = value;
                RaisePropertyChanged(() => VersioningOrSwagger);
            }
        }
        #endregion
    }
}
