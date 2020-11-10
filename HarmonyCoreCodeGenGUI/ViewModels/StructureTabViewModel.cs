using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HarmonyCoreGenerator.Model;

namespace HarmonyCoreCodeGenGUI.ViewModels
{
    public class StructureTabViewModel : ViewModelBase
    {
        public StructureTabViewModel()
        {
            // Initial state
            Messenger.Default.Register<Solution>(this, sender => {
                RPSMFIL = sender.RPSMFIL;
                RPSTFIL = sender.RPSTFIL;
                RepositoryProject = sender.RepositoryProject;
            });

            // Send updated state
            Messenger.Default.Register<NotificationMessageAction<StructureTabViewModel>>(this, callback => callback.Execute(this));
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
                _rpsmfil = value;
                RaisePropertyChanged(() => RPSMFIL);
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
                _rpstfil = value;
                RaisePropertyChanged(() => RPSTFIL);
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
                repositoryProject = value;
                RaisePropertyChanged(() => RepositoryProject);
            }
        }
        #endregion
    }
}
