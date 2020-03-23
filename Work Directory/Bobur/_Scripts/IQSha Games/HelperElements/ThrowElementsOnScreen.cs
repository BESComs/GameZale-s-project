using System;
using UnityEngine;
using System.Collections.Generic;
using Work_Directory.Bobur._Scripts.IQSha_Games.HelperElements;
using _Scripts.Utility;

public class ThrowElementsOnScreen
{
    private Position2D mPosition;
    private float mMarginX, mMarginY;
    private float mHeight, mWidth;
    //private List<Sprite> Sprites = new List<Sprite>();
    private List<List<SpriteID>> mSprites;
    private List<List<GameObject>> mObjectsList;
    //private List<int> Lines = new List<int>();

    public List<List<GameObject>> ObjectsList { get; }
    private bool isSet;
    
    public GameObject Throw()
    {
        if (isSet)
        {
            mObjectsList = new List<List<GameObject>>();
            var globalObj = new GameObject("Thrown Elements");
            var globalObjPosition = new Vector3(mPosition.X, mPosition.Y, 0);
            globalObj.transform.position = globalObjPosition;
            float lastYPosition = -mMarginY;
            
            for (var i = 0; i < mSprites.Count; ++i)
            {
                var tmpList = new List<GameObject>();
                float startMarginX = 0;
                float startMarginY = 0;
                float sumMarginX = 0;
                
                for (var j = 0; j < mSprites[i].Count; ++j)
                {
                    var tmpGo = new GameObject();
                    tmpGo.AddComponent<SpriteRenderer>().sprite = mSprites[i][j].Sprite;
                    tmpGo.AddComponent<ID>().Id = mSprites[i][j].ID;
                        
                    var sr = tmpGo.GetComponent<SpriteRenderer>();
                    sr.sortingOrder = 100;
                    var ratioX = mHeight / sr.size.x;
                    var ratioY = mWidth / sr.size.y;
                    
                    sr.drawMode = SpriteDrawMode.Sliced;
                    sr.size *= Mathf.Min(ratioX, ratioY);
                    
                    startMarginX += sr.size.x * tmpGo.transform.localScale.x + mMarginX;
                    startMarginY = (sr.size.y * tmpGo.transform.localScale.y + mMarginY > startMarginY)
                                    ? sr.size.y * tmpGo.transform.localScale.y + mMarginY
                                    : startMarginY;
                    
                    tmpList.Add(tmpGo);
                }

                lastYPosition += startMarginY;
                
                foreach (GameObject ob in tmpList)
                {
                    float size = ob.transform.localScale.x * ob.GetComponent<SpriteRenderer>().size.x + mMarginX;
                    sumMarginX += size;
                    
                    ob.transform.localPosition = new Vector3(-startMarginX / 2f + sumMarginX - size / 2f, -lastYPosition + startMarginY / 2f);
                    ob.transform.SetParent(globalObj.transform, false);
                }
                
                mObjectsList.Add(tmpList);
            }
    
            return globalObj;
        }
        throw new Exception("The Throw Object is not set up.");
    }

    public void Setup (Position2D positionOnScreen, List<List<SpriteID>> elements, float height = 1f, float width = 1f, float MarginX = 0.2f, float MarginY = 0.2f)
    {
        mPosition = positionOnScreen ?? new Position2D(0,0);
        mHeight = height;
        mWidth = width;
        mSprites = elements;
        mMarginX = MarginX;
        mMarginY = MarginY;
        
        isSet = true;
    }
    
    
    //todo goList
}
