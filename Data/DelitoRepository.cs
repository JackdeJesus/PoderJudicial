using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoderJudicial.Data
{
    public class DelitoRepository
    {
        public List<string> ObtenerDelitos()
        {
            return new List<string>()
            {
                "Robo",
                "Fraude",
                "Homicidio",
                "Violencia Familiar",
                "Narcomenudeo"
            };
        }
    }
}
