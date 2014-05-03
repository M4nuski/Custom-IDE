
//
// Class to enable string.Remove with an index equal to the length of the string 
// without causing and out-of-bound index exception.
// By: M4nuski (2014-04-12)
//

namespace ShaderIDE
{
    public static class StringSafeRemove
    {
        public static string SafeRemove(this string inputString, int index)
        {
            return (index <= 0) ? "" : (index < inputString.Length) ? inputString.Remove(index) : inputString;
        }
    }
}
