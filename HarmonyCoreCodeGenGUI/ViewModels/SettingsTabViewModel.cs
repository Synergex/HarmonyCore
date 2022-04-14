using Microsoft.Toolkit.Mvvm;
using Microsoft.Toolkit.Mvvm.Messaging;
using HarmonyCoreGenerator.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HarmonyCoreCodeGenGUI.ViewModels
{
    public class SettingsTabViewModel : ObservableObject
    {
        public SettingsTabViewModel(Solution solution)
        {
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
        }
        public SettingsTabViewModel()
        {
            
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
                SetProperty(ref _enableNewtonsoftJson, value);
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
                SetProperty(ref _signalRPath, value);
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
                SetProperty(ref _controllersFolder, value);
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
                SetProperty(ref _dataFolder, value);
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
                SetProperty(ref _isolatedFolder, value);
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
                SetProperty(ref _modelsFolder, value);
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
                SetProperty(ref _selfHostFolder, value);
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
                SetProperty(ref _servicesFolder, value);
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
                SetProperty(ref _solutionFolder, value);
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
                SetProperty(ref _templatesFolder, value);
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
                SetProperty(ref _traditionalBridgeFolder, value);
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
                SetProperty(ref _unitTestFolder, value);
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
                SetProperty(ref _clientModelsNamespace, value);
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
                SetProperty(ref _controllersNamespace, value);
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
                SetProperty(ref _modelsNamespace, value);
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
                SetProperty(ref _selfHostNamespace, value);
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
                SetProperty(ref _servicesNamespace, value);
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
                SetProperty(ref _traditionalBridgeNamespace, value);
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
                SetProperty(ref _unitTestsBaseNamespace, value);
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
                SetProperty(ref _unitTestsNamespace, value);
            }
        }
        #endregion
    }
}
