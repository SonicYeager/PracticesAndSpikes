﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadedLogger
{
    public interface IThreadedLogger
    {
        public void QueryLog(string msg);
    }
}
