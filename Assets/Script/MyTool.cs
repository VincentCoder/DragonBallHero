#region

using UnityEngine;

#endregion

public enum MyDirection
{
    Up,

    Down,

    Left,

    Right
}

public class MyTool
{
    #region Static Fields

    public static int CardWidth = 128;

    public static int ColumnCount = 4;

    public static int PanelWidth = 612;

    public static int RowCount = 4;

    public static int UnitGap = 20;

    #endregion

    //Left-Top (0,0)

    #region Public Methods and Operators

    public static Vector3 CalculatePositionByRowAndColumn(int rowId, int columnId)
    {
        int x = UnitGap + CardWidth / 2 + columnId * (UnitGap + CardWidth) - PanelWidth / 2;
        int y = PanelWidth / 2 - (UnitGap + CardWidth / 2 + rowId * (UnitGap + CardWidth));
        return new Vector3(x, y, 0);
    }

    public static int CalculateRowAndColumnByUnitId(int unitId, out int rowId)
    {
        if (unitId >= RowCount * ColumnCount)
        {
            rowId = -1;
            return -1;
        }
        rowId = unitId / ColumnCount;
        return unitId % ColumnCount;
    }

    public static int CalculateUnitIdByRowAndColumn(int rowId, int columnId)
    {
        if (rowId >= RowCount || columnId >= ColumnCount)
        {
            return -1;
        }
        return rowId * ColumnCount + columnId;
    }

    public static CardType GetNextLevelCardType(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Buma_0:
                return CardType.Shadan_1;
            case CardType.Qiqi_0:
                return CardType.Shadan_1;
            case CardType.Shadan_1:
                return CardType.Guixianren_2;
            case CardType.Guixianren_2:
                return CardType.Tianjinfan_3;
            case CardType.Tianjinfan_3:
                return CardType.Kelin_4;
            case CardType.Kelin_4:
                return CardType.Foulisha_5;
            case CardType.Foulisha_5:
                return CardType.Duandi_6;
            case CardType.Duandi_6:
                return CardType.Shalu_7;
            case CardType.Shalu_7:
                return CardType.Wufan_8;
            case CardType.Wufan_8:
                return CardType.Beijita_9;
            case CardType.Beijita_9:
                return CardType.Buou_10;
            case CardType.Buou_10:
                return CardType.Wukong_11;
        }
        return CardType.Buma_0;
    }

    #endregion
}