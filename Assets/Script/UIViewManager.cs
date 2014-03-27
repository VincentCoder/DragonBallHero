public class UIViewManager
{
    #region Static Fields

    private static readonly object lockObj;

    private static UIViewManager instance;

    #endregion

    #region Constructors and Destructors

    private UIViewManager()
    {
    }

    #endregion

    #region Public Methods and Operators

    public static UIViewManager GetInstance()
    {
        if (instance == null)
        {
            lock (lockObj)
            {
                instance = new UIViewManager();
            }
        }
        return instance;
    }



    #endregion
}