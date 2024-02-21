using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CapaNegocio
{
    public class CN_Contacto
    {
        private CD_Contacto objcd_contacto = new CD_Contacto();

        public List<Contacto> ListarXnombre(String nombre)
        {
            return objcd_contacto.ListarXnombre(nombre);
        }

        public List<Contacto> ListarXestado(String estado)
        {
            return objcd_contacto.ListarXestado(estado);
        }

        public int Registrar(Contacto contacto, out String Mensaje)
        {
            Mensaje = string.Empty;


            if (contacto.nombre == "")
            {
                Mensaje += "Es necesario el nombre del contacto\n";
            }
            if (contacto.tfono2 != "")
            {
                string patron = @"^\d{9}$";
                if (!(Regex.IsMatch(contacto.tfono2, patron)))
                {
                    Mensaje += "Número de teléfono2 no valido\n";
                }
            }
            if (contacto.tfono != "")
            {
                string patron = @"^\d{9}$";
                //comprobar cumple patron
                if (Regex.IsMatch(contacto.tfono, patron))
                {
                    string nombrePropietario = objcd_contacto.buscarTfono(contacto.tfono, out String Mensaje2);
                    //si el telefono esta ocupado
                    if (nombrePropietario != null)
                    {
                        //usuario activo tiene el numero de telefono
                        if (objcd_contacto.ObtenerEstadoContacto(nombrePropietario, out String Mensaje3))
                        {
                            Mensaje += "El número de teléfono introducido ya está siendo utilizado\nPuedes introducirlo como teléfono2";
                        }
                        //usuario inactivo tiene el numero de telefono
                        else
                        {
                            return -1;
                        }
                    }
                   

                }

                else
                {
                    Mensaje += "Número de teléfono no valido\n";
                }

            }
           

            if (Mensaje != string.Empty)
            {
                return 0;
            }
            else
            {
                return objcd_contacto.Registrar(contacto, out Mensaje);
            }
        }

        public bool Editar(Contacto contacto, out string Mensaje)
        {
            Mensaje = string.Empty;
            string patron = @"^\d{9}$";

            if (contacto.nombre == "")
            {
                Mensaje += "Es necesario el nombre del contacto\n";
            }
            if (contacto.tfono != "")
            {
                //comprobar cumple patron
                if (!Regex.IsMatch(contacto.tfono, patron))
                {
                    Mensaje += "Número de teléfono no valido\n";
                }
            }
            if (contacto.tfono2 != "")
            {
                if (!(Regex.IsMatch(contacto.tfono2, patron)))
                {
                    Mensaje += "Número de teléfono2 no valido\n";
                }
            }

            if (Mensaje != string.Empty)
            {
                return false;
            }
            else
            {
                return objcd_contacto.Editar(contacto, out Mensaje);
            }

        }

        public String buscarNombrePropietario(String tfono, out String Mensaje)
        {
            Mensaje = string.Empty;
            string nombrePropietario = objcd_contacto.buscarTfono(tfono, out String Mensaje4);
            return nombrePropietario;
        }

        public bool activarContacto(string nombreContacto, out String Mensaje)
        {
            return objcd_contacto.modificarEstado(nombreContacto, out Mensaje);
        }

        public bool ActualizarTelefonoYAgregarANotas(string nombreContacto, string telefono, out string mensaje)
        {
            return objcd_contacto.insertarTelefonoenNotas(nombreContacto, telefono, out mensaje);

        }


    }
}
