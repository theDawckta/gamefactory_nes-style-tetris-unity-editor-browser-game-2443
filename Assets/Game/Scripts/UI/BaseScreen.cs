using UnityEngine;
using UnityEngine.UIElements;

public class BaseScreen : MonoBehaviour
{
    private UIDocument _document;

    protected UIDocument Document => _document;

    public bool IsVisible
    {
        get
        {
            if (_document == null || _document.rootVisualElement == null) return false;
            return _document.rootVisualElement.style.display.value == DisplayStyle.Flex;
        }
    }

    protected virtual void Awake()
    {
        _document = GetComponent<UIDocument>();
        Hide();
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        _document.rootVisualElement.style.display = DisplayStyle.Flex;
        OnShow();
    }

    public virtual void Hide()
    {
        if (_document == null || _document.rootVisualElement == null)
        {
            OnHide();
            return;
        }
        _document.rootVisualElement.style.display = DisplayStyle.None;
        OnHide();
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
}
