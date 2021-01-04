using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchBehaviours
{
    public class CubeObject : PuzzleItem, Iinspectable
    {
        public Transform GetPrefCamPosition()
        {
            throw new System.NotImplementedException();
        }
        public LockState CurrentLockState;
        public LockState GetLockState() { return CurrentLockState; }
        public void Inspect()
        {

        }

        public float CameraZOffset()
        {
            return 0;
        }

        public Vector2 ClampX()
        {
            return Vector2.zero;
        }

        public Vector2 ClampY()
        {
            return Vector2.zero;
        }
    }
}
