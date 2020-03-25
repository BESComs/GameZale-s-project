using UnityEngine;

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Paint
{
    public class CollisionStay : MonoBehaviour
    {
        //скрипт для кисти 
        public Paint ms;

        private void OnTriggerStay2D(Collider2D other)
        {
            //если был выбран какой то цвет и кисть находится на элементе который можно покрасить то красит его в цвет на кисте
            if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
            var sc = other.transform.GetComponent<SetColor>();
            if (sc == null) return;
            if (ms.currentColor == null) return;
            sc.sr.color = ms.currentColor.Value;
            sc.currentColorId = ms.currentColorId;
        }
    }
}