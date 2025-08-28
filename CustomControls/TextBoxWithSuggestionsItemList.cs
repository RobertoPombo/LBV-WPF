using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace LBV_WPF.CustomControls
{    
    public class TextBoxWithSuggestionsItemList : Control
    {
        public ListBox ListBox = new();
        public Popup Popup = new();
        public TextBox TextBox = new();

        static TextBoxWithSuggestionsItemList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxWithSuggestionsItemList), new FrameworkPropertyMetadata(typeof(TextBoxWithSuggestionsItemList)));
        }

        internal void UpdateSuggestionList(ObservableCollection<string> listSuggestions)
        {
            ListBox.Items.Clear();
            if (listSuggestions is not null)
            {
                foreach (string suggestion in listSuggestions)
                {
                    ListBox.Items.Add(suggestion);
                }
            }
            ListBox.Items.Filter = _filter =>
            {
                string filter = (_filter as string ?? string.Empty).ToLower();
                return TextBox.Text.Length < filter.Length && TextBox.Text.ToLower() == filter[..TextBox.Text.Length];
            };
            Popup.IsOpen = ListBox.Items.Count > 0;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Template is not null)
            {
                ListBox = Template.FindName("TextBoxWithSuggestionsItemList_ListBox", this) as ListBox ?? new();
                Popup = Template.FindName("TextBoxWithSuggestionsItemList_Popup", this) as Popup ?? new();
                ListBox.PreviewMouseLeftButtonUp += (s, e) => { OnKey_PreviewMouseLeftButtonRelease(e); };
            }
        }

        private void OnKey_PreviewMouseLeftButtonRelease(MouseButtonEventArgs e)
        {
            TextBlock textBlock = e.OriginalSource as TextBlock ?? new();
            if (textBlock is not null)
            {
                TextBox.Text = textBlock.Text;
                Popup.IsOpen = false;
                e.Handled = true;
            }
        }
    }
}
