using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HarmonyCoreGenerator.Model;

namespace HarmonyCoreCodeGenGUI.ViewModels
{
    public class SettingsTabViewModel : ViewModelBase
    {
        public SettingsTabViewModel()
        {
            // Initial state
            Messenger.Default.Register<Solution>(this, solution => {
                EnableNewtonsoftJson = solution.EnableNewtonsoftJson;
                SignalRPath = solution.SignalRPath;

                ControllersFolder = solution.ControllersFolder;
                DataFolder = solution.DataFolder;
                IsolatedFolder = solution.IsolatedFolder;
                ModelsFolder = solution.ModelsFolder;
                SelfHostFolder = solution.SelfHostFolder;
                ServicesFolder = solution.ServicesFolder;
                SolutionFolder = solution.SolutionFolder;
                TemplatesFolder = solution.TemplatesFolder;
                TraditionalBridgeFolder = solution.TraditionalBridgeFolder;
                UnitTestFolder = solution.UnitTestFolder;

                ClientModelsNamespace = solution.ClientModelsNamespace;
                ControllersNamespace = solution.ControllersNamespace;
                ModelsNamespace = solution.ModelsNamespace;
                SelfHostNamespace = solution.SelfHostNamespace;
                ServicesNamespace = solution.ServicesNamespace;
                TraditionalBridgeNamespace = solution.TraditionalBridgeNamespace;
                UnitTestsBaseNamespace = solution.UnitTestsBaseNamespace;
                UnitTestsNamespace = solution.UnitTestsNamespace;
            });

            // Send updated state
            Messenger.Default.Register<NotificationMessageAction<SettingsTabViewModel>>(this, callback => callback.Execute(this));
        }

        #region EnableNewtonsoftJson
        private bool? _enableNewtonsoftJson;
        public bool? EnableNewtonsoftJson
        {
            get
            {
                return _enableNewtonsoftJson;
            }
            set
            {
                _enableNewtonsoftJson = value;
                RaisePropertyChanged(() => EnableNewtonsoftJson);
            }
        }
        #endregion
        #region SignalRPath
        private string _signalRPath;
        public string SignalRPath
        {
            get
            {
                return _signalRPath;
            }
            set
            {
                _signalRPath = value;
                RaisePropertyChanged(() => SignalRPath);
            }
        }
        #endregion

        #region ControllersFolder
        private string _controllersFolder;
        public string ControllersFolder
        {
            get
            {
                return _controllersFolder;
            }
            set
            {
                _controllersFolder = value;
                RaisePropertyChanged(() => ControllersFolder);
            }
        }
        #endregion
        #region DataFolder
        private string _dataFolder;
        public string DataFolder
        {
            get
            {
                return _dataFolder;
            }
            set
            {
                _dataFolder = value;
                RaisePropertyChanged(() => DataFolder);
            }
        }
        #endregion
        #region IsolatedFolder
        private string _isolatedFolder;
        public string IsolatedFolder
        {
            get
            {
                return _isolatedFolder;
            }
            set
            {
                _isolatedFolder = value;
                RaisePropertyChanged(() => IsolatedFolder);
            }
        }
        #endregion
        #region ModelsFolder
        private string _modelsFolder;
        public string ModelsFolder
        {
            get
            {
                return _modelsFolder;
            }
            set
            {
                _modelsFolder = value;
                RaisePropertyChanged(() => ModelsFolder);
            }
        }
        #endregion
        #region SelfHostFolder
        private string _selfHostFolder;
        public string SelfHostFolder
        {
            get
            {
                return _selfHostFolder;
            }
            set
            {
                _selfHostFolder = value;
                RaisePropertyChanged(() => SelfHostFolder);
            }
        }
        #endregion
        #region ServicesFolder
        private string _servicesFolder;
        public string ServicesFolder
        {
            get
            {
                return _servicesFolder;
            }
            set
            {
                _servicesFolder = value;
                RaisePropertyChanged(() => ServicesFolder);
            }
        }
        #endregion
        #region SolutionFolder
        private string _solutionFolder;
        public string SolutionFolder
        {
            get
            {
                return _solutionFolder;
            }
            set
            {
                _solutionFolder = value;
                RaisePropertyChanged(() => SolutionFolder);
            }
        }
        #endregion
        #region TemplatesFolder
        private string _templatesFolder;
        public string TemplatesFolder
        {
            get
            {
                return _templatesFolder;
            }
            set
            {
                _templatesFolder = value;
                RaisePropertyChanged(() => TemplatesFolder);
            }
        }
        #endregion
        #region TraditionalBridgeFolder
        private string _traditionalBridgeFolder;
        public string TraditionalBridgeFolder
        {
            get
            {
                return _traditionalBridgeFolder;
            }
            set
            {
                _traditionalBridgeFolder = value;
                RaisePropertyChanged(() => TraditionalBridgeFolder);
            }
        }
        #endregion
        #region UnitTestFolder
        private string _unitTestFolder;
        public string UnitTestFolder
        {
            get
            {
                return _unitTestFolder;
            }
            set
            {
                _unitTestFolder = value;
                RaisePropertyChanged(() => UnitTestFolder);
            }
        }
        #endregion

        #region ClientModelsNamespace
        private string _clientModelsNamespace;
        public string ClientModelsNamespace
        {
            get
            {
                return _clientModelsNamespace;
            }
            set
            {
                _clientModelsNamespace = value;
                RaisePropertyChanged(() => ClientModelsNamespace);
            }
        }
        #endregion
        #region ControllersNamespace
        private string _controllersNamespace;
        public string ControllersNamespace
        {
            get
            {
                return _controllersNamespace;
            }
            set
            {
                _controllersNamespace = value;
                RaisePropertyChanged(() => ControllersNamespace);
            }
        }
        #endregion
        #region ModelsNamespace
        private string _modelsNamespace;
        public string ModelsNamespace
        {
            get
            {
                return _modelsNamespace;
            }
            set
            {
                _modelsNamespace = value;
                RaisePropertyChanged(() => ModelsNamespace);
            }
        }
        #endregion
        #region SelfHostNamespace
        private string _selfHostNamespace;
        public string SelfHostNamespace
        {
            get
            {
                return _selfHostNamespace;
            }
            set
            {
                _selfHostNamespace = value;
                RaisePropertyChanged(() => SelfHostNamespace);
            }
        }
        #endregion
        #region ServicesNamespace
        private string _servicesNamespace;
        public string ServicesNamespace
        {
            get
            {
                return _servicesNamespace;
            }
            set
            {
                _servicesNamespace = value;
                RaisePropertyChanged(() => ServicesNamespace);
            }
        }
        #endregion
        #region TraditionalBridgeNamespace
        private string _traditionalBridgeNamespace;
        public string TraditionalBridgeNamespace
        {
            get
            {
                return _traditionalBridgeNamespace;
            }
            set
            {
                _traditionalBridgeNamespace = value;
                RaisePropertyChanged(() => TraditionalBridgeNamespace);
            }
        }
        #endregion
        #region UnitTestsBaseNamespace
        private string _unitTestsBaseNamespace;
        public string UnitTestsBaseNamespace
        {
            get
            {
                return _unitTestsBaseNamespace;
            }
            set
            {
                _unitTestsBaseNamespace = value;
                RaisePropertyChanged(() => UnitTestsBaseNamespace);
            }
        }
        #endregion
        #region UnitTestsNamespace
        private string _unitTestsNamespace;
        public string UnitTestsNamespace
        {
            get
            {
                return _unitTestsNamespace;
            }
            set
            {
                _unitTestsNamespace = value;
                RaisePropertyChanged(() => UnitTestsNamespace);
            }
        }
        #endregion
    }
}
