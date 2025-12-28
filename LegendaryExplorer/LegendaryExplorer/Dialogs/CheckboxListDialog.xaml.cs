using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LegendaryExplorer.Misc;
using LegendaryExplorer.SharedUI;

namespace LegendaryExplorer.Dialogs
{
    /// <summary>
    /// Generic checkbox list dialog for multi-selection.
    /// </summary>
    public partial class CheckboxListDialog : NotifyPropertyChangedWindowBase
    {
        public class CheckItem : NotifyPropertyChangedBase
        {
            public string Label { get; }

            private bool _isChecked;
            public bool IsChecked
            {
                get => _isChecked;
                set => SetProperty(ref _isChecked, value);
            }

            public CheckItem(string label, bool isChecked)
            {
                Label = label;
                _isChecked = isChecked;
            }
        }

        private CheckboxListDialog(
            Control owner,
            string promptText,
            string titleText,
            IEnumerable items,
            IEnumerable defaultSelected = null,
            bool topMost = false)
        {
            DirectionsText = promptText;
            Topmost = topMost;
            TitleText = titleText;

            DataContext = this;
            LoadCommands();
            InitializeComponent();

            if (owner != null)
            {
                Owner = owner as Window ?? GetWindow(owner);
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            var defaults = new HashSet<string>(
                (defaultSelected ?? Enumerable.Empty<object>()).Cast<object>().Select(o => o?.ToString() ?? ""),
                System.StringComparer.OrdinalIgnoreCase);

            foreach (var o in items.Cast<object>())
            {
                var s = o?.ToString() ?? "";
                Items.Add(new CheckItem(s, defaults.Contains(s)));
            }
        }

        public static List<string> GetSelected(
            Control owner,
            string promptText,
            string titleText,
            IEnumerable items,
            IEnumerable defaultSelected = null,
            bool topMost = false)
        {
            var dlg = new CheckboxListDialog(owner, promptText, titleText, items, defaultSelected, topMost);
            if (dlg.ShowDialog() == true)
            {
                return dlg.SelectedLabels;
            }

            return new List<string>();
        }

        public ObservableCollection<CheckItem> Items { get; } = new();

        public List<string> SelectedLabels =>
            Items.Where(i => i.IsChecked).Select(i => i.Label).ToList();

        public ICommand OKCommand { get; set; }

        private void LoadCommands()
        {
            OKCommand = new GenericCommand(AcceptSelection, CanAcceptSelection);
        }

        private bool CanAcceptSelection()
        {
            return Items.Any(i => i.IsChecked);
        }

        private void AcceptSelection()
        {
            DialogResult = true;
        }

        public string DirectionsText { get; }
        public string TitleText { get; } = @"TITLE NOT SET!";

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var it in Items)
                it.IsChecked = true;
        }

        private void SelectNone_Click(object sender, RoutedEventArgs e)
        {
            foreach (var it in Items)
                it.IsChecked = false;
        }
    }
}
