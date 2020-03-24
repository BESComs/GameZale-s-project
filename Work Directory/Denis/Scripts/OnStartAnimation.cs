using UnityEngine;

namespace Work_Directory.Denis.Scripts
{
    public class OnStartAnimation : MonoBehaviour
    {
        private async void Awake()
        {
            await new Fade(transform).RunTask();
        }
    }
}
