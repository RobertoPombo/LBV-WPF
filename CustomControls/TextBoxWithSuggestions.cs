using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LBV_WPF.CustomControls
{
    public class TextBoxWithSuggestions : Control
    {
        public static readonly DependencyProperty PropertySuggestionList = DependencyProperty.Register("SuggestionList", typeof(ObservableCollection<string>), typeof(TextBoxWithSuggestions), new PropertyMetadata(default));
        public TextBox TextBox = new();
        public TextBoxWithSuggestionsItemList ItemList = new();

        private int indexSelectedListBoxItem = -1;
        public int IndexSelectedListBoxItem
        {
            get { return indexSelectedListBoxItem; }
            set
            {
                indexSelectedListBoxItem = value;
                ItemList.ListBox.SelectedIndex = indexSelectedListBoxItem;
            }
        }

        static TextBoxWithSuggestions()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxWithSuggestions), new FrameworkPropertyMetadata(typeof(TextBoxWithSuggestions)));
        }

        public ObservableCollection<string> SuggestionList
        {
            get { return (ObservableCollection<string>)GetValue(PropertySuggestionList); }
            set { SetValue(PropertySuggestionList, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Template is not null)
            {
                TextBox = Template.FindName("TextBoxWithSuggestions_TextBox", this) as TextBox ?? new();
                ItemList = Template.FindName("TextBoxWithSuggestions_ItemList", this) as TextBoxWithSuggestionsItemList ?? new();
                if (TextBox is not null)
                {
                    TextBox.PreviewKeyDown += (s, e) => { OnKey_DownUpPress(e); };
                    TextBox.KeyDown += (s, e) => { OnKey_EnterPress(e); };
                    TextBox.GotFocus += (s, e) => { DoOpenClosePopup(true); };
                    TextBox.LostFocus += (s, e) => { DoOpenClosePopup(false); };
                    TextBox.TextChanged += (s, e) => { ItemList.UpdateSuggestionList(SuggestionList); };
                    if (ItemList is not null)
                    {
                        ItemList.TextBox = TextBox;
                    }
                }
            }
        }

        private void DoOpenClosePopup(bool doOpen)
        {
            if (doOpen) { ItemList.UpdateSuggestionList(SuggestionList); }
            if (ItemList.ListBox.Items.Count > 0)
            {
                ItemList.Popup.IsOpen = doOpen;
                IndexSelectedListBoxItem = -1;
            }
        }

        private void OnKey_EnterPress(KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ItemList.ListBox.SelectedIndex >= 0)
            {
                ListBoxItem listBoxItem = ItemList.ListBox.ItemContainerGenerator.ContainerFromIndex(ItemList.ListBox.SelectedIndex) as ListBoxItem ?? new();
                TextBox.Text = listBoxItem.Content as string;
                ItemList.Popup.IsOpen = false;
                IndexSelectedListBoxItem = -1;
                TextBox.Focus();
                e.Handled = true;
            }
        }

        private void OnKey_DownUpPress(KeyEventArgs e)
        {
            if ((e.Key == Key.Down || e.Key == Key.Up) && ItemList.ListBox.Items.Count > 0 && e.OriginalSource is not ListBoxItem)
            {
                if (e.Key == Key.Down && IndexSelectedListBoxItem < ItemList.ListBox.Items.Count - 1) { IndexSelectedListBoxItem += 1; }
                else if (e.Key == Key.Up && IndexSelectedListBoxItem > 0) { IndexSelectedListBoxItem -= 1; }
                else { IndexSelectedListBoxItem = 0; }
                e.Handled = true;
            }
        }
    }
}
