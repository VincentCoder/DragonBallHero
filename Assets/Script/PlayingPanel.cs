#region

using System.Collections.Generic;

using UnityEngine;

#endregion

public class PlayingPanel : MonoBehaviour
{
    #region Fields

    private CardType nextCardType;

    private UISprite nextSprite;
	
	private MyDirection dragDirection;

    #endregion

    #region Public Properties

    public CardType NextCardType
    {
        get
        {
            return this.nextCardType;
        }
        set
        {
            this.nextCardType = value;
            if (this.nextSprite != null)
            {
                this.nextSprite.spriteName = string.Format("{0}", this.nextCardType);
            }
        }
    }

    #endregion

    #region Methods

    private void AddNewCardToPanel()
    {
        List<int> emptyIndexes = UnitManager.GetInstance().GetEmptyUnits();
        if (emptyIndexes.Count >= 1)
        {
            UnitManager.GetInstance().AddCard(emptyIndexes[Random.Range(0, emptyIndexes.Count)], this.NextCardType);
        }
    }

    private void Awake()
    {
        this.Init();
    }

    private void GenerateNextCardType()
    {
        List<int> buMaList = UnitManager.GetInstance().GetCardsByType(CardType.Buma_0);
        List<int> qiQiList = UnitManager.GetInstance().GetCardsByType(CardType.Qiqi_0);
        if (buMaList.Count - qiQiList.Count >= 3)
        {
            this.NextCardType = CardType.Qiqi_0;
        }
        else if (qiQiList.Count - buMaList.Count >= 3)
        {
            this.NextCardType = CardType.Buma_0;
        }
        else
        {
            this.NextCardType = Random.Range(0, 2) == 0 ? CardType.Buma_0 : CardType.Qiqi_0;
        }
    }

    private void Init()
    {
        int randomUnit1 = Random.Range(0, MyTool.RowCount * MyTool.ColumnCount);
        int randomUnit2;
        while (true)
        {
            randomUnit2 = Random.Range(0, MyTool.RowCount * MyTool.ColumnCount);
            if (randomUnit1 != randomUnit2)
            {
                break;
            }
        }
		
        for (int i = 0; i < 2; i ++)
        {
            int randomLevel = Random.Range(0, 100);
            CardType cardType;
            if (randomLevel < 80)
            {
                cardType = Random.Range(0, 2) == 0 ? CardType.Buma_0 : CardType.Qiqi_0;
            }
            else if (randomLevel >= 80 && randomLevel < 95)
            {
                cardType = CardType.Shadan_1;
            }
            else
            {
                cardType = CardType.Guixianren_2;
            }
            if (i == 0)
            {
                UnitManager.GetInstance().AddCard(randomUnit1, cardType);
            }
            else
            {
                UnitManager.GetInstance().AddCard(randomUnit2, cardType);
            }
        }

        GameObject nextObj = GameObject.FindWithTag("NextCard");
        if (nextObj != null)
        {
            this.nextSprite = nextObj.GetComponent<UISprite>();
        }
        this.GenerateNextCardType();
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
			UnitManager.GetInstance().SendDragMessageToAllCards(gesture, this.dragDirection);
        }
        else if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
			UnitManager.GetInstance().SendDragMessageToAllCards(gesture, this.dragDirection);
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
			UnitManager.GetInstance().SendDragMessageToAllCards(gesture, this.dragDirection);
            this.AddNewCardToPanel();
            this.GenerateNextCardType();
        }
    }

    #endregion
}