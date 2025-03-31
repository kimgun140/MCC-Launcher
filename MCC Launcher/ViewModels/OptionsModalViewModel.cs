using DevExpress.Mvvm;

namespace MCC_Launcher.ViewModels
{
    public class OptionsModalViewModel : ViewModelBase
    {
        public  string SelectedOption
            // 선택된 옵션 이름 보여주기 
        {
            get => GetValue<string>();
            set => SetValue(SelectedOption, value);
        }
        // 마이그레이션 옵션 파일, 기존에 남아있던 옵션파일
        // 옵션 파일들 제목 읽어오기
        // 옵션 파일 바인딩 보여주기
        // 선택한 버전의 파일로 실행 옵션파일 바꾸기
        // 실행하기 
    }
}
