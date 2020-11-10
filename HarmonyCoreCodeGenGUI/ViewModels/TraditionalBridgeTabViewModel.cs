using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HarmonyCoreGenerator.Model;

namespace HarmonyCoreCodeGenGUI.ViewModels
{
    public class TraditionalBridgeTabViewModel : ViewModelBase
    {
        public TraditionalBridgeTabViewModel()
        {
            // Initial state
            Messenger.Default.Register<Solution>(this, sender => {
                ControllersProject = sender.ControllersProject;
                IsolatedProject = sender.IsolatedProject;
                ModelsProject = sender.ModelsProject;
                SelfHostProject = sender.SelfHostProject;
                ServicesProject = sender.ServicesProject;
                TraditionalBridgeProject = sender.TraditionalBridgeProject;
                UnitTestProject = sender.UnitTestProject;

                EnableOptionalParameters = sender.TraditionalBridge?.EnableOptionalParameters;
                EnableSampleDispatchers = sender.TraditionalBridge?.EnableSampleDispatchers;
                EnableXFServerPlusMigration = sender.TraditionalBridge?.EnableXFServerPlusMigration;
            });

            // Send updated state
            Messenger.Default.Register<NotificationMessageAction<TraditionalBridgeTabViewModel>>(this, callback => callback.Execute(this));
        }

        #region ControllersProject
        private string _controllersProject;
        public string ControllersProject
        {
            get
            {
                return _controllersProject;
            }
            set
            {
                _controllersProject = value;
                RaisePropertyChanged(() => ControllersProject);
            }
        }
        #endregion
        #region IsolatedProject
        private string _isolatedProject;
        public string IsolatedProject
        {
            get
            {
                return _isolatedProject;
            }
            set
            {
                _isolatedProject = value;
                RaisePropertyChanged(() => IsolatedProject);
            }
        }
        #endregion
        #region ModelsProject
        private string _modelsProject;
        public string ModelsProject
        {
            get
            {
                return _modelsProject;
            }
            set
            {
                _modelsProject = value;
                RaisePropertyChanged(() => ModelsProject);
            }
        }
        #endregion
        #region SelfHostProject
        private string _selfHostProject;
        public string SelfHostProject
        {
            get
            {
                return _selfHostProject;
            }
            set
            {
                _selfHostProject = value;
                RaisePropertyChanged(() => SelfHostProject);
            }
        }
        #endregion
        #region ServicesProject
        private string _servicesProject;
        public string ServicesProject
        {
            get
            {
                return _servicesProject;
            }
            set
            {
                _servicesProject = value;
                RaisePropertyChanged(() => ServicesProject);
            }
        }
        #endregion
        #region TraditionalBridgeProject
        private string _traditionalBridgeProject;
        public string TraditionalBridgeProject
        {
            get
            {
                return _traditionalBridgeProject;
            }
            set
            {
                _traditionalBridgeProject = value;
                RaisePropertyChanged(() => TraditionalBridgeProject);
            }
        }
        #endregion
        #region UnitTestProject
        private string _unitTestProject;
        public string UnitTestProject
        {
            get
            {
                return _unitTestProject;
            }
            set
            {
                _unitTestProject = value;
                RaisePropertyChanged(() => UnitTestProject);
            }
        }
        #endregion

        #region EnableOptionalParameters
        private bool? _enableOptionalParameters;
        public bool? EnableOptionalParameters
        {
            get
            {
                return _enableOptionalParameters;
            }
            set
            {
                _enableOptionalParameters = value;
                RaisePropertyChanged(() => EnableOptionalParameters);
            }
        }
        #endregion
        #region EnableSampleDispatchers
        private bool? _enableSampleDispatchers;
        public bool? EnableSampleDispatchers
        {
            get
            {
                return _enableSampleDispatchers;
            }
            set
            {
                _enableSampleDispatchers = value;
                RaisePropertyChanged(() => EnableSampleDispatchers);
            }
        }
        #endregion
        #region EnableXFServerPlusMigration
        private bool? _enableXFServerPlusMigration;
        public bool? EnableXFServerPlusMigration
        {
            get
            {
                return _enableXFServerPlusMigration;
            }
            set
            {
                _enableXFServerPlusMigration = value;
                RaisePropertyChanged(() => EnableXFServerPlusMigration);
            }
        }
        #endregion
    }
}
