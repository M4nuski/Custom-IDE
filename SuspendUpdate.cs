using System;
using System.Windows.Forms;

//
// Class to set and clear WM_REDRAW message of control
// By: ceztko
// Source: http://stackoverflow.com/questions/487661/how-do-i-suspend-painting-for-a-control-and-its-children
//

namespace ShaderIDE
{
    public static class SuspendUpdate
    {
        private const int WmSetredraw = 0x000B;

        public static void Suspend(Control control)
        {
            var msgSuspendUpdate = Message.Create(control.Handle, WmSetredraw, IntPtr.Zero,
                IntPtr.Zero);

            var window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        public static void Resume(Control control)
        {
            // Create a C "true" boolean as an IntPtr
            var wparam = new IntPtr(1);
            var msgResumeUpdate = Message.Create(control.Handle, WmSetredraw, wparam,
                IntPtr.Zero);

            var window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);

            control.Invalidate();
        }
    }
}
