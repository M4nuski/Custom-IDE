using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderIDE
{
    public delegate void ConsoleEvent(string text);
    static class Console
    {
        static public event ConsoleEvent OnConsoleMessage;

        static public void Message(string text)
        {
            if (OnConsoleMessage != null) OnConsoleMessage(text);
        }
    }
}
