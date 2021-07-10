using CodeGen.Engine;
using Microsoft.Toolkit.Mvvm;
using Microsoft.Toolkit.Mvvm.Messaging;

using HarmonyCoreGenerator.Model;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HarmonyCoreCodeGenGUI.ViewModels
{
    public class InterfacesTabViewModel : ObservableObject
    {
        public InterfacesTabViewModel()
        {
            // Initial state
            StrongReferenceMessenger.Default.Register<Solution>(this, (obj, sender) =>
            {
                XFServerSMCPath = sender.TraditionalBridge?.XFServerSMCPath;
                if (sender.TraditionalBridge?.ExtendedInterfaces != null)
                {
                    ExtendedInterfaces.Clear();
                    ExtendedInterfaces.AddRange(sender.TraditionalBridge.ExtendedInterfaces);
                }
            });

            // Send updated state
            StrongReferenceMessenger.Default.Register<NotificationMessageAction<InterfacesTabViewModel>>(this, (obj, sender) => sender.callback(this));
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
                SetProperty(ref _xfServerSMCPath, value);
            }
        }
        #endregion
        #region ExtendedInterfaces
        public ObservableCollection<InterfaceEx> ExtendedInterfaces { get; } = new ObservableCollection<InterfaceEx>();
        #endregion
    }
}
