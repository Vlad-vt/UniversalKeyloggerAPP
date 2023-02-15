using Linearstar.Windows.RawInput;
using System;

namespace UniversalKeylogger.KeyInput
{
    class RawInputEventArgs : EventArgs
    {
        public RawInputEventArgs(RawInputData data)
        {
            Data = data;
        }

        public RawInputData Data { get; }
    }
}
