using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.comm;
using HomeController.utils;

namespace HomeController.model
{
    public class LedHandler
    {
        private readonly LocalCentralUnit lcu;

        public LedHandler(LocalCentralUnit localCentralUnit)
        {
            this.lcu= localCentralUnit;
        }

        public void SetLedCorrectly()
        {
            if (lcu.LcuAlarmHandler.HasIntrusionOccurredLocally)
            {
                lcu.LcuLedController.SetLed_IntrusionHasOccurred();
            }
            else if (lcu.LcuRemoteCentralUnitsController.HasIntrusionOccurred())
            {
                lcu.LcuLedController.SetLed_RemoteIntrusionHasOccurred();
            }
            else if (lcu.LcuAlarmHandler.IsAlarmActive)
            {
                // Active
                if (!lcu.LcuDoorController.IsDoorOpen())
                {
                    // Closed
                    if (lcu.LcuDoorController.IsDoorLocked())
                    {
                        // Locked
                        if (lcu.LcuRemoteCentralUnitsController.IsAnyRemoteDoorUnlocked())
                        {
                            // Remote door(s) unlocked
                            lcu.LcuLedController.SetLed_AlarmIsActiveAndDoorIsLockedButNotAllTheOthersAreLocked();
                        }
                        else
                        {
                            // Remote doors locked as well.
                            lcu.LcuLedController.SetLed_AlarmIsActiveAndDoorIsLockedAndAllTheOthersAsWell();
                        }
                    }
                    else
                    {
                        // Unlocked
                        if(lcu.LcuRemoteCentralUnitsController.IsAnyRemoteDoorUnlocked())
                        {
                            // Remote door(s) unlocked
                            lcu.LcuLedController.SetLed_AlarmIsActiveAndDoorIsUnlockedAndNotAllTheOthersAreLocked();
                        }
                        else
                        {
                            // Remote doors locked as well.
                            lcu.LcuLedController.SetLed_AlarmIsActiveAndDoorIsUnlockedButAllTheOthersAreLocked();
                        }
                    }
                }
                else
                {
                    // Open
                    if(lcu.LcuDoorController.IsDoorLocked())
                    {
                        // Locked but also Open is an error!!
                        lcu.LcuLedController.SetLed_AlarmIsActiveAndDoorIsOpenAndLocked_StatusError();
                    }
                    else
                    {
                        // Unlocked
                        if(lcu.LcuRemoteCentralUnitsController.IsAnyRemoteDoorUnlocked())
                        {
                            // Remote door(s) unlocked
                            lcu.LcuLedController.SetLed_AlarmIsActiveAndDoorIsUnlockedAndNotAllTheOthersAreLocked();
                        }
                        else
                        {
                            // Remote doors are locked, though.
                            lcu.LcuLedController.SetLed_AlarmIsActiveAndDoorIsUnlockedButAllTheOthersAreLocked();
                        }
                    }
                }
            }
            else
            {
                // Inactive
                if(!lcu.LcuDoorController.IsDoorOpen())
                {
                    // Closed
                    if (lcu.LcuDoorController.IsDoorLocked())
                    {
                        // Locked
                        if(lcu.LcuRemoteCentralUnitsController.IsAnyRemoteDoorUnlocked())
                        {
                            // Remote door(s) unlocked
                            lcu.LcuLedController.SetLed_AlarmIsInactiveAndDoorIsLockedButNotAllTheOthersAreLocked();
                        }
                        else
                        {
                            // Remote doors locked as well.
                            lcu.LcuLedController.SetLed_AlarmIsInactiveAndDoorIsLockedAndSoAreAllTheOthers();
                        }
                    }
                    else
                    {
                        // Unlocked
                        if(lcu.LcuRemoteCentralUnitsController.IsAnyRemoteDoorUnlocked())
                        {
                            // Remote door(s) unlocked
                            lcu.LcuLedController.SetLed_AlarmIsInactiveAndDoorIsUnlockedAndNotAllOthersAreLocked();
                        }
                        else
                        {
                            // Remote doors locked as well.
                            lcu.LcuLedController.SetLed_AlarmIsInactiveAndDoorIsUnlockedButAllOthersAreLocked();
                        }
                    }
                }
                else
                {
                    // Open
                    
                    if(lcu.LcuRemoteCentralUnitsController.IsAnyRemoteDoorUnlocked())
                    {
                        // Remote door(s) unlocked
                        lcu.LcuLedController.SetLed_AlarmIsInactiveAndDoorIsOpenAndNotAllOtherDoorsAreLocked();
                    }
                    else
                    {
                        // Remote doors locked.
                        lcu.LcuLedController.SetLed_AlarmIsInactiveAndDoorIsOpenButAllOtherDoorsAreLocked();
                    }
                }

            }

        }
    }
}
