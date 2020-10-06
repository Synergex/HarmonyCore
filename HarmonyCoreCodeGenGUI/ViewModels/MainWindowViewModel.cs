using CodeGen.Engine;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using HarmonyCoreCodeGenGUI.Properties;
using HarmonyCoreGenerator.Model;
using Microsoft.Build.Locator;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace HarmonyCoreCodeGenGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _solutionDir;
        private Solution _solution;

        public MainWindowViewModel()
        {
            OpenMenuItemIsEnabled = true;

            StatusBarTextBlockText = "Ready";
            InstructionalTabTextBlockText = "Open a Harmony Core CodeGen JSON file to continue.";

            SettingsTabVisibility = Visibility.Collapsed;
            StructureTabVisibility = Visibility.Collapsed;
            InterfacesTabVisibility = Visibility.Collapsed;
            EntityFrameworkTabVisibility = Visibility.Collapsed;
            TraditionalBridgeTabVisibillity = Visibility.Collapsed;
            ODataTabVisibility = Visibility.Collapsed;
        }

        #region Methods
        private void NewMenuItemCommandMethod() { }
        private void OpenMenuItemCommandMethod()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Harmony Core CodeGen JSON File (*.json)|*.json|All files (*.*)|*.*",
                    FileName = "Harmony.Core.CodeGen.json",
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    StatusBarTextBlockText = "Loading...";

                    // Create Solution
                    _solutionDir = Path.GetDirectoryName(openFileDialog.FileName);

                    // Calling Register methods will subscribe to AssemblyResolve event. After this we can
                    // safely call code that use MSBuild types (in the Builder class).
                    if (!MSBuildLocator.IsRegistered)
                        MSBuildLocator.RegisterMSBuildPath(MSBuildLocator.QueryVisualStudioInstances().ToList().FirstOrDefault().MSBuildPath);
                    _solution = Solution.LoadSolution(openFileDialog.FileName, _solutionDir);

                    if (_solution != null)
                    {
                        Messenger.Default.Send(_solution);

                        InstructionalTabTextBlockText = "Select a tab to continue.";

                        // Determine visibility of tabs
                        bool? hasOdata = _solution.ExtendedStructures?.Any(k => k.EnabledGenerators.Contains("ODataGenerator"));
                        bool? hasModels = _solution.ExtendedStructures?.Any(k => k.EnabledGenerators.Contains("ModelGenerator"));
                        bool? hasTraditionalBridge = _solution.ExtendedStructures?.Any(k => k.EnabledGenerators.Contains("TraditionalBridgeGenerator"));

                        SettingsTabVisibility = Visibility.Visible;
                        ODataTabVisibility = hasOdata == true && hasModels == true ? Visibility.Visible : Visibility.Collapsed;
                        StructureTabVisibility = hasOdata == true && hasModels == true ? Visibility.Visible : Visibility.Collapsed;
                        InterfacesTabVisibility = hasTraditionalBridge == true ? Visibility.Visible : Visibility.Collapsed;
                        TraditionalBridgeTabVisibillity = hasTraditionalBridge == true ? Visibility.Visible : Visibility.Collapsed;

                        SaveMenuItemIsEnabled = true;
                        CloseMenuItemIsEnabled = true;
                        RegenerateFilesMenuItemIsEnabled = true;

                        StatusBarTextBlockText = "Loaded successfully";
                    }
                    else
                    {
                        MessageBox.Show($"Could not load the solution associated with this JSON file.{Environment.NewLine}{Environment.NewLine}Double check the paths inside the JSON file and try again. In addition, the JSON file must be placed at the root of the HarmonyCore solution.", Resources.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), e.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
        private void SaveMenuItemCommandMethod(bool setStatusBarTextBlockText = true)
        {
            try
            {
                if (setStatusBarTextBlockText)
                    StatusBarTextBlockText = "Saving...";

                // Get info from viewmodels, save altered solution
                Messenger.Default.Send(new NotificationMessageAction<SettingsTabViewModel>(string.Empty, settingsTabViewModel =>
                {
                    _solution.EnableNewtonsoftJson = settingsTabViewModel.EnableNewtonsoftJson;
                    _solution.SignalRPath = settingsTabViewModel.SignalRPath;

                    _solution.ControllersFolder = settingsTabViewModel.ControllersFolder;
                    _solution.DataFolder = settingsTabViewModel.DataFolder;
                    _solution.IsolatedFolder = settingsTabViewModel.IsolatedFolder;
                    _solution.ModelsFolder = settingsTabViewModel.ModelsFolder;
                    _solution.SelfHostFolder = settingsTabViewModel.SelfHostFolder;
                    _solution.ServicesFolder = settingsTabViewModel.ServicesFolder;
                    _solution.SolutionFolder = settingsTabViewModel.SolutionFolder;
                    _solution.TemplatesFolder = settingsTabViewModel.TemplatesFolder;
                    _solution.TraditionalBridgeFolder = settingsTabViewModel.TraditionalBridgeFolder;
                    _solution.UnitTestFolder = settingsTabViewModel.UnitTestFolder;

                    _solution.ClientModelsNamespace = settingsTabViewModel.ClientModelsNamespace;
                    _solution.ControllersNamespace = settingsTabViewModel.ControllersNamespace;
                    _solution.ModelsNamespace = settingsTabViewModel.ModelsNamespace;
                    _solution.SelfHostNamespace = settingsTabViewModel.SelfHostNamespace;
                    _solution.ServicesNamespace = settingsTabViewModel.ServicesNamespace;
                    _solution.TraditionalBridgeNamespace = settingsTabViewModel.TraditionalBridgeNamespace;
                    _solution.UnitTestsBaseNamespace = settingsTabViewModel.UnitTestsBaseNamespace;
                    _solution.UnitTestsNamespace = settingsTabViewModel.UnitTestsNamespace;
                }));
                if (ODataTabVisibility == Visibility.Visible)
                {
                    Messenger.Default.Send(new NotificationMessageAction<ODataTabViewModel>(string.Empty, odataTabViewModel =>
                    {
                        _solution.OAuthApi = odataTabViewModel.OAuthApi;
                        _solution.OAuthClient = odataTabViewModel.OAuthClient;
                        _solution.OAuthSecret = odataTabViewModel.OAuthSecret;
                        _solution.OAuthServer = odataTabViewModel.OAuthServer;
                        _solution.OAuthTestUser = odataTabViewModel.OAuthTestUser;
                        _solution.OAuthTestPassword = odataTabViewModel.OAuthTestPassword;
                        
                        _solution.CustomAuthController = odataTabViewModel.CustomAuthController;
                        _solution.CustomAuthEndpointPath = odataTabViewModel.CustomAuthEndpointPath;
                        _solution.CustomAuthUserName = odataTabViewModel.CustomAuthUserName;
                        _solution.CustomAuthPassword = odataTabViewModel.CustomAuthPassword;
                        
                        _solution.APIContactEmail = odataTabViewModel.APIContactEmail;
                        _solution.APIContactName = odataTabViewModel.APIContactName;
                        _solution.APIDescription = odataTabViewModel.APIDescription;
                        _solution.APIDocsPath = odataTabViewModel.APIDocsPath;
                        _solution.APIEnableQueryParams = odataTabViewModel.APIEnableQueryParams;
                        _solution.APILicenseName = odataTabViewModel.APILicenseName;
                        _solution.APILicenseUrl = odataTabViewModel?.APILicenseUrl?.ToString();
                        _solution.APITerms = odataTabViewModel.APITerms;
                        _solution.APITitle = odataTabViewModel.APITitle;
                        _solution.APIVersion = odataTabViewModel.APIVersion;
                        
                        _solution.ServerBasePath = odataTabViewModel.ServerBasePath;
                        _solution.ServerName = odataTabViewModel.ServerName;
                        _solution.ServerHttpPort = odataTabViewModel.ServerHttpPort;
                        _solution.ServerHttpsPort = odataTabViewModel.ServerHttpsPort;
                        _solution.ServerProtocol = odataTabViewModel.ServerProtocol;
                        
                        _solution.AlternateKeyEndpoints = odataTabViewModel.AlternateKeyEndpoints;
                        _solution.CollectionCountEndpoints = odataTabViewModel.CollectionCountEndpoints;
                        _solution.DeleteEndpoints = odataTabViewModel.DeleteEndpoints;
                        _solution.DocumentPropertyEndpoints = odataTabViewModel.DocumentPropertyEndpoints;
                        _solution.FullCollectionEndpoints = odataTabViewModel.FullCollectionEndpoints;
                        _solution.IndividualPropertyEndpoints = odataTabViewModel.IndividualPropertyEndpoints;
                        _solution.PatchEndpoints = odataTabViewModel.PatchEndpoints;
                        _solution.PostEndpoints = odataTabViewModel.PostEndpoints;
                        _solution.PrimaryKeyEndpoints = odataTabViewModel.PrimaryKeyEndpoints;
                        _solution.PutEndpoints = odataTabViewModel.PutEndpoints;
                        
                        _solution.GenerateOData = odataTabViewModel.GenerateOData;
                        _solution.GeneratePostmanTests = odataTabViewModel.GeneratePostmanTests;
                        _solution.GenerateSelfHost = odataTabViewModel.GenerateSelfHost;
                        _solution.GenerateUnitTests = odataTabViewModel.GenerateUnitTests;
                        
                        _solution.ODataFilter = odataTabViewModel.ODataFilter;
                        _solution.ODataOrderBy = odataTabViewModel.ODataOrderBy;
                        _solution.ODataRelations = odataTabViewModel.ODataRelations;
                        _solution.ODataRelationValidation = odataTabViewModel.ODataRelationValidation;
                        _solution.ODataSelect = odataTabViewModel.ODataSelect;
                        _solution.ODataSkip = odataTabViewModel.ODataSkip;
                        _solution.ODataTop = odataTabViewModel.ODataTop;
                        
                        _solution.AdapterRouting = odataTabViewModel.AdapterRouting;
                        _solution.AlternateFieldNames = odataTabViewModel.AlternateFieldNames;
                        _solution.Authentication = odataTabViewModel.Authentication;
                        _solution.CaseSensitiveUrls = odataTabViewModel.CaseSensitiveUrls;
                        _solution.CreateTestFiles = odataTabViewModel.CreateTestFiles;
                        
                        _solution.CrossDomainBrowsing = odataTabViewModel.CrossDomainBrowsing;
                        _solution.CustomAuthentication = odataTabViewModel.CustomAuthentication;
                        _solution.DisableFileLogicals = odataTabViewModel.DisableFileLogicals;
                        _solution.FieldOverlays = odataTabViewModel.FieldOverlays;
                        _solution.FieldSecurity = odataTabViewModel.FieldSecurity;
                        
                        _solution.IISSupport = odataTabViewModel.IISSupport;
                        _solution.ReadOnlyProperties = odataTabViewModel.ReadOnlyProperties;
                        _solution.SmcPostmanTests = odataTabViewModel.SmcPostmanTests;
                        _solution.SmcSignalRHubs = odataTabViewModel.SmcSignalRHubs;
                        _solution.StoredProcedureRouting = odataTabViewModel.StoredProcedureRouting;

                        _solution.VersioningOrSwagger = (VersioningOrSwaggerMode?)odataTabViewModel.VersioningOrSwagger;
                    }));
                }
                if (StructureTabVisibility == Visibility.Visible)
                {
                    Messenger.Default.Send(new NotificationMessageAction<StructureTabViewModel>(string.Empty, structureTabViewModel =>
                    {
                        _solution.RPSMFIL = structureTabViewModel.RPSMFIL;
                        _solution.RPSTFIL = structureTabViewModel.RPSTFIL;
                        _solution.RepositoryProject = structureTabViewModel.RepositoryProject;
                    }));
                }
                if (InterfacesTabVisibility == Visibility.Visible)
                {
                    Messenger.Default.Send(new NotificationMessageAction<InterfacesTabViewModel>(string.Empty, interfacesTabViewModel =>
                    {
                        _solution.TraditionalBridge.XFServerSMCPath = interfacesTabViewModel.XFServerSMCPath;
                        _solution.TraditionalBridge.ExtendedInterfaces = new List<InterfaceEx>(interfacesTabViewModel.ExtendedInterfaces);
                    }));
                }
                if (TraditionalBridgeTabVisibillity == Visibility.Visible)
                {
                    Messenger.Default.Send(new NotificationMessageAction<TraditionalBridgeTabViewModel>(string.Empty, traditionalBridgeTabViewModel =>
                    {
                        _solution.ControllersProject = traditionalBridgeTabViewModel.ControllersProject;
                        _solution.IsolatedProject = traditionalBridgeTabViewModel.IsolatedProject;
                        _solution.ModelsProject = traditionalBridgeTabViewModel.ModelsProject;
                        _solution.SelfHostProject = traditionalBridgeTabViewModel.SelfHostProject;
                        _solution.ServicesProject = traditionalBridgeTabViewModel.ServicesProject;
                        _solution.TraditionalBridgeProject = traditionalBridgeTabViewModel.TraditionalBridgeProject;
                        _solution.UnitTestProject = traditionalBridgeTabViewModel.UnitTestProject;
                        
                        _solution.TraditionalBridge.EnableOptionalParameters = traditionalBridgeTabViewModel.EnableOptionalParameters;
                        _solution.TraditionalBridge.EnableSampleDispatchers = traditionalBridgeTabViewModel.EnableSampleDispatchers;
                        _solution.TraditionalBridge.EnableXFServerPlusMigration = traditionalBridgeTabViewModel.EnableXFServerPlusMigration;
                    }));
                }

                // Save solution
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                };
                File.WriteAllText(Path.Combine(_solutionDir, "Harmony.Core.CodeGen.json"), JsonConvert.SerializeObject(_solution, settings));

                if (setStatusBarTextBlockText)
                    StatusBarTextBlockText = "Saved successfully";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), e.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
        private void RegenerateFilesMenuItemCommandMethod()
        {
            try
            {
                SaveMenuItemCommandMethod(false);
                StatusBarTextBlockText = "Regenerating files...";

                // Set current dir to solution dir since folders are partial pathed
                Directory.SetCurrentDirectory(_solutionDir);
                GenerateResult result = _solution.GenerateSolution((task, message) => { }, CancellationToken.None);
                
                // Display messages with errors
                // Get all tasks with errors
                IEnumerable<CodeGenTask> tasksWithErrors = result.CodeGenTasks.Where(k => k.Errors > 0);
                if (tasksWithErrors.Any())
                {
                    List<string> messages = new List<string>
                    {
                        "Errors during code generation:",
                        Environment.NewLine
                    };

                    // Construct list of messages that will be displayed
                    foreach (CodeGenTask item in tasksWithErrors)
                    {
                        // Get task descriptions
                        messages.Add(item.Description);
                        foreach (LogEntry item2 in item.Messages)
                        {
                            // Get those tasks' messages, if they aren't blank
                            if (!string.IsNullOrWhiteSpace(item2.Message))
                                messages.Add(item2.Message);
                        }
                        messages.Add(string.Empty);
                    }
                    MessageBox.Show(string.Join(Environment.NewLine, messages), Resources.Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                if (result.Failed)
                {
                    ;
                }

                StatusBarTextBlockText = "Regenerated successfully";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), e.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
        private void CloseMenuItemCommandMethod()
        {
            StatusBarTextBlockText = "Closing...";

            // Clean up everything
            _solutionDir = null;
            _solution = null;

            TabControlSelectedIndex = 0;
            InstructionalTabTextBlockText = "Open a Harmony Core CodeGen JSON file to continue.";

            SettingsTabVisibility = Visibility.Collapsed;
            StructureTabVisibility = Visibility.Collapsed;
            InterfacesTabVisibility = Visibility.Collapsed;
            EntityFrameworkTabVisibility = Visibility.Collapsed;
            TraditionalBridgeTabVisibillity = Visibility.Collapsed;
            ODataTabVisibility = Visibility.Collapsed;

            OpenMenuItemIsEnabled = true;
            SaveMenuItemIsEnabled = false;
            CloseMenuItemIsEnabled = false;
            RegenerateFilesMenuItemIsEnabled = false;

            Messenger.Default.Send(new Solution());

            StatusBarTextBlockText = "Closed successfully";
        }
        #endregion

        #region NewMenuItem
        #region NewMenuItemCommand
        private ICommand _newMenuItemCommand;
        public ICommand NewMenuItemCommand
        {
            get
            {
                return _newMenuItemCommand;
            }
            set
            {
                _newMenuItemCommand = value;
                RaisePropertyChanged(() => NewMenuItemCommand);
            }
        }
        #endregion
        #region NewMenuItemIsEnabled
        private bool _newMenuItemIsEnabled;
        public bool NewMenuItemIsEnabled
        {
            get
            {
                return _newMenuItemIsEnabled;
            }
            set
            {
                _newMenuItemIsEnabled = value;
                RaisePropertyChanged(() => NewMenuItemIsEnabled);


                NewMenuItemCommand = value ? new RelayCommand(() => NewMenuItemCommandMethod()) : null;
            }
        }
        #endregion
        #endregion
        #region OpenMenuItem
        #region OpenMenuItemCommand
        private ICommand _openMenuItemCommand;
        public ICommand OpenMenuItemCommand
        {
            get
            {
                return _openMenuItemCommand;
            }
            set
            {
                _openMenuItemCommand = value;
                RaisePropertyChanged(() => OpenMenuItemCommand);
            }
        }
        #endregion
        #region OpenMenuItemIsEnabled
        private bool _openMenuItemIsEnabled;
        public bool OpenMenuItemIsEnabled
        {
            get
            {
                return _openMenuItemIsEnabled;
            }
            set
            {
                _openMenuItemIsEnabled = value;
                RaisePropertyChanged(() => OpenMenuItemIsEnabled);

                OpenMenuItemCommand = value ? new RelayCommand(() => OpenMenuItemCommandMethod()) : null;
            }
        }
        #endregion
        #endregion
        #region SaveMenuItem
        #region SaveMenuItemCommand
        private ICommand _saveMenuItemCommand;
        public ICommand SaveMenuItemCommand
        {
            get
            {
                return _saveMenuItemCommand;
            }
            set
            {
                _saveMenuItemCommand = value;
                RaisePropertyChanged(() => SaveMenuItemCommand);
            }
        }
        #endregion
        #region SaveMenuItemIsEnabled
        private bool _saveMenuItemIsEnabled;
        public bool SaveMenuItemIsEnabled
        {
            get
            {
                return _saveMenuItemIsEnabled;
            }
            set
            {
                _saveMenuItemIsEnabled = value;
                RaisePropertyChanged(() => SaveMenuItemIsEnabled);

                SaveMenuItemCommand = value ? new RelayCommand(() => SaveMenuItemCommandMethod()) : null;
            }
        }
        #endregion
        #endregion
        #region RegenerateFilesMenuItem
        #region RegenerateFilesMenuItemCommand
        private ICommand _regenerateFilesMenuItemCommand;
        public ICommand RegenerateFilesMenuItemCommand
        {
            get
            {
                return _regenerateFilesMenuItemCommand;
            }
            set
            {
                _regenerateFilesMenuItemCommand = value;
                RaisePropertyChanged(() => RegenerateFilesMenuItemCommand);
            }
        }
        #endregion
        #region RegenerateFilesMenuItemIsEnabled
        private bool _regenerateFilesMenuItemIsEnabled;
        public bool RegenerateFilesMenuItemIsEnabled
        {
            get
            {
                return _regenerateFilesMenuItemIsEnabled;
            }
            set
            {
                _regenerateFilesMenuItemIsEnabled = value;
                RaisePropertyChanged(() => RegenerateFilesMenuItemIsEnabled);

                RegenerateFilesMenuItemCommand = value ? new RelayCommand(() => RegenerateFilesMenuItemCommandMethod()) : null;
            }
        }
        #endregion
        #endregion
        #region CloseMenuItem
        #region CloseMenuItemCommand
        private ICommand _closeMenuItemCommand;
        public ICommand CloseMenuItemCommand
        {
            get
            {
                return _closeMenuItemCommand;
            }
            set
            {
                _closeMenuItemCommand = value;
                RaisePropertyChanged(() => CloseMenuItemCommand);
            }
        }
        #endregion
        #region CloseMenuItemIsEnabled
        private bool _CloseMenuItemIsEnabled;
        public bool CloseMenuItemIsEnabled
        {
            get
            {
                return _CloseMenuItemIsEnabled;
            }
            set
            {
                _CloseMenuItemIsEnabled = value;
                RaisePropertyChanged(() => CloseMenuItemIsEnabled);

                CloseMenuItemCommand = value ? new RelayCommand(() => CloseMenuItemCommandMethod()) : null;
            }
        }
        #endregion
        #endregion

        #region StatusBarTextBlockText
        private string _statusBarTextBlockText;

        [SuppressMessage("Reliability", "CA2011:Avoid infinite recursion", Justification = "Text is only set if value is not 'Ready', and it sets the value to 'Ready'")]
        public string StatusBarTextBlockText
        {
            get
            {
                return _statusBarTextBlockText;
            }
            set
            {
                _statusBarTextBlockText = value;
                RaisePropertyChanged(() => StatusBarTextBlockText);

                // Reset text after 5 seconds to ready
                if (value != null && !value.Equals("Ready", StringComparison.Ordinal))
                {
                    new Thread(() =>
                    {
                        Thread.Sleep((int)TimeSpan.FromSeconds(5).TotalMilliseconds);
                        StatusBarTextBlockText = "Ready";
                    }).Start();
                }
            }
        }
        #endregion

        #region InstructionalTabTextBlockText
        private string _instructionalTabTextBlockText;
        public string InstructionalTabTextBlockText
        {
            get
            {
                return _instructionalTabTextBlockText;
            }
            set
            {
                _instructionalTabTextBlockText = value;
                RaisePropertyChanged(() => InstructionalTabTextBlockText);
            }
        }
        #endregion

        #region TabControlSelectedIndex
        private int _tabControlSelectedIndex;
        public int TabControlSelectedIndex
        {
            get
            {
                return _tabControlSelectedIndex;
            }
            set
            {
                _tabControlSelectedIndex = value;
                RaisePropertyChanged(() => TabControlSelectedIndex);
            }
        }
        #endregion

        #region SettingsTabVisibility
        private Visibility _settingsTabVisibility;
        public Visibility SettingsTabVisibility
        {
            get
            {
                return _settingsTabVisibility;
            }
            set
            {
                _settingsTabVisibility = value;
                RaisePropertyChanged(() => SettingsTabVisibility);
            }
        }
        #endregion
        #region StructureTabVisibility
        private Visibility _structureTabVisibility;
        public Visibility StructureTabVisibility
        {
            get
            {
                return _structureTabVisibility;
            }
            set
            {
                _structureTabVisibility = value;
                RaisePropertyChanged(() => StructureTabVisibility);
            }
        }
        #endregion
        #region InterfacesTabVisibility
        private Visibility _interfacesTabVisibility;
        public Visibility InterfacesTabVisibility
        {
            get
            {
                return _interfacesTabVisibility;
            }
            set
            {
                _interfacesTabVisibility = value;
                RaisePropertyChanged(() => InterfacesTabVisibility);
            }
        }
        #endregion
        #region EntityFrameworkTabVisibility
        private Visibility _entityFrameworkTabVisibility;
        public Visibility EntityFrameworkTabVisibility
        {
            get
            {
                return _entityFrameworkTabVisibility;
            }
            set
            {
                _entityFrameworkTabVisibility = value;
                RaisePropertyChanged(() => EntityFrameworkTabVisibility);
            }
        }
        #endregion
        #region ODataTabVisibility
        private Visibility _odataTabVisibility;
        public Visibility ODataTabVisibility
        {
            get
            {
                return _odataTabVisibility;
            }
            set
            {
                _odataTabVisibility = value;
                RaisePropertyChanged(() => ODataTabVisibility);
            }
        }
        #endregion
        #region TraditionalBridgeTabVisibillity
        private Visibility _traditionalBridgeTabVisibillity;
        public Visibility TraditionalBridgeTabVisibillity
        {
            get
            {
                return _traditionalBridgeTabVisibillity;
            }
            set
            {
                _traditionalBridgeTabVisibillity = value;
                RaisePropertyChanged(() => TraditionalBridgeTabVisibillity);
            }
        }
        #endregion
    }
}
