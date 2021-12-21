﻿using System;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using Bunifu.UI.WinForms;
using ControlesPerzonalizados.Ayudas;
using CapaNegocios;

namespace ControlesPerzonalizados
{
    public partial class C_CambioContraseñaCorreo : UserControl
    {
        string codigo_verificacion = "";
        public string Usuario;
        public string Correo;
        public C_CambioContraseñaCorreo()
        {
            InitializeComponent();
        }

        private void MensajeError(string Mensaje)
        {
            MessageBox.Show(Mensaje, "Sistema Sílabo Asistencia", MessageBoxButtons.OK, MessageBoxIcon.Error);
        } //Listo

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            Correo = txtUsuario.Text + lblDominio.Text;
            Usuario = txtUsuario.Text;
            Verificar();
        }

        public void Verificar()
        {
            string correoIngresado = Correo;

            string ans = validarpanelEnviarCodigo(correoIngresado);

            if (ans == "-1")
            {
                MessageBox.Show("El código no se pudo enviar");
            }
            // Correo no ingresado
            else if (ans == "01")
            {
                MensajeError("Correo no ingresado, intente de nuevo");
            }
            else
            {
                MessageBox.Show("Código enviado");

                codigo_verificacion = ans;
                BunifuLabel CorreoCC = (BunifuLabel)ParentForm.Controls.Find("pnContenedor", false)[0].Controls.Find("lblCorreo", false)[0];
                CorreoCC.Text = txtUsuario.Text + lblDominio.Text;
                BunifuLabel UsuarioCC = (BunifuLabel)ParentForm.Controls.Find("pnContenedor", false)[0].Controls.Find("lblUsuario", false)[0];
                UsuarioCC.Text = txtUsuario.Text;
                BunifuLabel CodigoVerificacionCC = (BunifuLabel)ParentForm.Controls.Find("pnContenedor", false)[0].Controls.Find("lblCodVerificacion", false)[0];
                CodigoVerificacionCC.Text = codigo_verificacion;
                new A_Paso().Siguiente(ParentForm, "Paso1", "Paso2", "C_CambioContraseñaCodigo");

                // mostrar panel de verificacion de codigo
                //lblEmail.Text = correoIngresado;
            }
        }

        public string validarpanelEnviarCodigo(string correoIngresado) //Cambiar
        {

            // Verificar correo ingresado
            if (correoIngresado != "")
                // verificar correo valido
                return EnviarCodigo(correoIngresado);
                /*
                if (correoIngresado == correoValido)
                    // enviar codigo de verificacion
                    return EnviarCodigo(correoValido);
                // Correo invalido
                else
                    return "00"; // 0: correo invalido
                */
            // Correo no ingresado
            else
                return "01"; // correo vacio
        }

        public string EnviarCodigo(string Correo)
        {
            // Enviar un correo con un codigo de verificiacion random
            try
            {
                // Crear codigo random de 6 caracteres
                Random r = new Random();
                var x = r.Next(0, 1000000);
                string s = x.ToString("000000");

                // Enviar codigo de verificacion
                SmtpClient clientDetails = new SmtpClient();
                clientDetails.Port = 587;
                clientDetails.Host = "smtp.gmail.com";
                clientDetails.EnableSsl = true;
                clientDetails.DeliveryMethod = SmtpDeliveryMethod.Network;
                clientDetails.UseDefaultCredentials = false;
                clientDetails.Credentials = new NetworkCredential("Correo", "Contraseña");

                MailMessage mailDetails = new MailMessage();
                mailDetails.From = new MailAddress("Correo");
                mailDetails.To.Add(Correo);
                mailDetails.Subject = "Código de verificación";
                mailDetails.IsBodyHtml = true;
                mailDetails.Body = "Ingresa el siguiente código: " + s;

                clientDetails.Send(mailDetails);
                return s;
            }
            catch (Exception ex)
            {
                // Mostrar error
                MessageBox.Show(ex.Message);
                return "-1";
            }
        }
    }
}
