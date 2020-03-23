using UnityEngine;

namespace Work_Directory.Denis.Scenes.OldGameSelectionMenu
{
    public class GamesLauncher : MonoBehaviour
    {
        /*
         * script used in old gameLauncher
         */
        public static GamesLauncher Instance;
    
        private void Awake()
        {
            Instance = this;
        }
  
        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
