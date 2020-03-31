using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HV_Test
{
    class StatusLabel : Label
    {
        private bool isPass;
        public StatusLabel()
        {
            isPass = false;
        }

        public bool IsPass
        {
            get { return isPass; }

            set
            {
                isPass = value;

                if (value)
                {
                    this.Text = "PASS >>";
                    this.ForeColor = Color.Gray;
                    this.BackColor = Color.LightGreen;
                }
                else
                {
                    this.Text = "FAIL >>";
                    this.ForeColor = Color.White;
                    this.BackColor = Color.Red;
                }
                Update();
            }
        }
    }
}
