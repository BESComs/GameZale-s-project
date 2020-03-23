using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Work_Directory.Denis.Scenes.NewLauncher.Scripts
{
    public class LauncherBackButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        //Кнопка на сцене осуществляющая переход из выбора игр выбор категорий
        private Button backButton;

        private void Awake()
        {
            backButton = GetComponent<Button>();
            backButton.onClick.AddListener(() =>
            {
                LauncherController.Instance.GoBack();
            });
        }

        //используется для того чтобы при нажатии на кнопку не запускалась игра 
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
