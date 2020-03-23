using System;
using UnityEngine;

namespace Work_Directory.Denis.Scenes.OldGameSelectionMenu
{
    public class SelectionController : MonoBehaviour
    {
        [HideInInspector] public Item curItem;
        private SpriteRenderer _spriteRenderer;
        private int _gameSceneId;
        private Transform _child;
        private Vector2 _childStartPos;
        private void Start()
        {
            _child = transform.GetChild(0);
            _childStartPos = _child.localPosition;
            _spriteRenderer = curItem.parentGo.GetComponentInChildren<SpriteRenderer>();
        }

        private void OnMouseEnter()
        {
            if (SceneTransitionManager.Instance.inTask)  return;
            if(curItem.sceneType == SceneType.Games)
                _child.localPosition = _childStartPos + new Vector2(.2f, 0);
            var spriteRendererColor = _spriteRenderer.color;
            spriteRendererColor.a = 1;
            _spriteRenderer.color = spriteRendererColor;
        }

        private void OnDisable()
        {
            if(curItem.sceneType == SceneType.Games && _child != null)
                _child.localPosition = _childStartPos;
        }

        private async void OnMouseDown()
        {
            if (SceneTransitionManager.Instance.inTask)  return;
            if(curItem.sceneType == SceneType.Games)
                _child.localPosition = _childStartPos;

            switch(curItem.sceneType)
            {
                case SceneType.Classes:
                    await SceneTransitionManager.Instance.TransitionBetweenScenes(curItem.itemId,curItem.activateSceneOfChoiceId,FromTo.ClassesCategories, transform.position,false,curItem);
                    break;
                case SceneType.Categorise:
                    await SceneTransitionManager.Instance.TransitionBetweenScenes(curItem.itemId,curItem.activateSceneOfChoiceId,FromTo.CategoriesGames, transform.position,false,curItem);
                    break;
                case SceneType.Games:
                    await SceneTransitionManager.Instance.TransitionBetweenScenes(curItem.itemId,curItem.activateSceneOfChoiceId,FromTo.GamesGame,transform.position,false,curItem);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void OnMouseExit()
        {
            if (SceneTransitionManager.Instance.inTask)  return;
            if(curItem.sceneType == SceneType.Games)
                _child.localPosition = _childStartPos;
            var spriteRendererColor = _spriteRenderer.color;
            spriteRendererColor.a = .75f;
            _spriteRenderer.color = spriteRendererColor;

        }
    }
}
