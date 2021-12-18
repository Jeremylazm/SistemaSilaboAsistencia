﻿using CapaPresentaciones.Ayudas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlesPerzonalizados
{
    public partial class C_CambioContraseñaCorreo : UserControl
    {
        public string Usuario = "";
        public string Correo = "";
        public C_CambioContraseñaCorreo()
        {
            InitializeComponent();
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            Usuario = txtUsuario.Text;
            Correo = txtUsuario.Text + "@unsaac.edu.pe";
            C_CambioContraseñaCodigo.Inicializar(Correo, Usuario);
            new A_Paso().Siguiente(ParentForm, "Paso1", "Paso2", "C_CambioContraseñaCodigo");
        }


    }
}
