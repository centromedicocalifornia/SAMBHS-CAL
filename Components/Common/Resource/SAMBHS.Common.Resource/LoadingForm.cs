using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LoadingClass
{
    public partial class LoadingForm : Form
    {
        private string _message;
        public LoadingForm(string message)
        {
            InitializeComponent();
            _message = message;
        }

        public LoadingForm()
        {
            InitializeComponent();

        }

        private void LoadingForm_Load(object sender, EventArgs e)
        {
            label1.Text = _message;
        }

        delegate void SetTextCallback(string text);

        public void ChangeMessage(string text)
        {
            if (this.label1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ChangeMessage);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.label1.Text = text;
            }
        }

        public void CloseWindow()
        {
            this.Invoke(new CloseMethod(CloseForm), this);
        }

        delegate void CloseMethod(Form form);

        static private void CloseForm(Form form)
        {
            if (!form.IsDisposed)
            {
                if (form.InvokeRequired)
                {
                    CloseMethod method = new CloseMethod(CloseForm);
                    form.Invoke(method, new object[] { form });
                }
                else
                {
                    form.Close();
                }
            }
        }
    }
}

