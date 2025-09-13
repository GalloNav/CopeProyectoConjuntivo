using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;



namespace CopeProyectoConjuntivo
{
    public partial class Errores : Form
    {
        private Panel headerPanel;
        private Panel contentPanel;
        private Label titleLabel;
        private Label subtitleLabel;

        // Controles personalizados
        private TextBox customTxtReal;
        private TextBox customTxtAprox;
        private TextBox customTxtAbs;
        private TextBox customTxtRel;
        private TextBox customTxtRelPct;
        private NumericUpDown customNumDecimales;
        private Button customBtnCalcular;
        private Button customBtnLimpiar;

        // Labels personalizados
        private Label lblReal;
        private Label lblAprox;
        private Label lblAbs;
        private Label lblRel;
        private Label lblRelPct;
        private Label lblDecimales;

        public Errores()
        {
            InitializeComponent();
            HideOriginalControls();
            CreateModernInterface();
            SetupEventHandlers();
            SetStatus("🚀 Listo para calcular errores");
        }

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HTCAPTION = 0x2;

        [DllImport("user32.dll")]
        static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private void HideOriginalControls()
        {
            // Ocultar controles originales del diseñador
            foreach (Control control in this.Controls)
            {
                if (!(control is StatusStrip))
                {
                    control.Visible = false;
                }
            }
        }

        private void CreateModernInterface()
        {
            // Configuración general del formulario
            this.BackColor = Color.FromArgb(15, 23, 42);
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(520, 680);
            this.StartPosition = FormStartPosition.CenterScreen;

            CreateHeader();
            CreateContentPanel();
            CreateCustomControls();
            EnableNativeHeaderDrag();   

        }

        private void CreateHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 96,
                BackColor = Color.Transparent
            };
            headerPanel.Paint += HeaderPanel_Paint;

            titleLabel = new Label
            {
                Text = "⚡ CALCULADORA DE ERRORES",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(20, 18)
            };

            subtitleLabel = new Label
            {
                Text = "Análisis Numérico Profesional",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(20, 52)
            };

            var closeButton = new Button
            {
                Text = "✕",
                Size = new Size(32, 32),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(148, 163, 184),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Location = new Point(this.ClientSize.Width - closeButton.Width - 8, 8);
            closeButton.Click += (s, e) => this.Close();
            closeButton.MouseEnter += (s, e) => closeButton.ForeColor = Color.FromArgb(239, 68, 68);
            closeButton.MouseLeave += (s, e) => closeButton.ForeColor = Color.FromArgb(148, 163, 184);

            // Añade al header (no al form)
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);
            headerPanel.Controls.Add(closeButton);

            // Inserta el header en el form
            this.Controls.Add(headerPanel);
        }


        private void CreateContentPanel()
        {
            // Panel de contenido
            contentPanel = new Panel
            {
                Size = new Size(this.Width - 30, this.Height - 110),
                Location = new Point(15, 105),
                BackColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(25)
            };
            contentPanel.Paint += ContentPanel_Paint;
            this.Controls.Add(contentPanel);
        }

        private void CreateCustomControls()
        {
            int yPos = 20;
            int labelHeight = 25;
            int controlHeight = 35;
            int spacing = 15;

            // Valor real
            lblReal = CreateLabel("💯 Valor Real", new Point(25, yPos), Color.FromArgb(34, 197, 94));
            customTxtReal = CreateTextBox(new Point(25, yPos + labelHeight), "Ingrese el valor real...");
            yPos += labelHeight + controlHeight + spacing;

            // Valor aproximado
            lblAprox = CreateLabel("🔢 Valor Aproximado", new Point(25, yPos), Color.FromArgb(59, 130, 246));
            customTxtAprox = CreateTextBox(new Point(25, yPos + labelHeight), "Ingrese el valor aproximado...");
            yPos += labelHeight + controlHeight + spacing;

            // Decimales
            lblDecimales = CreateLabel("⚙️ Decimales a mostrar", new Point(330, yPos - 75), Color.FromArgb(156, 163, 175));
            customNumDecimales = CreateNumericUpDown(new Point(330, yPos - 50));

            // Error absoluto
            lblAbs = CreateLabel("📊 Error Absoluto", new Point(25, yPos), Color.FromArgb(168, 85, 247));
            customTxtAbs = CreateReadOnlyTextBox(new Point(25, yPos + labelHeight));
            yPos += labelHeight + controlHeight + spacing;

            // Error relativo
            lblRel = CreateLabel("📈 Error Relativo", new Point(25, yPos), Color.FromArgb(236, 72, 153));
            customTxtRel = CreateReadOnlyTextBox(new Point(25, yPos + labelHeight));
            yPos += labelHeight + controlHeight + spacing;

            // Error relativo porcentual
            lblRelPct = CreateLabel("📋 Error Relativo (%)", new Point(25, yPos), Color.FromArgb(245, 158, 11));
            customTxtRelPct = CreateReadOnlyTextBox(new Point(25, yPos + labelHeight));
            yPos += labelHeight + controlHeight + spacing * 2;

            // Botones
            customBtnCalcular = CreateButton("🚀 CALCULAR", new Point(25, yPos), Color.FromArgb(34, 197, 94));
            customBtnLimpiar = CreateButton("🧹 LIMPIAR", new Point(200, yPos), Color.FromArgb(239, 68, 68));

            // Agregar eventos
            customBtnCalcular.Click += (s, e) => Calcular();
            customBtnLimpiar.Click += (s, e) => Limpiar();

            // Agregar controles al panel
            contentPanel.Controls.AddRange(new Control[] {
                lblReal, customTxtReal,
                lblAprox, customTxtAprox,
                lblDecimales, customNumDecimales,
                lblAbs, customTxtAbs,
                lblRel, customTxtRel,
                lblRelPct, customTxtRelPct,
                customBtnCalcular, customBtnLimpiar
            });
        }

        private Label CreateLabel(string text, Point location, Color color)
        {
            return new Label
            {
                Text = text,
                Location = location,
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = color,
                BackColor = Color.Transparent
            };
        }

        private TextBox CreateTextBox(Point location, string placeholder)
        {
            var textBox = new TextBox
            {
                Location = location,
                Size = new Size(280, 35),
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 11F, FontStyle.Regular),
                Text = placeholder
            };

            // Placeholder effect
            textBox.ForeColor = Color.FromArgb(100, 116, 139);
            textBox.Enter += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.White;
                }
                textBox.BackColor = Color.FromArgb(30, 41, 59);
            };

            textBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.FromArgb(100, 116, 139);
                }
                textBox.BackColor = Color.FromArgb(15, 23, 42);
            };

            return textBox;
        }

        private TextBox CreateReadOnlyTextBox(Point location)
        {
            return new TextBox
            {
                Location = location,
                Size = new Size(280, 35),
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = Color.FromArgb(148, 163, 184),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 11F, FontStyle.Regular),
                ReadOnly = true,
                TabStop = false
            };
        }

        private NumericUpDown CreateNumericUpDown(Point location)
        {
            return new NumericUpDown
            {
                Location = location,
                Size = new Size(70, 30),
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Minimum = 0,
                Maximum = 15,
                Value = 6
            };
        }

        private Button CreateButton(string text, Point location, Color backgroundColor)
        {
            var button = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(150, 45),
                BackColor = backgroundColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            button.FlatAppearance.BorderSize = 0;

            // Efectos hover
            Color originalColor = backgroundColor;
            Color hoverColor = ControlPaint.Light(backgroundColor, 0.2f);

            button.MouseEnter += (s, e) => button.BackColor = hoverColor;
            button.MouseLeave += (s, e) => button.BackColor = originalColor;

            return button;
        }

        private void HeaderPanel_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(
                headerPanel.ClientRectangle,
                Color.FromArgb(99, 102, 241),
                Color.FromArgb(139, 92, 246),
                LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(brush, headerPanel.ClientRectangle);
            }
        }

        private void ContentPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, contentPanel.Width - 1, contentPanel.Height - 1);
            using (GraphicsPath path = GetRoundedRectangle(rect, 15))
            {
                using (SolidBrush brush = new SolidBrush(contentPanel.BackColor))
                {
                    g.FillPath(brush, path);
                }
                using (Pen pen = new Pen(Color.FromArgb(51, 65, 85), 2))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void EnableNativeHeaderDrag()
        {
            void Drag(object s, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                }
            }

            // Drag solo en el header y sus hijos (no en todo el form)
            headerPanel.MouseDown += Drag;
            foreach (Control c in headerPanel.Controls)
                c.MouseDown += Drag;
        }


        private void SetupEventHandlers()
        {
            this.KeyPreview = true;
            this.KeyDown += Errores_KeyDown;
        }

        private void Calcular()
        {
            errorProvider1?.Clear();
            SetStatus("Procesando…");

            var culture = CultureInfo.CurrentCulture;

            // Usa los custom si están visibles; si no, usa los originales del diseñador
            TextBox tbReal = (customTxtReal != null && customTxtReal.Visible) ? customTxtReal : txtReal;
            TextBox tbAprox = (customTxtAprox != null && customTxtAprox.Visible) ? customTxtAprox : txtAprox;
            TextBox tbAbs = (customTxtAbs != null && customTxtAbs.Visible) ? customTxtAbs : txtAbs;
            TextBox tbRel = (customTxtRel != null && customTxtRel.Visible) ? customTxtRel : txtRel;
            TextBox tbRelPct = (customTxtRelPct != null && customTxtRelPct.Visible) ? customTxtRelPct : txtRelPct;
            NumericUpDown nudDec = (customNumDecimales != null && customNumDecimales.Visible) ? customNumDecimales : numDecimales;

            // Parseo flexible: admite . o , como separador decimal
            if (!TryParseFlexible(tbReal.Text, culture, out double Vr))
            { errorProvider1?.SetError(tbReal, "Ingresa un número válido."); tbReal.Focus(); SetStatus("Valor real inválido"); return; }

            if (!TryParseFlexible(tbAprox.Text, culture, out double Va))
            { errorProvider1?.SetError(tbAprox, "Ingresa un número válido."); tbAprox.Focus(); SetStatus("Valor aproximado inválido"); return; }

            int dec = (int)nudDec.Value;

            // Formato siempre decimal (sin notación científica)
            string F(double v) => v.ToString("N" + dec, culture);

            double Ea = Math.Abs(Vr - Va);
            tbAbs.Text = F(Ea);

            if (Vr == 0.0)
            {
                tbRel.Text = "—";
                tbRelPct.Text = "—";
                SetStatus("Vr = 0 → error relativo indefinido");
                return;
            }

            double Er = Ea / Math.Abs(Vr);
            double ErPct = Er * 100.0;

            // Evita -0.0000 por redondeo
            if (Math.Abs(Er) < Math.Pow(10, -(dec + 1))) Er = 0.0;
            if (Math.Abs(ErPct) < Math.Pow(10, -(dec + 1))) ErPct = 0.0;

            tbRel.Text = F(Er);
            tbRelPct.Text = F(ErPct) + " %";

            SetStatus("¡Cálculo completado con éxito!");
        }

        // Helper: intenta con cultura actual, y si falla, normaliza . y , y prueba InvariantCulture
        private static bool TryParseFlexible(string s, CultureInfo culture, out double value)
        {
            if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, culture, out value))
                return true;

            // Normaliza separadores a la cultura actual
            string decSep = culture.NumberFormat.NumberDecimalSeparator;
            string normalized = s.Replace(",", decSep).Replace(".", decSep);

            if (double.TryParse(normalized, NumberStyles.Float | NumberStyles.AllowThousands, culture, out value))
                return true;

            return double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value);
        }



        private void Limpiar()
        {
            try
            {
                if (errorProvider1 != null) errorProvider1.Clear();

                ClearTextBoxWithPlaceholder(customTxtReal, "Ingrese el valor real...");
                ClearTextBoxWithPlaceholder(customTxtAprox, "Ingrese el valor aproximado...");

                customTxtAbs.Clear();
                customTxtRel.Clear();
                customTxtRelPct.Clear();
                customNumDecimales.Value = 6;
                SetStatus("🧹 ¡Formulario limpio y listo para la acción!");
                customTxtReal.Focus();
            }
            catch (Exception ex)
            {
                SetStatus($"❌ Error al limpiar: {ex.Message}");
            }
        }

        private string GetTextBoxValue(TextBox textBox, string placeholder)
        {
            return textBox.ForeColor == Color.FromArgb(100, 116, 139) && textBox.Text == placeholder ? "" : textBox.Text;
        }

        private void ClearTextBoxWithPlaceholder(TextBox textBox, string placeholder)
        {
            textBox.Text = placeholder;
            textBox.ForeColor = Color.FromArgb(100, 116, 139);
        }

        private static bool TryParseDouble(string s, CultureInfo culture, out double value)
        {
            return double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, culture, out value);
        }

        private void SetStatus(string msg)
        {
            try
            {
                // Buscar StatusLabel en el formulario
                foreach (Control control in this.Controls)
                {
                    if (control is StatusStrip statusStrip)
                    {
                        foreach (ToolStripItem item in statusStrip.Items)
                        {
                            if (item is ToolStripStatusLabel statusLabel)
                            {
                                statusLabel.Text = msg;

                                if (msg.Contains("problema") || msg.Contains("🚨"))
                                    statusLabel.ForeColor = Color.FromArgb(239, 68, 68);
                                else if (msg.Contains("🎉") || msg.Contains("éxito"))
                                    statusLabel.ForeColor = Color.FromArgb(34, 197, 94);
                                else if (msg.Contains("🌟"))
                                    statusLabel.ForeColor = Color.FromArgb(245, 158, 11);
                                else
                                    statusLabel.ForeColor = Color.FromArgb(148, 163, 184);

                                break;
                            }
                        }
                        break;
                    }
                }
            }
            catch
            {
                // Si hay error con el status, continuar silenciosamente
            }
        }

        private void Errores_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Calcular();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Limpiar();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F1)
            {
                MessageBox.Show("🚀 ¡Calculadora de Errores v2.0!\n\n" +
                               "Enter = Calcular\n" +
                               "Esc = Limpiar\n" +
                               "¡Hecha con amor y café! ☕",
                               "Ayuda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Handled = true;
            }
        }

        // Event handlers originales (vacíos para compatibilidad)
        private void label1_Click(object sender, EventArgs e) { }
        private void Errores_Load(object sender, EventArgs e) { }
        private void txtRelPct_TextChanged(object sender, EventArgs e) { }
        private void btnCalcular_Click(object sender, EventArgs e) => Calcular();
        private void btnLimpiar_Click(object sender, EventArgs e) => Limpiar();
    }
}
