using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI buttonText;
    public Image leftArrow;
    public Image rightArrow;
    public Color normalColor;
    public Color hoverColor;

    bool isHovering = false;

    void Start()
    {
        leftArrow.enabled = false;
        rightArrow.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        SetHoverState(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        SetHoverState(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isHovering = false;
        SetHoverState(false);
    }

    void SetHoverState(bool isHovered)
    {
        buttonText.color = isHovered ? hoverColor : normalColor;
        leftArrow.enabled = isHovered;
        rightArrow.enabled = isHovered;
    }
}