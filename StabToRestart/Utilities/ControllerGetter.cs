using UnityEngine.XR;
using System.Collections.Generic;

namespace StabToRestart.Utilities
{
    internal class ControllerGetter
    {
        public static bool GetControllers(out InputDevice? leftController, out InputDevice? rightController)
        {
            bool success = true;

            List<InputDevice> controllers = new List<InputDevice>();

            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller, controllers);

            if (controllers.Count != 0)
            {
                leftController = controllers[0];
            }
            else
            {
                leftController = null;
                success = false;
            }

            controllers.Clear();

            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller, controllers);

            if (controllers.Count != 0)
            {
                rightController = controllers[0];
            }
            else
            {
                rightController = null;
                success = false;
            }

            return success;
        }
    }
}
