using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace GoStreamAudioGUI
{
    public class LocalizedForm : Form
    {
        /// <summary>
        /// Occurs when current UI culture is changed
        /// </summary>
        [Browsable(true)]
        [Description("Occurs when current UI culture is changed")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Category("Property Changed")]
        public event EventHandler CultureChanged;

        protected CultureInfo culture;
        protected ComponentResourceManager resManager;

        /// <summary>
        /// Current culture of this form
        /// </summary>
        [Browsable(false)]
        [Description("Current culture of this form")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public CultureInfo Culture
        {
            get { return this.culture; }
            set
            {
                if (this.culture != value)
                {
                    this.ApplyResources(this, value);

                    this.culture = value;
                    this.OnCultureChanged();
                }
            }
        }

        public LocalizedForm()
        {
            this.resManager = new ComponentResourceManager(this.GetType());
            this.culture = CultureInfo.CurrentUICulture;
        }

        private void ApplyResources(Control parent, CultureInfo culture)
        {
            this.resManager.ApplyResources(parent, parent.Name, culture);

            foreach (Control ctl in parent.Controls)
            {
                if (ctl.GetType() == typeof(AppMenuStrip))
                {
                    AppMenuStrip mStrip = (AppMenuStrip)ctl;
                    
                    foreach (ToolStripMenuItem item in mStrip.Items)
                    {
                        ApplyMenus(item.DropDownItems, culture);
                        this.resManager.ApplyResources(item, item.Name, culture);                        
                    }
                }
                else if (ctl.GetType() == typeof(ListView))
                {
                    ListView mList = (ListView)ctl;
                    
                    foreach (ColumnHeader item in mList.Columns)
                    {
                        resManager.ApplyResources(item, item.Name, culture);
                    }
                }
                else
                    this.ApplyResources(ctl, culture);
            }
            if (parent.GetType() == typeof(MainWndPlayer))
            {
                MainWndPlayer frm = (MainWndPlayer)parent;
                resManager.ApplyResources(frm.OpenSFileDlg, "openSFileDlg", culture);
            }
        }

        private void ApplyMenus(ToolStripItemCollection items, CultureInfo culture)
        {
            foreach (ToolStripMenuItem item in items)
            {
                if (item.Name.Equals("langTsmItem"))
                    ApplyMenus(item.DropDownItems, culture);
                this.resManager.ApplyResources(item, item.Name, culture);
            }
        }

        protected void OnCultureChanged()
        {
            var temp = this.CultureChanged;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        public static CultureInfo GlobalUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set
            {
                if (GlobalUICulture.Equals(value) == false)
                {
                    foreach (var form in Application.OpenForms.OfType<LocalizedForm>())
                    {
                        form.Culture = value;
                    }

                    Thread.CurrentThread.CurrentUICulture = value;
                }
            }
        }
    }
}
