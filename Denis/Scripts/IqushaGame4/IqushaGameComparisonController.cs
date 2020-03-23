using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Work_Directory.Denis.Scripts.IqushaGame4
{
    public class IqushaGameComparisonController : MonoBehaviour, IPointerDownHandler
    {
        //индекс текущего элемента
        public int currentIndex;
        private Image image;
        //выбран ли данный элемент
        private bool isSelected;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IqushaGameComparisonManager.Instance.inTask)
            {
                //если текущий элемент выбранный
                if (!isSelected)
                {
                    //если был выбран какой нибудь элемент пометить его как не выбранный
                    if (IqushaGameComparisonManager.Instance.currentSelected != null)
                        IqushaGameComparisonManager.Instance.currentSelected.TurnOffImage();
                    
                    //пометить текущий элемент как выбранный
                    IqushaGameComparisonManager.Instance.currentSelected = this;
                    TurnOnImage();
                }
                else 
                    TurnOffImage();
                    
            }
        }

        public void TurnOffImage()
        {
            image.color = Color.white;
            isSelected = false;
        }

        private void TurnOnImage()
        {
            image.color = Color.green;
            isSelected = true;
        }
    }
}
