﻿using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using CapaEntidades;
using CapaNegocios;
using Ayudas;

namespace CapaPresentaciones
{
    public partial class P_DatosSemestre : Form
    {
        readonly A_Validador Validador;
        readonly E_Semestre ObjEntidad;
        readonly N_Semestre ObjNegocio;
        public P_DatosSemestre()
        {
            Validador = new A_Validador();
            ObjEntidad = new E_Semestre();
            ObjNegocio = new N_Semestre();
            InitializeComponent();
        }
        private void LimpiarCajas()
        {
            txtDenominacionSemestre.Clear();
        }
        private void lblDatos_Click(object sender, EventArgs e)
        {

        }
        private void RegistrarSemestre()
        {
            //Validar semestre
            //bool CorreoCorrecto = Validador.ValidarCampoLleno(txtDenominacionSemestre);

            // Agregar
            if (Program.Evento == 0)
            {
                try
                {
                    DataTable Resultado = N_Semestre.SemestreActual();

                    if (Resultado.Rows.Count != 0)
                    {
                        ObjEntidad.Denominacion = txtDenominacionSemestre.Text.ToUpper();
                        ObjEntidad.FechaInicio = dpFechaInicialSemestre.Value.ToString("dd/MM/yyyy");

                        ObjNegocio.InsertarSemestre(ObjEntidad);
                        A_Dialogo.DialogoConfirmacion("Registro insertado exitosamente");
                        Program.Evento = 0;

                        Close();
                    }
                    else
                    {
                        A_Dialogo.DialogoError("El registro de semestre ya existe");
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
                        DataTable Resultado = N_Semestre.SemestreActual();

                        if (Resultado.Rows.Count != 0)
                        {
                            ObjEntidad.Denominacion = txtDenominacionSemestre.Text.ToUpper();
                            ObjEntidad.FechaInicio = dpFechaInicialSemestre.Text.ToUpper();
                            //ObjEntidad.Creditos = Convert.ToInt32(txtCreditos.Text);
                            

                            ObjNegocio.ActualizarSemestre(ObjEntidad,txtDenominacionSemestre.Text,dpFechaInicialSemestre.Value.ToString("dd/MM/yyyy"));
                            A_Dialogo.DialogoConfirmacion("Registro editado exitosamente");
                            Program.Evento = 0;
                            Close();
                        }
                        else
                        {
                            A_Dialogo.DialogoError("El registro de asignatura no existe");
                        }
                    }
                }
                catch (Exception)
                {
                    A_Dialogo.DialogoError("Error al editar el registro");
                }
            }
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            ActualizarColor();
            RegistrarSemestre();
        }
        private void ActualizarColor()
        {
            lblTitulo.Focus();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            ActualizarColor();
            LimpiarCajas();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Program.Evento = 0;
            Close();
        }
    }
}
