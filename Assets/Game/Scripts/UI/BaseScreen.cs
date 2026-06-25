using System;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseScreen : MonoBehaviour
{
    private UIDocument _document;
    private bool _isVisible;

    protected UIDocument Document => _document;
    public bool IsVisible => _isVisible;

    public event Action OnShown;

    protected virtual void Awake()
    {
        _document = GetComponent<UIDocument>();
        Hide();
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        _document.rootVisualElement.style.display = DisplayStyle.Flex;
        _isVisible = true;
        OnShow();
        OnShown?.Invoke();
    }

    public virtual void Hide()
    {
        if (_document != null && _document.rootVisualElement != null)
            _document.rootVisualElement.style.display = DisplayStyle.None;
        _isVisible = false;
        OnHide();
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
}
