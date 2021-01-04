using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchBehaviours
{
    public enum LockState
    {
        Locked,
        RotationLocked,
        PositionLocked, 
        NoLock,
    }

    public interface Iinspectable
    {
        Transform GetPrefCamPosition();
        LockState GetLockState();
        void Inspect();

        //float CameraZOffset();
    }
}
