#region

using UnityEngine;

#endregion

public class UIEventManager
{
    #region Static Fields

    private static readonly object lockObj;

    private static UIEventManager instance;

    #endregion

    #region Constructors and Destructors

    private UIEventManager()
    {
    }

    #endregion

    #region Public Methods and Operators

    public static UIEventManager GetInstance()
    {
        if (instance == null)
        {
            lock (lockObj)
            {
                instance = new UIEventManager();
            }
        }
        return instance;
    }

    public void Register(GameObject registerObj)
    {
        if (registerObj != null)
        {
            UIImageButton imageButton = registerObj.GetComponent<UIImageButton>();
            if (imageButton != null)
            {
                UIEventListener.Get(registerObj).onClick = this.HandleEvent;
            }
            else
            {
                UIButton button = registerObj.GetComponent<UIButton>();
                if (button != null)
                {
                    UIEventListener.Get(registerObj).onClick = this.HandleEvent;
                }
            }
        }
    }

    public void RegisterInHierarchy(GameObject registerObj)
    {
        if (registerObj != null)
        {
            UIImageButton[] imageButtons = registerObj.GetComponentsInChildren<UIImageButton>();
            foreach (UIImageButton imageButton in imageButtons)
            {
                UIEventListener.Get(imageButton.gameObject).onClick = this.HandleEvent;
            }
            UIButton[] buttons = registerObj.GetComponentsInChildren<UIButton>();
            foreach (UIButton button in buttons)
            {
                UIEventListener.Get(button.gameObject).onClick = this.HandleEvent;
            }
        }
    }

    public void UnRegister(GameObject registerObj)
    {
        if (registerObj != null)
        {
            UIEventListener.Get(registerObj).onClick = null;
        }
    }

    public void UnRegisterInHierarchy(GameObject registerObj)
    {
        if (registerObj != null)
        {
            UIImageButton[] imageButtons = registerObj.GetComponentsInChildren<UIImageButton>();
            foreach (UIImageButton imageButton in imageButtons)
            {
                UIEventListener.Get(imageButton.gameObject).onClick = null;
            }
            UIButton[] buttons = registerObj.GetComponentsInChildren<UIButton>();
            foreach (UIButton button in buttons)
            {
                UIEventListener.Get(button.gameObject).onClick = null;
            }
        }
    }

    #endregion

    #region Methods

    private void HandleEvent(GameObject eventObj)
    {
    }

    #endregion
}