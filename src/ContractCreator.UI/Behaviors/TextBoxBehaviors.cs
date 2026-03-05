namespace ContractCreator.UI.Behaviors
{
    public static class TextBoxBehaviors
    {
        public static readonly AttachedProperty<bool> IsNumericOnlyProperty =
            AvaloniaProperty.RegisterAttached<TextBox, bool>("IsNumericOnly", typeof(TextBoxBehaviors));

        public static bool GetIsNumericOnly(AvaloniaObject element) => element.GetValue(IsNumericOnlyProperty);
        public static void SetIsNumericOnly(AvaloniaObject element, bool value) => element.SetValue(IsNumericOnlyProperty, value);

        static TextBoxBehaviors()
        {
            IsNumericOnlyProperty.Changed.AddClassHandler<TextBox>(OnIsNumericOnlyChanged);
        }

        private static void OnIsNumericOnlyChanged(TextBox textBox, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is true)
            {
                textBox.TextInput += OnTextInput; // Перехватываем ввод с клавиатуры
                textBox.PastingFromClipboard += OnPastingFromClipboard; // Перехватываем вставку из буфера обмена
            }
            else
            {
                textBox.TextInput -= OnTextInput;
                textBox.PastingFromClipboard -= OnPastingFromClipboard;
            }
        }

        private static void OnTextInput(object? sender, TextInputEventArgs e)
        {
            if (e.Text != null && !e.Text.All(char.IsDigit))
                e.Handled = true;
        }

        private static async void OnPastingFromClipboard(object? sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            e.Handled = true;

            var clipboard = TopLevel.GetTopLevel(textBox)?.Clipboard;
            if (clipboard == null) return;

            var text = await clipboard.TryGetTextAsync();

            if (string.IsNullOrEmpty(text)) return;

            var digitsOnly = new string(text.Where(char.IsDigit).ToArray());

            if (!string.IsNullOrEmpty(digitsOnly))
            {
                var currentText = textBox.Text ?? string.Empty;
                var caretIndex = textBox.CaretIndex;

                var newText = currentText.Insert(caretIndex, digitsOnly);
                if (textBox.MaxLength > 0 && newText.Length > textBox.MaxLength)
                    newText = newText.Substring(0, textBox.MaxLength);

                textBox.Text = newText;
                textBox.CaretIndex = caretIndex + digitsOnly.Length;
            }
        }
    }
}
