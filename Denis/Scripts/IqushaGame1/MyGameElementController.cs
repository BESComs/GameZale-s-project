using UnityEngine;

namespace Work_Directory.Denis.Scripts.IqushaGame1
{
    public class MyGameElementController : MonoBehaviour
    {
        public SpriteRenderer backFon;
        public int index;
        //выбор игрового обьекта
        private void OnMouseDown()
        {
            if (!MyGameManager.Instance.inTask)
            {
                MyGameManager.Instance.Select(this);
            }
        }
    }
}
