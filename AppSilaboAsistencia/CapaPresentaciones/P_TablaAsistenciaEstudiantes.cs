﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaNegocios;
using CapaEntidades;
using System.IO;
using SpreadsheetLight;
using ClosedXML.Excel;
using CapaPresentaciones.Ayudas;

namespace CapaPresentaciones
{
    public partial class P_TablaAsistenciaEstudiantes : Form
    {
        readonly N_Catalogo ObjNegocio;
        public string CodSemestre = "2021-II";
        public string CodAsignatura;
        public string CodDocente;
        readonly E_AsistenciaEstudiante ObjEntidadEstd;
        readonly N_AsistenciaEstudiante ObjNegocioEstd;
        readonly E_AsistenciaDocente ObjEntidadDoc;
        readonly N_AsistenciaDocente ObjNegocioDoc;
        public string hora = DateTime.Now.ToString("hh:mm:ss");
        private DataTable PlanSesion;
        public DataTable dgvTabla;
        public string horainicioAsignatura;
        public string LmFechaInf = "15/12/2021";
        
        public P_TablaAsistenciaEstudiantes(string pCodAsignatura, string pCodDocente, DataTable pdgv)
        {
            ObjNegocio = new N_Catalogo();
            CodAsignatura = pCodAsignatura;
            CodDocente = pCodDocente;
            dgvTabla = pdgv;
            ObjEntidadEstd = new E_AsistenciaEstudiante();
            ObjNegocioEstd = new N_AsistenciaEstudiante();
            ObjEntidadDoc = new E_AsistenciaDocente();
            ObjNegocioDoc = new N_AsistenciaDocente();
            InitializeComponent();

            Bunifu.Utils.DatagridView.BindDatagridViewScrollBar(dgvDatos, sbDatos);
            lblTitulo.Text += CodAsignatura;
            lblFecha.Text += "    ";
            PlanSesion = N_Catalogo.RecuperarPlanDeSesionAsignatura(CodSemestre, CodAsignatura, CodDocente);
            //MostrarEstudiantes();
        }

        private void AccionesTabla()
        {
            dgvDatos.Columns[0].DisplayIndex = 6;
            dgvDatos.Columns[1].DisplayIndex = 6;
            dgvDatos.Columns[2].HeaderText = "Id.";
            dgvDatos.Columns[2].ReadOnly = true;
            dgvDatos.Columns[3].HeaderText = "Código";
            dgvDatos.Columns[3].ReadOnly = true;
            dgvDatos.Columns[4].HeaderText = "Apellido Paterno";
            dgvDatos.Columns[4].ReadOnly = true;
            dgvDatos.Columns[5].HeaderText = "Apellido Materno";
            dgvDatos.Columns[5].ReadOnly = true;
            dgvDatos.Columns[6].HeaderText = "Nombre";
            dgvDatos.Columns[6].ReadOnly = true;
            //dgvDatos.Columns[7].Visible = false;
            //dgvDatos.Columns[8].Visible = false;
            //dgvDatos.Columns[9].Visible = false;



            //dgvDatos.Rows[6].Cells[0].Value = ListaImagenes.Images[0];
        }
        private void AccionesTablaEditar()
        {
            dgvDatos.Columns[0].DisplayIndex = 9;
            dgvDatos.Columns[1].DisplayIndex = 9;
            dgvDatos.Columns[2].HeaderText = "Id.";
            dgvDatos.Columns[2].ReadOnly = true;
            dgvDatos.Columns[3].HeaderText = "Código";
            dgvDatos.Columns[3].ReadOnly = true;
            dgvDatos.Columns[4].HeaderText = "Apellido Paterno";
            dgvDatos.Columns[4].ReadOnly = true;
            dgvDatos.Columns[5].HeaderText = "Apellido Materno";
            dgvDatos.Columns[5].ReadOnly = true;
            dgvDatos.Columns[6].HeaderText = "Nombre";
            dgvDatos.Columns[6].ReadOnly = true;
            dgvDatos.Columns[7].Visible = false;
            dgvDatos.Columns[8].Visible = false;
            dgvDatos.Columns[9].Visible = false;




            //dgvDatos.Rows[6].Cells[0].Value = ListaImagenes.Images[0];
        }
        public void InicializarValores()
        {
            foreach (DataGridViewRow fila in dgvDatos.Rows)
            {
                DataGridViewTextBoxCell textBoxcell = (DataGridViewTextBoxCell)(fila.Cells["txtObservaciones"]);
                textBoxcell.Value = "";
                fila.Cells[0].Value = ListaImagenes.Images[0];
                fila.Cells[0].Tag = false;
            }
        }
        public void InicializarValoresEditar()
        {
            foreach (DataGridViewRow fila in dgvDatos.Rows)
            {
                DataGridViewTextBoxCell textBoxcell = (DataGridViewTextBoxCell)(fila.Cells["txtObservaciones"]);
                textBoxcell.Value = fila.Cells[9].Value;
                fila.Cells[0].Value = (fila.Cells[8].Value.Equals("SI")) ? ListaImagenes.Images[1] : ListaImagenes.Images[0];
                if(fila.Cells[8].Value.Equals("SI"))
                {
                    fila.Cells[0].Tag = true;
                }
                else
				{
                    fila.Cells[0].Tag = false;

                }
                //fila.Cells[0].Tag = (fila.Cells[0].Value == ListaImagenes.Images[1]) ? true : false;
            }
        }

        private void MostrarEstudiantesNuevoRegistro()
        {

            dgvDatos.DataSource = dgvTabla;
            AccionesTabla();

        }
        public void MostrarEstudiantesRegistrados()
        {
            dgvDatos.DataSource = dgvTabla;
            AccionesTablaEditar();
        }

        public void BuscarEstudiantes()
        {
            dgvDatos.DataSource = N_Matricula.BuscarEstudiantesMatriculadosAsignatura(CodSemestre, CodAsignatura.Substring(6), CodAsignatura, txtBuscar.Text);
        }

        public void AgregarRgistroEstudiantes()
        {


            foreach (DataGridViewRow dr in dgvDatos.Rows)
            {
                ObjEntidadEstd.CodSemestre = CodSemestre;
                ObjEntidadEstd.CodAsignatura = CodAsignatura;
                ObjEntidadEstd.HoraInicio = horainicioAsignatura;//asigantura
                ObjEntidadEstd.Fecha = txtFecha.Text.ToString();//asistencia
                ObjEntidadEstd.Hora = hora;//asistencia
                ObjEntidadEstd.CodEstudiante = dr.Cells[3].Value.ToString();
                ObjEntidadEstd.Estado = (dr.Cells[0].Tag.Equals(true)) ? "SI" : "NO";

                ObjEntidadEstd.Observacion = (dr.Cells[1].Value == null) ? "" : dr.Cells[1].Value.ToString();
                ObjNegocioEstd.RegistrarAsistenciaEstudiante(ObjEntidadEstd);


            }
            //A_Dialogo.DialogoConfirmacion("El registro de la asistencia de los estudiantes se insertó éxitosamente");
        }
        public void EditarRegistroEstudiantes()
        {
            foreach (DataGridViewRow dr in dgvDatos.Rows)
            {
                ObjEntidadEstd.CodSemestre = CodSemestre;
                ObjEntidadEstd.CodAsignatura = CodAsignatura;
                ObjEntidadEstd.HoraInicio = horainicioAsignatura;//asigantura
                ObjEntidadEstd.Fecha = txtFecha.Text.ToString();//asistencia
                ObjEntidadEstd.CodEstudiante = dr.Cells[3].Value.ToString();
                //ObjEntidadEstd.Estado = (dr.Cells[0].Tag.Equals(true)) ? "SI" : "NO";
                string EstadoActualizado = (dr.Cells[0].Tag.Equals(true)) ? "SI" : "NO";

                
                string ObsActualizada = (dr.Cells[1].Value==null)?"":dr.Cells[1].Value.ToString();
                ObjNegocioEstd.ActualizarAsistenciaEstudiante(ObjEntidadEstd, CodSemestre, CodAsignatura, horainicioAsignatura, txtFecha.Text.ToString(), dr.Cells[3].Value.ToString(), EstadoActualizado, ObsActualizada);


            }
            //A_Dialogo.DialogoConfirmacion("El registro de la asistencia de los estudiantes se Editó éxitosamente");

        }
        public void GuardarRegistroDocente()
        {

            if (Program.Evento == 0)//add
            {
                try
                {
                    // buscar el registro de asistencia de Docente de la fecha actual
                    DataTable Resultado = N_AsistenciaDocente.AsistenciaDocenteAsignatura(CodSemestre, "IF", CodDocente, CodAsignatura, horainicioAsignatura, txtFecha.Text.ToString(), txtFecha.Text.ToString());

                    if (Resultado.Rows.Count == 0)
                    {
                        ObjEntidadDoc.CodSemestre = CodSemestre;
                        ObjEntidadDoc.CodAsignatura = CodAsignatura;
                        ObjEntidadDoc.HoraInicio = horainicioAsignatura;
                        ObjEntidadDoc.Fecha = txtFecha.Text.ToString();
                        ObjEntidadDoc.Hora = hora;
                        ObjEntidadDoc.CodDocente = CodDocente;
                        ObjEntidadDoc.NombreTema = txtTema.Text.ToString();

                        ObjNegocioDoc.RegistrarAsistenciaDocente(ObjEntidadDoc);
                        //A_Dialogo.DialogoConfirmacion("El registro de Asistencia Docente se insertó éxitosamente");
                        AgregarRgistroEstudiantes();
                        A_Dialogo.DialogoConfirmacion("Se ha registrado correctamente la asistencia" + Environment.NewLine+" del Docente y los Estudiantes");
                        Program.Evento = 0;
                        Close();
                    }
                    else
                    {
                        A_Dialogo.DialogoInformacion("El registro de Hoy, ¡Ya existe!");
                        
                    }
                }
                catch (Exception)
                {
                    A_Dialogo.DialogoError("Error al insertar el registro");
                }
            }
            // Editar
            else
            {
                try
                {
                    if (A_Dialogo.DialogoPreguntaAceptarCancelar("¿Realmente desea editar el registro?") == DialogResult.Yes)
                    {
                        DataTable Resultado = N_AsistenciaDocente.AsistenciaDocenteAsignatura(CodSemestre, "IF", CodDocente, CodAsignatura, horainicioAsignatura, txtFecha.Text.ToString(), txtFecha.Text.ToString());
                        
                        if (Resultado.Rows.Count != 0)
                        {

                            ObjEntidadDoc.CodSemestre = CodSemestre;
                            ObjEntidadDoc.CodAsignatura = CodAsignatura;
                            ObjEntidadDoc.HoraInicio = horainicioAsignatura;
                            ObjEntidadDoc.Fecha = txtFecha.Text.ToString();
                            ObjEntidadDoc.Hora = hora;
                            ObjEntidadDoc.CodDocente = CodDocente;
                            ObjEntidadDoc.NombreTema = txtTema.Text.ToString();

                            string NombreTemaActualizado = txtTema.Text.ToString();
                            string fechaActualizada = txtFecha.Text.ToString();
                            string NuevaHoraInicioAsignatura = horainicioAsignatura;

                            ObjNegocioDoc.ActualizarAsistenciaDocente(ObjEntidadDoc, CodSemestre, CodAsignatura, NuevaHoraInicioAsignatura, fechaActualizada, CodDocente, NombreTemaActualizado);
                            A_Dialogo.DialogoConfirmacion("Se ha Editado  la Asistencia" + Environment.NewLine+" del Docente y los Estudiantes");
                            EditarRegistroEstudiantes();
                            Program.Evento = 0;

                            Close();

                        }
                        else
                        {
                            A_Dialogo.DialogoError("El registro de Docente no existe");
                        }

                    }
                }
                catch (Exception)
                {
                    A_Dialogo.DialogoError("Error al editar el registro");
                }
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Program.Evento = 0;
            Close();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            BuscarEstudiantes();
        }

        private void btnSesiones_Click(object sender, EventArgs e)
        {
            P_TablaSesiones Sesiones = new P_TablaSesiones(CodAsignatura, CodDocente);

            Sesiones.ShowDialog();
            Sesiones.Dispose();

        }

        private void dgvDatos_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex < 0) || (e.RowIndex < 0))
            {
                return;
            }

            var DataGrid = (sender as DataGridView);

            if (e.ColumnIndex == 0)
                DataGrid.Cursor = Cursors.Hand;
            else
                DataGrid.Cursor = Cursors.Default;
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex < 0) || (e.RowIndex < 0))
            {
                return;
            }

            var DataGrid = (sender as DataGridView);

            if (e.ColumnIndex == 0)
            {
                var Celda = DataGrid.Rows[e.RowIndex].Cells[0];

                if ((Celda.Tag == null) || !((bool)Celda.Tag))
                {
                    // Falso
                    DataGrid.Rows[e.RowIndex].Cells[0].Value = ListaImagenes.Images[1];
                    DataGrid.Rows[e.RowIndex].Cells[0].Tag = true;
                }
                else
                {
                    DataGrid.Rows[e.RowIndex].Cells[0].Value = ListaImagenes.Images[0];
                    DataGrid.Rows[e.RowIndex].Cells[0].Tag = false;
                }
            }
        
            
        }

        private void ckbMarcarTodos_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuCheckBox.CheckedChangedEventArgs e)
        {
            if (ckbMarcarTodos.Checked)
            {
                foreach (DataGridViewRow Fila in dgvDatos.Rows)
                {
                    Fila.Cells[0].Value = ListaImagenes.Images[1];
                    Fila.Cells[0].Tag = true;
                }
            }
            else
            {
                foreach (DataGridViewRow Fila in dgvDatos.Rows)
                {
                    Fila.Cells[0].Value = ListaImagenes.Images[0];
                    Fila.Cells[0].Tag = false;
                }
            }
        }


        private void P_TablaAsistenciaEstudiantes_Load(object sender, EventArgs e)
        {
            if (Program.Evento == 0)
            {
                if (PlanSesion.Rows.Count > 0)
                {
                    int valor = 9;
                    // Se crea un archivo temporal, para después abrirlo con ClosedXML
                    string path = AppDomain.CurrentDomain.BaseDirectory;
                    string folder = path + "/temp/";
                    string fullFilePath = folder + "temp.xlsx";


                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    if (File.Exists(fullFilePath))
                    {
                        File.Delete(fullFilePath);
                    }

                    byte[] archivo = PlanSesion.Rows[0]["PlanSesiones"] as byte[];

                    File.WriteAllBytes(fullFilePath, archivo);

                    SLDocument sl = new SLDocument(fullFilePath);
                    while (sl.GetCellValueAsString(valor, 8) == "Hecho")
                    {
                        valor++;
                        if (sl.GetCellValueAsString(valor, 3) == "")
                        {
                            valor++;
                        }
                    }
                    txtTema.Text = sl.GetCellValueAsString(valor, 3);
                }
                else
                {
                    A_Dialogo.DialogoInformacion("Aun no subio un plan de sesiones");
                    txtTema.Text = "No hay tema a sugerir";
                    btnGuardar.Enabled = false;
                }
                MostrarEstudiantesNuevoRegistro();
                InicializarValores();
                //Program.Evento = 0;
            }
            else
            {
                MostrarEstudiantesRegistrados();
                InicializarValoresEditar();
                //Program.Evento = 1;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (Program.Evento == 0)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                string folder = path + "/temp/";
                string fullFilePath = folder + "temp.xlsx";

                int valor = 9;
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                if (File.Exists(fullFilePath))
                {
                    File.Delete(fullFilePath);
                }

                byte[] archivo = PlanSesion.Rows[0]["PlanSesiones"] as byte[];

                File.WriteAllBytes(fullFilePath, archivo);
                SLDocument sl = new SLDocument(fullFilePath);
                while (sl.GetCellValueAsString(valor, 8) == "Hecho")
                {
                    valor++;
                    if (sl.GetCellValueAsString(valor, 3) == "")
                    {
                        valor++;
                    }
                }

                XLWorkbook wb = new XLWorkbook(fullFilePath);
                wb.Worksheet(1).Cell("H" + valor.ToString()).Value = wb.Worksheet(1).Cell("H" + valor.ToString()).Value + "Hecho";
                wb.SaveAs(fullFilePath);
                byte[] arreglo = null;
                arreglo = File.ReadAllBytes(fullFilePath);

                ObjNegocio.ActualizarPlanSesionesAsignatura(CodSemestre, CodAsignatura, CodDocente, arreglo);
                //GuardarRegistroDocente();
                //Close();
            }
            else
            {
                //GuardarRegistroDocente();
                //Close();
                
            }
            
            GuardarRegistroDocente();
            Close();
        }
    }
}
