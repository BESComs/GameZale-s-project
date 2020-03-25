using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Work_Directory.Denis.Scripts.IqushaGame3
{
    public class PositionInSpaceElementController : MonoBehaviour, IPointerDownHandler
    {
        //является ли данный элемент выбранным
        public bool isSelected;
        //индекс текущего элемента
        public int index;
        //Image текущего элемента
        private Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
        }


        //по нажатию на ui элемент идет обработка
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!PositionInSpaceGameManager.Instance.inTask)
            {
                //если данный элемент уже выбран пометить как не выбранный
                if (isSelected)
                {
                    isSelected = false;
                    PositionInSpaceGameManager.Instance.selectedElement = null;
                    TurnOffImage();
                }
                else
                {
                    //если выбран элемент (не текущий) то пометить его как не выбраный
                    if (PositionInSpaceGameManager.Instance.selectedElement != null)
                    {
                        PositionInSpaceGameManager.Instance.selectedElement.TurnOffImage();
                        PositionInSpaceGameManager.Instance.selectedElement.isSelected = false;
                    }
                    //сделать выбранный элемент текущим и пометить его как выбранный 
                    isSelected = true;
                    PositionInSpaceGameManager.Instance.selectedElement = this;
                    TurnOnImage();
                }
            }
        }

        public void TurnOffImage()
        {
            image.color = Color.white;
        }

        private void TurnOnImage()
        {
            image.color = Color.green;
        }
    }
}
