﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoDesigner.CustomEventArgs
{
    public class OpenFileEventArgs : EventArgs
    {
        public OpenFileEventArgs(string filePathToUse)
        {
            FilePath = filePathToUse;
        }

        public string FilePath { get; private set; }
    }
}
