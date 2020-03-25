using UnityEngine;
using UnityEngine.EventSystems;

namespace Work_Directory.Denis.Scenes.NewLauncher.Scripts
{
    //используется для того чтобы не регистрировалась нажатие в верхней части экрана где находится спрайт полоски
    public class MouseOnLine : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            LauncherController.Instance.mouseOnLine = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            LauncherController.Instance.mouseOnLine = false;
        }
    }
}
