﻿using Bunifu.UI.WinForms;
using System.Text.RegularExpressions;

namespace ControlesPerzonalizados.Ayudas
{
    public class A_Validador
    {
        private bool Validar(string ExpresionRegular, BunifuTextBox TextBox, BunifuLabel Label, BunifuImageButton Imagen, string Mensaje)
        {
            Regex Validador = new Regex(ExpresionRegular);

            if (!Validador.IsMatch(TextBox.Text))
            {
                Imagen.Visible = true;
                Label.Visible = true;
                Label.Text = Mensaje;
                return false;
            }

            return true;
        }

        public bool ValidarCampoLleno(BunifuTextBox TextBox, BunifuLabel Label, BunifuImageButton Imagen)
        {
            return Validar(@"^(?!\s*$).+", TextBox, Label, Imagen, "El campo no debe estar vacío");
        }

        public bool ValidarUsuario(BunifuTextBox TextBox, BunifuLabel Label, BunifuImageButton Imagen)
        {
            bool UsuarioLleno = Validar(@"^(?!\s*$).+", TextBox, Label, Imagen, "El campo no debe estar vacío");
            bool UsuarioDocente = Validar(@"(^$)|(^(AD)[A-Z]{2}$)|(^\d{5}$)", TextBox, Label, Imagen, "El usuario debe contener 5 dígitos");
            bool UsuarioAdministrador = Validar(@"(^$)|(^(?!AD).+)|(^(AD)[A-Z]{2}$)", TextBox, Label, Imagen, "El usuario debe ser AD[Código de Depart.]");

            return UsuarioLleno && UsuarioDocente && UsuarioAdministrador;
        }

        public bool ValidarContraseña(BunifuTextBox TextBox, BunifuLabel Label, BunifuImageButton Imagen)
        {
            bool ContraseñaLlena = Validar(@"^(?!\s*$).+", TextBox, Label, Imagen, "El campo no debe estar vacío");
            bool ContraseñaCorrecta = Validar(@"(^$)|(^(?=.*?\d)(?=.*?[A-Z])(?=.*?[a-z]).{8,20}$)", TextBox, Label, Imagen, "La contraseña debe tener por lo menos una mayúscula y un número");

            return ContraseñaLlena && ContraseñaCorrecta;
        }

        public bool ValidarComparar(BunifuTextBox TextBox1, BunifuLabel Label, string Texto, BunifuImageButton Imagen, string Objeto)
        {
            bool ContraseñaLlena = Validar(@"^(?!\s*$).+", TextBox1, Label, Imagen, "El campo no debe estar vacío");
            bool CompararContraseñas = (TextBox1.Text == Texto) || Validar(@"(^$)", TextBox1, Label, Imagen, Objeto + " no es igual");

            return ContraseñaLlena && CompararContraseñas;
        }

        public bool ValidarCodigoAsignatura(BunifuTextBox TextBox, BunifuLabel Label, BunifuImageButton Imagen, string CodDepartamento)
        {
            bool DigitoLleno = Validar(@"^(?!\s*$).+", TextBox, Label, Imagen, "El campo no debe estar vacío");
            bool DigitoCorrecto = Validar(@"(^$)|(^(" + CodDepartamento + @")\d{3}$)", TextBox, Label, Imagen, "El código deber ser " + CodDepartamento + "[3 dígitos]");

            return DigitoLleno && DigitoCorrecto;
        }

        public bool ValidarNumeroLimitado(BunifuTextBox TextBox, BunifuLabel Label, BunifuImageButton Imagen, int NroDigitos)
        {
            bool NumeroLleno = Validar(@"^(?!\s*$).+", TextBox, Label, Imagen, "El campo no debe estar vacío");
            bool NumeroCorrecto = Validar(@"(^$)|(^\d{" + NroDigitos + @"}$)", TextBox, Label, Imagen, "El campo debe ser de " + NroDigitos + " dígitos");

            return NumeroLleno && NumeroCorrecto;
        }

        public bool ValidarDigitoIntervalo(BunifuTextBox TextBox, BunifuLabel Label, BunifuImageButton Imagen, int Min, int Max)
        {
            bool DigitoLleno = Validar(@"^(?!\s*$).+", TextBox, Label, Imagen, "El campo no debe estar vacío");
            bool DigitoCorrecto = Validar(@"(^$)|(^[" + Min + @"-" + Max + @"]$)", TextBox, Label, Imagen, "El campo debe estar entre " + Min + " y " + Max);

            return DigitoLleno && DigitoCorrecto;
        }

        public void EnfocarCursor(BunifuTextBox TextBox)
        {
            string PlaceHolder = TextBox.PlaceholderText;
            TextBox.PlaceholderText = "";
            TextBox.Focus();
            TextBox.PlaceholderText = PlaceHolder;
        }
    }
}
