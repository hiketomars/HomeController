using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

namespace HomeController.model
{
    public class DoorController : IDoorController
    {
        public IDoor Door { get; set; }

        public bool IsDoorOpen() 
        {
            return Door.IsOpen; 
        }

        public bool IsDoorLocked()
        {
            return Door.IsLocked;
        }

        public void Reset()
        {
        }

        //private static IDoorController instance;

        //public static void SetInstance(IDoorController doorController)
        //{
        //    instance = doorController;
        //}
        //public static IDoorController GetInstance()
        //{
        //    if (instance == null)
        //    {
        //        instance = new DoorController();
        //    }

        //    return instance;
        //}
    }
}
