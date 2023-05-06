using SAMBHS.Common.Resource;
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

//using Excel = Microsoft.Office.Interop.Excel;
//using Microsoft.Office.Core;
//using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Configuration;
using Infragistics.Win.UltraWinGrid;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmProfessional : Form
    {
        PacientBL _objBL = new PacientBL();
        List<string> dias = new List<string>();
        int grupo = 0;
        int count = 0;

        public frmProfessional(string N)
        {
            InitializeComponent();
        }

        private void frmProfessional_Load(object sender, EventArgs e)
        {
            UltraGridColumn c = grdDataDias.DisplayLayout.Bands[0].Columns["Select"];
            c.CellActivation = Activation.AllowEdit;
            c.CellClickAction = CellClickAction.Edit;

            BindGrid();
        }

        private void BindGrid()
        {
            var objData = GetData(txtFirstLastNameDocNumber.Text.ToUpper());

            grdDataProfesionales.DataSource = objData;
            if (objData != null)
            {
                lblRecordCount.Text = string.Format("Se encontraron {0} registros.", objData.Count());

            }

            if (grdDataProfesionales.Rows.Count > 0)
            {
                grdDataProfesionales.Rows[0].Selected = true;
            }
            grdDataProfesionales.DataBind();
        }

        private List<Professional> GetData(string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();

            var pacients = _objBL.GetListProfessionales(ref objOperationResult, 9999, "");
            pacients = pacients.FindAll(p => p.Nombre.Contains(txtFirstLastNameDocNumber.Text.ToUpper())).ToList();
            return pacients;
        }

        private void btnEditarProfesion_Click(object sender, EventArgs e)
        {
            int idProfesion = int.Parse(grdDataProfesionales.Selected.Rows[0].Cells["i_CodigoProfesion"].Value.ToString());
            string PersonID = grdDataProfesionales.Selected.Rows[0].Cells["PersonId"].Value.ToString();
            frmProfesion frm = new frmProfesion(idProfesion);
            frm.ShowDialog();
            if (frm.grupo == 0)
            {

            }
            else
            {
                var resultado = _objBL.EditarProfesional(PersonID, frm.grupo);

                if (resultado != 0)
                {
                    MessageBox.Show("GUARDADO CORRECTAMENTE", "INFORMACIÓN", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                }
                else
                {
                    MessageBox.Show("ERROR AL GUARDAR", "VALIDACIÓN", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }

                BindGrid();
            }
        }

        private void btnAddHorarios_Click(object sender, EventArgs e)
        {
            string idUsuario = grdDataProfesionales.Selected.Rows[0].Cells["IdUsuario"].Value.ToString();
            frmHorarios frm = new frmHorarios(int.Parse(idUsuario), Globals.ClientSession.GetAsList(), "New", DateTime.Now.Date, DateTime.Now.Date, "", 0);
            frm.ShowDialog();
            FiltrarHorarios();
        }

        private void FiltrarHorarios()
        {
            //btnAddProfessional.Enabled = false;
            //btnAddTurno.Enabled = true;
            //btnAddHorario.Enabled = false;
            OperationResult objOperationResult = new OperationResult();


            string GrupoHorario = grdDataProfesionales.Selected.Rows[0].Cells["IdUsuario"].Value.ToString();

            var objData = GetDataHorarios(GrupoHorario);

            grdDataHorarios.DataSource = objData;
            if (objData != null)
            {
                lblRecordCountTurnos.Text = string.Format("Se encontraron {0} registros.", objData.Count());
                if (objData.Count() != 0)
                {
                    btnAddHorarios.Enabled = true;
                    btnEditHorarios.Enabled = true;
                    btnDeleteHorarios.Enabled = true;
                }
                else
                {

                    var obj = _objBL.GetListDias(ref objOperationResult, 9999, int.Parse("-1"), 0);


                    grdDataDias.DataSource = obj;


                    if (objData != null)
                    {
                        lblRecordCountTurnos.Text = string.Format("Se encontraron {0} registros.", objData.Count());

                    }

                    var objData2 = GetDataTurno("-1");

                    grdDataTurnos.DataSource = objData2;
                    if (objData != null)
                    {
                        lblRecordCountTurnos.Text = string.Format("Se encontraron {0} registros.", objData.Count());

                    }

                    btnAddHorarios.Enabled = true;
                    btnEditHorarios.Enabled = false;
                    btnDeleteHorarios.Enabled = false;
                    btnAisganarGrupo.Enabled = false;
                    btnEliminarGrupo.Enabled = false;
                    btnAddTurno.Enabled = false;
                    btnAddHorario.Enabled = false;
                }

            }
            else
            {
                btnAddHorarios.Enabled = false;
                btnEditHorarios.Enabled = false;
                btnDeleteHorarios.Enabled = false;
            }

            grdDataHorarios.DataBind();
        }

        private List<SystemParameter_Horario> GetDataTurno(string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();

            var pacients = _objBL.GetListTurnosIncludeDelete(ref objOperationResult, 9999, pstrFilterExpression);

            return pacients;
        }
        private List<Horarios> GetDataHorarios(string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();

            var obj = _objBL.GetListHorarios(ref objOperationResult, 9999, pstrFilterExpression);

            return obj;
        }

        private void txtFirstLastNameDocNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            BindGrid();
        }

        private void btnEditHorarios_Click(object sender, EventArgs e)
        {
            string idHorario = grdDataHorarios.Selected.Rows[0].Cells["IdHorario"].Value.ToString();
            string idUsuario = grdDataProfesionales.Selected.Rows[0].Cells["IdUsuario"].Value.ToString();
            string d_FechaInicio = grdDataHorarios.Selected.Rows[0].Cells["d_FechaInicio"].Value.ToString();
            string d_FechaFin = grdDataHorarios.Selected.Rows[0].Cells["d_FechaFin"].Value.ToString();
            string v_Comentary = grdDataHorarios.Selected.Rows[0].Cells["v_Comentary"].Value.ToString();
            frmHorarios frm = new frmHorarios(int.Parse(idUsuario), Globals.ClientSession.GetAsList(), "Edit", DateTime.Parse(d_FechaInicio), DateTime.Parse(d_FechaFin), v_Comentary, int.Parse(idHorario));
            frm.ShowDialog();

            FiltrarHorarios();
        }

        private void btnDeleteHorarios_Click(object sender, EventArgs e)
        {
            int idHorario = Int32.Parse(grdDataHorarios.Selected.Rows[0].Cells["IdHorario"].Value.ToString());
            int usuario = Int32.Parse(Globals.ClientSession.GetAsList()[2]);
            var resultado = _objBL.DeleteHorarios(idHorario, usuario);

            if (resultado != null)
            {
                MessageBox.Show("GUARDADO CORRECTAMENTE", "INFORMACIÓN", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                FiltrarHorarios();
            }
            else
            {
                MessageBox.Show("ERROR AL GUARDAR", "VALIDACIÓN", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }
        
        private void grdDataHorarios_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (grdDataHorarios.Selected.Rows.Count == 0)
            {
                return;
            }
            else
            {
                btnClonar.Enabled = true;
                FiltrarDias();
            }
        }

        private void FiltrarDias()
        {
            btnClonar.Enabled = true;
            string GrupoHorario = grdDataHorarios.Selected.Rows[0].Cells["IdHorario"].Value.ToString();

            var objData = GetDataDias(int.Parse(GrupoHorario));

            grdDataDias.DataSource = objData;
            if (objData != null)
            {
                lblRecordCountTurnos.Text = string.Format("Se encontraron {0} registros.", objData.Count());

            }
        }

        private List<Dias> GetDataDias(int pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            int Mes = int.Parse(grdDataHorarios.Selected.Rows[0].Cells["d_FechaInicio"].Value.ToString().Split('/')[1]);
            int Año = int.Parse(grdDataHorarios.Selected.Rows[0].Cells["d_FechaFin"].Value.ToString().Split('/', ' ')[2]);
            int Dias = DateTime.DaysInMonth(Año, Mes);
            var obj = _objBL.GetListDias(ref objOperationResult, 9999, pstrFilterExpression, Dias);

            return obj;
        }

        private void btnAisganarGrupo_Click(object sender, EventArgs e)
        {
            if (grdDataDias.Selected.Rows[0].Cells["ESTADO"].Value.ToString() == "SIN GRUPO HORARIO")
            {
                string person = grdDataProfesionales.Selected.Rows[0].Cells["IdUsuario"].Value.ToString();
                string horario = grdDataHorarios.Selected.Rows[0].Cells["IdHorario"].Value.ToString();
                string dia = grdDataDias.Selected.Rows[0].Cells["Orden"].Value.ToString();

                var resultado = _objBL.CreateGrupoHorarioMedico(person, horario, dia);

                if (resultado != null)
                {
                    MessageBox.Show("GUARDADO CORRECTAMENTE", "INFORMACIÓN", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    FiltrarDias();
                }
                else
                {
                    MessageBox.Show("ERROR AL GUARDAR", "VALIDACIÓN", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }

            }
            else
            {
                MessageBox.Show("El día ya cuenta con GRUPO HORARIO", "VALIDACIÓN", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

        private void btnEliminarGrupo_Click(object sender, EventArgs e)
        {


            if (grdDataDias.Selected.Rows[0].Cells["ESTADO"].Value.ToString() != "SIN GRUPO HORARIO")
            {
                string horario = grdDataHorarios.Selected.Rows[0].Cells["IdHorario"].Value.ToString();
                string dia = grdDataDias.Selected.Rows[0].Cells["Orden"].Value.ToString();

                var resultado = _objBL.DeleteGrupoHorarioMedico("", horario, dia);

                if (resultado == 1)
                {
                    MessageBox.Show("GUARDADO CORRECTAMENTE", "INFORMACIÓN", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    FiltrarDias();
                }
                else
                {
                    MessageBox.Show("ERROR AL GUARDAR", "VALIDACIÓN", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }

            }
            else
            {
                MessageBox.Show("¿Para eliminar, tiene que contar con GRUPO HOARRIO!", "VALIDACIÓN", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }



        }

        private void btnClonar_Click(object sender, EventArgs e)
        {
            string person = grdDataProfesionales.Selected.Rows[0].Cells["IdUsuario"].Value.ToString();
            string horario = grdDataHorarios.Selected.Rows[0].Cells["IdHorario"].Value.ToString();
            //string dia = grdDataDias.Selected.Rows[0].Cells["Orden"].Value.ToString();
            int resultado = 0;
            foreach (var item in dias)
            {
                resultado = _objBL.ClonarHorarioMedico(person, horario, item, grupo);

                var grupoClonar = _objBL.ClonarTurno(resultado.ToString(), grupo.ToString());
            }
            if (resultado != null && resultado != 0)
            {
                MessageBox.Show("GUARDADO CORRECTAMENTE", "INFORMACIÓN", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                FiltrarDias();
            }
            else
            {
                MessageBox.Show("ERROR AL GUARDAR", "VALIDACIÓN", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            grupo = 0;
            dias.Clear();
        }

        private void grdDataDias_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (grdDataDias.Selected.Rows.Count == 0)
            {
                return;
            }
            else
            {


                if (grdDataDias.Selected.Rows[0].Cells["ESTADO"].Value.ToString() != "SIN GRUPO HORARIO")
                {
                    FiltrarTurnos();
                    btnAisganarGrupo.Enabled = false;
                    btnEliminarGrupo.Enabled = true;
                }
                else
                {
                    btnAisganarGrupo.Enabled = true;
                    btnEliminarGrupo.Enabled = false;

                    btnAddHorarios.Enabled = true;

                    btnAddTurno.Enabled = false;
                    btnAddHorario.Enabled = false;

                    var objData = GetDataTurno("-1");

                    grdDataTurnos.DataSource = objData;
                    if (objData != null)
                    {
                        lblRecordCountTurnos.Text = string.Format("Se encontraron {0} registros.", objData.Count());

                    }

                    if (grdDataTurnos.Rows.Count > 0)
                    {
                        grdDataTurnos.Rows[1].Selected = true;
                    }
                    grdDataTurnos.DataBind();
                }
            }
        }

        private void grdDataDias_ClickCell(object sender, ClickCellEventArgs e)
        {

            if (e.Cell.Column.Key == "Select")
            {
                if (e.Cell.Value.ToString() == "False")
                {
                    count++;
                    e.Cell.Value = true;
                    var rowIndex = e.Cell.Row.Index;
                    if (count == 1)
                    {
                        if (grdDataDias.Rows[rowIndex].Cells["Grupo"].Text != "0")
                        {
                            grupo = int.Parse(grdDataDias.Rows[rowIndex].Cells["Grupo"].Text);
                            count++;
                            btnClonar.Enabled = true;
                        }
                        else
                        {
                            MessageBox.Show("Para clonar horario a otro dia, debe seleccionar primero un día asignado a un horario fijo", "ADVERTENCIA", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                            btnClonar.Enabled = false;
                        }
                    }
                    else
                    {
                        count++;
                        var _grupo = grdDataDias.Rows[rowIndex].Cells["Dia"].Text;
                        btnClonar.Enabled = true;
                        dias.Add(_grupo);
                    }

                    e.Cell.Activated = false;
                }
                else
                {
                    count = 0;
                    e.Cell.Value = false;
                    e.Cell.Activated = false;
                }

            }
            else
            {
                count = 0;
            }
        }

        private void btnAddTurno_Click(object sender, EventArgs e)
        {
            if (grdDataDias.Selected.Rows[0].Cells["ESTADO"].Value.ToString() != "SIN GRUPO HORARIO")
            {
                frmDetalle frm = new frmDetalle("");
                frm.ShowDialog();
                if (frm.estadoVentana == 1)
                {
                    string detalle = "";
                    if (frm.detalle == string.Empty)
                    {
                        MessageBox.Show("ERROR EN DESCRICIÓN", "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        detalle = frm.detalle;
                    }

                    string GrupoHorario = grdDataDias.Selected.Rows[0].Cells["Grupo"].Value.ToString();

                    var resultado = _objBL.CreateTurno(GrupoHorario, detalle);

                    if (resultado != null)
                    {
                        MessageBox.Show("GUARDADO CORRECTAMENTE", "INFORMACIÓN", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        FiltrarTurnos();
                    }
                    else
                    {
                        MessageBox.Show("ERROR AL GUARDAR", "VALIDACIÓN", MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }

                }

            }
        }

        private void FiltrarTurnos()
        {
            //btnAddProfessional.Enabled = false;
            btnAddTurno.Enabled = true;
            btnAddHorario.Enabled = false;
            string GrupoHorario = grdDataDias.Selected.Rows[0].Cells["Grupo"].Value.ToString();

            var objData = GetDataTurno(GrupoHorario);

            grdDataTurnos.DataSource = objData;
            if (objData != null)
            {
                lblRecordCountTurnos.Text = string.Format("Se encontraron {0} registros.", objData.Count());

            }

            //if (grdDataTurnos.Rows.Count > 0)
            //{
            //    grdDataTurnos.Rows[1].Selected = true;
            //}
            grdDataTurnos.DataBind();
        }
        private void btnEditTurno_Click(object sender, EventArgs e)
        {
            var v_Value1 = grdDataTurnos.Selected.Rows[0].Cells["v_Value1"].Value.ToString();
            if (v_Value1 != null)
            {
                frmDetalle frm = new frmDetalle(v_Value1);
                frm.ShowDialog();
                string detalle = frm.detalle;

                string GrupoHorario = grdDataTurnos.Selected.Rows[0].Cells["i_GroupId"].Value.ToString();
                string ParentGrupoHorario = grdDataTurnos.Selected.Rows[0].Cells["i_ParameterId"].Value.ToString();
                if (frm.estadoVentana == 1)
                {
                    var resultado = _objBL.EditarDetalle(GrupoHorario, detalle, ParentGrupoHorario, "");

                    if (resultado != 0)
                    {
                        MessageBox.Show("GUARDADO CORRECTAMENTE", "INFORMACIÓN", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        FiltrarTurnos();
                    }
                    else
                    {
                        MessageBox.Show("ERROR AL GUARDAR", "VALIDACIÓN", MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    FiltrarTurnos();
                }

            }
        }

        private void btnDeleteTurno_Click(object sender, EventArgs e)
        {
            var v_Value1 = grdDataTurnos.Selected.Rows[0].Cells["v_Value1"].Value.ToString();
            if (v_Value1 != null)
            {
                string GrupoHorario = grdDataTurnos.Selected.Rows[0].Cells["i_GroupId"].Value.ToString();
                string ParentGrupoHorario = grdDataTurnos.Selected.Rows[0].Cells["i_ParameterId"].Value.ToString();

                var resultado = _objBL.EliminarDetalleDeBaja2(GrupoHorario, ParentGrupoHorario);

                if (resultado != null)
                {
                    MessageBox.Show("GUARDADO CORRECTAMENTE", "INFORMACIÓN", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    FiltrarTurnos();
                }
                else
                {
                    MessageBox.Show("ERROR AL GUARDAR", "VALIDACIÓN", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
        }

        private void grdDataTurnos_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (grdDataTurnos.Selected.Rows.Count == 0)
            {
                btnAddTurno.Enabled = false;
                btnEditTurno.Enabled = false;
                btnDeleteTurno.Enabled = false;

                btnAddHorario.Enabled = false;
                btnEditHorario.Enabled = false;
                btnDeleteHorario.Enabled = false;

                var objData = GetDataHoras("-1", "-1");

                grdDataHorario.DataSource = objData;
                if (objData != null)
                {
                    lblRecordCountHoras.Text = string.Format("Se encontraron {0} registros.", objData.Count());

                }
                if (grdDataHorario.Rows.Count > 0)
                {
                    grdDataHorario.Rows[1].Selected = true;
                }
                grdDataHorario.DataBind();
                return;
            }
            else
            {
                var Turno = grdDataTurnos.Selected.Rows[0].Cells["v_Value1"].Value.ToString();
                if (Turno != null)
                {
                    if (Turno.Split(' ')[0].Trim() == "(X)")
                    {
                        btnAddHorario.Enabled = false;
                    }
                    else
                    {
                        btnAddHorario.Enabled = true;
                    }

                }
                FiltrarHorario();
            }
        }

        private List<SystemParameter_Horario> GetDataHoras(string grupo, string horas)
        {
            OperationResult objOperationResult = new OperationResult();

            var pacients = _objBL.GetListHoras(ref objOperationResult, 9999, grupo, horas);

            return pacients;
        }

        private void FiltrarHorario()
        {
            btnAddTurno.Enabled = true;
            btnEditTurno.Enabled = true;
            btnDeleteTurno.Enabled = true;

            btnEditHorario.Enabled = false;
            btnDeleteHorario.Enabled = false;

            string GrupoHorario = grdDataTurnos.Selected.Rows[0].Cells["i_GroupId"].Value.ToString();
            string ParentHorario = grdDataTurnos.Selected.Rows[0].Cells["i_ParameterId"].Value.ToString();

            var objData = GetDataHoras(GrupoHorario, ParentHorario);

            grdDataHorario.DataSource = objData;
            if (objData != null)
            {
                lblRecordCountHoras.Text = string.Format("Se encontraron {0} registros.", objData.Count());
            }

            grdDataHorario.DataBind();
        }

        private void btnAddHorario_Click(object sender, EventArgs e)
        {
            frmDetalleHora frm = new frmDetalleHora("", "");
            frm.ShowDialog();
            if (frm.estadoRegistro == 1)
            {
                string detalle = "";
                string descripcion = "";
                int intervalo = 0;
                if (frm.detalle == string.Empty)
                {
                    MessageBox.Show("ERROR EN HORAS ASIGNADAS", "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    detalle = frm.detalle;
                    descripcion = frm.descrpcion;
                    intervalo = frm.Intervalo;
                }

                string Time1 = detalle.Split('-')[0].Trim();
                string Time2 = detalle.Split('-')[1].Trim();

                var HoraInicio = DateTime.Parse(Time1, System.Globalization.CultureInfo.CurrentCulture);
                var HoraFin = DateTime.Parse(Time2, System.Globalization.CultureInfo.CurrentCulture);
                do
                {
                    string GrupoHorario = grdDataTurnos.Selected.Rows[0].Cells["i_GroupId"].Value.ToString();
                    string ParentGrupoHorario = grdDataTurnos.Selected.Rows[0].Cells["i_ParameterId"].Value.ToString();

                    detalle = HoraInicio.TimeOfDay.ToString() + " - " + HoraInicio.AddMinutes(intervalo).TimeOfDay.ToString();
                    HoraInicio = HoraInicio.AddMinutes(intervalo);
                    var resultado = _objBL.CreateHorario(GrupoHorario, detalle, ParentGrupoHorario, descripcion);

                    if (resultado != 0)
                    {
                        FiltrarHorario();
                    }
                    else
                    {
                        MessageBox.Show("ERROR AL GUARDAR", "VALIDACIÓN", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    }

                } while (HoraInicio < HoraFin);
            }
            else
            {
                FiltrarHorario();
            }
        }

        private void btnEditHorario_Click(object sender, EventArgs e)
        {
            string v_Value1 = grdDataHorario.Selected.Rows[0].Cells["v_Value1"].Value.ToString();
            string v_ComentaryUpdate = grdDataHorario.Selected.Rows[0].Cells["v_ComentaryUpdate"].Value.ToString();
            frmDetalleHora frm = new frmDetalleHora(v_Value1, v_ComentaryUpdate);
            frm.ShowDialog();
            string detalle = frm.detalle;
            string v_ComentaryUpdate_ = frm.descrpcion;
            string GrupoHorario = grdDataHorario.Selected.Rows[0].Cells["i_GroupId"].Value.ToString();
            string ParentGrupoHorario = grdDataHorario.Selected.Rows[0].Cells["i_ParameterId"].Value.ToString();
            if (frm.estadoRegistro == 1)
            {
                var resultado = _objBL.EditarDetalle(GrupoHorario, detalle, ParentGrupoHorario, v_ComentaryUpdate_);

                if (resultado != null)
                {
                    MessageBox.Show("GUARDADO CORRECTAMENTE", "INFORMACIÓN", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    FiltrarHorario();
                }
                else
                {
                    MessageBox.Show("ERROR AL GUARDAR", "VALIDACIÓN", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                FiltrarHorario();
            }
        }

        private void btnDeleteHorario_Click(object sender, EventArgs e)
        {
            string GrupoHorario = grdDataHorario.Selected.Rows[0].Cells["i_GroupId"].Value.ToString();
            string ParentGrupoHorario = grdDataHorario.Selected.Rows[0].Cells["i_ParameterId"].Value.ToString();

            var resultado = _objBL.EliminarDetalle(GrupoHorario, ParentGrupoHorario);

            if (resultado != null)
            {
                MessageBox.Show("GUARDADO CORRECTAMENTE", "INFORMACIÓN", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                FiltrarHorario();
            }
            else
            {
                MessageBox.Show("ERROR AL GUARDAR", "VALIDACIÓN", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

        private void grdDataHorario_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (grdDataHorario.Selected.Rows.Count == 0)
            {
                btnAddHorario.Enabled = false;
                btnEditHorario.Enabled = false;
                btnDeleteHorario.Enabled = false;
            }
            else
            {
                var Turno = grdDataTurnos.Selected.Rows[0].Cells["v_Value1"].Value.ToString();
                if (Turno != null)
                {
                    if (Turno.Split(' ')[0].Trim() == "(X)" || grdDataTurnos.Selected.Rows.Count == 0)
                    {
                        btnAddHorario.Enabled = false;
                        btnEditHorario.Enabled = false;
                        btnDeleteHorario.Enabled = false;
                    }
                    else
                    {
                        btnAddHorario.Enabled = true;
                        btnEditHorario.Enabled = true;
                        btnDeleteHorario.Enabled = true;
                    }

                }
            }
        }

        private void grdDataProfesionales_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (grdDataProfesionales.Selected.Rows.Count == 0)
            {
                return;
            }
            else
            {
                btnClonar.Enabled = true;
                FiltrarHorarios();
            }
        }

    }
}
