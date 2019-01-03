﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.utils;
namespace HomeController.model
{
    
    public interface IHouseModel
    {
        event Definition.VoidEventHandler ModelHasChanged;
        List<string> GetLoggings();
    }
}
