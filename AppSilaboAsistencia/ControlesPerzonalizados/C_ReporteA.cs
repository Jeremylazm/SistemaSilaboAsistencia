﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MathNet.Numerics.Statistics;
using Ayudas;

namespace ControlesPerzonalizados
{
    public partial class C_ReporteA : UserControl
    {
        private string CriterioAsistenciasEstudiantes;

        public C_ReporteA()
        {
            InitializeComponent();

            Bunifu.Utils.DatagridView.BindDatagridViewScrollBar(dgvResultados, sbResultados);
            /*dgvResultados.MouseWheel += new MouseEventHandler(dataGridView1_MouseWheel);
            sbResultados.MouseWheel += new MouseEventHandler(dataGridView1_MouseWheel);*/
        }

        void dataGridView1_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
            int currentIndex = this.dgvResultados.FirstDisplayedScrollingRowIndex;
            int scrollLines = SystemInformation.MouseWheelScrollLines;

            if (e.Delta > 0)
            {
                this.dgvResultados.FirstDisplayedScrollingRowIndex
                    = Math.Max(0, currentIndex - scrollLines);
                this.sbResultados.Value = Math.Max(0, currentIndex - scrollLines);
            }
            else if (e.Delta < 0)
            {
                this.dgvResultados.FirstDisplayedScrollingRowIndex
                    = currentIndex + scrollLines;
                this.sbResultados.Value = currentIndex + scrollLines;
            }
        }

        public void LimpiarCampos()
        {
            pnSubcampos.Controls.Clear();
        }

        public C_ReporteA(string Titulo, string[] Titulos, string[] Valores, DataTable Datos, string CriterioAsistenciasEstudiantes, string CodAsignatura)
        {
            InitializeComponent();

            Bunifu.Utils.DatagridView.BindDatagridViewScrollBar(dgvResultados, sbResultados);
            /*dgvResultados.MouseWheel += new MouseEventHandler(dataGridView1_MouseWheel);
            sbResultados.MouseWheel += new MouseEventHandler(dataGridView1_MouseWheel);*/

            this.CriterioAsistenciasEstudiantes = CriterioAsistenciasEstudiantes;

            lblTitulo.Text = Titulo;

            if (Titulos.Length.Equals(Valores.Length))
            {
                if (Titulos.Length != 0)
                {
                    for (int K = 0; K < Titulos.Length; K++)
                    {
                        C_Campo Nuevo = new C_Campo(Titulos[K], Valores[K]);
                        pnSubcampos.Controls.Add(Nuevo);
                    }
                }
                else
                {
                    MessageBox.Show("No existen parametros");
                }
            }
            else
            {
                MessageBox.Show("Error de parametros");
            }

            if (Datos.Rows.Count == 0)
            {
                A_Dialogo.DialogoInformacion("No hay registros entre estas fechas, por favor selecciona otro rango de fechas");
            }
            else
            {
                // Crear columna
                DataGridViewImageColumn btnVerReporte = new DataGridViewImageColumn
                {
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                    Frozen = false,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
                    DividerWidth = 0,
                    FillWeight = 100,
                    MinimumWidth = 5,
                    Width = 1032
                };

                dgvResultados.Columns.Clear();
                dgvResultados.Columns.Add(btnVerReporte);
                dgvResultados.Columns[0].HeaderText = "Ver Reporte";

                dgvResultados.DataSource = Datos;
                dgvResultados.Columns[0].DisplayIndex = 5;

                // Mostrar cuadro de resumen
                pnInferior.Controls[1].Show();

                DataTable dtEstadisticos = (dgvResultados.DataSource as DataTable).Copy();
                dtEstadisticos.Rows.Clear();

                // Asistencias
                // Solo donde SesiónDictada es SI para los estadísticos
                foreach (DataRow row in Datos.Rows)
                {
                    if (row["SesiónDictada"].ToString() == "SI")
                    {
                        dtEstadisticos.ImportRow(row);
                    }
                }

                // Listas de valores
                List<double> Asistieron = dtEstadisticos.AsEnumerable().Select(x => Convert.ToDouble(x.Field<int>("TotalAsistieron"))).ToList();
                List<double> Faltaron = dtEstadisticos.AsEnumerable().Select(x => Convert.ToDouble(x.Field<int>("TotalFaltaron"))).ToList();

                foreach (int i in Asistieron)
                {
                    Console.WriteLine(i);
                }
                Console.WriteLine("-------------");

                foreach (int i in Faltaron)
                {
                    Console.WriteLine(i);
                }

                // Cuadro de resumen
                DataTable cuadroResumen = new DataTable();
                cuadroResumen.Columns.Add(" ");
                cuadroResumen.Columns.Add("Asistieron");
                cuadroResumen.Columns.Add("Faltaron");

                // Máximo
                cuadroResumen.Rows.Add("Máximo", Statistics.Maximum(Asistieron), Statistics.Maximum(Faltaron));

                // Mínimos
                cuadroResumen.Rows.Add("Mínimo", Statistics.Minimum(Asistieron), Statistics.Minimum(Faltaron));

                // Media
                cuadroResumen.Rows.Add("Media", Statistics.Mean(Asistieron), Statistics.Mean(Faltaron));

                // Mediana
                cuadroResumen.Rows.Add("Mediana", Statistics.Median(Asistieron), Statistics.Median(Faltaron));

                // Moda
                var modeAsistieron = Asistieron.GroupBy(a => a).OrderByDescending(b => b.Count()).Select(b => b.Key).FirstOrDefault();
                var modeFaltaron = Faltaron.GroupBy(a => a).OrderByDescending(b => b.Count()).Select(b => b.Key).FirstOrDefault();
                cuadroResumen.Rows.Add("Moda", modeAsistieron, modeFaltaron);

                // Varianza
                cuadroResumen.Rows.Add("Varianza", Statistics.Variance(Asistieron), Statistics.Variance(Faltaron));

                // Desviación Estándar
                var dvA = Statistics.StandardDeviation(Asistieron);
                var dvF = Statistics.StandardDeviation(Faltaron);
                cuadroResumen.Rows.Add("Desv. Estándar", String.Format("{0:0.00}", dvA), String.Format("{0:0.00}", dvF));

                dgvResumen.DataSource = cuadroResumen;

                // Gráficos
                // Gráfico 1
                tcGraficos.TabPages.Clear();

                Chart Grafico1 = new Chart
                {
                    Dock = DockStyle.Fill,
                    Palette = ChartColorPalette.Excel
                };

                Grafico1.Titles.Clear();
                Grafico1.Series.Clear();
                Grafico1.ChartAreas.Clear();
                Grafico1.Titles.Add("Asignatura" + " - " + CodAsignatura);
                Grafico1.Titles[0].Font = new Font("Montserrat Alternates", 13f);

                TabPage tpGrafico1 = new TabPage("Gráfico 1");
                tpGrafico1.Controls.Add(Grafico1);

                ChartArea areaGrafico1 = new ChartArea();

                // Propiedades de los ejes
                areaGrafico1.AxisX.Interval = 1;
                areaGrafico1.AxisX.Title = "Fecha";
                areaGrafico1.AxisX.TitleFont = new Font("Montserrat Alternates", 12f, FontStyle.Bold);
                areaGrafico1.AxisX.LabelStyle.Font = new Font("Montserrat Alternates", 11f);
                areaGrafico1.AxisY.Title = "Cantidad";
                areaGrafico1.AxisY.TitleFont = new Font("Montserrat Alternates", 12f, FontStyle.Bold);
                areaGrafico1.AxisY.LabelStyle.Font = new Font("Montserrat Alternates", 11f);
                areaGrafico1.AxisX.MajorGrid.Enabled = false;
                //areaGrafico2.AxisY.MajorGrid.Enabled = false;

                double MaxYAxis = 0;
                for (int i = 0; i < Asistieron.Count; i++)
                {
                    double temp = Asistieron.ElementAt(i) + Faltaron.ElementAt(i);
                    if (temp > MaxYAxis) MaxYAxis = temp;
                }
                areaGrafico1.AxisY.Maximum = (float)MaxYAxis;

                Grafico1.ChartAreas.Add(areaGrafico1);

                Series serie1Grafico1 = new Series("TotalAsistieron")
                {
                    ChartType = SeriesChartType.StackedBar,
                    XValueMember = "Fecha",
                    YValueMembers = "TotalAsistieron",
                    IsValueShownAsLabel = true,
                    MarkerSize = 14,
                    Font = new Font("Montserrat Alternates", 11f)
                };

                Series serie2Grafico1 = new Series("TotalFaltaron")
                {
                    ChartType = SeriesChartType.StackedBar,
                    XValueMember = "Fecha",
                    YValueMembers = "TotalFaltaron",
                    MarkerSize = 14,
                    Font = new Font("Montserrat Alternates", 11f)
                };

                // Leyenda
                Grafico1.Legends.Add(new Legend("Total Asistieron"));
                Grafico1.Legends["Total Asistieron"].Alignment = StringAlignment.Center;
                //Grafico1.Legends["Total Asistieron"].LegendStyle = LegendStyle.Column;
                Grafico1.Legends["Total Asistieron"].Docking = Docking.Bottom;
                Grafico1.Legends["Total Asistieron"].IsDockedInsideChartArea = false;
                Grafico1.Legends["Total Asistieron"].Font = new Font("Montserrat Alternates", 11f);

                Grafico1.Legends.Add(new Legend("Total Faltaron"));
                Grafico1.Legends["Total Faltaron"].Alignment = StringAlignment.Center;
                //Grafico1.Legends["Total Faltaron"].LegendStyle = LegendStyle.Column;
                Grafico1.Legends["Total Faltaron"].Docking = Docking.Bottom;
                Grafico1.Legends["Total Faltaron"].IsDockedInsideChartArea = false;
                Grafico1.Legends["Total Faltaron"].Font = new Font("Montserrat Alternates", 11f);

                serie1Grafico1.Legend = "Total Asistieron";
                serie2Grafico1.Legend = "Total Faltaron";

                Grafico1.Series.Add(serie1Grafico1);
                Grafico1.Series.Add(serie2Grafico1);

                Grafico1.DataSource = dtEstadisticos;

                tcGraficos.TabPages.Add(tpGrafico1);

                // Gráfico 2
                Chart Grafico2 = new Chart
                {
                    Dock = DockStyle.Fill,
                    Palette = ChartColorPalette.Excel,
                };

                TabPage tpGrafico2 = new TabPage("Gráfico 2");
                tpGrafico2.Controls.Add(Grafico2);

                Grafico2.Titles.Clear();
                Grafico2.Series.Clear();
                Grafico2.ChartAreas.Clear();
                Grafico2.Titles.Add("Evolución de las Asistencias" + " - " + CodAsignatura);

                ChartArea areaGrafico2 = new ChartArea();

                // Propiedades de los ejes
                areaGrafico2.AxisX.Interval = 1;
                areaGrafico2.AxisX.Title = "Fecha";
                areaGrafico2.AxisX.TitleFont = new Font("Montserrat Alternates", 12f, FontStyle.Bold);
                areaGrafico2.AxisX.LabelStyle.Font = new Font("Montserrat Alternates", 11f);
                areaGrafico2.AxisY.Title = "Porcentaje de Asistencia";
                areaGrafico2.AxisY.TitleFont = new Font("Montserrat Alternates", 12f, FontStyle.Bold);
                areaGrafico2.AxisY.LabelStyle.Font = new Font("Montserrat Alternates", 11f);
                areaGrafico2.AxisX.MajorGrid.Enabled = false;
                //areaGrafico2.AxisY.MajorGrid.Enabled = false;

                Grafico2.ChartAreas.Add(areaGrafico2);

                // Hacer porcentajes
                int n = (int)(Asistieron.ElementAt(0) + Faltaron.ElementAt(0));

                DataTable dtGrafico2 = dtEstadisticos.Copy();

                foreach (DataRow row in dtGrafico2.Rows)
                {
                    row["TotalAsistieron"] = Convert.ToDouble(row["TotalAsistieron"]) / n * 100;
                }

                Series serie1Grafico2 = new Series("Porcentaje")
                {
                    ChartType = SeriesChartType.Line,
                    XValueMember = "Fecha",
                    YValueMembers = "TotalAsistieron",
                    IsValueShownAsLabel = true,
                    MarkerSize = 14,
                    Font = new Font("Montserrat Alternates", 11f),
                    BorderWidth = 3
                };

                Grafico2.Series.Add(serie1Grafico2);

                Grafico2.DataSource = dtGrafico2;

                tcGraficos.TabPages.Add(tpGrafico2);
            }
        }

        private void C_ReporteA_Resize(object sender, EventArgs e)
        {
            int AnchoTotal = 0;
            int Filas = 1;

            foreach (C_Campo cpControl in this.pnSubcampos.Controls)
            {
                if ((AnchoTotal + cpControl.Width + 6) > this.pnSubcampos.Width)
                {
                    Filas++;
                    AnchoTotal = cpControl.Width + 6;
                }
                else
                {
                    AnchoTotal += cpControl.Width + 6;
                }
            }

            this.Cuadricula.RowStyles[0].Height = Filas * 92 + 51;
            this.Height = (int)this.Cuadricula.RowStyles[0].Height + (int)this.Cuadricula.RowStyles[1].Height + 73;
        }

        public void fnReporte1(string Titulo, string[] Titulos, string[] Valores, DataTable Datos, string CriterioAsistenciasEstudiantes, string CodAsignatura)
        {
            this.CriterioAsistenciasEstudiantes = CriterioAsistenciasEstudiantes;

            LimpiarCampos();

            lblTitulo.Text = Titulo;

            if (Titulos.Length.Equals(Valores.Length))
            {
                if (Titulos.Length != 0)
                {
                    for (int K = 0; K < Titulos.Length; K++)
                    {
                        C_Campo Nuevo = new C_Campo(Titulos[K], Valores[K]);
                        pnSubcampos.Controls.Add(Nuevo);
                    }
                }
                else
                {
                    MessageBox.Show("No existen parametros");
                }
            }
            else
            {
                MessageBox.Show("Error de parametros");
            }

            if (Datos.Rows.Count == 0)
            {
                A_Dialogo.DialogoInformacion("No hay registros entre estas fechas, por favor selecciona otro rango de fechas");

                lblTitulo.Text = "";
                pnSubcampos.Controls.Clear();
                dgvResumen.Columns.Clear();
                dgvResultados.Columns.Clear();
                dgvResultados.Refresh();

                tcGraficos.Controls.Clear();
            }
            else
            {
                // Crear columna
                DataGridViewImageColumn btnVerReporte = new DataGridViewImageColumn
                {
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                    Frozen = false,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
                    DividerWidth = 0,
                    FillWeight = 100,
                    MinimumWidth = 5,
                    Width = 1032
                };

                dgvResultados.Columns.Clear();
                dgvResultados.Columns.Add(btnVerReporte);
                dgvResultados.Columns[0].HeaderText = "Ver Reporte";

                dgvResultados.DataSource = Datos;
                dgvResultados.Columns[0].DisplayIndex = 5;

                // Mostrar cuadro de resumen
                pnInferior.Controls[1].Show();

                DataTable dtEstadisticos = (dgvResultados.DataSource as DataTable).Copy();
                dtEstadisticos.Rows.Clear();

                // Asistencias
                // Solo donde SesiónDictada es SI para los estadísticos
                foreach (DataRow row in Datos.Rows)
                {
                    if (row["SesiónDictada"].ToString() == "SI")
                    {
                        dtEstadisticos.ImportRow(row);
                    }
                }

                // Listas de valores
                List<double> Asistieron = dtEstadisticos.AsEnumerable().Select(x => Convert.ToDouble(x.Field<int>("TotalAsistieron"))).ToList();
                List<double> Faltaron = dtEstadisticos.AsEnumerable().Select(x => Convert.ToDouble(x.Field<int>("TotalFaltaron"))).ToList();

                foreach (int i in Asistieron)
                {
                    Console.WriteLine(i);
                }
                Console.WriteLine("-------------");

                foreach (int i in Faltaron)
                {
                    Console.WriteLine(i);
                }

                // Cuadro de resumen
                DataTable cuadroResumen = new DataTable();
                cuadroResumen.Columns.Add(" ");
                cuadroResumen.Columns.Add("Asistieron");
                cuadroResumen.Columns.Add("Faltaron");

                // Máximo
                cuadroResumen.Rows.Add("Máximo", Statistics.Maximum(Asistieron), Statistics.Maximum(Faltaron));

                // Mínimos
                cuadroResumen.Rows.Add("Mínimo", Statistics.Minimum(Asistieron), Statistics.Minimum(Faltaron));

                // Media
                cuadroResumen.Rows.Add("Media", Statistics.Mean(Asistieron), Statistics.Mean(Faltaron));

                // Mediana
                cuadroResumen.Rows.Add("Mediana", Statistics.Median(Asistieron), Statistics.Median(Faltaron));

                // Moda
                var modeAsistieron = Asistieron.GroupBy(a => a).OrderByDescending(b => b.Count()).Select(b => b.Key).FirstOrDefault();
                var modeFaltaron = Faltaron.GroupBy(a => a).OrderByDescending(b => b.Count()).Select(b => b.Key).FirstOrDefault();
                cuadroResumen.Rows.Add("Moda", modeAsistieron, modeFaltaron);

                // Varianza
                cuadroResumen.Rows.Add("Varianza", Statistics.Variance(Asistieron), Statistics.Variance(Faltaron));

                // Desviación Estándar
                var dvA = Statistics.StandardDeviation(Asistieron);
                var dvF = Statistics.StandardDeviation(Faltaron);
                cuadroResumen.Rows.Add("Desv. Estándar", String.Format("{0:0.00}", dvA), String.Format("{0:0.00}", dvF));

                dgvResumen.DataSource = cuadroResumen;

                // Gráficos
                // Gráfico 1
                tcGraficos.TabPages.Clear();

                Chart Grafico1 = new Chart
                {
                    Dock = DockStyle.Fill,
                    Palette = ChartColorPalette.Excel
                };

                Grafico1.Titles.Clear();
                Grafico1.Series.Clear();
                Grafico1.ChartAreas.Clear();
                Grafico1.Titles.Add("Asignatura" + " - " + CodAsignatura);
                Grafico1.Titles[0].Font = new Font("Montserrat Alternates", 13f);

                TabPage tpGrafico1 = new TabPage("Gráfico 1");
                tpGrafico1.Controls.Add(Grafico1);

                ChartArea areaGrafico1 = new ChartArea();

                // Propiedades de los ejes
                areaGrafico1.AxisX.Interval = 1;
                areaGrafico1.AxisX.Title = "Fecha";
                areaGrafico1.AxisX.TitleFont = new Font("Montserrat Alternates", 12f, FontStyle.Bold);
                areaGrafico1.AxisX.LabelStyle.Font = new Font("Montserrat Alternates", 11f);
                areaGrafico1.AxisY.Title = "Cantidad";
                areaGrafico1.AxisY.TitleFont = new Font("Montserrat Alternates", 12f, FontStyle.Bold);
                areaGrafico1.AxisY.LabelStyle.Font = new Font("Montserrat Alternates", 11f);
                areaGrafico1.AxisX.MajorGrid.Enabled = false;
                //areaGrafico2.AxisY.MajorGrid.Enabled = false;

                double MaxYAxis = 0;
                for (int i = 0; i < Asistieron.Count; i++)
                {
                    double temp = Asistieron.ElementAt(i) + Faltaron.ElementAt(i);
                    if (temp > MaxYAxis) MaxYAxis = temp;
                }
                areaGrafico1.AxisY.Maximum = (float)MaxYAxis;

                Grafico1.ChartAreas.Add(areaGrafico1);

                Series serie1Grafico1 = new Series("TotalAsistieron")
                {
                    ChartType = SeriesChartType.StackedBar,
                    XValueMember = "Fecha",
                    YValueMembers = "TotalAsistieron",
                    IsValueShownAsLabel = true,
                    MarkerSize = 14,
                    Font = new Font("Montserrat Alternates", 11f)
                };

                Series serie2Grafico1 = new Series("TotalFaltaron")
                {
                    ChartType = SeriesChartType.StackedBar,
                    XValueMember = "Fecha",
                    YValueMembers = "TotalFaltaron",
                    MarkerSize = 14,
                    Font = new Font("Montserrat Alternates", 11f)
                };

                // Leyenda
                Grafico1.Legends.Add(new Legend("Total Asistieron"));
                Grafico1.Legends["Total Asistieron"].Alignment = StringAlignment.Center;
                //Grafico1.Legends["Total Asistieron"].LegendStyle = LegendStyle.Column;
                Grafico1.Legends["Total Asistieron"].Docking = Docking.Bottom;
                Grafico1.Legends["Total Asistieron"].IsDockedInsideChartArea = false;
                Grafico1.Legends["Total Asistieron"].Font = new Font("Montserrat Alternates", 11f);

                Grafico1.Legends.Add(new Legend("Total Faltaron"));
                Grafico1.Legends["Total Faltaron"].Alignment = StringAlignment.Center;
                //Grafico1.Legends["Total Faltaron"].LegendStyle = LegendStyle.Column;
                Grafico1.Legends["Total Faltaron"].Docking = Docking.Bottom;
                Grafico1.Legends["Total Faltaron"].IsDockedInsideChartArea = false;
                Grafico1.Legends["Total Faltaron"].Font = new Font("Montserrat Alternates", 11f);

                serie1Grafico1.Legend = "Total Asistieron";
                serie2Grafico1.Legend = "Total Faltaron";

                Grafico1.Series.Add(serie1Grafico1);
                Grafico1.Series.Add(serie2Grafico1);

                Grafico1.DataSource = dtEstadisticos;

                tcGraficos.TabPages.Add(tpGrafico1);

                // Gráfico 2
                Chart Grafico2 = new Chart
                {
                    Dock = DockStyle.Fill,
                    Palette = ChartColorPalette.Excel,
                };

                TabPage tpGrafico2 = new TabPage("Gráfico 2");
                tpGrafico2.Controls.Add(Grafico2);

                Grafico2.Titles.Clear();
                Grafico2.Series.Clear();
                Grafico2.ChartAreas.Clear();
                Grafico2.Titles.Add("Evolución de las Asistencias" + " - " + CodAsignatura);

                ChartArea areaGrafico2 = new ChartArea();

                // Propiedades de los ejes
                areaGrafico2.AxisX.Interval = 1;
                areaGrafico2.AxisX.Title = "Fecha";
                areaGrafico2.AxisX.TitleFont = new Font("Montserrat Alternates", 12f, FontStyle.Bold);
                areaGrafico2.AxisX.LabelStyle.Font = new Font("Montserrat Alternates", 11f);
                areaGrafico2.AxisY.Title = "Porcentaje de Asistencia";
                areaGrafico2.AxisY.TitleFont = new Font("Montserrat Alternates", 12f, FontStyle.Bold);
                areaGrafico2.AxisY.LabelStyle.Font = new Font("Montserrat Alternates", 11f);
                areaGrafico2.AxisX.MajorGrid.Enabled = false;
                //areaGrafico2.AxisY.MajorGrid.Enabled = false;

                Grafico2.ChartAreas.Add(areaGrafico2);

                // Hacer porcentajes
                int n = (int)(Asistieron.ElementAt(0) + Faltaron.ElementAt(0));

                DataTable dtGrafico2 = dtEstadisticos.Copy();

                foreach (DataRow row in dtGrafico2.Rows)
                {
                    row["TotalAsistieron"] = Convert.ToDouble(row["TotalAsistieron"]) / n * 100;
                }

                Series serie1Grafico2 = new Series("Porcentaje")
                {
                    ChartType = SeriesChartType.Line,
                    XValueMember = "Fecha",
                    YValueMembers = "TotalAsistieron",
                    IsValueShownAsLabel = true,
                    MarkerSize = 14,
                    Font = new Font("Montserrat Alternates", 11f),
                    BorderWidth = 3
                };

                Grafico2.Series.Add(serie1Grafico2);

                Grafico2.DataSource = dtGrafico2;

                tcGraficos.TabPages.Add(tpGrafico2);
            }
        }

        public void fnReporte3(string Titulo, string[] Titulos, string[] Valores, DataTable Datos, string CriterioAsistenciasEstudiantes, string CodAsignatura)
        {
            this.CriterioAsistenciasEstudiantes = CriterioAsistenciasEstudiantes;

            LimpiarCampos();

            lblTitulo.Text = Titulo;

            if (Titulos.Length.Equals(Valores.Length))
            {
                if (Titulos.Length != 0)
                {
                    for (int K = 0; K < Titulos.Length; K++)
                    {
                        C_Campo Nuevo = new C_Campo(Titulos[K], Valores[K]);
                        pnSubcampos.Controls.Add(Nuevo);
                    }
                }
                else
                {
                    MessageBox.Show("No existen parametros");
                }
            }
            else
            {
                MessageBox.Show("Error de parametros");
            }

            if (Datos.Rows.Count == 0)
            {
                A_Dialogo.DialogoInformacion("No hay registros entre estas fechas, por favor selecciona otro rango de fechas");

                lblTitulo.Text = "";
                pnSubcampos.Controls.Clear();
                dgvResumen.Columns.Clear();
                dgvResultados.Columns.Clear();
                dgvResultados.Refresh();

                tcGraficos.Controls.Clear();
            }
            else
            {
                // Crear columna
                DataGridViewImageColumn btnVerReporte = new DataGridViewImageColumn
                {
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                    Frozen = false,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
                    DividerWidth = 0,
                    FillWeight = 100,
                    MinimumWidth = 5,
                    Width = 1032
                };

                dgvResultados.Columns.Clear();
                dgvResultados.Columns.Add(btnVerReporte);
                dgvResultados.Columns[0].HeaderText = "Ver Reporte";

                dgvResultados.DataSource = Datos;
                dgvResultados.Columns[1].Visible = false;
                dgvResultados.Columns[0].DisplayIndex = 7;

                // Ocultar cuadro de resumen
                pnInferior.Controls[1].Hide();

                // Gráficos
                // Gráfico 1
                tcGraficos.TabPages.Clear();

                Chart Grafico1 = new Chart
                {
                    Dock = DockStyle.Fill,
                    Palette = ChartColorPalette.Excel
                };

                TabPage tpGrafico1 = new TabPage("Gráfico 1");
                tpGrafico1.Controls.Add(Grafico1);

                Grafico1.Titles.Clear();
                Grafico1.Series.Clear();
                Grafico1.ChartAreas.Clear();

                Grafico1.Titles.Add("Asignatura" + " - " + CodAsignatura);

                ChartArea areaGrafico1 = new ChartArea();

                // Propiedades de los ejes
                areaGrafico1.AxisX.Interval = 1;
                areaGrafico1.AxisX.Title = "Cód. Estudiante";
                areaGrafico1.AxisX.TitleFont = new Font("Montserrat Alternates", 12f, FontStyle.Bold);
                areaGrafico1.AxisX.LabelStyle.Font = new Font("Montserrat Alternates", 14f);
                areaGrafico1.AxisY.Title = "Cantidad";
                areaGrafico1.AxisY.TitleFont = new Font("Montserrat Alternates", 12f, FontStyle.Bold);
                areaGrafico1.AxisY.LabelStyle.Font = new Font("Montserrat Alternates", 11f);
                areaGrafico1.AxisX.MajorGrid.LineColor = Color.Red;
                areaGrafico1.AxisX.MajorGrid.Enabled = false;
                //areaGrafico2.AxisY.MajorGrid.Enabled = false;

                Grafico1.ChartAreas.Add(areaGrafico1);

                Series serie1 = new Series("TotalAsistencias")
                {
                    ChartType = SeriesChartType.StackedBar,
                    XValueMember = "CodEstudiante",
                    YValueMembers = "TotalAsistencias",
                    IsValueShownAsLabel = true,
                    MarkerSize = 14,
                    Font = new Font("Montserrat Alternates", 10f)
                };

                Series serie2 = new Series("TotalFaltas")
                {
                    ChartType = SeriesChartType.StackedBar,
                    XValueMember = "CodEstudiante",
                    YValueMembers = "TotalFaltas",
                    MarkerSize = 14,
                    Font = new Font("Montserrat Alternates", 11f)
                };

                Grafico1.Series.Add(serie1);
                Grafico1.Series.Add(serie2);

                Grafico1.DataSource = dgvResultados.DataSource;

                tcGraficos.TabPages.Add(tpGrafico1);
            }
        }

        public void fnReporte5(string Titulo, string[] Titulos, string[] Valores, DataTable Datos, string CodAsignatura)
        {
            // Limpiar los Antiguos Reportes
            LimpiarCampos();

            #region ===================== TÍTULO =====================
            // Cambiar el Título
            lblTitulo.Text = Titulo;
            #endregion ===================== TÍTULO =====================

            #region ===================== DESCRIPCIÓN =====================
            // Verificar que los Títulos y los Valores dados Coincidan
            if (Titulos.Length.Equals(Valores.Length))
            {
                if (Titulos.Length != 0)
                {
                    for (int K = 0; K < Titulos.Length; K++)
                    {
                        C_Campo Nuevo = new C_Campo(Titulos[K], Valores[K]);
                        pnSubcampos.Controls.Add(Nuevo);
                    }
                }
                else
                {
                    MessageBox.Show("No existen parametros");
                }
            }
            else
            {
                MessageBox.Show("Error de parametros");
            }
            #endregion ===================== DESCRIPCIÓN =====================

            // Validar las Fechas dadas
            if (Datos.Rows.Count == 0)
            {
                A_Dialogo.DialogoInformacion("No hay registros entre estas fechas, por favor selecciona otro rango de fechas");

                lblTitulo.Text = "";
                pnSubcampos.Controls.Clear();
                dgvResumen.Columns.Clear();
                dgvResultados.Columns.Clear();
                dgvResultados.Refresh();

                tcGraficos.Controls.Clear();
            }
            else
            {
                #region ===================== CUADRO DE RESULTADOS =====================
                dgvResultados.Columns.Clear();
                dgvResultados.DataSource = Datos;
                dgvResultados.Columns[0].HeaderText = "Sesión";
                dgvResultados.Columns[1].HeaderText = "Tema";
                dgvResultados.Columns[2].HeaderText = "Fecha";
                #endregion ===================== CUADRO DE RESULTADOS =====================

                /*
                #region ===================== CUADRO DE RESUMEN =====================
                DataTable cuadroResumen = new DataTable();
                cuadroResumen.Columns.Add(" ");
                cuadroResumen.Columns.Add("Asistieron");
                cuadroResumen.Columns.Add("Faltaron");

                // Máximo
                cuadroResumen.Rows.Add("Máximo", Statistics.Maximum(Asistieron), Statistics.Maximum(Faltaron));

                // Mínimos
                cuadroResumen.Rows.Add("Mínimo", Statistics.Minimum(Asistieron), Statistics.Minimum(Faltaron));

                // Media
                cuadroResumen.Rows.Add("Media", Statistics.Mean(Asistieron), Statistics.Mean(Faltaron));

                // Mediana
                cuadroResumen.Rows.Add("Mediana", Statistics.Median(Asistieron), Statistics.Median(Faltaron));

                // Moda
                var modeAsistieron = Asistieron.GroupBy(a => a).OrderByDescending(b => b.Count()).Select(b => b.Key).FirstOrDefault();
                var modeFaltaron = Faltaron.GroupBy(a => a).OrderByDescending(b => b.Count()).Select(b => b.Key).FirstOrDefault();
                cuadroResumen.Rows.Add("Moda", modeAsistieron, modeFaltaron);

                // Varianza
                cuadroResumen.Rows.Add("Varianza", Statistics.Variance(Asistieron), Statistics.Variance(Faltaron));

                // Desviación Estándar
                var dvA = Statistics.StandardDeviation(Asistieron);
                var dvF = Statistics.StandardDeviation(Faltaron);
                cuadroResumen.Rows.Add("Desv. Estándar", String.Format("{0:0.00}", dvA), String.Format("{0:0.00}", dvF));

                dgvResumen.DataSource = cuadroResumen;
                #endregion ===================== CUADRO DE RESUMEN =====================
                */

                /*
                dgvResultados.Columns.Clear();
                dgvResultados.Columns.Add(btnVerReporte);

                dgvResultados.DataSource = Datos;
                

                // Ocultar cuadro de resumen
                pnInferior.Controls[1].Hide();

                // GRÁFICOS

                // Gráfico 1
                tcGraficos.TabPages.Clear();

                Chart Grafico1 = new Chart
                {
                    Dock = DockStyle.Fill,
                    Palette = ChartColorPalette.Excel
                };

                TabPage tpGrafico1 = new TabPage("Gráfico 1");
                tpGrafico1.Controls.Add(Grafico1);

                Grafico1.Titles.Clear();
                Grafico1.Series.Clear();
                Grafico1.ChartAreas.Clear();

                Grafico1.Titles.Add("Asignatura" + " - " + CodAsignatura);

                ChartArea areaGrafico1 = new ChartArea();

                // Propiedades de los ejes
                areaGrafico1.AxisX.Interval = 1;
                areaGrafico1.AxisX.Title = "Cód. Estudiante";
                areaGrafico1.AxisX.TitleFont = new Font("Montserrat Alternates", 12f, FontStyle.Bold);
                areaGrafico1.AxisX.LabelStyle.Font = new Font("Montserrat Alternates", 14f);
                areaGrafico1.AxisY.Title = "Cantidad";
                areaGrafico1.AxisY.TitleFont = new Font("Montserrat Alternates", 12f, FontStyle.Bold);
                areaGrafico1.AxisY.LabelStyle.Font = new Font("Montserrat Alternates", 11f);
                areaGrafico1.AxisX.MajorGrid.LineColor = Color.Red;
                areaGrafico1.AxisX.MajorGrid.Enabled = false;
                //areaGrafico2.AxisY.MajorGrid.Enabled = false;

                Grafico1.ChartAreas.Add(areaGrafico1);

                Series serie1 = new Series("TotalAsistencias")
                {
                    ChartType = SeriesChartType.StackedBar,
                    XValueMember = "CodEstudiante",
                    YValueMembers = "TotalAsistencias",
                    IsValueShownAsLabel = true,
                    MarkerSize = 14,
                    Font = new Font("Montserrat Alternates", 10f)
                };

                Series serie2 = new Series("TotalFaltas")
                {
                    ChartType = SeriesChartType.StackedBar,
                    XValueMember = "CodEstudiante",
                    YValueMembers = "TotalFaltas",
                    MarkerSize = 14,
                    Font = new Font("Montserrat Alternates", 11f)
                };

                Grafico1.Series.Add(serie1);
                Grafico1.Series.Add(serie2);

                Grafico1.DataSource = dgvResultados.DataSource;

                tcGraficos.TabPages.Add(tpGrafico1);
                */
            }
        }

        public void fnReporte6(string Titulo, string[] Titulos, string[] Valores, DataTable Datos, string CodAsignatura)
        {
            // Limpiar los Antiguos Reportes
            LimpiarCampos();

            #region ===================== TÍTULO =====================
            // Cambiar el Título
            lblTitulo.Text = Titulo;
            #endregion ===================== TÍTULO =====================

            #region ===================== DESCRIPCIÓN =====================
            // Verificar que los Títulos y los Valores dados Coincidan
            if (Titulos.Length.Equals(Valores.Length))
            {
                if (Titulos.Length != 0)
                {
                    for (int K = 0; K < Titulos.Length; K++)
                    {
                        C_Campo Nuevo = new C_Campo(Titulos[K], Valores[K]);
                        pnSubcampos.Controls.Add(Nuevo);
                    }
                }
                else
                {
                    MessageBox.Show("No existen parametros");
                }
            }
            else
            {
                MessageBox.Show("Error de parametros");
            }
            #endregion ===================== DESCRIPCIÓN =====================

            // Validar las Fechas dadas
            if (Datos.Rows.Count == 0)
            {
                A_Dialogo.DialogoInformacion("No hay registros entre estas fechas, por favor selecciona otro rango de fechas");

                lblTitulo.Text = "";
                pnSubcampos.Controls.Clear();
                dgvResumen.Columns.Clear();
                dgvResultados.Columns.Clear();
                dgvResultados.Refresh();

                tcGraficos.Controls.Clear();
            }
            else
            {
                #region ===================== CUADRO DE RESULTADOS =====================
                dgvResultados.Columns.Clear();
                dgvResultados.DataSource = Datos;
                dgvResultados.Columns[0].HeaderText = "Código Asignatura";
                dgvResultados.Columns[1].HeaderText = "Nombre Asignatura";
                dgvResultados.Columns[2].HeaderText = "Temas Avanzados";
                #endregion ===================== CUADRO DE RESULTADOS =====================

                #region ===================== CUADRO DE RESUMEN =====================
                // Ocultar cuadro de resumen
                pnInferior.Controls[1].Hide();
                #endregion ===================== CUADRO DE RESUMEN =====================

                /*
                #region ===================== GRÁFICO =====================
                // Gráfico 1
                tcGraficos.TabPages.Clear();

                Chart Grafico1 = new Chart
                {
                    Dock = DockStyle.Fill,
                    Palette = ChartColorPalette.Excel
                };

                TabPage tpGrafico1 = new TabPage("Gráfico 1");
                tpGrafico1.Controls.Add(Grafico1);

                Grafico1.Titles.Clear();
                Grafico1.Series.Clear();
                Grafico1.ChartAreas.Clear();

                Grafico1.Titles.Add("Avance de Asignatura");

                ChartArea areaGrafico1 = new ChartArea();

                // Propiedades de los ejes
                areaGrafico1.AxisX.Interval = 1;
                areaGrafico1.AxisX.Title = "Cód. Estudiante";
                areaGrafico1.AxisX.TitleFont = new Font("Montserrat Alternates", 12f, FontStyle.Bold);
                areaGrafico1.AxisX.LabelStyle.Font = new Font("Montserrat Alternates", 14f);
                areaGrafico1.AxisY.Title = "Cantidad";
                areaGrafico1.AxisY.TitleFont = new Font("Montserrat Alternates", 12f, FontStyle.Bold);
                areaGrafico1.AxisY.LabelStyle.Font = new Font("Montserrat Alternates", 11f);
                areaGrafico1.AxisX.MajorGrid.LineColor = Color.Red;
                areaGrafico1.AxisX.MajorGrid.Enabled = false;
                //areaGrafico2.AxisY.MajorGrid.Enabled = false;

                Grafico1.ChartAreas.Add(areaGrafico1);

                Series serie1 = new Series("TotalAsistencias")
                {
                    ChartType = SeriesChartType.StackedBar,
                    XValueMember = "CodEstudiante",
                    YValueMembers = "TotalAsistencias",
                    IsValueShownAsLabel = true,
                    MarkerSize = 14,
                    Font = new Font("Montserrat Alternates", 10f)
                };

                Series serie2 = new Series("TotalFaltas")
                {
                    ChartType = SeriesChartType.StackedBar,
                    XValueMember = "CodEstudiante",
                    YValueMembers = "TotalFaltas",
                    MarkerSize = 14,
                    Font = new Font("Montserrat Alternates", 11f)
                };

                Grafico1.Series.Add(serie1);
                Grafico1.Series.Add(serie2);

                Grafico1.DataSource = dgvResultados.DataSource;

                tcGraficos.TabPages.Add(tpGrafico1);
                #endregion ===================== GRÁFICO =====================
                */
            }
        }

        private void dgvResultados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex >= 0) && (e.ColumnIndex == 0))
            {
                if (CriterioAsistenciasEstudiantes == "Por Fechas")
                {
                    MessageBox.Show("Por fechas");
                }
                if (CriterioAsistenciasEstudiantes == "Por Estudiantes")
                {
                    MessageBox.Show("Por Estudiantes");
                }
            }
        }
    }
}