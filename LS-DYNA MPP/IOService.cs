using System;
using Microsoft.Win32;

namespace LS_DYNA_MPP
{
    public interface IOService
    {
        string OpenFileDialog(string defaultPath);
    }
}
