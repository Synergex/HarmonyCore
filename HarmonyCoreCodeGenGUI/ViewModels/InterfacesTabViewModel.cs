using CodeGen.Engine;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

using HarmonyCoreGenerator.Model;
using System.Collections.ObjectModel;

namespace HarmonyCoreCodeGenGUI.ViewModels
{
    public class InterfacesTabViewModel : ViewModelBase
    {
        public InterfacesTabViewModel()
        {
            // Initial state
            Messenger.Default.Register<Solution>(this, sender =>
            {
                XFServerSMCPath = sender.TraditionalBridge?.XFServerSMCPath;
                if (sender.TraditionalBridge?.ExtendedInterfaces != null)
                {
                    ExtendedInterfaces.Clear();
                    ExtendedInterfaces.AddRange(sender.TraditionalBridge.ExtendedInterfaces);
                }
            });

            // Send updated state
            Messenger.Default.Register<NotificationMessageAction<InterfacesTabViewModel>>(this, callback => callback.Execute(this));
        }

        #region XFServerSMCPath
        private string _xfServerSMCPath;
        public string XFServerSMCPath
        {
            get
            {
                return _xfServerSMCPath;
            }
            set
            {
                _xfServerSMCPath = value;
                RaisePropertyChanged(() => XFServerSMCPath);
            }
        }
        #endregion
        #region ExtendedInterfaces
        public ObservableCollection<InterfaceEx> ExtendedInterfaces { get; } = new ObservableCollection<InterfaceEx>();
        #endregion
    }
}
