#region

using System.Collections.Generic;

using UnityEngine;

#endregion

public class UnitManager
{
    #region Static Fields

    private static readonly object lockObj = new object();

    private static UnitManager instance;

    #endregion

    #region Fields

    private Unit[,] unitArray = new Unit[MyTool.RowCount, MyTool.ColumnCount];

    #endregion

    #region Constructors and Destructors

    private UnitManager()
    {
        // Initial UnitArray
        for (int i = 0; i < MyTool.RowCount; i ++)
        {
            for (int j = 0; j < MyTool.ColumnCount; j ++)
            {
                Unit unit = new Unit();
                unit.UnitId = MyTool.CalculateUnitIdByRowAndColumn(i, j);
                unit.RowIndex = i;
                unit.ColumnIndex = j;
                unit.CardController = null;
                this.unitArray[i, j] = unit;
            }
        }
    }

    #endregion

    #region Public Methods and Operators

    public static UnitManager GetInstance()
    {
        if (instance == null)
        {
            lock (lockObj)
            {
                instance = new UnitManager();
            }
        }
        return instance;
    }

    public void AddCard(int cardId, CardType cardType)
    {
        GameObject cardObj = (GameObject)Object.Instantiate(Resources.Load("Card"));
        int rowId;
        int columnId = MyTool.CalculateRowAndColumnByUnitId(cardId, out rowId);
        if (rowId == -1 || columnId == -1)
        {
            return;
        }
        cardObj.name = "Card_" + cardId + "_" + rowId + "_" + columnId;
        cardObj.transform.parent = GameObject.FindWithTag("PlayingPanel").transform;
        cardObj.transform.localPosition = MyTool.CalculatePositionByRowAndColumn(rowId, columnId);
        cardObj.transform.localScale = new Vector3(1, 1, 1);

        CardController cardCtrl = cardObj.GetComponent<CardController>();
        cardCtrl.Card.CardId = cardId;
        cardCtrl.Card.RowIndex = rowId;
        cardCtrl.Card.ColumnIndex = columnId;
		cardCtrl.Card.CardState = CardState.Normal;
        cardCtrl.ResetCardType(cardType);

        this.unitArray[rowId, columnId].CardController = cardCtrl;
    }

    public void ClearUnitById(int unitId)
    {
        int rowId;
        int columnId = MyTool.CalculateRowAndColumnByUnitId(unitId, out rowId);
        if (rowId == -1 || columnId == -1)
        {
            return;
        }
        this.unitArray[rowId, columnId].CardController = null;
    }

    public int GetCardIdOfCurrentTopLevel()
    {
        int level = -1;
        int cardId = -1;
        for (int i = 0; i < MyTool.RowCount; i ++)
        {
            for (int j = 0; j < MyTool.ColumnCount; j ++)
            {
                CardController cardCtrl = this.unitArray[i, j].CardController;
                if (cardCtrl != null)
                {
                    string[] strArr = cardCtrl.Card.CardType.ToString().Split('_');
                    int currentLevel;
                    if (int.TryParse(strArr[1], out currentLevel))
                    {
                        if (currentLevel > level)
                        {
                            level = currentLevel;
                            cardId = cardCtrl.Card.CardId;
                        }
                    }
                }
            }
        }
        return cardId;
    }

    public List<int> GetCardsByType(CardType cardType)
    {
        List<int> indexes = new List<int>();
        for (int i = 0; i < MyTool.RowCount; i ++)
        {
            for (int j = 0; j < MyTool.ColumnCount; j ++)
            {
                if (this.unitArray[i, j] != null && this.unitArray[i, j].CardController != null
                    && this.unitArray[i, j].CardController.Card.CardType == cardType)
                {
                    indexes.Add(this.unitArray[i, j].CardController.Card.CardId);
                }
            }
        }
        return indexes;
    }

    public List<int> GetEmptyUnits()
    {
        List<int> emptyIndexes = new List<int>();
        for (int i = 0; i < MyTool.RowCount; i ++)
        {
            for (int j = 0; j < MyTool.ColumnCount; j ++)
            {
                if (this.unitArray[i, j] != null && this.unitArray[i, j].CardController == null)
                {
                    emptyIndexes.Add(this.unitArray[i, j].UnitId);
                }
            }
        }
        return emptyIndexes;
    }

    public Unit GetUnitInDirection(int cardId, MyDirection direction)
    {
        int rowId;
        int columnId = MyTool.CalculateRowAndColumnByUnitId(cardId, out rowId);
        if (rowId == -1 || columnId == -1)
        {
            return null;
        }
        switch (direction)
        {
            case MyDirection.Up:
                return rowId - 1 < 0 ? null : this.unitArray[rowId - 1, columnId];
            case MyDirection.Down:
                return rowId + 1 >= MyTool.RowCount ? null : this.unitArray[rowId + 1, columnId];
            case MyDirection.Left:
                return columnId - 1 < 0 ? null : this.unitArray[rowId, columnId - 1];
            case MyDirection.Right:
                return columnId + 1 >= MyTool.ColumnCount ? null : this.unitArray[rowId, columnId + 1];
        }
        return null;
    }
	
	public void SendDragMessageToAllCards(DragGesture gesture, MyDirection direction)
	{
		switch(direction)
		{
			case MyDirection.Up:
				{
					for(int j = 0; j < MyTool.ColumnCount; j ++)
					{
						for(int i = 0; i < MyTool.RowCount; i ++)
						{
							Unit unit = this.unitArray[i, j];
							if(unit != null && unit.CardController != null)
							{
								unit.CardController.OnDragCard(gesture, direction);
							}
						}
					}
				}
				break;
			case MyDirection.Down:
				{
					for(int j = 0; j < MyTool.ColumnCount; j ++)
					{
						for(int i = MyTool.RowCount - 1; i >= 0; i --)
						{
							Unit unit = this.unitArray[i, j];
							if(unit != null && unit.CardController != null)
							{
								unit.CardController.OnDragCard(gesture, direction);
							}
						}
					}
				}
				break;
			case MyDirection.Left:
				{
					for(int i = 0; i < MyTool.RowCount; i ++)
					{
						for(int j = 0; j < MyTool.ColumnCount; j ++)
						{
							Unit unit = this.unitArray[i, j];
							if(unit != null && unit.CardController != null)
							{
								unit.CardController.OnDragCard(gesture, direction);
							}
						}
					}
				}
				break;
			case MyDirection.Right:
				{
					for(int i = 0; i < MyTool.RowCount; i ++)
					{
						for(int j = MyTool.ColumnCount - 1; j >= 0; j --)
						{
							Unit unit = this.unitArray[i, j];
							if(unit != null && unit.CardController != null)
							{
								unit.CardController.OnDragCard(gesture, direction);
							}
						}
					}
				}
				break;
			
		}
	}

    #endregion
}