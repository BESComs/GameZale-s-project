using UnityEngine;

namespace Work_Directory.Denis.Scenes.Colorings.Scripts
{
    public class StretchThePicture : MonoBehaviour
    {
        public float factor;
        //растягивание игровых объектов
        private void Awake()
        {
            var tmp = (float)Screen.width / Screen.height / 16f * 9;
            transform.localScale = new Vector3(tmp,1f, 1f) * factor;
        }
    }
}
