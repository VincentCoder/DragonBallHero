#region

using UnityEngine;

#endregion

public class CardController : MonoBehaviour
{
    #region Fields

    private Card card;

    private UISprite cardSprite;

    private MyDirection dragDirection;

    private UISprite frameSprite;

    private Transform myTransform;

    #endregion

    #region Public Properties

    public Card Card
    {
        get
        {
            if (this.card == null)
            {
                this.card = new Card();
            }
            return this.card;
        }
    }

    #endregion

    #region Public Methods and Operators

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public void ResetCardType(CardType cardType)
    {
        this.Card.CardType = cardType;
        if (this.cardSprite != null && this.frameSprite != null)
        {
            this.cardSprite.spriteName = "" + cardType;
            if (cardType == CardType.Buma_0)
            {
                this.frameSprite.spriteName = "Frame_BuMa";
            }
            else if (cardType == CardType.Qiqi_0)
            {
                this.frameSprite.spriteName = "Frame_QiQi";
            }
            else if (UnitManager.GetInstance().GetCardIdOfCurrentTopLevel() == this.Card.CardId)
            {
                this.frameSprite.spriteName = "Frame_TopLevel";
            }
            else
            {
                this.frameSprite.spriteName = "Frame_Normal";
            }
        }
    }

    #endregion

    #region Methods

    private void Awake()
    {
        this.myTransform = this.transform;
        this.cardSprite = this.myTransform.FindChild("Card").GetComponent<UISprite>();
        this.frameSprite = this.myTransform.FindChild("Frame").GetComponent<UISprite>();
    }

    private void MergeWithUnit(Unit targetUnit)
    {
        if (targetUnit == null)
        {
            Debug.LogError("Invalid Unit!!");
        }
        if (targetUnit.CardController == null)
        {
            UnitManager.GetInstance().ClearUnitById(this.Card.CardId);
            this.Card.CardId = targetUnit.UnitId;
            this.Card.RowIndex = targetUnit.RowIndex;
            this.Card.ColumnIndex = targetUnit.ColumnIndex;
            targetUnit.CardController = this;
            this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(
                this.Card.RowIndex,
                this.Card.ColumnIndex);
        }
        else if (targetUnit.CardController.Card.CardType == this.Card.CardType
                 && this.Card.CardType != CardType.Wukong_11)
        {
            UnitManager.GetInstance().ClearUnitById(this.Card.CardId);
            targetUnit.CardController.Card.CardType = MyTool.GetNextLevelCardType(this.Card.CardType);
            this.DestroySelf();
        }
    }

    private void OnDrag(DragGesture gesture)
    {
        Debug.Log("OnDrag");
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            if (gesture.DeltaMove.x < gesture.DeltaMove.y)
            {
                if (gesture.DeltaMove.y > 0)
                {
                    this.dragDirection = MyDirection.Up;
                }
                else
                {
                    this.dragDirection = MyDirection.Down;
                }
            }
            else
            {
                if (gesture.DeltaMove.x > 0)
                {
                    this.dragDirection = MyDirection.Right;
                }
                else
                {
                    this.dragDirection = MyDirection.Left;
                }
            }
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            Unit unitDragTo = UnitManager.GetInstance().GetUnitInDirection(this.Card.CardId, this.dragDirection);
            if (unitDragTo != null
                && (unitDragTo.CardController == null || unitDragTo.CardController.Card.CardType == this.Card.CardType))
            {
                Vector3 posDragTo = MyTool.CalculatePositionByRowAndColumn(unitDragTo.RowIndex, unitDragTo.ColumnIndex);
                switch (this.dragDirection)
                {
                    case MyDirection.Up:
                        {
                            if (gesture.TotalMove.y > MyTool.CardWidth + MyTool.UnitGap)
                            {
                                this.myTransform.localPosition = posDragTo;
                            }
                            else if (gesture.TotalMove.y < 0)
                            {
                                this.myTransform.localPosition = new Vector3(
                                    gesture.StartPosition.x,
                                    gesture.StartPosition.y,
                                    0);
                            }
                            else
                            {
                                this.myTransform.localPosition += new Vector3(
                                    gesture.DeltaMove.x,
                                    gesture.DeltaMove.y,
                                    0);
                            }
                        }
                        break;
                    case MyDirection.Down:
                        {
                            if (gesture.TotalMove.y < - (MyTool.CardWidth + MyTool.UnitGap))
                            {
                                this.myTransform.localPosition = posDragTo;
                            }
                            else if (gesture.TotalMove.y > 0)
                            {
                                this.myTransform.localPosition = new Vector3(
                                    gesture.StartPosition.x,
                                    gesture.StartPosition.y,
                                    0);
                            }
                            else
                            {
                                this.myTransform.localPosition += new Vector3(
                                    gesture.DeltaMove.x,
                                    gesture.DeltaMove.y,
                                    0);
                            }
                        }
                        break;
                    case MyDirection.Left:
                        {
                            if (gesture.TotalMove.x < - (MyTool.CardWidth + MyTool.UnitGap))
                            {
                                this.myTransform.localPosition = posDragTo;
                            }
                            else if (gesture.TotalMove.x > 0)
                            {
                                this.myTransform.localPosition = new Vector3(
                                    gesture.StartPosition.x,
                                    gesture.StartPosition.y,
                                    0);
                            }
                            else
                            {
                                this.myTransform.localPosition += new Vector3(
                                    gesture.DeltaMove.x,
                                    gesture.DeltaMove.y,
                                    0);
                            }
                        }
                        break;
                    case MyDirection.Right:
                        {
                            if (gesture.TotalMove.x > MyTool.CardWidth + MyTool.UnitGap)
                            {
                                this.myTransform.localPosition = posDragTo;
                            }
                            else if (gesture.TotalMove.x < 0)
                            {
                                this.myTransform.localPosition = new Vector3(
                                    gesture.StartPosition.x,
                                    gesture.StartPosition.y,
                                    0);
                            }
                            else
                            {
                                this.myTransform.localPosition += new Vector3(
                                    gesture.DeltaMove.x,
                                    gesture.DeltaMove.y,
                                    0);
                            }
                        }
                        break;
                }
            }
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            Unit unitDragTo = UnitManager.GetInstance().GetUnitInDirection(this.Card.CardId, this.dragDirection);
            switch (this.dragDirection)
            {
                case MyDirection.Up:
                    {
                        if (gesture.TotalMove.y > MyTool.UnitGap)
                        {
                            this.MergeWithUnit(unitDragTo);
                        }
                        else
                        {
                            this.myTransform.localPosition = new Vector3(
                                gesture.StartPosition.x,
                                gesture.StartPosition.y,
                                0);
                        }
                    }
                    break;
                case MyDirection.Down:
                    {
                        if (gesture.TotalMove.y < -MyTool.UnitGap)
                        {
                            this.MergeWithUnit(unitDragTo);
                        }
                        else
                        {
                            this.myTransform.localPosition = new Vector3(
                                gesture.StartPosition.x,
                                gesture.StartPosition.y,
                                0);
                        }
                    }
                    break;
                case MyDirection.Left:
                    {
                        if (gesture.TotalMove.x < - MyTool.UnitGap)
                        {
                            this.MergeWithUnit(unitDragTo);
                        }
                        else
                        {
                            this.myTransform.localPosition = new Vector3(
                                gesture.StartPosition.x,
                                gesture.StartPosition.y,
                                0);
                        }
                    }
                    break;
                case MyDirection.Right:
                    {
                        if (gesture.TotalMove.x > MyTool.UnitGap)
                        {
                            this.MergeWithUnit(unitDragTo);
                        }
                        else
                        {
                            this.myTransform.localPosition = new Vector3(
                                gesture.StartPosition.x,
                                gesture.StartPosition.y,
                                0);
                        }
                    }
                    break;
            }
        }
    }

    #endregion
}