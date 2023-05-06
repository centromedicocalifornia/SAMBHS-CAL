using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.ActivoFijo.BL ;
using SAMBHS.Common.Resource; 

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.ActivoFijo
{
    public partial class frmBuscarTipoActivo : Form
    {
       
        ActivoFijoBL _objActivoFijoBL= new ActivoFijoBL ();
        public string CodigoTipoActivoFijo = string.Empty;
        public string Modo = String.Empty;
        public int Grupo = -1;
        public frmBuscarTipoActivo(string pstrModo, int pIntGrupo)
        {

            InitializeComponent();
            Modo = pstrModo;
            Grupo = pIntGrupo;
        }

        private void frmBuscarTipoActivo_Load(object sender, EventArgs e)
        {

            this.BackColor = new GlobalFormColors().FormColor;
            if (Modo == "CRUD")
            {
                this.Location = new Point(1000, 200);
            }
            else if (Modo == "MOVIMIENTO")
            {
            }
            else
            {
                this.Location = new Point(50, 50);
            }
           
            var TipoActivoFijo = _objActivoFijoBL.ObtenerTiposActivoFijos(Grupo);
            var TipoActivoFijoPadre = TipoActivoFijo.Where(x => x.Padre != string.Empty).OrderBy (x=>x.Codigo);

            foreach (var item in TipoActivoFijoPadre)
            {
                var xx = item.Codigo.Substring(0,2);
                var Nodos = TipoActivoFijo.Where(x => x.Codigo.Substring (0,2)== item.Codigo.Substring (0,2) &&  x.Codigo !=item.Codigo ).ToList();
                List<TreeNode> array = new List<TreeNode>();
                foreach (var nodo in Nodos)
                {
                    TreeNode Nodo = new TreeNode(nodo.Nodos);
                    Nodo.Tag = "C";
                    array.Add(Nodo);
                    
                }
               
                TreeNode treeNode = new TreeNode(item.Padre,array.ToArray());
                treeNode.BackColor = Color.AliceBlue;
                treeNode.Tag = "P";
                trrTipoActivo.Nodes.Add(treeNode);


            }
            
        }

        private void trrTipoActivo_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //nodo TreeNode = TreeView1.SelectedNode;
            TreeNode nodo = trrTipoActivo.SelectedNode;
            string[] NodoElegido ;
            if (nodo.Tag  == "C")
            {
                NodoElegido = nodo.Text.Split(new Char[] { '|' });
                CodigoTipoActivoFijo = NodoElegido[0].Trim();
                //MessageBox.Show(string.Format("Usted seleccionó Nodo Hijo: {0} ", nodo.Text));
                this.Close();
            }
            else
            {
                UltraMessageBox.Show("No se retorna Códigos Genéricos  ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
        }
    }
}
