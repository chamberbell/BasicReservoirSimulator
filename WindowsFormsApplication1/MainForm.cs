using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class MainForm : Form
    {

        double time_frame; // [days] this is how long the simulation will run
        double delta_t; // [days] the number of days between time steps
        double time_steps; // time_frame divided by delta_t

        double delta_x; //[ft]
        double delta_y; //[ft]
        double delta_z; //[ft]
        double length; //[ft]
        double width; //[ft]
        double height; //[ft]

        double porosity;
        double perm;
        double Sw;
        double Pb;
        double Pinitial;
        double Pmin;
     
        bool UseWell1;
        bool Well1_Inj;
        bool Well1_Qw_or_Pwf; //1=Qw, 0=Pwf
        double Well1Pwf; 
        double Well1Qw;
        double Well1Skin;
        double Well1Rw; //[ft]
        double Well1XLoc; //[ft]
        double Well1YLoc; //[ft]

        bool UseWell2;
        bool Well2_Inj;
        bool Well2_Qw_or_Pwf; //1=Qw, 0=Pwf
        double Well2Pwf;
        double Well2Qw;
        double Well2Skin;
        double Well2Rw; //[ft]
        double Well1XLoc; //[ft]
        double Well1YLoc; //[ft]

        bool UseWell3;
        bool Well3_Inj;
        bool Well3_Qw_or_Pwf; //1=Qw, 0=Pwf
        double Well3Pwf;
        double Well3Qw;
        double Well3Skin;
        double Well3Rw; //[ft]
        double Well1XLoc; //[ft]
        double Well1YLoc; //[ft]




        public MainForm()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void label37_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        //this is what heppens what you clidk "Calculate"
        private void button1_Click(object sender, EventArgs e)
        {
            refreshData();
        }
   
        private void refreshData()
        {
            //double.TryParse(tbBubblePoint.Text, out foo);
            //foo = foo + 200;
            //tbBubblePoint.Text = Convert.ToString(foo);
        }


        private void ThomasMethod(double[] a, double[] b, double[] c, double[] d, int n)
        {
            n--; 
            c[0] /= b[0];
            d[0] /= b[0];

            for (int i = 1; i < n; i++)
            {
                c[i] /= b[i] - a[i] * c[i - 1];
                d[i] = (d[i] - a[i] * d[i - 1]) / (b[i] - a[i] * c[i - 1]);
            }

            d[n] = (d[n] - a[n] * d[n - 1]) / (b[n] - a[n] * c[n - 1]);

            for (int i = n; i-- > 0; )
            {
                d[i] -= c[i] * d[i + 1];
            }
        }

        private void cbWell1Active_CheckedChanged(object sender, EventArgs e)
        {
                
                
        }

    }
}
