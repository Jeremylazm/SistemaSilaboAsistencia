﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidades;
using CapaNegocios;
using ClosedXML.Excel;
using ControlesPerzonalizados;

namespace CapaPresentaciones
{
    public partial class P_ReporteDirector : Form
    {
        readonly N_Catalogo ObjCatalogo;
        private readonly string CodSemestre;
        readonly string CodDocente = E_InicioSesion.Usuario;
        private readonly string CodDepartamentoA = "IF";
        C_Reporte Reportes = new C_Reporte();
        string nombreDocente;

        public P_ReporteDirector()
        {
            DataTable Semestre = N_Semestre.SemestreActual();
            CodSemestre = Semestre.Rows[0][0].ToString();
            ObjCatalogo = new N_Catalogo();
            InitializeComponent();
            LLenarCampos();
        }

        private void LLenarCampos()
        {
            // Data table de todos los alumnos

            cxtTipoReporte.SelectedIndex = 0;
            cxtCriterioSeleccion.SelectedIndex = 0;

            DataTable Estudiantes = N_Matricula.MostrarEstudiantesMatriculados(CodSemestre, CodDepartamentoA);
            txtCodEstudiante.Text = "";
            txtEstudiante = "";

            
            //DataTable Asignaturas = N_Catalogo.BuscarAsignaturasDocente(CodSemestre, CodDepartamentoA, CodDocente);
            //txtCodigo.Text = Estudiantes.Rows[0].ItemArray[0].ToString();
            //txtNombre.Text = Estudiantes.Rows[0].ItemArray[1].ToString();
            //txtEscuelaP.Text = Estudiantes.Rows[0].ItemArray[2].ToString();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void P_ReporteDirector_Load(object sender, EventArgs e)
        {
            /*dpFechaInicial.MaxDate = DateTime.Now;
            dpFechaFinal.MaxDate = DateTime.Now;*/
            dpFechaFinal.MaxDate = new DateTime(2022, 03, 01);
            dpFechaFinal.MinDate = new DateTime(2021, 09, 01);
            dpFechaInicial.MaxDate = new DateTime(2022, 03, 01);
            dpFechaInicial.MinDate = new DateTime(2021, 09, 01);

            //
            dpFechaInicial.Value = new DateTime(2021, 10, 18);
            dpFechaFinal.Value = new DateTime(2021, 11, 05);

            pnReporte.Parent = pnPadre;
            pnReporte.Location = new Point(0, 0);
            pnReporte.Width = pnPadre.ClientSize.Width + SystemInformation.VerticalScrollBarWidth;
            pnReporte.Height = pnPadre.ClientSize.Height + SystemInformation.HorizontalScrollBarHeight;

            //DataTable datosDocente = N_Docente.BuscarDocente(CodDepartamentoA, CodDocente);
            //nombreDocente = datosDocente.Rows[0]["Nombre"].ToString() + " " + datosDocente.Rows[0]["APaterno"].ToString() + " " + datosDocente.Rows[0]["AMaterno"].ToString();

            string Titulo = "REPORTE DE ASISTENCIA ESTUDIANTES" + Environment.NewLine + "Desde: " + dpFechaInicial.Value.ToString("dd/MM/yyyy") + " - " + "Hasta: " + dpFechaFinal.Value.ToString("dd/MM/yyyy");
            /*string[] Titulos = { "Semestre", "Cod. Docente", "Docente", "Cod. Asignatura", "Asignatura", "Escuela Profesional" };
            string[] Valores = { CodSemestre, CodDocente, nombreDocente, txtCodigo.Text, txtNombre.Text, txtEscuelaP.Text };*/
            string[] Titulos = { "Semestre", "Escuela Profesional" };
            string[] Valores = { CodSemestre, txtEscuelaP.Text };

            DataTable resultados = N_AsistenciaEstudiante.AsistenciaEstudiantesPorFechas(CodSemestre, CodDocente, txtCodigo.Text, dpFechaInicial.Value.ToString("yyyy/MM/dd", CultureInfo.GetCultureInfo("es-ES")), dpFechaFinal.Value.ToString("yyyy/MM/dd", CultureInfo.GetCultureInfo("es-ES")));

            C_Reporte Reporte = new C_Reporte(Titulo, Titulos, Valores, resultados, cxtCriterioSeleccion.SelectedItem.ToString(), txtCodigo.Text)
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            Reportes = Reporte;

            Responsivo();
            pnReporte.Controls.Add(Reporte);
        }

        private void Responsivo()
        {
            int AnchoTotal = 0;
            int Filas = 1;

            foreach (C_Campo cpControl in Reportes.pnSubcampos.Controls)
            {
                if ((AnchoTotal + cpControl.Width + 6) > Reportes.pnSubcampos.Width)
                {
                    Filas++;
                    AnchoTotal = cpControl.Width + 6;
                }
                else
                {
                    AnchoTotal += cpControl.Width + 6;
                }
            }

            Reportes.Cuadricula.RowStyles[0].Height = Filas * 92 + 51;
            Reportes.Height = (int)Reportes.Cuadricula.RowStyles[0].Height + (int)Reportes.Cuadricula.RowStyles[1].Height + 73;
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {

        }

        private void btnGeneral_Click(object sender, EventArgs e)
        {

        }

        private void cxtTipoReporte_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cxtTipoReporte.SelectedItem.Equals("Asistencia Estudiantes"))
            {
                lblCriterioSeleccion.Visible = true;
                cxtCriterioSeleccion.Visible = true;

                lblFechaInicial.Visible = true;
                dpFechaInicial.Visible = true;

                lblFechaFinal.Visible = true;
                dpFechaFinal.Visible = true;

                if (cxtCriterioSeleccion.SelectedItem.Equals("Por Estudiantes"))
                {
                    lblCodEstudiante.Visible = true;
                    txtCodEstudiante.Visible = true;
                    lnCodEstudiante.Visible = true;
                    lblEstudiante.Visible = true;
                    txtEstudiante.Visible = true;
                    lnEstudiante.Visible = true;

                    lblCodigo.Visible = false;
                    txtCodigo.Visible = false;
                    lnCodigo.Visible = false;
                    pnCajas.Visible = false;

                    btnGeneral.Visible = false;
                    btnSeleccionar.Location = new Point(btnGeneral.Location.X, 152);
                }
                else if (cxtCriterioSeleccion.SelectedItem.Equals("Por Asignaturas"))
                {
                    lblCodEstudiante.Visible = false;
                    txtCodEstudiante.Visible = false;
                    lnCodEstudiante.Visible = false;
                    lblEstudiante.Visible = false;
                    txtEstudiante.Visible = false;
                    lnEstudiante.Visible = false;

                    lblCodigo.Visible = true;
                    txtCodigo.Visible = true;
                    lnCodigo.Visible = true;
                    pnCajas.Visible = true;

                    btnGeneral.Visible = true;
                    btnSeleccionar.Location = new Point(btnGeneral.Location.X, 131);
                }
            }
            else if (cxtTipoReporte.SelectedItem.Equals("Avance Asignaturas"))
            {
                lblCriterioSeleccion.Visible = false;
                cxtCriterioSeleccion.Visible = false;

                lblFechaInicial.Visible = false;
                dpFechaInicial.Visible = false;

                lblFechaFinal.Visible = false;
                dpFechaFinal.Visible = false;

                btnGeneral.Visible = true;
                btnSeleccionar.Location = new Point(btnGeneral.Location.X, 131);
            }
        }

        private void cxtCriterioSeleccion_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cxtCriterioSeleccion.SelectedItem.Equals("Por Estudiantes"))
            {
                lblCodEstudiante.Visible = true;
                txtCodEstudiante.Visible = true;
                lnCodEstudiante.Visible = true;
                lblEstudiante.Visible = true;
                txtEstudiante.Visible = true;
                lnEstudiante.Visible = true;

                lblCodigo.Visible = false;
                txtCodigo.Visible = false;
                lnCodigo.Visible = false;
                pnCajas.Visible = false;

                btnGeneral.Visible = false;
                btnSeleccionar.Location = new Point(btnGeneral.Location.X, 152);
            }
            else if (cxtCriterioSeleccion.SelectedItem.Equals("Por Asignaturas"))
            {
                lblCodEstudiante.Visible = false;
                txtCodEstudiante.Visible = false;
                lnCodEstudiante.Visible = false;
                lblEstudiante.Visible = false;
                txtEstudiante.Visible = false;
                lnEstudiante.Visible = false;

                lblCodigo.Visible = true;
                txtCodigo.Visible = true;
                lnCodigo.Visible = true;
                pnCajas.Visible = true;

                btnGeneral.Visible = true;
                btnSeleccionar.Location = new Point(btnGeneral.Location.X, 131);
            }
        }
    }
}
