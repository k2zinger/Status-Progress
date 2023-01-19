using System;
using System.Activities;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;


namespace UiPathTeam.StatusProgress.Activities
{
    [DisplayName("Modal DataTable Status"), Description("Displays the DataTable status form")]
    public class ModalDataTable : NativeActivity
    {

        #region Properties

        [Category("Input"), Description("Message to be displayed.  Default: Status Progress")]
        public InArgument<String> Message { get; set; } = new InArgument<String>("Status Progress");

        [Category("Input"), Description("Background opacity.  Value should be betwen 0 and 1.  Default: 0")]
        public InArgument<Double> Opacity { get; set; } = new InArgument<Double>(0.8);

        [Category("Input"), Description("Location on the screen.  Vertical options are \"center, top, bottom\" , and Horizontal options are \"center, right, and left\" .  Provide a string array indicating desired location.  Default: {\"center\",\"center\"}")]
        public InArgument<String[]> Location { get; set; }

        [Category("Input"), Description("Display time in seconds before closing automatically.  Set to zero to wait on user input.  Default: 0")]
        public InArgument<Double> DisplayTime { get; set; }

        [Category("Input"), Description("Progress to display.  Set between 0 and 1 to display progrss bar.  Negative (-1) will hide the progress bar.  Default: 0")]
        public InArgument<Double> Progress { get; set; } = new InArgument<Double>(-1);

        [Category("Input"), Description("Show control box. Default: False")]
        public InArgument<Boolean> Mobile { get; set; }

        [Category("Input"), Description("Bring progress bar to the front of all other windows.  Default: False")]
        public InArgument<Boolean> Top { get; set; } = new InArgument<Boolean>(true);

        [Category("Input"), Description("Border Color.  Default: System.Drawing.Color.OrangeRed")]
        public InArgument<Color> ColorBorder { get; set; }

        [Category("Input"), Description("Background Color.  Default: System.Drawing.Color.Black")]
        public InArgument<Color> ColorBackground { get; set; }

        [Category("Input"), Description("Text Color.  Default: System.Drawing.Color.White")]
        public InArgument<Color> ColorText { get; set; }

        [Category("Input"), Description("Close Button Color.  Default: System.Drawing.Color.Red")]
        public InArgument<Color> ColorButton { get; set; }

        [Category("Input"), Description("Datatable to show in the Status message.")]
        [RequiredArgument]
        public InArgument<DataTable> TableData { get; set; }

        #endregion

        #region CodeActivity

        protected override void Execute(NativeActivityContext context)
        {
            ApplyDefaults(context);

            if (Message.Get(context).Length > 120)
            {
                Message.Set(context, Message.Get(context).Substring(0, 120));
                Console.WriteLine("Message was too long.  Truncating to 120 characters");
            }

            Progress.Set(context, Progress.Get(context) > 1 ? 1 : Progress.Get(context));

            DisplayTime.Set(context, Convert.ToInt32(1000 * DisplayTime.Get(context)));

            int ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            int ScreenHeight = Screen.PrimaryScreen.Bounds.Height;

            Form frm = new Form();
            Label lbl = new Label();
            ProgressBar bar = new ProgressBar();
            Panel pnl = new Panel();
            Button btn = new Button();

            Timer timer;
            if (DisplayTime.Get(context) > 0)
            {
                timer = new Timer
                {
                    Interval = Convert.ToInt32(DisplayTime.Get(context))
                };
                timer.Tick += (s, a) => { frm.Close(); };
                timer.Start();
            }

            if (Location.Get(context) == null)
            {
                Location.Set(context, new[] { "", "" });
            }
            if (Location.Get(context).Length < 2)
            {
                Location.Set(context, new[] { Location.Get(context)[0], "" });
            }
            string loc = (Location.Get(context)[0] + Location.Get(context)[1]).ToLower();

            frm.StartPosition = FormStartPosition.Manual;

            loc = Utilities.LocPosition(loc);

            btn.BackColor = ColorButton.Get(context);
            lbl.ForeColor = ColorText.Get(context);
            frm.BackColor = ColorBorder.Get(context);
            pnl.BackColor = ColorBackground.Get(context);

            frm.TopMost = Top.Get(context) || frm.TopMost;

            if (Mobile.Get(context))
            {
                btn.Visible = false;
                frm.ControlBox = true;
            }
            else
            {
                frm.FormBorderStyle = FormBorderStyle.None;
                frm.ControlBox = false;
            }

            frm.Opacity = Opacity.Get(context);
            frm.Padding = new Padding(5);
            frm.ShowInTaskbar = false;
            frm.Text = string.Empty;
            frm.AutoSize = true;
            frm.Location = new Point(0, 0);
            frm.ClientSize = new Size(100, 100); // minimun

            pnl.Location = new Point(7, 7);
            pnl.Size = new Size(frm.Width, frm.Height);
            pnl.AutoSize = true;
            // pnl.BackColor = system.Drawing.Color.Black

            // lbl.ForeColor = system.Drawing.Color.White
            lbl.BackColor = System.Drawing.Color.Transparent;
            lbl.Font = new Font(lbl.Font.FontFamily, 10);
            lbl.Size = new Size(pnl.Width, pnl.Height);
            lbl.AutoSize = true;
            lbl.Padding = new Padding(5);
            lbl.TextAlign = ContentAlignment.MiddleCenter;

            lbl.Text = Message.Get(context);

            // btn.Text = "X"
            btn.Name = "myBtn";

            btn.Size = new Size(15, 15);
            // btn.BackColor = System.Drawing.Color.Red
            btn.ForeColor = Color.Red;
            // btn.Padding = New System.Windows.Forms.Padding(5)
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;

            // bar.ForeColor = system.Drawing.Color.OrangeRed
            // bar.backColor = system.Drawing.Color.Blue
            bar.Visible = !(Progress.Get(context) < 0);
            bar.Minimum = 0;
            bar.Maximum = 100;
            bar.Value = (Progress.Get(context) > 0) ? Convert.ToInt32(Progress.Get(context)) : bar.Value;
            bar.Step = 1;

            pnl.Controls.Add(bar);
            pnl.Controls.Add(lbl);
            pnl.Controls.Add(btn);


            // Build DataTable-----------------------------------------------------------
            int dvSRowLimit = 20;
            Label block = new Label();
            if (TableData.Get(context).Rows.Count > 0)
            {
                DataGridView dgv = new DataGridView
                {
                    DataSource = TableData.Get(context),
                    AllowUserToAddRows = false,
                    Width = 60 + TableData.Get(context).Columns.Count * 100
                };
                dvSRowLimit = (TableData.Get(context).Rows.Count < dvSRowLimit) ? TableData.Get(context).Rows.Count : dvSRowLimit;
                dgv.Height = 40 + 20 * dvSRowLimit;
                pnl.Controls.Add(dgv);
                dgv.Top = lbl.Bottom;
                dgv.Left = 10;
                // dgv.rows(0).DefaultCellStyle.BackColor = system.Drawing.Color.Black
                dgv.BackgroundColor = Color.Black;
                block.Width = dgv.Width;
                block.Height = 40;
                block.Top = dgv.Bottom;
                // block.Text = "block";
                // pnl.Controls.Add(block);
            }
            else
            {
                Console.WriteLine("DataTable is empty!");
            }
            // ----------------------------------------------------------------------------

            frm.Controls.Add(pnl);

            btn.BringToFront();
            btn.Location = new Point(frm.Width - 35, 3);

            bar.Width = pnl.Width - 13;
            bar.Height = Convert.ToInt32(bar.Height / (double)2);
            bar.Location = new Point(10, frm.Height - bar.Height);

            btn.Click += (s, a) => { frm.Close(); };

            if (lbl.Width > (ScreenWidth - 20))
            {
                lbl.AutoSize = false;
                lbl.Width = 1000 - 20;
                lbl.Height = 300 - 10;

                pnl.AutoSize = false;
                pnl.Width = 1000 - 15;
                pnl.Height = 300 - 5;

                frm.AutoSize = false;
                frm.ClientSize = new System.Drawing.Size(1000, 300);
            }

            frm = Utilities.LocLocation(loc, frm, ScreenHeight, ScreenWidth);

            if (Progress.Get(context) < 0)
            {
                bar.Visible = false;
                // Return
            }
            else
            {
                if (Progress.Get(context) > 1)
                {
                    Progress.Set(context, 1);
                }
                Progress.Set(context, Convert.ToInt32(Progress.Get(context) * 100));
                bar.Value = Convert.ToInt32(Progress.Get(context));
                bar.Visible = true;
                bar.Value = (bar.Value >= 100) ? 100 : bar.Value;
            }

            frm.ShowDialog();
        }

        private void ApplyDefaults(NativeActivityContext context)
        {
            Message.Set(context, Message.Get(context) == null ? "Status Progress" : Message.Get(context));

            if (Opacity.Get(context) == 0)
            {
                Console.WriteLine("Opacity set to 0, Progress Status bar will not be displayed!");
            }

            ColorBackground.Set(context, ColorBackground.Get(context) == null ? System.Drawing.Color.Black : ColorBackground.Get(context));
            ColorBorder.Set(context, ColorBorder.Get(context) == null ? System.Drawing.Color.OrangeRed : ColorBorder.Get(context));
            ColorText.Set(context, ColorText.Get(context) == null ? System.Drawing.Color.White : ColorText.Get(context));
            ColorButton.Set(context, ColorButton.Get(context) == null ? System.Drawing.Color.Red : ColorButton.Get(context));
            TableData.Set(context, TableData.Get(context) == null ? new DataTable() : TableData.Get(context));
        }

        #endregion

        #region CheckProperties

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            if (TableData == null)
            {
                metadata.AddValidationError($"TableData must be set before ModalDataTable activity '{DisplayName}' can be used.");
            }
        }

        #endregion

    }
}