using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App_Demo.UI
{
    public partial class DownloadingUpdate : Form
    {
        
        bool PermiteCerrar = false;

        public DownloadingUpdate()
        {
            InitializeComponent();
        }

        private void DownloadingUpdate_Load(object sender, EventArgs e)
        {
            
        }

        

        private void DownloadingUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (PermiteCerrar) e.Cancel = false;
            else e.Cancel = true;
        }
    }
}
