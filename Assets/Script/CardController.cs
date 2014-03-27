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
        if (targetUnit != null && targetUnit.CardController == null)
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
        else if (targetUnit != null && targetUnit.CardController.Card.CardType == this.Card.CardType
                 && this.Card.CardType != CardType.Wukong_11)
        {
            UnitManager.GetInstance().ClearUnitById(this.Card.CardId);
            targetUnit.CardController.ResetCardType(MyTool.GetNextLevelCardType(this.Card.CardType));
            this.DestroySelf();
        }
		else
		{
			this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(
                this.Card.RowIndex,
                this.Card.ColumnIndex);
		}
    }

    private void OnDrag(DragGesture gesture)
    {
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            if (Mathf.Abs(gesture.DeltaMove.x) < Mathf.Abs(gesture.DeltaMove.y))
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
			Debug.Log("DragGesture Started : " + this.Card.CardId + " " + this.dragDirection + " " + this.myTransform.localPosition);
        } 
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
			Unit unitDragTo = UnitManager.GetInstance().GetUnitInDirection(this.Card.CardId, this.dragDirection);
			Debug.Log("DragGesture Updated : " + this.Card.CardId + " " + unitDragTo);
            if(unitDragTo != null) Debug.Log(" unitDragTo.CardController " + unitDragTo.CardController );
			if(unitDragTo != null && unitDragTo.CardController != null) Debug.Log(" unitDragTo.CardController.Card.CardType " + unitDragTo.CardController.Card.CardType + " this.Card.CardType  " + this.Card.CardType);
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
                                this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
                            }
                            else
                            {
                                this.myTransform.localPosition += new Vector3(
                                    0,
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
                                this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
                            }
                            else
                            {
                                this.myTransform.localPosition += new Vector3(
                                    0,
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
                                this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
                            }
                            else
                            {
                                this.myTransform.localPosition += new Vector3(
                                    gesture.DeltaMove.x,
                                    0,
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
                                this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
							}
							else
							{
								this.myTransform.localPosition += new Vector3(
                                    gesture.DeltaMove.x,
                                    0,
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
                            this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
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
                            this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
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
                            this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
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
                            this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
                        }
                    }
                    break;
            }
        }
    }

    #endregion
}