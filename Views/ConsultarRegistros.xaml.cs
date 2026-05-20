using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using PoderJudicial.Data;
using PoderJudicial.Models;
using PoderJudicial.ViewModels;
namespace PoderJudicial.Views
{
    public partial class ConsultarRegistros : Page
    {
        private ConsultarRegistrosViewModel _vm;
        private const string Placeholder = "Buscar por causa, NUC, imputado o fecha...";

        public ConsultarRegistros()
        {
            InitializeComponent();
            _vm = new ConsultarRegistrosViewModel();
            DataContext = _vm;

            txtBuscar.Text = Placeholder;
            txtBuscar.Foreground = Brushes.Gray;
        }

        // Placeholder
        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == Placeholder)
            {
                txtBuscar.Text = "";
                txtBuscar.Foreground = Brushes.Black;
            }
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = Placeholder;
                txtBuscar.Foreground = Brushes.Gray;
                _vm.TextoBusqueda = "";
            }
        }

        // Buscador
        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBuscar.Text == Placeholder) return;

            _vm.TextoBusqueda = txtBuscar.Text;

            if (_vm.Sugerencias?.Count > 0)
            {
                lstSugerencias.ItemsSource = _vm.Sugerencias;
                popupSugerencias.IsOpen = true;
            }
            else
            {
                popupSugerencias.IsOpen = false;
            }
        }

        private void txtBuscar_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                popupSugerencias.IsOpen = false;
                return;
            }

            if (e.Key == Key.Down && popupSugerencias.IsOpen && lstSugerencias.Items.Count > 0)
            {
                lstSugerencias.Focus();
                lstSugerencias.SelectedIndex = 0;
                var item = lstSugerencias.ItemContainerGenerator
                    .ContainerFromIndex(0) as ListBoxItem;
                item?.Focus();
            }
        }

        private void lstSugerencias_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lstSugerencias.SelectedItem != null)
                AplicarSugerencia(lstSugerencias.SelectedItem.ToString());
            else if (e.Key == Key.Escape)
            {
                popupSugerencias.IsOpen = false;
                txtBuscar.Focus();
            }
        }

        private void lstSugerencias_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (lstSugerencias.SelectedItem != null)
                AplicarSugerencia(lstSugerencias.SelectedItem.ToString());
        }

        private void AplicarSugerencia(string valor)
        {
            txtBuscar.Text = valor;
            txtBuscar.Foreground = Brushes.Black;
            _vm.TextoBusqueda = valor;
            popupSugerencias.IsOpen = false;
            txtBuscar.CaretIndex = txtBuscar.Text.Length;
            txtBuscar.Focus();
        }
    }
}