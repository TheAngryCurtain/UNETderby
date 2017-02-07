using UnityEngine;
using System.Collections;

public class InteractiveVehicleTrigger : InteractiveTrigger
{
    [System.Serializable]
    public class InteractionVehicleInfo : InteractionInfo
    {
        public Transform ActionLocation;
        public Transform ActiveSeat;
        public bool IsDriverDoor;
    }

    [SerializeField] protected Transform _actionLocation;
    [SerializeField] protected Transform _activeSeat;
    [SerializeField] protected bool _isDriverDoor;

    protected override void Start()
    {
        InteractionVehicleInfo vehicleInfo = new InteractionVehicleInfo();

        vehicleInfo.ActionButton = _actionButton;
        vehicleInfo.CalloutText = _calloutText;
        vehicleInfo.Action = _action;
        vehicleInfo.ActionLocation = _actionLocation;
        vehicleInfo.ActiveSeat = _activeSeat;
        vehicleInfo.IsDriverDoor = _isDriverDoor;

        _info = vehicleInfo;
    }
}
