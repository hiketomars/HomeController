﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public interface ISiren
    {
        void TurnOn();
        void TurnOff();
        bool IsOn();
    }
}
