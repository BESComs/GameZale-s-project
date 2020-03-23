using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace _Scripts.Utility
{
    public static class AnimReferenceUtility
    {
        public static void ChangeTransparency(object ob, float transparency)
        {
            if(ob.GetType().GetProperty("color") != null){
                Color color = (Color) ob.GetType().GetProperty("color")?.GetValue(ob);
                color = new Color(color.r, color.g, color.b, transparency);
                ob.GetType().GetProperty("color").SetValue(ob, color);
                return;
            }
            
            Debug.Log("The object does not contain Color property.");
        }

        public static void ChangeScale(object ob, float scale)
        {
            if (ob.GetType().GetProperty("transform") != null)
            {
                Vector3 localScale = new Vector3(scale, scale, 1);
                Transform transform = (Transform) ob.GetType().GetProperty("transform").GetValue(ob);
                ob.GetType().GetProperty("transform").GetValue(ob).GetType().GetProperty("localScale").SetValue(transform, localScale);

                return;
            }
            
            Debug.Log("The object does not contain Transform property.");
        }
    }
}