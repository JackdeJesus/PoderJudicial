using System.Windows;
using System.Windows.Input;

namespace PoderJudicial.Views
{
    public partial class VerDetalleEjecucion : Window
    {
        public VerDetalleEjecucion()
        {
            InitializeComponent();
        }

        public void CargarDatos(
            string id, string expediente, string causa, string fechaAudiencia,
            string tipoAudiencia, string horaTermino, string juez, string sala,
            string imputado, string delito, string victima, string totalDiscos,
            string observaciones)
        {
            TxtID.Text = id;
            TxtExpediente.Text = expediente;
            TxtCausa.Text = causa;
            TxtFechaAudiencia.Text = fechaAudiencia;
            TxtTipoAudiencia.Text = tipoAudiencia;
            TxtHoraTermino.Text = horaTermino;
            TxtJuez.Text = juez;
            TxtSala.Text = sala;
            TxtImputado.Text = imputado;
            TxtDelito.Text = delito;
            TxtVictima.Text = victima;
            TxtTotalDiscos.Text = totalDiscos;
            TxtObservaciones.Text = observaciones;
        }

        // Permite arrastrar la ventana desde cualquier parte
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
