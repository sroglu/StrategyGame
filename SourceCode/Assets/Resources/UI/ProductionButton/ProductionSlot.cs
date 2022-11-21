using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProductionSlot : MonoBehaviour, IPointerClickHandler
{
    public int index = -1;
    public Image image;
    public UnityAction OnClick;

    /// <summary>
    /// Pointer click action call.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }

    /// <summary>
    /// Clears slot properties.
    /// </summary>
    public void Clear()
    {
        OnClick = null;
        image.sprite = null;
        index = -1;
    }
}
