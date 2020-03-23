using UnityEngine;
using UnityEngine.UI;

namespace Work_Directory.Denis.Scripts.IqushaGame3
{
    public class PositionInSpaceReplayButton : MonoBehaviour
    {
        //кнопка рестарта игры
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();

            _button.onClick.AddListener(() =>
            {
                if(!PositionInSpaceGameManager.Instance.inTask)
                    PositionInSpaceGameManager.Instance.RestartGame();
            });
        }
    }
}
