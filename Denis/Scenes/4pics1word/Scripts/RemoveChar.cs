using UnityEngine;

namespace Work_Directory.Denis.Scenes._4pics1word.Scripts
{
    //скрипт отвечает за удаление буквы из выбранных
    public class RemoveChar : MonoBehaviour
    {
        [HideInInspector] public int charBoxId;
        [HideInInspector] public int curCharBoxId;
        [HideInInspector] public FourWordOnePicture fourWordOnePicture;

        private void OnMouseDown()
        {
            if(fourWordOnePicture.inTask) return;
            fourWordOnePicture.RemoveChar(charBoxId, curCharBoxId);
        }
    }
}
