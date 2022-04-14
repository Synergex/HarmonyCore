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
        public InterfacesTabViewModel(Solution solution)
        {
            XFServerSMCPath = solution.TraditionalBridge?.XFServerSMCPath;
            if (solution.ExtendedInterfaces != null)
            {
                ExtendedInterfaces.Clear();
                ExtendedInterfaces.AddRange(solution.ExtendedInterfaces);
            }
        }
        public InterfacesTabViewModel()
        {
            
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
