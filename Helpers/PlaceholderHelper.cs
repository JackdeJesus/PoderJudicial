using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace PoderJudicial.Helpers
{
    public static class PlaceholderHelper
    {
        // COLOR PLACEHOLDER
        private static readonly Brush PlaceholderBrush =
            new SolidColorBrush(
                Color.FromRgb(0x9C, 0xA3, 0xAF));

        // COLOR TEXTO NORMAL (MODO OSCURO)
        private static readonly Brush NormalBrush =
            Brushes.White;

        // AGREGAR PLACEHOLDER
        public static void AddPlaceholder(
            TextBox tb,
            string? placeholder = null)
        {
            string ph =
                placeholder ??
                (tb.Tag as string ?? "");

            if (string.IsNullOrEmpty(ph))
                return;

            // Placeholder inicial
            tb.Foreground = PlaceholderBrush;
            tb.Text = ph;

            tb.GotFocus += (s, e) =>
            {
                if (tb.Text == ph)
                {
                    tb.Text = "";
                    tb.Foreground = NormalBrush;
                }
            };

            tb.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Text = ph;
                    tb.Foreground = PlaceholderBrush;
                }
                else
                {
                    tb.Foreground = NormalBrush;
                }
            };

            // Cuando escriba texto
            tb.TextChanged += (s, e) =>
            {
                if (tb.Text != ph &&
                    !string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Foreground = NormalBrush;
                }
            };
        }

        // OBTENER TEXTO REAL
        public static string GetText(TextBox tb)
        {
            string ph =
                tb.Tag as string ?? "";

            string val =
                tb.Text.Trim();

            return val == ph
                ? ""
                : val;
        }

        // VALIDAR SI ES PLACEHOLDER
        public static bool IsPlaceholder(TextBox tb)
        {
            string ph =
                tb.Tag as string ?? "";

            return tb.Text == ph;
        }
    }
}