using System.Collections;
using UnityEngine;

public delegate void Methods();

public static class Timeout
{
	public static void Set(MonoBehaviour behaviour, float timeout, Methods methods)
	{
		behaviour.StartCoroutine(TimeoutRoutine(timeout, methods));
	}

	private static IEnumerator TimeoutRoutine(float timeout, Methods methods)
	{
		yield return new WaitForSeconds(timeout);
		methods.Invoke();
	}
}