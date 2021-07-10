using Microsoft.Toolkit.Mvvm;
using Microsoft.Toolkit.Mvvm.Messaging;
using HarmonyCoreGenerator.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HarmonyCoreCodeGenGUI.ViewModels
{
    public class StructureTabViewModel : ObservableObject
    {
        public StructureTabViewModel()
        {
            // Initial state
            StrongReferenceMessenger.Default.Register<Solution>(this, (obj, sender) => {
                RPSMFIL = sender.RPSMFIL;
                RPSTFIL = sender.RPSTFIL;
                RepositoryProject = sender.RepositoryProject;
            });

            // Send updated state
            StrongReferenceMessenger.Default.Register<NotificationMessageAction<StructureTabViewModel>>(this, (obj, sender) => sender.callback(this));
        }

        #region RPSMFIL
        private string _rpsmfil;
        public string RPSMFIL
        {
            get
            {
                return _rpsmfil;
            }
            set
            {
                SetProperty(ref _rpsmfil, value);
            }
        }
        #endregion
        #region RPSTFIL
        private string _rpstfil;
        public string RPSTFIL
        {
            get
            {
                return _rpstfil;
            }
            set
            {
                SetProperty(ref _rpstfil, value);
            }
        }
        #endregion
        #region RepositoryProject
        private string repositoryProject;
        public string RepositoryProject
        {
            get
            {
                return repositoryProject;
            }
            set
            {
                SetProperty(ref repositoryProject, value);
            }
        }
        #endregion
    }
}
