using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectMouseWheel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float sensitivity = 1;
    private bool hover = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hover = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hover)
            return;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0)
            return;
        ScrollRect scrollRect = GetComponent<ScrollRect>();
        scrollRect.verticalScrollbar.value += (scroll / scrollRect.content.sizeDelta.y) * sensitivity;
        scrollRect.verticalScrollbar.value = Mathf.Clamp(scrollRect.verticalScrollbar.value,0,1);
    }
}
