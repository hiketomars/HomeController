using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.utils;

namespace HomeController.model
{
    public interface ILEDController
    {
        IRgbLed ControlledRgbLed { get; }
        void SetTotalColor(RGBValue green);
        RGBValue GetLedColor();
        void SetLed_IntrusionHasOccurred();

        void Reset();
        void SetLed_RemoteIntrusionHasOccurred();
        void SetLed_AlarmIsInactiveAndDoorIsUnlockedAndNotAllOthersAreLocked();
        void SetLed_AlarmIsInactiveAndDoorIsLockedAndSoAreAllTheOthers();
        void SetLed_AlarmIsInactiveAndDoorIsLockedButNotAllTheOthersAreLocked();
        void SetLed_AlarmIsActiveAndDoorIsLockedAndAllTheOthersAsWell();
        void SetLed_AlarmIsActiveAndDoorIsLockedButNotAllTheOthersAreLocked();
        void SetLed_AlarmIsActiveAndDoorIsUnlockedButAllTheOthersAreLocked();
        void SetLed_AlarmIsActiveAndDoorIsUnlockedAndNotAllTheOthersAreLocked();
        void SetLed_AlarmIsInactiveAndDoorIsUnlockedButAllOthersAreLocked();
        void SetLed_AlarmIsInactiveAndDoorIsOpenButAllOtherDoorsAreLocked();
        void SetLed_AlarmIsInactiveAndDoorIsOpenAndNotAllOtherDoorsAreLocked();
        void SetLed_AlarmIsActiveAndDoorIsOpenAndLocked_StatusError();
    }
}
