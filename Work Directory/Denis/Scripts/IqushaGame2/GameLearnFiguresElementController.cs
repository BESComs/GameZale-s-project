using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Work_Directory.Denis.Scripts.IqushaGame2
{
    public class GameLearnFiguresElementController : MonoBehaviour, IPointerDownHandler
    {
        //индекс текущего элемента
        public int currentElementIndex;
        //выбран ли данный элемент
        public bool selected;
        private Image image;

        private void Awake()
        {
            image = transform.parent.GetComponent<Image>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(GameLearnFigures.Instance.inTask) return;
            //если данный элемент выбран то пометить его как не выбранный
            if(!selected)
            {
                selected = true;
                //если был выбран какой нибудь элемент (не текущий) то пометить его как не выбранный
                if (GameLearnFigures.Instance.selectElement != null)
                {
                    GameLearnFigures.Instance.selectElement.selected = false;
                    GameLearnFigures.Instance.selectElement.TurnOffImage();
                }
                //пометить данный элемент как текущий выбранный элемент
                GameLearnFigures.Instance.selectElement = this;
                TurnOnImage();
                return;
            }
            GameLearnFigures.Instance.selectElement = null;
            TurnOffImage();
            selected = false;
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
