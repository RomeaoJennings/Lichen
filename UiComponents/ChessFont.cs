using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using UiComponents.Properties;

namespace UiComponents
{
    public static class ChessFonts
    {
        public static readonly FontFamily Magnetic = InitMagnetic();

        private static FontFamily InitMagnetic()
        {
            var pfc = new PrivateFontCollection();
            int length = Resources.MAGNFONT.Length;
            IntPtr data = Marshal.AllocCoTaskMem(length);
            Marshal.Copy(Resources.MAGNFONT, 0, data, length);
            pfc.AddMemoryFont(data, length);
            Marshal.FreeCoTaskMem(data);
            return pfc.Families[0];
        }
    }


}
