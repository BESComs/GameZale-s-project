using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Work_Directory.Denis.Scripts
{
    /*
     * script for button to repeat audio description if button is pressed
     */
    public class RepeatIntroAudioClip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Image _image;
        private Button _introSoundButton;
        private AudioController _audioController;
        private void Awake()
        {
            _image = GetComponent<Image>();
            var imageColor = _image.color;
            imageColor.a = .75f;
            _image.color = imageColor;
            _introSoundButton = GetComponent<Button>();
            _audioController = GetComponentInParent<AudioController>();
            _introSoundButton.onClick.AddListener(() =>
            {
                _audioController.RepeatIntroAudioClip();
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