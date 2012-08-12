using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TAIniEditor
{
    public partial class MainForm : Form
    {
        private Ini ini;
        private IniSpec spec;

        public MainForm()
        {
            InitializeComponent();

        }

        public void MainForm_Load(object sender, EventArgs e)
        {
            // test data
            LoadIni(@"C:\Users\mark\Documents\My Games\tribes.ini");
            // make sure mousewheel events scroll the right panel
            // FIXME this overrides the tab scroll
            this.MouseWheel += new MouseEventHandler(Mouse_Wheel);
        }

        public void Mouse_Wheel(object sender, EventArgs e)
        {
            try
            {
                // FIXME this needs to get the currently active tab
                gridContainer.TabPages[gridContainer.TabIndex].Controls[0].Focus();
            }
            catch (Exception ex)
            {
            }
        }

        void Bool_Bind_Format(object sender, ConvertEventArgs e)
        {
            e.Value = ini.ParseBool(e.Value.ToString());
        }

        void Bool_Bind_Parse(object sender, ConvertEventArgs e)
        {
            e.Value = ini.FormatBool((bool)e.Value);
        }

        private void LoadIni(string path)
        {
            ini = new Ini(path);
            spec = new IniSpec(@"Specs\TribesAscend.xml");

            // NOTE we're going to have to refactor this at some point to make
            // it possible to reuse control creation if the user adds options
            // via the UI.

            gridContainer.Controls.Clear();
            Label l;

            TabPage tabPage;

            foreach (Category c in ini.Root.Categories)
            {
                tabPage = new TabPage();
                tabPage.Text = c.Name;
                tabPage.Dock = DockStyle.Fill;
                tabPage.AutoScroll = true;
                tabPage.Padding = new Padding(10);
             
                TableLayoutPanel p = new TableLayoutPanel();
                p.Font = new Font(p.Font, 0);
                p.AutoSize = true;
                p.AutoScroll = true;
                p.Dock = DockStyle.Fill;
                p.ColumnCount = 2;
                int row = 0;
                foreach (Option o in c.Options)
                {
                    string min, max;
                    l = new Label();
                    l.AutoSize = true;
                    l.Text = o.Name;
                    
                    l.TextAlign = ContentAlignment.TopCenter;
                    Control control;
                    Binding b;

                    if (o.Type == OptionType.Bool)
                    {
                        CheckBox cbox;
                        control = new CheckBox();
                        cbox = (CheckBox)control;
                        b = new Binding("checked", o, "Value", true, DataSourceUpdateMode.OnPropertyChanged);
                        b.Format += new ConvertEventHandler(Bool_Bind_Format);
                        b.Parse += new ConvertEventHandler(Bool_Bind_Parse);
                        cbox.DataBindings.Add(b);
                    }
                    else
                    {
                        control = new TextBox();
                        ((TextBox)(control)).TextChanged += Control_Focus;
                        b = new Binding("text", o, "Value", true, DataSourceUpdateMode.OnPropertyChanged);
                        control.DataBindings.Add(b);
                    }
                    var data = new Dictionary<string, object>();
                    data["option"] = o;
                    control.Tag = data;
                    l.Tag = data;
                    control.MouseHover += Control_Focus;
                    control.Click += Control_Focus;
                    control.Validating += Validate_Control;
                    l.MouseHover += Control_Focus;
                    l.Click += Control_Focus;
                    
                    p.Controls.Add(l, 0, row);
                    p.Controls.Add(control, 1, row);
                    row++;
                }
                tabPage.Controls.Add(p);
                gridContainer.Controls.Add(tabPage);
            }
        }


        private void Validate_Control(object sender, CancelEventArgs  e)
        {
            string failMsg;
            Dictionary<string, object> data;
            Control control = (Control)sender;
            data = (Dictionary<string, object>)control.Tag;
            Option o = (Option)data["option"];
            if (!spec.ValidateFor(o, control.Text, out failMsg))
            {
                e.Cancel = true;
                validationLbl.Text = failMsg;
            }
            else {
                validationLbl.Text = "";
            }
        }

        private void Control_Focus(object sender, EventArgs e)
        {
            Dictionary<string, object> data;
            Control control = (Control)sender;
            data = (Dictionary<string, object>)control.Tag;
            Option o = (Option)data["option"];
            infoPanel.Text = o.Name;
            infoLbl.Text = spec.DescriptionFor(o);

            string typeText = "";
            switch(o.Type) {
                case OptionType.Bool: typeText = "Boolean"; break;
                case OptionType.Float: typeText = "Float"; break;
                case OptionType.Int: typeText = "Integer"; break;
                case OptionType.String: typeText = "String/text"; break;
            }
            infoTypeLbl.Text = typeText;
            infoValueLbl.Text = o.Value;
        }


        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult result = ofd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                LoadIni(ofd.FileName);
            }
        }
    
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ini.Save();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void infoPanel_Enter(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void infoValueLbl_Click(object sender, EventArgs e)
        {

        }

    }
}
