using Linearstar.Windows.RawInput;
using System;
using System.Windows.Forms;

namespace UniversalKeylogger.KeyInput
{
    internal class RawInputReceiverWindow : NativeWindow
    {
        public event EventHandler<RawInputEventArgs> Input;
        public RawInputReceiverWindow()
        {
            CreateHandle(new CreateParams
            {
                X = 0,
                Y = 0,
                Width = 0,
                Height = 0,
                Style = 0x800000,
            });
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_INPUT = 0x00FF;
            const int WM_LBUTTONDOWN = 0x0201;

            if (m.Msg == WM_INPUT)
            {
                var data = RawInputData.FromHandle(m.LParam);
                    Input?.Invoke(this, new RawInputEventArgs(data));
            }


            base.WndProc(ref m);
        }
    }
}
