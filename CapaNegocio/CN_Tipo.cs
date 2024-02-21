using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Tipo
    {
        private CD_Tipo objcd_tipo = new CD_Tipo();

        public List<Tipo> Listar()
        {
            return objcd_tipo.Listar();
        }
    }

}
