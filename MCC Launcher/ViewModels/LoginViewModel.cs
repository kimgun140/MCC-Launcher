using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using MCC_Launcher.Models;
using MCC_Launcher.Services;
using Microsoft.VisualBasic.ApplicationServices;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MCC_Launcher.ViewModels
{
    public class LoginViewModel : ViewModelBase, ISupportParameter
    {
        LauncherService _launcherService;
        public string UserIdInput
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string PasswordInput
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        protected ICurrentDialogService CurrentDialogService => GetService<ICurrentDialogService>();

        public UserInfo? AuthenticatedUser { get; private set; }
        public class LoginResultContext
        {
            public UserInfo? AuthenticatedUser { get; set; }
        }


        private LoginResultContext _context;

        public object Parameter
        {
            get => null!;
            set
            {
                if (value is LoginResultContext context)
                    _context = context;
            }
        }
        public AsyncCommand LoginCommand { get; set; }
        public DelegateCommand<KeyEventArgs> EnterKeyCommand { get; }


        private void OnEnterKeyPressed(KeyEventArgs e)
            // 엔터 입력 
        {
            if (e.Key == Key.Enter)
            {
                LoginCommand.Execute(null);
            }
        }

        public LoginViewModel()
        {
            _launcherService = new LauncherService();
            UserIdInput = string.Empty;
            PasswordInput = string.Empty;
            LoginCommand = new AsyncCommand(OnLogin);
            {
                EnterKeyCommand = new DelegateCommand<KeyEventArgs>(OnEnterKeyPressed);
                //async
            }
        }

        protected IMessageBoxService MessageBoxService => GetService<IMessageBoxService>();

        public async Task OnLogin()
        {


            AuthenticatedUser = await _launcherService.Authenticate(UserIdInput, PasswordInput);

            if (AuthenticatedUser == null)
            {

                MessageBox.Show("로그인 실패", "로그인", MessageBoxButton.OK, MessageBoxImage.Error);
                //실패 
                //DialogService.Equals(null);
                return;
            }
            if (_context != null)
                _context.AuthenticatedUser = AuthenticatedUser;

            CurrentDialogService.Close(MessageResult.OK);

        }

    }
}
