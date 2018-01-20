using System;
using System.Windows.Forms;

namespace GoStreamAudioGUI
{
    public class AppMenuStrip : MenuStrip
    {
        private const int WM_MOUSEACTIVATE = 0x21;

        protected override void WndProc(ref Message m)
        {
            // Set focus on WM_MOUSEACTIVATE message
            if (m.Msg == WM_MOUSEACTIVATE  && this.CanFocus && !this.Focused) {
                this.Focus();
            }

            base.WndProc(ref m);
        } 
    }
}
