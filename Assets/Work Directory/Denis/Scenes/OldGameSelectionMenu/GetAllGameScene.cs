using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Work_Directory.Denis.Scenes.OldGameSelectionMenu
{
    public class GetAllGameScene : MonoBehaviour
    {
        [SerializeField]public List<string> sceneNames;
        [SerializeField]private Dictionary<string, char> _dictionary = new Dictionary<string, char>();
        [SerializeField]public List<string> latinChar;
        [SerializeField]private List<string> latinNames = new List<string>();
        [SerializeField]public List<string> cyrillicNames;
#if UNITY_EDITOR

        [Button]
        public void AddAssociation()
        {
            cyrillicNames = new List<string>();
            for (var i = 0; i < 33; ++i)
            {
                if(!_dictionary.Keys.Contains(latinChar[i]))
                    _dictionary.Add(latinChar[i],(char)('а' + i));
            }

            foreach (var latinName in latinNames)
            {
                var tmpString = string.Empty;
                for (var i = 0; i < latinName.Length; i++)
                {
                    string c;
                    if (i != latinName.Length - 1)
                    {
                        c = latinName[i].ToString() + latinName[i + 1];
                        if (_dictionary.Keys.Contains(c))
                        {
                            tmpString += _dictionary[c];
                            i++;
                        }
                        else
                        {
                            c = latinName[i].ToString();
                            if (_dictionary.Keys.Contains(c))
                                tmpString += _dictionary[c];
                        }
                    }
                    else
                    {
                        c = latinName[i].ToString();
                        if (_dictionary.Keys.Contains(c))
                            tmpString += _dictionary[c];
                    }
                }
                tmpString = tmpString.Substring(0, tmpString.Length - 5);
                cyrillicNames.Add(tmpString);
            }
        }

        [Button]
        public void FindAllScene()
        {
            var listsSize = new List<string>();
            var sizes = new List<int>();
            sceneNames = new List<string>();
            var scenesAsset = Resources.LoadAll<SceneAsset>("/");
            foreach (var sceneAsset in scenesAsset)
            {
                var tmpName = string.Empty;
                var curCateg = "";
                var newPath = AssetDatabase.GetAssetPath(sceneAsset);
                var slashCounter = 0;
                foreach (var c in newPath)
                {
                    if (c == '/')
                    {
                        slashCounter++;
                    }
                    else if (slashCounter > 3 && slashCounter < 6)
                    {
                        curCateg += c;
                    }
                    else if(slashCounter == 6)
                    {
                        tmpName += c;
                    }
                }
                latinNames.Add(tmpName);
                listsSize.Add(curCateg);
                sceneNames.Add(newPath);
            }
            sizes.Add(1);
            for (var i = 1; i < listsSize.Count; i++)
            {
                if (listsSize[i - 1] == listsSize[i])
                {
                    sizes[sizes.Count - 1]++;
                }
                else
                {
                    sizes.Add(1);
                }
            }
            var counter = 0;
        
            for (var i = 0; i < transform.childCount; i++)
            {
                var tmpList2 = transform.GetChild(i).GetComponent<ThrowItemsOnScene>().sceneOfChoice.items;
                if (tmpList2.Count != sizes[i])
                {
                    tmpList2.Add(new Item(){                
                        gameScenePath = sceneNames[counter]
                    });       
                    Array.Sort(tmpList2.ToArray(), (item, item1) => string.CompareOrdinal(item.gameScenePath, item1.gameScenePath));
                }
                foreach (var item in tmpList2)
                {
                    item.activateSceneOfChoiceId = 0;
                    item.gameScenePath = sceneNames[counter];
                    item.itemId = counter;
                    counter++;
                }
            }
            counter = 0;
            for (var i = 0; i < transform.childCount; i++)
            {
                var tmp = transform.GetChild(i).GetComponent<ThrowItemsOnScene>().sceneOfChoice.items;
                foreach (var item in tmp)
                {
                    Debug.Log(item.itemDescription + " / " + latinNames[counter]);
                    counter++;
                }
            }
        }
#endif
    }
}
