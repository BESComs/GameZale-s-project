using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Work_Directory.Denis.Scripts
{
    public class BackFonAudioClip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /*
         * Button script used for mute or unmute audio accompaniment in game
         */
        
        public Sprite sprite;
        private Image _image;
        private Button _gameSoundButton;
        private AudioController _audioController;
        private void Awake()
        {
            _image = GetComponent<Image>();
            _audioController = GetComponentInParent<AudioController>();
            _gameSoundButton = GetComponent<Button>();
        
            _gameSoundButton.onClick.AddListener(() =>
            {
                if(!_audioController.inGame) return; 
                var buttonSprite = GetComponent<Image>().sprite;
                var tmpSprite = buttonSprite;
                GetComponent<Image>().sprite = sprite;
                sprite = tmpSprite;
                _audioController.MuteUnmuteGameSounds();
            });
        }
    
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            var imageColor = _image.color;
            imageColor.a = 1f;
            _image.color = imageColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        
            var imageColor = _image.color;
            imageColor.a = .75f;
            _image.color = imageColor;
        }
    }
}
