using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchBehaviours
{
    public enum DirectionVector
    {
        Forward,
        Right,
        Up,
        Down,
        Left,
        Back
    }

    public class InteractableObject : MonoBehaviour, Iinspectable
    {
        public InteractableObject Back;

        public Transform PrefferedCamTransformPosition;
        public LockState CurrentLockState;
        public LockState GetLockState() { return CurrentLockState; }
        public bool allowPinchBack = true;
        public bool inverseMovement = false;
        public bool recallPosition;

        public float cameraDistanceTo;
        public DirectionVector inverseLookDirection;
        public Vector3 cameraPositionOffset;
        public Vector3 cameraRotationOffset;

        public float MaxRotateUpTo;
        public float MaxRotateDownTo;
        public bool limitHorizontalRotation;
        public float MaxRotateLeftTo;
        public float MaxRotateRightTo;

        public bool dontRotateBack; // dont rotate to the target rotation when going back from this position
        public bool dontRotateTo;

        private GameObject mymoveBackButtons;

        public InteractableObject[] partOf;
        public InteractableObject[] canComeHereFrom;
        public GameObject[] enableWhenHere;
        public GameObject[] disableWhenHere;
        public float disableDelay = 0.15f;
        public bool delayEnable = false;
        public GameState minimumGamestate = GameState.Tutorial;

        CameraController cameraScript;

        [HideInInspector]
        public Quaternion cameraRotation;
        [HideInInspector]
        public Quaternion finalRotation;  // camera rotation including offset
        [HideInInspector]
        public Vector3 finalPosition; // camera position including offset

        [HideInInspector]
        public Vector3 up; //only used for moving around object, not the transition to/from it.

        [HideInInspector]
        public bool skiprelocate;


        public void Inspect()
        {
            if(!GameManager.Instance.InspectableGameObjects.ContainsKey(this))
                GameManager.Instance.InspectableGameObjects.Add(this, this.gameObject);

            this.GetComponent<Collider>().enabled = false;
        }

        public Transform GetPrefCamPosition()
        {
            return PrefferedCamTransformPosition;
        }

        void Awake()
        {
            // RecalcPositionRotation (); // in OnEnable()

            MaxRotateUpTo = Mathf.Min(Mathf.Abs(MaxRotateUpTo), 89);
            MaxRotateDownTo = Mathf.Min(Mathf.Abs(MaxRotateDownTo), 89);
            MaxRotateLeftTo = Mathf.Min(Mathf.Abs(MaxRotateLeftTo), 359);
            MaxRotateRightTo = Mathf.Min(Mathf.Abs(MaxRotateRightTo), 359);
        }

        void Start()
        {
            cameraScript = GameManager.Instance.CameraController;
        }

        void OnEnable()
        {
            if (!skiprelocate)
            {
                RecalcPositionRotation();
            }
        }

        public void RecalcPositionRotation()
        {
            Quaternion rotation = transform.rotation;
            up = Vector3.up;
            // rotate camera to inverse of transform.forward /right /up etc. 
            switch (inverseLookDirection)
            {
                case DirectionVector.Forward: rotation *= Quaternion.Euler(new Vector3(0, 180, 0)); break;
                case DirectionVector.Right: rotation *= Quaternion.Euler(new Vector3(0, 270, 0)); break;
                case DirectionVector.Up: rotation *= Quaternion.Euler(new Vector3(90, 180, 0)); break;
                case DirectionVector.Down: rotation *= Quaternion.Euler(new Vector3(270, 180, 0)); break;
                case DirectionVector.Left: rotation *= Quaternion.Euler(new Vector3(0, 90, 0)); break;
                case DirectionVector.Back: break;
            }
            cameraRotation = rotation;
            finalRotation = cameraRotation * Quaternion.Euler(cameraRotationOffset);

            finalPosition = transform.TransformPoint(cameraPositionOffset);
        }

        // a new GoTo has been tapped (returns whether a move is possible)
        public bool MoveToPosition()
        {
            // error avoidance if you somehow end up here on a diabled object
            if (!this.enabled || !this.gameObject.activeInHierarchy || GameManager.Instance.CurrentGameState < minimumGamestate)
                return false;

            string name = GameManager.Instance.CurrentlyViewing;
            bool foundTarget = false;

            if (this.name == name)
                return false;

            // see if you can get here from current position;
            for (int i = 0; i < canComeHereFrom.Length; i++)
            {
                if (canComeHereFrom[i] != null && canComeHereFrom[i].name == name)
                {
                    if (recallPosition)
                        RecalcPositionRotation();

                    //actually move the camera to this GoTo
                    cameraScript.ChangeView(this);

                    return true;
                }
            }

            // If you cant come here,  see if this is part of something else you can get to
            for (int k = 0; k < partOf.Length; k++)
            {
                if (partOf[k] != null && partOf[k].enabled && partOf[k].gameObject.activeInHierarchy)
                {
                    InteractableObject parent = partOf[k];
                    foundTarget = parent.MoveToPosition();
                    // if the parent found a target, this can stop
                    if (foundTarget)
                        return true;
                }
            }
            return false;

        }

        // disallow the player to come to this location
        public void RemoveAccess()
        {
            canComeHereFrom = new InteractableObject[0];
        }

    }
}
