using System;
using System.Activities;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace UiPathTeam.StatusProgress.Activities
{
    [DisplayName("Live Status"), Description("Updates the status form")]
    public class LiveStatus : NativeActivity
    {

        #region Properties

        [Category("Input"), Description("Message to be displayed.  Default: Status Progress")]
        public InArgument<String> Message { get; set; } = new InArgument<String>("Status Progress");

        [Category("Input"), Description("Background opacity.  Value should be betwen 0 and 1.  Default: 0")]
        public InArgument<Double> Opacity { get; set; } = new InArgument<Double>(0.8);

        [Category("Input"), Description("Location on the screen.  Vertical options are \"center, top, bottom\" , and Horizontal options are \"center, right, and left\" .  Provide a string array indicating desired location.  Default: {\"center\",\"center\"}")]
        public InArgument<String[]> Location { get; set; }

        [Category("Input"), Description("Progress to display.  Set between 0 and 1 to display progrss bar.  Negative (-1) will hide the progress bar.  Default: 0")]
        public InArgument<Double> Progress { get; set; } = new InArgument<Double>(-1);

        [Category("Input"), Description("Show control box. Default: False")]
        public InArgument<Boolean> Mobile { get; set; }

        [Category("Input"), Description("Set to True if you want the form to close automatically when the progress bar reaches 100%.  Default: False")]
        public InArgument<Boolean> ProgressAutoClose { get; set; }

        [Category("Input"), Description("Bring progress bar to the front of all other windows.  Default: False")]
        public InArgument<Boolean> Top { get; set; } = new InArgument<Boolean>(true);

        [Category("Input"), Description("Background Color.  Default: System.Drawing.Color.Black")]
        public InArgument<Color> ColorBackground { get; set; }

        [Category("Input"), Description("Border Color.  Default: System.Drawing.Color.OrangeRed")]
        public InArgument<Color> ColorBorder { get; set; }

        [Category("Input"), Description("Text Color.  Default: System.Drawing.Color.White")]
        public InArgument<Color> ColorText { get; set; }

        [Category("Input"), Description("Close Button Color.  Default: System.Drawing.Color.Red")]
        public InArgument<Color> ColorButton { get; set; }

        [Category("Input/Output"), Description("Required Parameter.  This stores the container for the  status message.  It enables you to update the same status bar.  Failure to provide it will result in multiple status messsage objects after each call.")]
        [RequiredArgument]
        public InOutArgument<Form> Container { get; set; }

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

            int ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            int ScreenHeight = Screen.PrimaryScreen.Bounds.Height;

            Form frm = (Container.Get(context) == null) ? new Form() : Container.Get(context);
            Label lbl = new Label();
            ProgressBar bar = new ProgressBar();
            Panel pnl = new Panel();
            Button btn = new Button();
            DataGridView dgv = new DataGridView();

            if (Container.Get(context) != null)
            {
                // Get Panel
                object oObj = frm.Controls.Find("myPanel", true).FirstOrDefault();
                pnl = (Panel)oObj;
                // Get Label
                oObj = pnl.Controls.Find("myLabel", true).FirstOrDefault();
                lbl = (Label)oObj;
                // Get Bar
                oObj = pnl.Controls.Find("myBar", true).FirstOrDefault();
                bar = (ProgressBar)oObj;

                // Get Button
                oObj = pnl.Controls.Find("myBtn", true).FirstOrDefault();
                btn = (Button)oObj;

                // Get Datatable
                oObj = pnl.Controls.Find("myDgv", true).FirstOrDefault();
                dgv = (DataGridView)oObj;
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

            lbl.Text = Message.Get(context);

            if (Container.Get(context) == null)
            {
                if (Mobile.Get(context))
                {
                    // btn.Visible = false;
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

                pnl.Name = "myPanel";
                btn.Name = "myBtn";
                lbl.Name = "myLabel";
                bar.Name = "myBar";
                dgv.Name = "myDgv";

                lbl.BackColor = Color.Transparent;
                lbl.Font = new Font(lbl.Font.FontFamily, 20);
                lbl.Size = new Size(pnl.Width, pnl.Height);
                lbl.AutoSize = true;
                lbl.Padding = new Padding(30);
                lbl.TextAlign = ContentAlignment.MiddleCenter;

                // btn.Text = "X"
                btn.Size = new Size(15, 15);
                btn.BackColor = Color.Transparent;
                btn.ForeColor = Color.Red;
                // btn.Padding = New System.Windows.Forms.Padding(5)
                // btn.Visible = true
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;

                bar.ForeColor = Color.OrangeRed;
                bar.BackColor = Color.Blue;
                bar.Visible = !(Progress.Get(context) < 0);
                bar.Minimum = 0;
                bar.Maximum = 100;
                bar.Value = (Progress.Get(context) > 0) ? Convert.ToInt32(Progress.Get(context)) : bar.Value;
                bar.Step = 1;

                pnl.Controls.Add(bar);
                pnl.Controls.Add(lbl);
                pnl.Controls.Add(btn);

                frm.Controls.Add(pnl);

                btn.BringToFront();
                btn.Location = new Point(frm.Width - 3, 3);

                bar.Width = pnl.Width - 5;
                bar.Height = Convert.ToInt32(bar.Height / (double)2);
                bar.Location = new Point(2, frm.Height - bar.Height);

                // btn.Click += (s, a) => { frm.Close(); };

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
            }
            else
            {
                bar.Width = lbl.Width - 1;

                frm.ClientSize = new Size(lbl.Width + 5, lbl.Height + 5);

                bar.Width = pnl.Width - 5;
                // bar.Height = Convert.ToInt32(bar.Height / (double)2);
                // bar.Location = new Point(2, frm.Height - bar.Height);

                frm.Refresh();

                if (!Mobile.Get(context))
                {
                    frm = Utilities.LocLocation(loc, frm, ScreenHeight, ScreenWidth, 15);
                }
            }

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
                if (Container.Get(context) != null)
                {
                    if (bar.Value >= 100 && ProgressAutoClose.Get(context))
                    {
                        frm.Close();
                        return;
                    }
                    bar.Location = new Point(2, lbl.Bottom);
                }
                bar.Visible = true;
                bar.Value = (bar.Value >= 100) ? 100 : bar.Value;
            }

            if (Container.Get(context) == null)
            {
                frm.Show();
            }

            Container.Set(context, frm);
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
        }

        #endregion

        #region CheckProperties

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            if (Container == null)
            {
                metadata.AddValidationError($"Container must be set before LiveStatus activity '{DisplayName}' can be used.");
            }
        }

        #endregion

    }
}