using SAMBHS.Common.Resource;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmVacunasCovid19 : Form
    {
        private string PersonId = "";
        private AgendaBl agendaBl_ = new AgendaBl();
        private string _mode = "";
        public frmVacunasCovid19(string _PersonId)
        {
            PersonId = _PersonId;

            InitializeComponent();
        }

        private void frmVacunasCovid19_Load(object sender, EventArgs e)
        {
            List<EsoDto> Esodto_1 = new List<EsoDto>();
            List<EsoDto> Esodto_2 = new List<EsoDto>();
            List<EsoDto> Esodto_3 = new List<EsoDto>();

            List<InmunizacionesDto> listaInmunizaciones = new List<InmunizacionesDto>();
            Task.Factory.StartNew(() =>
            {
                Esodto_1 = agendaBl_.LlenarComboMarcaVacunaCovid(cboMarca1);
                Esodto_2 = agendaBl_.LlenarComboMarcaVacunaCovid(cboMarca2);
                Esodto_3 = agendaBl_.LlenarComboMarcaVacunaCovid(cboMarca3);

                listaInmunizaciones = AgendaBl.InmunizacionesListadas(PersonId);

            }).ContinueWith(t =>
            {
                using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
                {
                    if (Esodto_1 == null) return;
                    else
                    {
                        cboMarca1.DataSource = Esodto_1;
                        cboMarca1.SelectedValue = -1;
                    }

                    if (Esodto_2 == null) return;
                    else
                    {
                        cboMarca2.DataSource = Esodto_2;
                        cboMarca2.SelectedValue = -1;
                    }

                    if (Esodto_3 == null) return;
                    else
                    {
                        cboMarca3.DataSource = Esodto_3;
                        cboMarca3.SelectedValue = -1;
                    }


                    foreach (var item in listaInmunizaciones)
                    {
                        if (item.i_Dosis == 1)
                        {
                            cboMarca1.SelectedValue = int.Parse(item.i_Marca.ToString());
                            txtLote1.Text = item.v_Lote;
                            dtpFecha1.Checked = true;
                            dtpFecha1.Value = item.d_FechaVacuna;
                            txtLugar1.Text = item.v_Lugar;

                        }
                        else if (item.i_Dosis == 2)
                        {
                            cboMarca2.SelectedValue = item.i_Marca;
                            txtLote2.Text = item.v_Lote;
                            dtpFecha2.Checked = true;
                            dtpFecha2.Value = item.d_FechaVacuna;
                            txtLugar2.Text = item.v_Lugar;
                        }
                        else if (item.i_Dosis == 3)
                        {
                            cboMarca3.SelectedValue = item.i_Marca;
                            txtLote3.Text = item.v_Lote;
                            dtpFecha3.Checked = true;
                            dtpFecha3.Value = item.d_FechaVacuna;
                            txtLugar3.Text = item.v_Lugar;
                        }
                    }
                }

                ;
            }, TaskScheduler.FromCurrentSynchronizationContext());

            
 
        }

        private void btnSavePacient_Click(object sender, EventArgs e)
        {
            //i_TipoVacuna == COVID-19
            //if (cboMarca1.SelectedValue.ToString() == "-1")
            //{
                InmunizacionesDto InmunizacionesDto_1 = new InmunizacionesDto();
                InmunizacionesDto_1.v_PersonId = PersonId;
                InmunizacionesDto_1.i_TipoVacuna = 1;
                InmunizacionesDto_1.i_Marca = int.Parse(cboMarca1.SelectedValue.ToString());
                InmunizacionesDto_1.v_Lote = txtLote1.Text;
                InmunizacionesDto_1.d_FechaVacuna = dtpFecha1.Value;
                InmunizacionesDto_1.i_IsDeleted = 0;
                InmunizacionesDto_1.i_InsertUserId = Int32.Parse(Globals.ClientSession.GetAsList()[2]);
                InmunizacionesDto_1.v_ComentaryUpdate = "";
                InmunizacionesDto_1.i_Dosis = 1;
                InmunizacionesDto_1.v_Lugar = txtLugar1.Text;
                InmunizacionesDto_1.i_UpdateUserId = Int32.Parse(Globals.ClientSession.GetAsList()[2]);
                var mensaje_1 = AgendaBl.AddUpdateVaacunaciones(InmunizacionesDto_1);

                InmunizacionesDto InmunizacionesDto_2 = new InmunizacionesDto();
                InmunizacionesDto_2.v_PersonId = PersonId;
                InmunizacionesDto_2.i_TipoVacuna = 1;
                InmunizacionesDto_2.i_Marca = int.Parse(cboMarca2.SelectedValue.ToString());
                InmunizacionesDto_2.v_Lote = txtLote2.Text;
                InmunizacionesDto_2.d_FechaVacuna = dtpFecha2.Value;
                InmunizacionesDto_2.i_IsDeleted = 0;
                InmunizacionesDto_2.i_InsertUserId = Int32.Parse(Globals.ClientSession.GetAsList()[2]);
                InmunizacionesDto_2.v_ComentaryUpdate = "";
                InmunizacionesDto_2.i_Dosis = 2;
                InmunizacionesDto_2.v_Lugar = txtLugar2.Text;
                InmunizacionesDto_2.i_UpdateUserId = Int32.Parse(Globals.ClientSession.GetAsList()[2]);
                var mensaje_2 = AgendaBl.AddUpdateVaacunaciones(InmunizacionesDto_2);

                InmunizacionesDto InmunizacionesDto_3 = new InmunizacionesDto();
                InmunizacionesDto_3.v_PersonId = PersonId;
                InmunizacionesDto_3.i_TipoVacuna = 1;
                InmunizacionesDto_3.i_Marca = int.Parse(cboMarca3.SelectedValue.ToString());
                InmunizacionesDto_3.v_Lote = txtLote3.Text;
                InmunizacionesDto_3.d_FechaVacuna = dtpFecha3.Value;
                InmunizacionesDto_3.i_IsDeleted = 0;
                InmunizacionesDto_3.i_InsertUserId = Int32.Parse(Globals.ClientSession.GetAsList()[2]);
                InmunizacionesDto_3.v_ComentaryUpdate = "";
                InmunizacionesDto_3.i_Dosis = 3;
                InmunizacionesDto_3.v_Lugar = txtLugar3.Text;
                InmunizacionesDto_3.i_UpdateUserId = Int32.Parse(Globals.ClientSession.GetAsList()[2]);
                var mensaje_3 = AgendaBl.AddUpdateVaacunaciones(InmunizacionesDto_3);

                if (mensaje_1 != "" || mensaje_2 != "" || mensaje_3 != "")
                {
                    MessageBox.Show(@"Vacunas  guardadas correctamente.", @"Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.Close();

            
        }
    }
}
