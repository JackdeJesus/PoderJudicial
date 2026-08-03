using PoderJudicial.Models;
using System;
using System.Windows.Controls;
using System.Windows;

namespace PoderJudicial.Views
{
    /// <summary>
    /// Host de "Editar Registro de Copias". Aloja una única instancia de
    /// <see cref="CopiasFormControl"/> precargada con el registro
    /// seleccionado (ver CopiasFormControl.CargarParaEditar) — mismo patrón
    /// que EditarRegistro para Audiencias/Ejecución.
    /// </summary>
    public partial class EditarCopias : Page
    {
        private readonly CopiasFormControl _control;

        public EditarCopias(RegistroCopia registro)
        {
            InitializeComponent();

            _control = new CopiasFormControl();
            _control.GuardarClick += Control_GuardarClick;
            PanelFormulario.Children.Add(_control);

            _control.CargarParaEditar(registro);
        }

        private void Control_GuardarClick(object sender, EventArgs e)
        {
            if (!_control.Validar(out string mensajeError))
            {
                MessageBox.Show(mensajeError, "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var registro = _control.ConstruirModelo();
                _control.PersistirModelo(registro);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Registro actualizado correctamente.", "Éxito",
                MessageBoxButton.OK, MessageBoxImage.Information);

            NavigationService?.GoBack();
        }
    }
}
