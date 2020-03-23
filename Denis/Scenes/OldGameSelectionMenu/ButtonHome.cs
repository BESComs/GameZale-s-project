using UnityEngine;

namespace Work_Directory.Denis.Scenes.OldGameSelectionMenu
{
    public class ButtonHome : MonoBehaviour
    {
        /*
         * script used in old game launcher
         */
        private bool _doubleTransition;
        private bool _buttonInTask;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponentInParent<SpriteRenderer>();
        }

        private async void OnMouseDown()
        {
            if(SceneTransitionManager.Instance.inTask || !SceneTransitionManager.Instance.classesButton.gameObject.activeSelf) return;
            if(_buttonInTask) return;
            _buttonInTask = true;
        
            if (SceneTransitionManager.Instance.categoriesButton.gameObject.activeSelf)
            {
                _doubleTransition = true;
                await SceneTransitionManager.Instance.TransitionBetweenScenes(
                    SceneTransitionManager.Instance.sceneIdOfCategories,
                    SceneTransitionManager.Instance.sceneIdOfGames, FromTo.GamesCategories,
                    SceneTransitionManager.Instance.scenePositionOfGames, _doubleTransition);
            }
            await SceneTransitionManager.Instance.TransitionBetweenScenes(0,
                SceneTransitionManager.Instance.sceneIdOfCategories, FromTo.CategoriesClasses,
                SceneTransitionManager.Instance.scenePositionOfCategories, _doubleTransition);
            _doubleTransition = false;
            _buttonInTask = false;
        }

        private void OnMouseEnter()
        {
            if(SceneTransitionManager.Instance.inTask || !SceneTransitionManager.Instance.classesButton.gameObject.activeSelf || _buttonInTask) return;

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
