using System.Windows.Controls;

namespace OnlyIn
{
    public class DebugBuild : ContentControl
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

#if !DEBUG
            this.Content = null;
#endif
        }

    }
}