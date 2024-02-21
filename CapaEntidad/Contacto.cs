using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Contacto
    {
        public int idContacto { get; set; }
        public string nombre { get; set; }
        public string nick { get; set; }
        public string apellidos { get; set; }
        public string empresa { get; set; }
        public string tfono { get; set; }
        public string tfono2 { get; set; }
        public DateTime cumple { get; set; }
        public string  direccion { get; set; }
        public int tipoContactoNum {  get; set; }
        public string tipoContacto { get; set; }
        public Tipo tipo { get; set; }
        public string notas { get; set; }
        public bool estado { get; set; }      
    }   
}
