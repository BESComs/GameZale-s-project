using UnityEngine;
using UnityEngine.UI;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Card_Match;

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Paint
{
    public class ButtonScript : MonoBehaviour
    {
        //кнопка помощьник в игре
        [HideInInspector]public Button helpButton;
        //оригинальное изображение
        [HideInInspector]public Sprite helpImage;
        //позиция изображения
        [HideInInspector]public Vector2 helpImagePosition;
        private GameObject _spriteGo;
    
        public void StartFunc()
        {
            //создание оригинального изображения
            _spriteGo = new GameObject();
            _spriteGo.transform.position = helpImagePosition;
            //настройки спрайта
            var sr = _spriteGo.AddComponent<SpriteRenderer>();
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.sprite = helpImage;
            sr.sortingLayerName = SortingLayer.layers[SortingLayer.layers.Length - 2].name;
            sr.sortingOrder = 20;
            _spriteGo.SetActive(false);
     
            helpButton = GetComponent<Button>();
        
            helpButton.onClick.AddListener(
                async () => {
                    if(_spriteGo.activeSelf) return;
                    //активировать оригинальное изображение на 2.5 секунды
                    _spriteGo.SetActive(true);
                    await (new ScaleIn(_spriteGo.transform,AnimationCurve.EaseInOut(0,0,.2f,1))).RunTask();
                    await new WaitForSeconds(2.5f);
                    await (new ScaleOut(_spriteGo.transform,AnimationCurve.EaseInOut(0,0,.2f,1))).RunTask();
                    _spriteGo.gameObject.SetActive(false);
                }
            );
        }

        private void OnDestroy()
        {
            helpButton.onClick.RemoveAllListeners();
        }
    }
}
