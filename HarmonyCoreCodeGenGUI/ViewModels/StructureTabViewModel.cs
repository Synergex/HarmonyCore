using Microsoft.Toolkit.Mvvm;
using Microsoft.Toolkit.Mvvm.Messaging;
using HarmonyCoreGenerator.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HarmonyCoreCodeGenGUI.ViewModels
{
    public class StructureTabViewModel : ObservableObject
    {
        public StructureTabViewModel(Solution solution)
        {
            RPSMFIL = solution.RPSMFIL;
            RPSTFIL = solution.RPSTFIL;
            RepositoryProject = solution.RepositoryProject;
        }

        public StructureTabViewModel()
        {
            
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
