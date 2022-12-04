using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace STFC_EventLogger
{
    public class AliasAdministration : INotifyPropertyChanged
    {
        #region #- Private Fields -#

        private AliasClass? selectedMember;

        #endregion

        #region #- Constructor -#

        public AliasAdministration()
        {
            Aliase = new();
        }

        #endregion

        #region #- Public Properties -#

        public List<AliasClass> Aliase { get; set; }
        public AliasClass? SelectedMember
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
