using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UIEffect
{
    public static void AnimateShow(GameObject gameObject)
    {
        gameObject.transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        gameObject.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack); ;
    }

    public static void AnimateHide(GameObject gameObject)
    {
        gameObject.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public static void SetupButtonAnimation(Button button)
    {
        // Thêm hiệu ứng scale khi hover
        var eventTrigger = button.gameObject.AddComponent<EventTrigger>();
        var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) =>
        {
            button.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutQuad);
        });
        eventTrigger.triggers.Add(pointerEnter);

        var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) =>
        {
            button.transform.DOScale(1f, 0.3f).SetEase(Ease.OutQuad);
        });
        eventTrigger.triggers.Add(pointerExit);

        var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener((data) =>
        {
            button.transform.DOScale(0.8f, 0.2f).SetEase(Ease.InQuad);
        });
        eventTrigger.triggers.Add(pointerDown);

        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) =>
        {
            button.transform.DOScale(1f, 0.2f).SetEase(Ease.OutQuad);
        });
        eventTrigger.triggers.Add(pointerUp);
    }
}