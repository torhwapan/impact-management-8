using System.Windows.Input;
using ImpactManagement.Commands;

namespace ImpactManagement.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object? _currentView;

        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public DbConnectManageViewModel DbConnectManageVM { get; }
        public ImpactManageViewModel ImpactManageVM { get; }
        public ActionManageViewModel ActionManageVM { get; }
        public ImpactExecuteViewModel ImpactExecuteVM { get; }

        public ICommand ShowDbConnectCommand { get; }
        public ICommand ShowImpactManageCommand { get; }
        public ICommand ShowActionManageCommand { get; }
        public ICommand ShowImpactExecuteCommand { get; }

        public MainViewModel()
        {
            // Initialize all ViewModels
            DbConnectManageVM = new DbConnectManageViewModel();
            ImpactManageVM = new ImpactManageViewModel();
            ActionManageVM = new ActionManageViewModel();
            ImpactExecuteVM = new ImpactExecuteViewModel();

            // Initialize commands
            ShowDbConnectCommand = new RelayCommand(_ => CurrentView = DbConnectManageVM);
            ShowImpactManageCommand = new RelayCommand(_ => CurrentView = ImpactManageVM);
            ShowActionManageCommand = new RelayCommand(_ => CurrentView = ActionManageVM);
            ShowImpactExecuteCommand = new RelayCommand(_ => CurrentView = ImpactExecuteVM);

            // Set default view
            CurrentView = DbConnectManageVM;
        }
    }
}
