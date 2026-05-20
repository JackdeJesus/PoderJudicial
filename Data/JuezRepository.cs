using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoderJudicial.Data
{
    public class JuezRepository
    {
        public List<string> ObtenerJueces()
        {
            return new List<string>()
            {
                "Lic. García Ramírez",
                "Lic. Torres Mendoza",
                "Lic. Herrera López"
            };
        }
    }
}