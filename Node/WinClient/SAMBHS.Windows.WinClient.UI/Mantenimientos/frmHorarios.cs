using SAMBHS.Windows.SigesoftIntegration.UI.BLL;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmHorarios : Form
    {
        int usuarioHorario;
        List<string> usuarioGuarda;
        string modo;
        DateTime fi;
        DateTime ff;
        string descp;
        PacientBL _objBL = new PacientBL();

        int idHorario;
        public frmHorarios(int usuarioHorario_, List<string> usuarioGuarda_, string modo_, DateTime fi_, DateTime ff_, string descp_, int idHorario_)
        {
            usuarioHorario = usuarioHorario_;
            usuarioGuarda = usuarioGuarda_;
            modo = modo_;
            fi = fi_;
            ff = ff_;
            descp = descp_;
            idHorario = idHorario_;
            InitializeComponent();
        }

        private void frmHorarios_Load(object sender, EventArgs e)
        {
            if (modo == "Edit")
            {
                dtFin.Value = ff;
                dtInicio.Value = fi;
                txtDescrpcion.Text = descp;
                if (fi.Date >= DateTime.Now.Date && ff.Date <= DateTime.Now.Date)
                {
                    uckActivo.Checked = true;
                }
                else
                {
                    uckActivo.Checked = false;
                }
            }
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            Horarios Horariosobj = new Horarios();
            if (modo == "New")
            {
                Horariosobj.i_SystemUserId = usuarioHorario;
                Horariosobj.d_FechaInicio = dtInicio.Value;
                Horariosobj.d_FechaFin = dtFin.Value;
                Horariosobj.v_Comentary = txtDescrpcion.Text;
                Horariosobj.i_IsDeleted = 0;
                Horariosobj.i_InsertUserId = Int32.Parse(usuarioGuarda[2]);
                Horariosobj.d_InsertDate = DateTime.Now.Date;

                var resultado = _objBL.CreateHorarios(Horariosobj);
            }
            else
            {
                Horariosobj.IdHorario = idHorario;
                Horariosobj.i_SystemUserId = usuarioHorario;
                Horariosobj.d_FechaInicio = dtInicio.Value;
                Horariosobj.d_FechaFin = dtFin.Value;
                Horariosobj.v_Comentary = txtDescrpcion.Text;
                Horariosobj.i_UpdateUserId = Int32.Parse(usuarioGuarda[2]);
                Horariosobj.d_UpdateDate = DateTime.Now.Date;

                var resultado = _objBL.UpdateHorarios(Horariosobj);
            }
            this.Close();
        }


    }
}
