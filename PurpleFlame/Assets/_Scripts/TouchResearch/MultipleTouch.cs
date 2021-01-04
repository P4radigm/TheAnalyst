using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchBehaviours
{
    public class MultipleTouch : MonoBehaviour
    {
        public GameObject Circle;
        public List<TouchLocation> Touches = new List<TouchLocation>();

        private void Update()
        {
            int i = 0;
            while(i < Input.touchCount)
            {
                Touch t = Input.GetTouch(i);

                if(t.phase == TouchPhase.Began)
                {
                    Touches.Add(new TouchLocation(t.fingerId, CreateCircle(t)));
                }
                else if(t.phase == TouchPhase.Ended)
                {
                    TouchLocation thisTouchLocation = Touches.Find(tl => tl.TouchId == t.fingerId);
                    Destroy(thisTouchLocation.Circle);
                    Touches.RemoveAt(Touches.IndexOf(thisTouchLocation));
                }
                else if(t.phase == TouchPhase.Moved)
                {
                    TouchLocation thisTouchLocation = Touches.Find(tl => tl.TouchId == t.fingerId);
                    thisTouchLocation.Circle.transform.position = GetTouchPosition(t.position);
                }
                ++i;
            }
        }

        private Vector2 GetTouchPosition(Vector2 touchPosition)
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, transform.position.z));
        }

        private GameObject CreateCircle(Touch t)
        {
            GameObject c = Instantiate(Circle) as GameObject;
            c.name = "Touch" + t.fingerId;
            c.transform.position = GetTouchPosition(t.position);
            return c;
        }
    }
}