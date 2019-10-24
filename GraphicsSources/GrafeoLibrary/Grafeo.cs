using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrafeoLibrary
{
    public class Grafeo
    {
        private GrafeoForm _formGrafeo;
        private Commander _commander = new Commander();

    #region Show
        public void Show()
        {
            if ((_formGrafeo == null) || (_formGrafeo.IsDisposed)) _formGrafeo = new GrafeoForm();
            if (!_formGrafeo.Visible) _formGrafeo.Show();
        }

        public void ShowDialog()
        {
            if ((_formGrafeo == null) || (_formGrafeo.IsDisposed)) _formGrafeo = new GrafeoForm();
            if (!_formGrafeo.Visible) _formGrafeo.ShowDialog();
        }
    #endregion Show

    #region Interface setting
        public bool AllowDeleteGraphic
        {
            get { return true; /*_formGraphic.AllowDeleteGraphic;*/ }
            set { /*_formGraphic.AllowDeleteGraphic = value;*/ }
        }

        public void Init(DateTime timeBegin, DateTime timeEnd) { }
    #endregion Interface setting
    }
}
