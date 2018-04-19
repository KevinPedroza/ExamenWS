using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsumirWS.Pronostico;
using static ConsumirWS.XMLprono;

namespace ConsumirWS
{
    public partial class MostrarProno : Form
    {
        public MostrarProno()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void CargarEfemerides()
        {
            WSMeteorologicoClient ws = new WSMeteorologicoClient("WSMeteorologico");
            EFEMERIDES ef = ws.efemerides(new efemerides()).ParseXML<EFEMERIDES>();
            lblSolSale.Text = String.Format("Sale: {0}", ef.EFEMERIDE_SOL.SALE);
            lblSolPone.Text = String.Format("Se oculta: {0}", ef.EFEMERIDE_SOL.SEPONE);

            lblLunaSale.Text = String.Format("Sale: {0}", ef.EFEMERIDE_LUNA.SALE);
            lblLunaPone.Text = String.Format("Se oculta: {0}", ef.EFEMERIDE_LUNA.SEPONE);

        }
        private void CargarCiudad()
        {
            WSMeteorologicoClient ws = new WSMeteorologicoClient("WSMeteorologico");
            int id = (Form1.lugar as PRONOSTICO_REGIONALREGIONCIUDAD)?.id ?? 5;
            try
            {
                PRONOSTICO_REGIONALREGIONCIUDAD reg = ws.pronosticoPorCiudadxID(id).ParseXML<PRONOSTICO_REGIONALREGIONCIUDAD>();
                panel1.Hide();
                panel2.Hide();
                panel3.Hide();

                if (id != -1)
                {
                    if (reg.ESTADOMANANA != null)
                    {
                        pbxCM.ImageLocation = String.Format("https://www.imn.ac.cr{0}", reg.ESTADOMANANA.imgPath);
                        lblCCM.Text = reg.COMENTARIOMANANA;
                        lblCTM.Text = reg.ESTADOMANANA.Value;
                        panel1.Show();
                    }

                    if (reg.ESTADOTARDE != null)
                    {
                        pbxCT.ImageLocation = String.Format("https://www.imn.ac.cr{0}", reg.ESTADOTARDE.imgPath);
                        lblCCT.Text = reg.COMENTARIOTARDE;
                        lblCTT.Text = reg.ESTADOTARDE.Value;
                        panel2.Show();
                    }

                    if (reg.ESTADONOCHE != null)
                    {
                        pbxCN.ImageLocation = String.Format("https://www.imn.ac.cr{0}", reg.ESTADONOCHE.imgPath);
                        lblCCN.Text = reg.COMENTARIONOCHE;
                        lblCTN.Text = reg.ESTADONOCHE.Value;
                        panel3.Show();
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(id.ToString());
            }
        }

        private void MostrarProno_Load(object sender, EventArgs e)
        {
            CargarEfemerides();
            CargarCiudad();
        }
    }
}
