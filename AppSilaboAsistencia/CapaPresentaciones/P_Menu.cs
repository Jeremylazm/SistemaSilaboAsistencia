﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using CapaEntidades;
using System.IO;
using System.Drawing.Drawing2D;

namespace CapaPresentaciones
{
    public partial class P_Menu : Form
    {

        public string Acceso = "";

        public P_Menu()
        {
            InitializeComponent();
            Control[] Controles = { pnOpciones, pnContenedor, lblSuperior, lblInferior, pbLogo, pbPerfil, lblDatos, lblAcceso, lblUsuario };
            Docker.SubscribeControlsToDragEvents(Controles);
        }

        bool DrawerOpen = true;

        public Image HacerImagenCircular(Image img)
        {
            int x = img.Width / 2;
            int y = img.Height / 2;
            int r = Math.Min(x, y);

            Bitmap tmp = null;
            tmp = new Bitmap(2 * r, 2 * r);
            using (Graphics g = Graphics.FromImage(tmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TranslateTransform(tmp.Width / 2, tmp.Height / 2);
                GraphicsPath gp = new GraphicsPath();
                gp.AddEllipse(0 - r, 0 - r, 2 * r, 2 * r);
                Region rg = new Region(gp);
                g.SetClip(rg, CombineMode.Replace);
                Bitmap bmp = new Bitmap(img);
                g.DrawImage(bmp, new Rectangle(-r, -r, 2 * r, 2 * r), new Rectangle(x - r, y - r, 2 * r, 2 * r), GraphicsUnit.Pixel);
            }

            return tmp;
        }

        private void CargarDatosUsuario()
        {
            if (E_InicioSesion.Perfil == null)
            {
                if ((E_InicioSesion.Acceso == "Director de Escuela") || (E_InicioSesion.Acceso == "Administrador"))
                {
                    pbPerfil.Image = Properties.Resources.Perfil as Image;
                    pbEditarPerfil.Image = Properties.Resources.Perfil as Image;
                }
                
                if (E_InicioSesion.Acceso == "Docente")
                {
                    pbPerfil.Image = Properties.Resources.Perfil_Docente as Image;
                    pbEditarPerfil.Image = Properties.Resources.Perfil_Docente as Image;
                }
            }
            else
            {
                byte[] Perfil = new byte[0];
                Perfil = E_InicioSesion.Perfil;
                MemoryStream MemoriaPerfil = new MemoryStream(Perfil);
                pbPerfil.Image = HacerImagenCircular(Bitmap.FromStream(MemoriaPerfil));
                pbEditarPerfil.Image = HacerImagenCircular(Bitmap.FromStream(MemoriaPerfil));
            }
            lblDatos.Text = E_InicioSesion.Datos;
            lblAcceso.Text = E_InicioSesion.Acceso;
            lblUsuario.Text = E_InicioSesion.Usuario;
        }

        private void ActualizarPerfil(object sender, EventArgs e)
        {
            CargarDatosUsuario();
        }

        private void GestionarAcceso()
        {
            if (Acceso == "Administrador")
            {
                /*btnTutorias.Visible = false;
                btnTutorados.Visible = false;
                btnDocentes.Visible = true;
                btnTutores.Visible = true;
                btnEstudiantes.Visible = true;
                btnMiTutor.Visible = false;
                btnSolicitarCita.Visible = false;
                separador.Visible = false;*/

                // Administrador 
            }
            else if (Acceso == "Jefe de Departamento")
            {
                // Docentes y catálogo
                btnAsistencia.Visible = false;
                btnAsignaturasAsignadas.Visible = false;
                btnSilabos.Visible = false;
                btnSesiones.Visible = false;
                btnCatálogo.Visible = true;
                btnAsignaturas.Visible = true;
                btnDocentes.Visible = true;
            }
            else if (Acceso == "Director de Escuela")
            {
                // Asignaturas
                btnAsistencia.Visible = false;
                btnAsignaturasAsignadas.Visible = false;
                btnSilabos.Visible = false;
                btnSesiones.Visible = false;
                btnCatálogo.Visible = false;
                btnAsignaturas.Visible = true;
                btnDocentes.Visible = false;
            }
            else if (Acceso == "Docente")
            {
                btnAsistencia.Visible = true;
                btnAsignaturasAsignadas.Visible = true;
                btnSilabos.Visible = true;
                btnSesiones.Visible = true;
                btnCatálogo.Visible = false;
                btnAsignaturas.Visible = false;
                btnDocentes.Visible = true;
            }
        }

        private void btnContraer_Click(object sender, EventArgs e)
        {
            DrawerOpen = !DrawerOpen;
            pnOpciones.Visible = false;
            pnContenedor.Visible = false;

            if (DrawerOpen)
            {
                pnOpciones.Width = 220;
                pbLogo.Location = new Point(225, 5);
                SeparadorMenu1.Width = 209;
                SeparadorMenu2.Width = 209;
                pbPerfil.Visible = true;
                btnEditarPerfil.Visible = true;
                pbEditarPerfil.Visible = false;
                btnCerrarSesion.Visible = true;
                pbCerrarSesion.Visible = false;
                lblDatos.Visible = true;
                lblAcceso.Visible = true;
                lblUsuario.Visible = true;
                Transicion.ShowSync(pnOpciones);
                Transicion.ShowSync(pnContenedor);

            }
            else
            {
                pnOpciones.Width = 44;
                pbLogo.Location = new Point(49, 5);
                SeparadorMenu1.Width = 35;
                SeparadorMenu2.Width = 35;
                pbPerfil.Visible = false;
                btnEditarPerfil.Visible = false;
                pbEditarPerfil.Visible = true;
                btnCerrarSesion.Visible = false;
                pbCerrarSesion.Visible = true;
                lblDatos.Visible = false;
                lblAcceso.Visible = false;
                lblUsuario.Visible = false;
                Transicion.ShowSync(pnOpciones);
                Transicion.ShowSync(pnContenedor);
            }
        }

        private const int TamañoGrid = 10;
        private const int AreaMouse = 132;
        private const int BotonIzquierdo = 17;
        private Rectangle RectanguloGrid;

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            var Region = new Region(new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height));
            RectanguloGrid = new Rectangle(ClientRectangle.Width - TamañoGrid, ClientRectangle.Height - TamañoGrid, TamañoGrid, TamañoGrid);
            Region.Exclude(RectanguloGrid);
            pnPrincipal.Region = Region;
            Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case AreaMouse:
                    base.WndProc(ref m);

                    var ReferenciaPunto = PointToClient(new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16));

                    if (!RectanguloGrid.Contains(ReferenciaPunto))
                        break;

                    m.Result = new IntPtr(BotonIzquierdo);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            SolidBrush Solido = new SolidBrush(Color.FromArgb(232, 158, 31));
            e.Graphics.FillRectangle(Solido, RectanguloGrid);

            base.OnPaint(e);

            ControlPaint.DrawSizeGrip(e.Graphics, Color.Transparent, RectanguloGrid);
        }

        int Lx, Ly;
        int Sw, Sh;

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            Size = new Size(Sw, Sh);
            Location = new Point(Lx, Ly);
            btnRestaurar.Visible = false;
            btnMaximizar.Visible = true;
        }

        private void btnMaximizar_Click(object sender, EventArgs e)
        {
            Lx = Location.X;
            Ly = Location.Y;
            Sw = Size.Width;
            Sh = Size.Height;

            Size = Screen.PrimaryScreen.WorkingArea.Size;
            Location = Screen.PrimaryScreen.WorkingArea.Location;

            btnMaximizar.Visible = false;
            btnRestaurar.Visible = true;
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void ActualizarColor()
        {
            lblSuperior.Focus();
        }

        private void CerrarSesion()
        {
            P_DialogoRespuesta2 Dialogo = new P_DialogoRespuesta2("¿Desea cerrar sesión?", true);
            Dialogo.ShowDialog();
            DialogResult Opcion = Dialogo.DialogResult;
            if (Opcion == DialogResult.Yes)
            {
                Close();
                P_InicioSesion Login = new P_InicioSesion();
                Login.Show();
            }
        }

        private void EditarPerfil()
        {
            ActualizarColor();

            if (lblAcceso.Text == "Jefe de Departamento Academico")
            {
                P_EditarPerfilDocente Editar = new P_EditarPerfilDocente
                {
                    Usuario = E_InicioSesion.Usuario,
                    TopLevel = false,
                    Dock = DockStyle.Fill
                };
                Editar.btnGuardar.Click += new EventHandler(ActualizarPerfil);

                pnContenedor.Controls.Add(Editar);
                pnContenedor.Tag = Editar;
                Editar.Show();
                Editar.BringToFront();
            }
            else if (lblAcceso.Text == "Director de Escuela Profesional")
            {

            }
            else if (lblAcceso.Text == "Docente")
            {
                P_EditarPerfilDocente Editar = new P_EditarPerfilDocente
                {
                    Usuario = E_InicioSesion.Usuario,
                    TopLevel = false,
                    Dock = DockStyle.Fill
                };
                Editar.btnGuardar.Click += new EventHandler(ActualizarPerfil);

                pnContenedor.Controls.Add(Editar);
                pnContenedor.Tag = Editar;
                Editar.Show();
                Editar.BringToFront();
            }
            /*else
            {
                P_EditarPerfilDirector Editar = new P_EditarPerfilDirector
                {
                    Usuario = E_InicioSesion.Usuario,
                    TopLevel = false,
                    Dock = DockStyle.Fill
                };
                Editar.btnGuardar.Click += new EventHandler(ActualizarPerfil);

                pnContenedor.Controls.Add(Editar);
                pnContenedor.Tag = Editar;
                Editar.Show();
                Editar.BringToFront();
            }*/
        }

        private void btnDocentes_Click(object sender, EventArgs e)
        {
            ActualizarColor();
            AbrirFormularios<P_TablaDocentes>();
        }

        private void btnAsignaturas_Click(object sender, EventArgs e)
        {
            ActualizarColor();
            AbrirFormularios<P_TablaAsignaturas>();
        }

        private void btnSilabos_Click(object sender, EventArgs e)
        {
            ActualizarColor();
            AbrirFormularios<P_TablaAsignaturasAsignadasSilabos>();
        }

        private void btnAsignaturasAsignadas_Click(object sender, EventArgs e)
        {
            ActualizarColor();
            AbrirFormularios<P_TablaAsignaturasAsignadasEstudiantes>();
        }

        private void btnEditarPerfil_Click(object sender, EventArgs e)
        {
            EditarPerfil();
        }

        private void btnCatálogo_Click(object sender, EventArgs e)
        {
            ActualizarColor();
            AbrirFormularios<P_TablaCatálogo>();
        }

        private void btnSesiones_Click(object sender, EventArgs e)
        {
            ActualizarColor();
            AbrirFormularios<P_TablaAsignaturasAsignadasSesiones>();
        }

        private void btnAsistencia_Click(object sender, EventArgs e)
        {
            ActualizarColor();
            AbrirFormularios<P_TablaAsignaturasAsignadasAsistencias>();
        }

        private void P_Menu_Load(object sender, EventArgs e)
        {
            CargarDatosUsuario();
            GestionarAcceso();
        }

        private void pbEditarPerfil_Click(object sender, EventArgs e)
        {
            EditarPerfil();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            CerrarSesion();
        }

        private void pbCerrarSesion_Click(object sender, EventArgs e)
        {
            CerrarSesion();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            P_DialogoRespuesta2 Dialogo = new P_DialogoRespuesta2("¿Desea salir de la aplicación?", true);
            Dialogo.ShowDialog();
            DialogResult Opcion = Dialogo.DialogResult;
            if (Opcion == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // Abrir Formularios
        private void AbrirFormularios<FormularioAbrir>() where FormularioAbrir : Form, new()
        {
            Form Formularios = pnContenedor.Controls.OfType<FormularioAbrir>().FirstOrDefault();

            if (Formularios == null)
            {
                Formularios = new FormularioAbrir
                {
                    TopLevel = false,
                    Dock = DockStyle.Fill
                };

                pnContenedor.Controls.Add(Formularios);
                pnContenedor.Tag = Formularios;
                Formularios.Show();
                Formularios.BringToFront();
            }
            else
            {
                Formularios.BringToFront();
            }
        }

    }
}
