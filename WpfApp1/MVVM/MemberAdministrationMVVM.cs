using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace STFC_EventLogger.MVVM
{
    public class MemberAdministrationMVVM : INotifyPropertyChanged
    {
        #region #- Private Fields -#

        private MemberAdministrationAlias? selectedMember;

        #endregion

        #region #- Constructor -#

        public MemberAdministrationMVVM()
        {
            Members = new();
        }

        #endregion

        #region #- Public Properties -#

        public List<MemberAdministrationAlias> Members { get; set; }
        public MemberAdministrationAlias? SelectedMember
        {
            get
            {
                return selectedMember;
            }
            set
            {
                selectedMember = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region #- Events -#

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region #- Instance Methods -#



        #endregion

        #region #- Static Methods -#



        #endregion

        #region #- Interface/Overridden Methods -#



        #endregion

        #region #- Operators -#



        #endregion
    }
}
