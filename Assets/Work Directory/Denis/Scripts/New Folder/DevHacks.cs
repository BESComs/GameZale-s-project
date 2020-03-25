#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public static class DevHacks
{
	[MenuItem("DevHacks/Transpose %t")]
	static void Transpose()
	{
		Transform[] transforms = Selection.transforms;
		int count = transforms.Length;
		if (count < 2) return;
		Transform firstParent = transforms[0].parent;
		Vector3 firstPosition = transforms[0].position;
		Quaternion firstRotation = transforms[0].rotation;
		Vector3 firstLScale = transforms[0].localScale;
		for (int i = 0; i < count; i++)
		{
			int nexti = (i + 1) % count;
			transforms[i].SetParent((nexti.Equals(0)) ? firstParent : transforms[nexti].parent);
			transforms[i].position = (nexti.Equals(0)) ? firstPosition : transforms[nexti].position;
			transforms[i].rotation = (nexti.Equals(0)) ? firstRotation : transforms[nexti].rotation;
			transforms[i].localScale = (nexti.Equals(0)) ? firstLScale : transforms[nexti].localScale;
		}
	}
}

#endif