using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using MCC_Launcher.Models;
using MCC_Launcher.Services;
//using Microsoft.VisualBasic.ApplicationServices;
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
        protected IMessageBoxService MessageBoxService => GetService<IMessageBoxService>();

        public Models.User? AuthenticatedUser { get; private set; }
        public class LoginResultContext
        {
            public User? AuthenticatedUser { get; set; }
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


        public async Task OnLogin()
        {
            if (string.IsNullOrWhiteSpace(UserIdInput))
            {
                MessageBoxService.Show("아이디를 입력해주세요");
                return;
            }
            if (string.IsNullOrWhiteSpace(PasswordInput))
            {
                MessageBoxService.Show("비밀번호를 입력해주세요");
                return;
            }
            AuthenticatedUser = await _launcherService.Authenticate(UserIdInput, PasswordInput);

            if (AuthenticatedUser == null)
            {

                MessageBoxService.Show("아이디 또는 비밀번호 입력 오류입니다.", "로그인 실패", MessageBoxButton.OK);
                return;
            }
            if (_context != null)
                _context.AuthenticatedUser = AuthenticatedUser;
            MessageBoxService.ShowMessage($"로그인 성공 \n{AuthenticatedUser.Name}님");

            CurrentDialogService.Close(MessageResult.OK);

        }

    }
}
