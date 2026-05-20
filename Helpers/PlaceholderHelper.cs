using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace PoderJudicial.Helpers
{
    public static class PlaceholderHelper
    {
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

            tb.Foreground =
                new SolidColorBrush(
                    Color.FromRgb(0x9C, 0xA3, 0xAF));

            tb.Text = ph;

            tb.GotFocus += (s, e) =>
            {
                if (tb.Text == ph)
                {
                    tb.Text = "";

                    tb.Foreground =
                        new SolidColorBrush(
                            Color.FromRgb(0x37, 0x41, 0x51));
                }
            };

            tb.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Text = ph;

                    tb.Foreground =
                        new SolidColorBrush(
                            Color.FromRgb(0x9C, 0xA3, 0xAF));
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