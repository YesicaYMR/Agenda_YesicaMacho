using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Contacto
    {

        public List<Contacto> ListarXnombre (String nombre)
        {
            List<Contacto> lista = new List<Contacto>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select c.IdContacto, c.nombre, c.nick, c.apellidos, c.empresa, c.telefono, c.telefono2, c.cumpleanos,c.direccion,t.nombre_tipo,c.notas,c.estado from Contacto c");
                    query.AppendLine("inner join Tipo_Contacto t on t.idTipo = c.idTipoContacto ");

                    if(nombre != null)
                    {
                        query.AppendLine("where c.nombre = '"+nombre+"'");
                    }
  

                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.CommandType = System.Data.CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Contacto()
                            {
                               // idContacto = Convert.ToInt32(dr["idContacto"]),
                                nombre = dr["nombre"].ToString(),
                                nick = dr["nick"].ToString(),
                                apellidos = dr["apellidos"].ToString(),
                                empresa = dr["empresa"].ToString(),
                                tfono = dr["telefono"].ToString(),
                                tfono2 = dr["telefono2"].ToString(),
                                cumple = dr.GetDateTime(7),
                                //cumple = Convert.ToDateTime(dr["cumpleanos"]),
                                direccion = dr["direccion"].ToString(),
                                tipoContacto = dr["nombre_tipo"].ToString(),
                                //tipo = new Tipo() { idTipo = Convert.ToInt32(dr["idTipo"]), nombre = dr["nombre_tipo"].ToString() },
                                notas = dr["notas"].ToString(),
                                estado = Convert.ToBoolean(dr["estado"])                                
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    lista = new List<Contacto>();
                }
                return lista;
            }
        }

        public List<Contacto> ListarXestado(String estado)
        {
            List<Contacto> lista = new List<Contacto>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select c.IdContacto, c.nombre, c.nick, c.apellidos, c.empresa, c.telefono, c.telefono2, c.cumpleanos,c.direccion,t.nombre_tipo,c.notas,c.estado from Contacto c");
                    query.AppendLine("inner join Tipo_Contacto t on t.idTipo = c.idTipoContacto ");

                    if (estado != null)
                    {
                        query.AppendLine("where c.estado = '" + estado+"'");
                    }


                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.CommandType = System.Data.CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Contacto()
                            {
                                idContacto = Convert.ToInt32(dr["idContacto"]),
                                nombre = dr["nombre"].ToString(),
                                nick = dr["nick"].ToString(),
                                apellidos = dr["apellidos"].ToString(),
                                empresa = dr["empresa"].ToString(),
                                tfono = dr["telefono"].ToString(),
                                tfono2 = dr["telefono2"].ToString(),
                                cumple = dr.GetDateTime(7).Date,
                                //cumple = Convert.ToDateTime(dr["cumpleanos"]),
                                direccion = dr["direccion"].ToString(),
                                tipoContacto = dr["nombre_tipo"].ToString(),
                                //tipo = new Tipo() { idTipo = Convert.ToInt32(dr["idTipo"]), nombre = dr["nombre_tipo"].ToString() },
                                notas = dr["notas"].ToString(),
                                estado = Convert.ToBoolean(dr["estado"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    lista = new List<Contacto>();
                }
                return lista;
            }
        }

        public int Registrar(Contacto contacto, out string Mensaje)
        {
            int idusuariogenerado = 0;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_INSERTAR_CONTACTO", oconexion);
                    cmd.Parameters.AddWithValue("nombre", contacto.nombre);
                    cmd.Parameters.AddWithValue("nick", contacto.nick);
                    cmd.Parameters.AddWithValue("apellidos", contacto.apellidos);
                    cmd.Parameters.AddWithValue("empresa", contacto.empresa);
                    cmd.Parameters.AddWithValue("telefono", contacto.tfono);
                    cmd.Parameters.AddWithValue("telefono2", contacto.tfono2);
                    cmd.Parameters.AddWithValue("cumpleanos", contacto.cumple);
                    cmd.Parameters.AddWithValue("direccion", contacto.direccion);
                    cmd.Parameters.AddWithValue("notas", contacto.notas);
                    cmd.Parameters.AddWithValue("idTipoContacto", contacto.tipoContactoNum);
                    cmd.Parameters.AddWithValue("estado", contacto.estado);
                    cmd.Parameters.Add("IdUsuarioResultado", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;
                    //cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    idusuariogenerado = Convert.ToInt32(cmd.Parameters["IdUsuarioResultado"].Value);
                    //Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                idusuariogenerado = 0;
                Mensaje = ex.Message;
            }
            return idusuariogenerado;
        }


        public bool Editar(Contacto contacto, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_EDITARUSUARIO", oconexion);
                    cmd.Parameters.AddWithValue("idContacto", contacto.idContacto);
                    cmd.Parameters.AddWithValue("nombre", contacto.nombre);
                    cmd.Parameters.AddWithValue("nick", contacto.nick);
                    cmd.Parameters.AddWithValue("apellidos", contacto.apellidos);
                    cmd.Parameters.AddWithValue("empresa", contacto.empresa);
                    cmd.Parameters.AddWithValue("telefono", contacto.tfono);
                    cmd.Parameters.AddWithValue("telefono2", contacto.tfono2);
                    cmd.Parameters.AddWithValue("cumpleanos", contacto.cumple);
                    cmd.Parameters.AddWithValue("direccion", contacto.direccion);
                    cmd.Parameters.AddWithValue("idTipoContacto", contacto.tipoContactoNum);
                    cmd.Parameters.AddWithValue("estado", contacto.estado);
                    cmd.Parameters.AddWithValue("notas", contacto.notas);
                    cmd.Parameters.Add("Respuesta", SqlDbType.Int).Direction = ParameterDirection.Output;
                    //cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    respuesta = Convert.ToBoolean(cmd.Parameters["Respuesta"].Value);
                    //Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                Mensaje = ex.Message;
            }
            return respuesta;
        }

        public string buscarTfono(string tfono, out string Mensaje)
        {
            string nombreContacto = "";
            Mensaje = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT c.nombre FROM Contacto c");
                    query.AppendLine("WHERE c.telefono = '"+tfono+"'");

                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.CommandType = System.Data.CommandType.Text;

                    oconexion.Open();

                    nombreContacto = cmd.ExecuteScalar() as string;
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
                catch (Exception ex)
                {
                    Mensaje = ex.Message;
                }
                return nombreContacto;
            }

        }

        public bool ObtenerEstadoContacto(string nombreContacto, out string Mensaje)
        {
            bool estadoContacto = false;
            Mensaje = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = "SELECT c.estado FROM Contacto c WHERE c.nombre = '"+nombreContacto+"'";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                   
                    oconexion.Open();

                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        estadoContacto = Convert.ToBoolean(result);
                    }
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
                catch (Exception ex)
                {
                    Mensaje = ex.Message;
                }
            }

            return estadoContacto;
        }

        public bool modificarEstado(string nombreContacto, out string Mensaje)
        {
            bool exito = false;
            Mensaje = string.Empty;

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = "UPDATE Contacto SET estado = 1 WHERE nombre = '"+nombreContacto+"'";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                  
                    oconexion.Open();

                    int filasActualizadas = cmd.ExecuteNonQuery();
                    exito = filasActualizadas > 0; // Si se actualizó al menos una fila, consideramos que la operación fue exitosa

                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
                catch (Exception ex)
                {
                    Mensaje = ex.Message;
                }
            }

            return exito;
        }

        public bool insertarTelefonoenNotas(string nombreContacto, string telefono, out string mensaje)
        {
            mensaje = string.Empty;
            bool exito = false;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
                {
                    string query = "UPDATE Contacto SET telefono = '', notas = ISNULL(notas, '') + "+telefono+" WHERE nombre = '"+nombreContacto+"'";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                   
                    oconexion.Open();

                    int filasActualizadas = cmd.ExecuteNonQuery();
                    exito = filasActualizadas > 0; // Si se actualizó al menos una fila, consideramos que la operación fue exitosa
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al actualizar el teléfono y las notas: " + ex.Message;
            }

            return exito;
        }

    }
}
