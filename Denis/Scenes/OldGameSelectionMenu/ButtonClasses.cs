using UnityEngine;

namespace Work_Directory.Denis.Scenes.OldGameSelectionMenu
{
    public class ButtonClasses : MonoBehaviour
    {
        /*
         * script used in old game launcher
         */
        private bool _buttonInTask;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponentInParent<SpriteRenderer>();
        }

        private async void OnMouseDown()
        {
            if (SceneTransitionManager.Instance.inTask || !SceneTransitionManager.Instance.categoriesButton.gameObject.activeSelf) return;
            if(_buttonInTask) return;
            _buttonInTask = true;
            await SceneTransitionManager.Instance.TransitionBetweenScenes(
                SceneTransitionManager.Instance.sceneIdOfCategories, SceneTransitionManager.Instance.sceneIdOfGames,
                FromTo.GamesCategories, SceneTransitionManager.Instance.scenePositionOfGames);
            _buttonInTask = false;

        }
    
        private void OnMouseEnter()
        {
            if(SceneTransitionManager.Instance.inTask || !SceneTransitionManager.Instance.categoriesButton.gameObject.activeSelf || _buttonInTask) return;

            var spriteRendererColor = _spriteRenderer.color;
            spriteRendererColor.a = .75f;
            _spriteRenderer.color = spriteRendererColor;
        }

        private void OnMouseExit()
        {
            var spriteRendererColor = _spriteRenderer.color;
            spriteRendererColor.a = 1f;
            _spriteRenderer.color = spriteRendererColor;
        }
    }
}
