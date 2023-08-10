using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
 
public class ChoppySlider : Slider
{
    [SerializeField]private float stepsIncrement = .1f;
 
    public override void OnMove(AxisEventData eventData)
    {
        if((direction == Direction.RightToLeft || direction == Direction.LeftToRight) &&
           (eventData.moveDir == MoveDirection.Left || eventData.moveDir == MoveDirection.Right))
        {
            value += eventData.moveDir == MoveDirection.Left ? -stepsIncrement : stepsIncrement;
            return;
        }
 
        if ((direction == Direction.BottomToTop || direction == Direction.TopToBottom) &&
            (eventData.moveDir == MoveDirection.Down || eventData.moveDir == MoveDirection.Up))
        {
            value += eventData.moveDir == MoveDirection.Down ? -stepsIncrement : stepsIncrement;
            return;
        }
     
        base.OnMove(eventData);
    }
 
}