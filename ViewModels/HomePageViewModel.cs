using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoderJudicial.Models;

namespace PoderJudicial.ViewModels
{

    public class HomePageViewModel : BaseViewModel
    {
        public int TotalAudienciasMes { get; set; }

        public int TotalEjecucionesMes { get; set; }

        public int TotalCopiasMes { get; set; }

        public int AudienciasHoy { get; set; }

        public ObservableCollection<ActividadReciente>
            Actividades
        { get; set; }
    }


}
