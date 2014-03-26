using UnityEngine;
using System.Collections;

public class CardController : MonoBehaviour
{
    private Transform myTransform;

    private void Awake()
    {
        this.myTransform = this.transform;
    }

    void OnDrag(DragGesture gesture)
    {
        Debug.Log("OnDrag");
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            this.myTransform.localPosition += new Vector3(gesture.DeltaMove.x, gesture.DeltaMove.y, 0);
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            
        }
    }
}
