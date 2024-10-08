using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


//modified and simplified from https://gist.github.com/mandarinx/eae10c9e8d1a5534b7b19b74aeb2a665
[RequireComponent(typeof(ScrollRect))]
public class DropdownAutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ScrollRect scrollRect;
    private List<Selectable> selectables = new List<Selectable>();
    private Vector2 nextScrollPosition = Vector2.up;
    private bool mouseUsed = false;

    void OnEnable()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.content.GetComponentsInChildren(selectables);
        ScrollToSelected(); 
    }

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.content.GetComponentsInChildren(selectables);
        ScrollToSelected();
    }

    void Update()
    {
        // Scroll via input.
        InputScroll();
        if (!mouseUsed)
            scrollRect.normalizedPosition = nextScrollPosition;
        else
            nextScrollPosition = scrollRect.normalizedPosition;
    }

    void InputScroll()
    {
        if (Input.GetButtonDown("Vertical") || Input.GetButton("Vertical"))
        {
            ScrollToSelected();
        }
    }

    void ScrollToSelected()
    {
        Selectable selectedElement = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        int selectedIndex = selectables.IndexOf(selectedElement);
        nextScrollPosition = new Vector2(0, 1 - (selectedIndex / ((float)selectables.Count - 1)));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseUsed = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseUsed = false;
    }
}
