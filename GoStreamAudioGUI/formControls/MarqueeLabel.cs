using System;
using System.Drawing;
using System.Windows.Forms;

namespace GoStreamAudioGUI
{
    public class MarqueeLabel : Label
    {
        public Timer MarqueeTimer { get; set; }
        public int Speed { get; set; }
        public int yOffset { get; set; }

        public void Start() { MarqueeTimer.Start(); }
        public void Stop() { MarqueeTimer.Stop(); }

        private int offset;
        SolidBrush backBrush;
        SolidBrush textBrush;

        public MarqueeLabel()
        {
            textBrush = new SolidBrush(this.ForeColor);
            backBrush = new SolidBrush(this.BackColor);
            yOffset = 0;
            Speed = 1;
            MarqueeTimer = new Timer();
            MarqueeTimer.Interval = 25;
            MarqueeTimer.Enabled = true;
            MarqueeTimer.Tick += (aSender, eArgs) =>
            {
                offset = (offset - Speed);
                if (offset < -this.ClientSize.Width) offset = 0;
                this.Invalidate();
            };
        }

        public void ResetPosition(int w)
        {
            offset = w;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.FillRectangle(backBrush, e.ClipRectangle);
            e.Graphics.DrawString(this.Text, this.Font, textBrush, offset, yOffset);
            e.Graphics.DrawString(this.Text, this.Font, textBrush,
                                  this.ClientSize.Width + offset, yOffset);
        }
    }
}
