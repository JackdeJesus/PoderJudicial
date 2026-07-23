using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PoderJudicial.Views
{
    /// <summary>
    /// Host de "Nuevo Registro". Administra una cadena de 1 a 7
    /// <see cref="AudienciaFormControl"/> (Audiencias Concentradas):
    /// agrega/quita formularios, valida todos antes de guardar y persiste
    /// cada uno como un registro independiente en la base de datos.
    /// </summary>
    public partial class NuevoRegistro : Page
    {
        private const int MaximoFormularios = 7;

        private readonly List<AudienciaFormControl> _formularios = new();

        public NuevoRegistro()
        {
            InitializeComponent();
            AgregarFormulario();
        }

        // ══════════════════════════════════════════════
        //  ALTA / BAJA DE FORMULARIOS (Concentrada)
        // ══════════════════════════════════════════════
        private void AgregarFormulario()
        {
            var control = new AudienciaFormControl();
            control.ConcentradaClick += Formulario_ConcentradaClick;
            control.GuardarClick += Formulario_GuardarClick;

            _formularios.Add(control);
            PanelFormularios.Children.Add(control);

            ActualizarNumeracionYBotones();
        }

        private void Formulario_ConcentradaClick(object sender, EventArgs e)
        {
            if (_formularios.Count >= MaximoFormularios) return;
            AgregarFormulario();
        }

        /// <summary>
        /// Renumera los formularios ("Audiencia concentrada #N") y solo deja
        /// visible el botón "Concentrada" en el último, ocultándolo por
        /// completo al llegar al máximo permitido.
        /// </summary>
        private void ActualizarNumeracionYBotones()
        {
            for (int i = 0; i < _formularios.Count; i++)
            {
                _formularios[i].EstablecerNumero(i + 1);

                bool esUltimo = i == _formularios.Count - 1;
                bool puedeAgregarMas = _formularios.Count < MaximoFormularios;
                _formularios[i].MostrarBotonConcentrada(esUltimo && puedeAgregarMas);
            }
        }

        // ══════════════════════════════════════════════
        //  GUARDADO CONJUNTO
        // ══════════════════════════════════════════════
        private void Formulario_GuardarClick(object sender, EventArgs e)
        {
            // 1) Validar TODOS los formularios antes de guardar cualquiera.
            for (int i = 0; i < _formularios.Count; i++)
            {
                if (!_formularios[i].Validar(out string mensajeError))
                {
                    string prefijo = _formularios.Count > 1 ? $"Formulario {i + 1}: " : string.Empty;
                    MessageBox.Show(prefijo + mensajeError, "Validación",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            // 2) Calcular folios (Id) secuenciales en memoria: como ninguno
            //    de los formularios concentrados se ha insertado aún, cada
            //    uno pediría el mismo "siguiente Id" a la base de datos si
            //    se le preguntara por separado. Se reserva un contador por
            //    tabla destino (Audiencia vs Ejecución) y se incrementa a
            //    medida que se recorren los formularios, en el mismo orden
            //    en que se van a guardar.
            var siguienteIdPorTabla = new Dictionary<string, int>();
            bool esConcentrada = _formularios.Count > 1;
            var modelos = new List<object>();

            foreach (var formulario in _formularios)
            {
                string tabla = formulario.TipoCausaActual == "EXP" ? "EXP" : "AUD";

                if (!siguienteIdPorTabla.ContainsKey(tabla))
                    siguienteIdPorTabla[tabla] = formulario.VM.ObtenerSiguienteId(formulario.TipoCausaActual);

                int id = siguienteIdPorTabla[tabla]++;

                modelos.Add(formulario.ConstruirModelo(id, esConcentrada));
            }

            // 3) Persistir. Cada formulario concentrado es un registro
            //    independiente (no se combinan en una sola fila).
            try
            {
                for (int i = 0; i < _formularios.Count; i++)
                    _formularios[i].PersistirModelo(modelos[i]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ocurrió un error al guardar y el proceso se detuvo. " +
                    $"Verifique los registros ya guardados antes de reintentar.\n\n{ex.Message}",
                    "Error al guardar", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show(
                esConcentrada
                    ? $"Se guardaron {_formularios.Count} audiencias concentradas correctamente."
                    : "Registro guardado correctamente.",
                "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

            ReiniciarCadena();
        }

        /// <summary>Tras guardar, vuelve a dejar un único formulario limpio.</summary>
        private void ReiniciarCadena()
        {
            foreach (var formulario in _formularios.Skip(1))
                formulario.DetenerReloj();

            PanelFormularios.Children.Clear();
            _formularios.Clear();

            AgregarFormulario();
        }
    }
}