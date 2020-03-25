using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Work_Directory.Denis.Scripts;

namespace Work_Directory.Denis.Scenes.OldGameSelectionMenu
{
    public class ThrowItemsOnScene : MonoBehaviour
    {
        public SceneOfChoice sceneOfChoice;
        private void Start()
        {
            string parentName;
            switch (sceneOfChoice.sceneType)
            {
                case SceneType.Classes:
                    parentName = "Classes";
                    break;
                case SceneType.Categorise:
                    parentName = "Categories" + SceneTransitionManager.Instance.categories.Count;
                    break;
                case SceneType.Games:
                    parentName = "Games" + SceneTransitionManager.Instance.games.Count;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            sceneOfChoice.parent = new GameObject(parentName).transform;
            sceneOfChoice.itemSize = sceneOfChoice.spriteSize + new Vector2(0, sceneOfChoice.inputHeight);
            foreach (var item in sceneOfChoice.items)
            {
                var tmpGoWithText = Instantiate(sceneOfChoice.textMeshProPrefab).gameObject;
                var tmpGoWithSprite = new GameObject();
                var goSr = tmpGoWithSprite.AddComponent<SpriteRenderer>();
                goSr.sprite = item.sprite;
                goSr.drawMode = SpriteDrawMode.Sliced;
                var size = goSr.size;
                goSr.size *= Mathf.Min(sceneOfChoice.spriteSize.x / size.x,
                    sceneOfChoice.spriteSize.y / size.y);
                var goSrColor = goSr.color;
                goSrColor.a = .75f;
                goSr.color = goSrColor;
                var text = tmpGoWithText.GetComponent<TextMeshPro>();
                text.text = item.itemDescription;
                text.rectTransform.sizeDelta = new Vector2(sceneOfChoice.spriteSize.x, sceneOfChoice.inputHeight);
                var tmpParentGo = new GameObject();
                tmpParentGo.AddComponent<SelectionController>().curItem = item;
                var bc = tmpParentGo.AddComponent<BoxCollider>();
                bc.size = sceneOfChoice.itemSize;
                bc.center = new Vector3(0, -sceneOfChoice.inputHeight / 2f, 0);
                tmpGoWithText.transform.SetParent(tmpParentGo.transform);
                tmpGoWithSprite.transform.SetParent(tmpParentGo.transform);
                tmpGoWithSprite.transform.localPosition = Vector2.zero;
                tmpGoWithText.transform.localPosition = new Vector2(0, -sceneOfChoice.itemSize.y / 2f);
                item.parentGo = tmpParentGo;
                item.parentGo.transform.SetParent(sceneOfChoice.parent);
                item.sceneType = sceneOfChoice.sceneType;
                item.itemId = sceneOfChoice.sceneId;
            }

            var lastItemPos = sceneOfChoice.firstItemPos;
            if (sceneOfChoice.items.Count != 0)
                sceneOfChoice.items[0].parentGo.transform.position = lastItemPos;

            for (var i = 1; i < sceneOfChoice.items.Count; i++)
            {
                sceneOfChoice.items[i].parentGo.transform.position = i % sceneOfChoice.numberOfItemsInline != 0
                    ? lastItemPos + new Vector2(sceneOfChoice.margin.x + sceneOfChoice.itemSize.x, 0)
                    : new Vector2(sceneOfChoice.firstItemPos.x,
                        lastItemPos.y - sceneOfChoice.margin.y - sceneOfChoice.itemSize.y);
                lastItemPos = sceneOfChoice.items[i].parentGo.transform.position;
            }

            switch (sceneOfChoice.sceneType)
            {
                case SceneType.Classes:
                    SceneTransitionManager.Instance.classes = sceneOfChoice;
                    break;
                case SceneType.Categorise:
                    SceneTransitionManager.Instance.categories.Add(sceneOfChoice);
                    sceneOfChoice.parent.gameObject.SetActive(false);
                    break;
                case SceneType.Games:
                    SceneTransitionManager.Instance.games.Add(sceneOfChoice);
                    sceneOfChoice.parent.gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            sceneOfChoice.parent.SetParent(GetComponentInParent<GamesLauncher>().transform);
        }
    }

    public enum SceneType
    {
        Classes,
        Categorise,
        Games
    }
    
    [Serializable]
    public class SceneOfChoice
    {
        public int sceneId;
        [HideInInspector]public Transform parent;
        public SceneType sceneType;
        public Vector2 firstItemPos;
        public int numberOfItemsInline;
        public Vector2 spriteSize;
        public float inputHeight;
        public Vector2 margin;
        public Transform textMeshProPrefab;
        public List<Item> items;
        [HideInInspector]public Vector2 itemSize;   
    }

    [Serializable]
    public class Item
    {
        public string gameScenePath;
        [HideInInspector] public SceneType sceneType;
        [HideInInspector] public int itemId;
        public int activateSceneOfChoiceId;
        [HideInInspector]public GameObject parentGo;
        public Sprite sprite;
        public string itemDescription;
    }
}