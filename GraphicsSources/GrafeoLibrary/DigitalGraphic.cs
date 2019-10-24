using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GrafeoLibrary
{
    public class DigitalGraphic : Graphic
    {
        private Label _axCap;

        public DigitalGraphic(/*FormGraphic formGraphic,*/ Param param, int num)
            : base(param, num)
        {
        }

    #region Overrride
        public override bool IsAnalog { get { return false; } }
        public override bool IsDigital { get { return true; } }
        public override string DataTypeString { get { return "Дискретный"; } }

        public override bool Visible
        {
            get { return base.Visible; }
            internal set
            {
                base.Visible = Visible;
                _axCap.Visible = value;
            }
        }

        internal override void NumDecrease()
        {
            base.NumDecrease();
            _axCap.Text = Num.ToString();
        }
    #endregion Overrride
    }
}
