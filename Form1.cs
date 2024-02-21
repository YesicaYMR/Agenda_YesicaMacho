using CapaEntidad;
using CapaNegocio;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Agenda_YesicaMacho
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dgvDatos.CellPainting += dgvDatos_CellPainting;
            //cargar el combo del estado Activo / No Activo
            cmbEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cmbEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No activo" });
            cmbInactivo.Items.Add("Activo");
            cmbInactivo.Items.Add("Inactivo");
            cmbInactivo.SelectedIndex = 0;
            cmbEstado.DisplayMember = "Texto";
            cmbEstado.ValueMember = "Valor";
            cmbEstado.SelectedIndex = 0;

            //cargar info combo buscar con nombres de las columnas
            cmbBusqueda.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Estado" });
            cmbBusqueda.Items.Add(new OpcionCombo() { Valor = 0, Texto = "Nombre" });
            cmbBusqueda.DisplayMember = "Texto";
            cmbBusqueda.ValueMember = "Valor";
            cmbBusqueda.SelectedIndex = 0;
          

            //cargar info combo tipo de contactos extrayendo de la bbdd           
            List<Tipo> listaTipos = new CN_Tipo().Listar();
            foreach (Tipo item in listaTipos)
            {
                cmbTipo.Items.Add(new OpcionCombo() { Texto = item.nombre, Valor = item.idTipo });
            }
            cmbTipo.DisplayMember = "Texto";
            cmbTipo.ValueMember = "Valor";
            cmbTipo.SelectedIndex = 0;

            //cargar info combo tipo de contactos extrayendo de la bbdd           
            //List<Tipo> listaTipos = new CN_Tipo().Listar();
            List<Contacto> listContacto = new CN_Contacto().ListarXestado("true");
            dgvDatos.DataSource = listContacto;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Variable para almacenar mensajes de estado
            string mensaje = string.Empty;


            //recoger la fecha marcada
            DateTime fecha = monthCalendar1.SelectionRange.Start; ;
            int tipoSeleccionado = cmbTipo.SelectedIndex+1;

            // Crear un objeto Usuario y asignar valores desde los controles del formulario
            Contacto contacto = new Contacto()
            {
                idContacto = Convert.ToInt32(txtId.Text),
                nombre = txtNombre.Text,
                nick = txtNick.Text,
                apellidos = txtApellidos.Text,
                empresa = txtEmpresa.Text,
                tfono = txtTfono.Text,
                tfono2 = txtTfono2.Text,
                cumple = fecha,
                direccion = txtDireccion.Text,
                notas= txtNotas.Text,
                tipoContactoNum = tipoSeleccionado,
                estado = Convert.ToInt32(((OpcionCombo)cmbEstado.SelectedItem).Valor) == 1 ? true : false
            };

            // Verificar si es una inserción de un nuevo usuario
            if (contacto.idContacto == 0)
            {
                // Registrar el nuevo usuario en la base de datos y obtener el ID generado
                int idusuariogenerado = new CN_Contacto().Registrar(contacto, out mensaje);

                // Si el registro fue exitoso, agregar una nueva fila al DataGridView
                if (idusuariogenerado != 0 && idusuariogenerado != -1)
                {
                    
                    dgvDatos.DataSource = new CN_Contacto().ListarXestado("true");
                    //registrarNuevoUsuario(idusuariogenerado);
                    // Limpiar los controles del formulario
                    limpiar();
                }
                //telefono ocupado por un inactivo
                else if (idusuariogenerado == -1)
                {
                    String nombrePropietario = new CN_Contacto().buscarNombrePropietario(txtTfono.Text, out String Mensaje);
                    DialogResult result = MessageBox.Show("El número introducido pertenece a " + nombrePropietario + "\n¿Quieres activar el contacto?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                    // Puedes manejar la respuesta del usuario aquí según corresponda
                    if (result == DialogResult.Yes)
                    {
                        bool exito = new CN_Contacto().activarContacto(nombrePropietario, out String Mensaje2);
                        dgvDatos.DataSource = new CN_Contacto().ListarXestado("true");
                        limpiar();
                    }
                    else
                    {
                        bool exito = new CN_Contacto().ActualizarTelefonoYAgregarANotas(nombrePropietario, txtTfono.Text, out String Mensaje3);
                        idusuariogenerado = new CN_Contacto().Registrar(contacto, out mensaje);
                        dgvDatos.DataSource = new CN_Contacto().ListarXestado("true");
                    }
                }
                else
                {
                    // Mostrar mensaje de error en caso de fallo en el registro
                    MessageBox.Show(mensaje);
                }
            }
            //no es u contacto nuevo. edita el existente
            else
            {
                // Editar el usuario existente en la base de datos
                bool resultado = new CN_Contacto().Editar(contacto, out mensaje);
                if (resultado)
                {
                    dgvDatos.DataSource = new CN_Contacto().ListarXestado("true");

                    // Limpiar los controles del formulario después de la edición
                    limpiar();
                }
                else
                {
                    // Mostrar mensaje de error en caso de fallo en la edición
                    MessageBox.Show(mensaje);
                }
            }

        }

       /**
        * Pinta una tarta en todas las filas cuando detecta que se cumlpe las restricción en una de las filas.
        * Si ponemos que se oculte la columna en esa fila, lo oculta para todas las filas
        * 
        */
        
        private void dgvDatos_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            /** if (e.RowIndex < 0)
                 return;

             // Verificar si la celda actual es de la columna btnCumple
             if (e.ColumnIndex == 0) // Verificamos si la columna es la que nos interesa
             {
                 //dgvDatos.Columns["btnCumpleanos"].Visible = false;

                 // Obtener la fecha actual
                 DateTime fechaActual = DateTime.Now.Date; // Tomamos solo la parte de la fecha sin la hora

                 foreach (DataGridViewRow fila in dgvDatos.Rows)
                 {
                     // Obtenemos el valor de la celda de la columna "cumpleanos" en formato DateTime
                     DateTime cumpleanos = Convert.ToDateTime(fila.Cells["cumple"].Value).Date;

                     // Tomamos solo el día y el mes de la fecha de cumpleaños
                     DateTime cumpleanosSinAnio = new DateTime(fechaActual.Year, cumpleanos.Month, cumpleanos.Day).Date;

                     // Calculamos la diferencia de días entre la fecha de cumpleaños y la fecha actual
                     TimeSpan diferencia = cumpleanosSinAnio - fechaActual;

                     // Verificar si la diferencia es menor o igual a 7 días
                     if (diferencia.Days <= 7 && diferencia.Days >= 0)
                     {
                         dgvDatos.Columns["btnCumpleanos"].Visible = true;
                         // Mostrar un icono en la celda
                         e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                         var w = Properties.Resources.tarta.Width;
                         var h = Properties.Resources.tarta.Height;
                         var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                         var y = e.CellBounds.Top + (e.CellBounds.Width - h) / 2;
                         e.Graphics.DrawImage(Properties.Resources.tarta, new Rectangle(x, y, w, h));
                         e.Handled = true;
                         break; // Salimos del bucle para no seguir dibujando en las demás filas
                     }
                 }
             }*/
        }

        /**
         * Pinta una tarta en la celda que cumple con la restricción. 
         * Aunque no carga la imagen, detecta dónde tiene que ir 
         * 
         */
        private void dgvDatos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dgvDatos.Columns["idContacto"].Visible = false;
            dgvDatos.Columns["tipoContactoNum"].Visible = false;
            dgvDatos.Columns["tipo"].Visible = false;


            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvDatos.Columns[e.ColumnIndex].Name == "btnCumpleanos")
            {

                // Obtén la fecha de cumpleaños de la fila actual
                DateTime fechaCumple = Convert.ToDateTime(dgvDatos.Rows[e.RowIndex].Cells["cumple"].Value).Date;
                DateTime fechaActual = DateTime.Now.Date;
                DateTime cumpleanosSinAnio = new DateTime(fechaActual.Year, fechaCumple.Month, fechaCumple.Day).Date;
                // Calcula la diferencia de días entre la fecha de cumpleaños y la fecha actual
                TimeSpan diferencia = cumpleanosSinAnio - DateTime.Today;

                // Verifica si la diferencia es menor a 7 días y mayor o igual a 0
                if (diferencia.Days < 7 && diferencia.Days >= 0)
                {
                    //e.Value = Properties.Resources.tarta; // Asigna la imagen desde los recursos
                    // Cargar la imagen desde los recursos como un objeto Image
                    Image imagen = Properties.Resources.tarta;

                    // Convertir la imagen a un objeto Bitmap
                    Bitmap imagenBitmap = new Bitmap(imagen);

                    // Asignar la imagen Bitmap a la celda
                    e.Value = imagenBitmap;
                }
                else
                {
                    // Establece una imagen predeterminada para las celdas que no cumplen la condición
                    e.Value = dgvDatos.DefaultCellStyle.NullValue;
                }
            }
        }

       /**
        * carga los datos en el panel lateral cuando se hace click sobre el btnCumpleanos
        * 
        * */
        private void dgvDatos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int indice = e.RowIndex;
            txtId.Text = "0";
            txtIndice.Text = indice.ToString();

            if (dgvDatos.Columns[e.ColumnIndex].Name == "btnCumpleanos" &&
                dgvDatos.Rows[indice].Cells["idContacto"].Value != null)
            {
                //fila del dgv en la que esta seleccionada
                txtId.Text = dgvDatos.Rows[indice].Cells["idContacto"].Value.ToString();
                txtNombre.Text = dgvDatos.Rows[indice].Cells["nombre"].Value.ToString();
                txtNick.Text = dgvDatos.Rows[indice].Cells["nick"].Value.ToString();
                txtApellidos.Text = dgvDatos.Rows[indice].Cells["apellidos"].Value.ToString();
                txtEmpresa.Text = dgvDatos.Rows[indice].Cells["Empresa"].Value.ToString();
                txtTfono.Text = dgvDatos.Rows[indice].Cells["tfono"].Value.ToString();
                txtTfono2.Text = dgvDatos.Rows[indice].Cells["tfono2"].Value.ToString();
                monthCalendar1.SetDate(Convert.ToDateTime(dgvDatos.Rows[indice].Cells["cumple"].Value));
                //txtFecha.Text = dgvDatos.Rows[indice].Cells["cumple"].Value.ToString();
                txtDireccion.Text = dgvDatos.Rows[indice].Cells["direccion"].Value.ToString();
                txtNotas.Text = dgvDatos.Rows[indice].Cells["notas"].Value.ToString();


                string valorCelda = dgvDatos.Rows[indice].Cells["tipoContacto"].Value.ToString();

                foreach (OpcionCombo oc in cmbTipo.Items)
                {
                    if (oc.Texto == valorCelda)
                    {
                        cmbTipo.SelectedItem = oc;
                        break;
                    }
                }

                foreach (OpcionCombo oc in cmbEstado.Items)
                {
                    if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvDatos.Rows[indice].Cells["estado"].Value))
                    {
                        int indice_combo = cmbEstado.Items.IndexOf(oc);
                        cmbEstado.SelectedIndex = indice_combo;
                        break;
                    }
                }
            }
        }
        /*
         * Busca en la base de datos los contactos que cumplan con los requisitos y los muestra en la tabla
         * 
         */
        private void btnBuscar_Click(object sender, EventArgs e)
        {

            string columnaFiltro = ((OpcionCombo)cmbBusqueda.SelectedItem).Valor.ToString();

            if (columnaFiltro == "1")
            {
                if (cmbInactivo.SelectedIndex==0)
                {
                    dgvDatos.DataSource = new CN_Contacto().ListarXestado("true");
                }
                else if (cmbInactivo.SelectedIndex==1)
                {
                    dgvDatos.DataSource = new CN_Contacto().ListarXestado("false");
                }

            }
            else
            {
                string nombreABuscar = txtBusqueda.Text.Trim();
                dgvDatos.DataSource = new CN_Contacto().ListarXnombre(nombreABuscar);
            }
        }

        /*
         * limpia los campos de busqueda y establece la tabla a su valor inicial
         * 
         */
        private void btnLimpiarBuscar_Click(object sender, EventArgs e)
        {
            {
                txtBusqueda.Text = "";
                cmbBusqueda.SelectedIndex = 0;
                dgvDatos.DataSource = new CN_Contacto().ListarXestado("true");
                cmbInactivo.SelectedIndex = 0;
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiar();
        }

        /*
         * limpia el panel izquierdo de los contactos
         */
        private void limpiar()
        {
            txtId.Text = "0";
            txtIndice.Text = "-1";
            txtNombre.Text = "";
            txtNick.Text = "";
            txtApellidos.Text = "";
            txtEmpresa.Text = "";
            txtTfono.Text = "";
            txtTfono2.Text = "";
            monthCalendar1.SetDate(DateTime.Today);
            txtDireccion.Text = "";
            cmbEstado.SelectedIndex = 0;
            cmbTipo.SelectedIndex = 0;
            txtNotas.Text = "";

            //para que situe el foco
            txtNombre.Select();
        }



        /**
         * Pinta una tarta en todas las celdas
         */
        private void dgvDatos_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            /**DataGridView dgv = sender as DataGridView;

            // Verifica si el índice de la fila está dentro del rango de filas visibles
            if (e.RowIndex >= 0 && e.RowIndex < dgv.Rows.Count)
            {
                // Obtener la fecha actual
                DateTime fechaActual = DateTime.Now.Date; // Tomamos solo la parte de la fecha sin la hora
                // Obtén la fecha de cumpleaños de la fila actual
                DateTime fechaCumple = Convert.ToDateTime(dgv.Rows[e.RowIndex].Cells["cumple"].Value).Date;
                // Tomamos solo el día y el mes de la fecha de cumpleaños
                DateTime cumpleanosSinAnio = new DateTime(fechaActual.Year, fechaCumple.Month, fechaCumple.Day).Date;

                // Calculamos la diferencia de días entre la fecha de cumpleaños y la fecha actual
                TimeSpan diferencia = cumpleanosSinAnio - fechaActual;
                // Obtén el DataGridView del evento

                // Verifica si la diferencia es menor a 7 días
                if (diferencia.Days < 7 && diferencia.Days >= 0)
                {
                    e.PaintBackground(e.ClipBounds, true);
                    e.Graphics.DrawImage(Properties.Resources.tarta, e.CellBounds.Left + 2, e.CellBounds.Top + 2);
                    e.Handled = true;
                }
            }*/
        }

        /*
         * Exporta los datos del datagrid en un excel
         * 
         * 
         */
        private void btnImprimir_Click(object sender, EventArgs e)
        {

            //importar libreria
            //recordar que nos situamos en nuestra capa de presentacion (proyecto)
            //buscamos el paquete closedxml

            //comprobamos que nuestro data grid al menos tenga 1 fila
            if (dgvDatos.Rows.Count < 1)
            {
                MessageBox.Show("No hay datos para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                //xa exportar a excel hay que almacenar los datos en una tabla
                DataTable dt = new DataTable();
                //guardar las cabeceras
                foreach (DataGridViewColumn columna in dgvDatos.Columns)
                {
                    if (columna.HeaderText != "" && columna.Visible)
                    {//añadir la columna a la tabla, es neceszario indicar el tipo de dato que va a contener la colukna
                        dt.Columns.Add(columna.HeaderText, typeof(string));
                    }
                }
                //recorrer las filas
                foreach (DataGridViewRow row in dgvDatos.Rows)
                {
                    //solo exportar las filas visibles
                    if (row.Visible)
                        //añadir la fila
                        dt.Rows.Add(new object[]
                        {
                            //detallar el numero de columna del data grid que tienen que aparecer en el excel
                           
                            row.Cells[2].Value.ToString(),
                            row.Cells[3].Value.ToString(),
                            row.Cells[4].Value.ToString(),
                            row.Cells[5].Value.ToString(),
                            row.Cells[6].Value.ToString(),
                            row.Cells[7].Value.ToString(),
                            row.Cells[8].Value.ToString(),
                            row.Cells[9].Value.ToString(),                            
                            row.Cells[11].Value.ToString(),
                        });
                }
                //preguntar donde guardar el fichero con el objeto safeFileDialog,que 
                //abre una ventana windows donde elegir donde almacenar el fichero
                SaveFileDialog saveFile = new SaveFileDialog();

                saveFile.FileName = string.Format("InformeUsuarios_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                saveFile.Filter = "Excel Files |*.xlsx";

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        //crear el fichero
                        XLWorkbook wb = new XLWorkbook();
                        //agregar hoja
                        var hoja = wb.Worksheets.Add(dt, "Informe Usuarios");

                        hoja.ColumnsUsed().AdjustToContents();

                        wb.SaveAs(saveFile.FileName);
                        MessageBox.Show("Informe exportado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    catch
                    {
                        MessageBox.Show("Error al generar el informe", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        /*
         * Muestra u oculta el combo de los estados y el txt busqueda
         */
        private void cmbBusqueda_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbBusqueda.SelectedIndex == 0){
                cmbInactivo.Visible= true;
                txtBusqueda.Visible = false;
       
            }
            else
            {
                cmbInactivo.Visible = false;
                txtBusqueda.Visible = true;
            }
        }
    }
}
