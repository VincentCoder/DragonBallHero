#region

using System;

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
	
	public bool CanMove;
	
	public float TweenMoveSpeed;

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
		this.CanMove = false;
		this.TweenMoveSpeed = 200f;
    }

    private void MergeWithUnit(Unit targetUnit)
    {
        if (targetUnit != null && targetUnit.CardController == null)
        {
			Debug.Log("Merge 1111111");
            UnitManager.GetInstance().ClearUnitById(this.Card.CardId);
            this.Card.CardId = targetUnit.UnitId;
            this.Card.RowIndex = targetUnit.RowIndex;
            this.Card.ColumnIndex = targetUnit.ColumnIndex;
            targetUnit.CardController = this;
			this.TweenMoveTo(this.Card.RowIndex, this.Card.ColumnIndex);
            //this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(
            //    this.Card.RowIndex,
           //     this.Card.ColumnIndex);
        }
        else if (targetUnit != null && targetUnit.CardController.Card.CardType == this.Card.CardType
                 && this.Card.CardType != CardType.Wukong_11)
        {
			Debug.Log("Merge 222222222");
            //EventDelegate.Callback callback = delegate
           //     {
           //         UnitManager.GetInstance().ClearUnitById(this.Card.CardId);
           //         targetUnit.CardController.ResetCardType(MyTool.GetNextLevelCardType(this.Card.CardType));
            //        this.DestroySelf();
           //     };
            this.TweenMoveTo(targetUnit.RowIndex, targetUnit.ColumnIndex, delegate
            {
                UnitManager.GetInstance().ClearUnitById(this.Card.CardId);
                targetUnit.CardController.ResetCardType(MyTool.GetNextLevelCardType(this.Card.CardType));
                this.DestroySelf();
            });
        }
		else
		{
			Debug.Log("Merge 333333333333");
			this.TweenMoveTo(this.Card.RowIndex, this.Card.ColumnIndex);
			//this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(
            //    this.Card.RowIndex,
           //     this.Card.ColumnIndex);
		}
    }

    public void OnDragCard(DragGesture gesture, MyDirection direction)
    {
		this.dragDirection = direction;
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
			Unit unitDragTo = UnitManager.GetInstance().GetUnitInDirection(this.Card.CardId, this.dragDirection);
			if(unitDragTo != null && (unitDragTo.CardController == null || unitDragTo.CardController.CanMove || unitDragTo.CardController.Card.CardType == this.Card.CardType))
			{
				this.CanMove = true;
			}
			Debug.Log("DragGesture Started : " + this.Card.CardId + " " + this.dragDirection + " " + this.myTransform.localPosition);
        } 
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
			Unit unitDragTo = UnitManager.GetInstance().GetUnitInDirection(this.Card.CardId, this.dragDirection);
			Debug.Log("DragGesture Updated : " + this.Card.CardId + "_" + this.Card.RowIndex + "_" + this.Card.ColumnIndex + " " + unitDragTo);
            if(unitDragTo != null) Debug.Log(" unitDragTo.CardController " + unitDragTo.CardController + " " + unitDragTo.UnitId + " " + unitDragTo.RowIndex + " " + unitDragTo.ColumnIndex);
			if(unitDragTo != null && unitDragTo.CardController != null) Debug.Log(" unitDragTo.CardController.Card.CardType " + unitDragTo.CardController.Card.CardType + " this.Card.CardType  " + this.Card.CardType);
            if (this.CanMove)
            {
				this.Card.CardState = CardState.Moving;
                Vector3 posDragTo = MyTool.CalculatePositionByRowAndColumn(unitDragTo.RowIndex, unitDragTo.ColumnIndex);
				
                switch (this.dragDirection)
                {
                    case MyDirection.Up:
                        {
                            if (gesture.TotalMove.y > MyTool.CardWidth + MyTool.UnitGap)
                            {
								Debug.Log("1111111111111");
								this.TweenMoveTo(unitDragTo.RowIndex, unitDragTo.ColumnIndex);
                                //this.myTransform.localPosition = posDragTo;
                            }
                            else if (gesture.TotalMove.y < 0)
                            {
								Debug.Log("22222222222222");
								this.TweenMoveTo(this.Card.RowIndex, this.Card.ColumnIndex);
                                //this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
                            }
                            else
                            {
								Debug.Log("333333333333");
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
								Debug.Log("1111111111111");
                                this.TweenMoveTo(unitDragTo.RowIndex, unitDragTo.ColumnIndex);
                                //this.myTransform.localPosition = posDragTo;
                            }
                            else if (gesture.TotalMove.y > 0)
                            {
								Debug.Log("22222222222222");
                                this.TweenMoveTo(this.Card.RowIndex, this.Card.ColumnIndex);
                                //this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
                            }
                            else
                            {
								Debug.Log("333333333333");
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
								Debug.Log("1111111111111");
                                this.TweenMoveTo(unitDragTo.RowIndex, unitDragTo.ColumnIndex);
                                //this.myTransform.localPosition = posDragTo;
                            }
                            else if (gesture.TotalMove.x > 0)
                            {
								Debug.Log("22222222222222");
                                this.TweenMoveTo(this.Card.RowIndex, this.Card.ColumnIndex);
                                //this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
                            }
                            else
                            {
								Debug.Log("333333333333");
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
								Debug.Log("1111111111111");
                                this.TweenMoveTo(unitDragTo.RowIndex, unitDragTo.ColumnIndex);
                                //this.myTransform.localPosition = posDragTo;
                            }
                            else if (gesture.TotalMove.x < 0)
                            {
								Debug.Log("22222222222222");
                                this.TweenMoveTo(this.Card.RowIndex, this.Card.ColumnIndex);
                                //this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
							}
							else
							{
								Debug.Log("333333333333");
								this.myTransform.localPosition += new Vector3(
                                    gesture.DeltaMove.x,
                                    0,
                                    0);
                            }
                        }
                        break;
                }
            }
			else
			{
				this.Card.CardState = CardState.Normal;
			}
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
			Debug.Log("End CardID" + this.Card.CardId);
            Unit unitDragTo = UnitManager.GetInstance().GetUnitInDirection(this.Card.CardId, this.dragDirection);
            switch (this.dragDirection)
            {
                case MyDirection.Up:
                    {
                        if (gesture.TotalMove.y > MyTool.UnitGap)
                        {
							Debug.Log("End 1111111");
                            this.MergeWithUnit(unitDragTo);
                        }
                        else
                        {
							Debug.Log("End 222222222");
							this.TweenMoveTo(this.Card.RowIndex, this.Card.ColumnIndex);
                            //this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
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
							this.TweenMoveTo(this.Card.RowIndex, this.Card.ColumnIndex);
                            //this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
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
							this.TweenMoveTo(this.Card.RowIndex, this.Card.ColumnIndex);
                            //this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
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
							this.TweenMoveTo(this.Card.RowIndex, this.Card.ColumnIndex);
                            //this.myTransform.localPosition = MyTool.CalculatePositionByRowAndColumn(this.Card.RowIndex, this.Card.ColumnIndex);
                        }
                    }
                    break;
            }
			this.Card.CardState = CardState.Normal;
			this.CanMove = false;
        }
    }
	
	public void TweenMoveTo(int rowId, int columnId, EventDelegate.Callback callback = null)
	{
		TweenPosition tweenPos = this.gameObject.GetComponent<TweenPosition>();
		if(tweenPos == null)
		{
			tweenPos = this.gameObject.AddComponent<TweenPosition>();
		}
		tweenPos.style = UITweener.Style.Once;
		tweenPos.method = UITweener.Method.Linear;
		tweenPos.from = this.myTransform.localPosition;
		tweenPos.to = MyTool.CalculatePositionByRowAndColumn(rowId, columnId);
		//tweenPos.duration = Vector3.Distance(tweenPos.from, tweenPos.to) / this.TweenMoveSpeed;
		tweenPos.duration = 0.3f;
        tweenPos.onFinished.Add(new EventDelegate(callback));
		tweenPos.PlayForward();
	}

    #endregion
}