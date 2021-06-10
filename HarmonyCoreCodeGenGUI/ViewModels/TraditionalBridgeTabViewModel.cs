using Microsoft.Toolkit.Mvvm;
using Microsoft.Toolkit.Mvvm.Messaging;
using HarmonyCoreGenerator.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HarmonyCoreCodeGenGUI.ViewModels
{
    public class TraditionalBridgeTabViewModel : ObservableObject
    {
        public TraditionalBridgeTabViewModel()
        {
            // Initial state
            StrongReferenceMessenger.Default.Register<Solution>(this, (obj, sender) => {
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
            StrongReferenceMessenger.Default.Register<NotificationMessageAction<TraditionalBridgeTabViewModel>>(this, (obj, sender) => sender.callback(this));
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
                SetProperty(ref _controllersProject, value);
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
                SetProperty(ref _isolatedProject, value);
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
                SetProperty(ref _modelsProject, value);
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
                SetProperty(ref _selfHostProject, value);
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
                SetProperty(ref _servicesProject, value);
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
                SetProperty(ref _traditionalBridgeProject, value);
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
                SetProperty(ref _unitTestProject, value);
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
                SetProperty(ref _enableOptionalParameters, value);
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
                SetProperty(ref _enableSampleDispatchers, value);
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
                SetProperty(ref _enableXFServerPlusMigration, value);
            }
        }
        #endregion
    }
}
