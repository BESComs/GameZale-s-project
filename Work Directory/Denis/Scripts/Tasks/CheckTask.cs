using Tasks.Mosaic;
using UnityEngine;
using UnityEngine.UI;

namespace Work_Directory.Denis.Scripts.Tasks
{
	public class CheckTask : MonoBehaviour
	{
		private bool _inTask;
		private Button _checkButton;
		private void Awake()
		{
			_checkButton = GetComponent<Button>();
			
			_checkButton.onClick.AddListener(async delegate
			{
				var taskMosaic = FindObjectOfType<TaskMosaic>();
				if (taskMosaic == null || taskMosaic.CheckTaskComplete())
				{
					gameObject.SetActive(false);
					return;
				}
				var tmpGo = taskMosaic.transform;
				var message = tmpGo.GetChild(tmpGo.childCount - 1);
				if (message.gameObject.activeSelf) return;
				taskMosaic.RegisterAnswer(false);
				message.gameObject.SetActive(true);
				await new Fade(message, Mode.In).RunTask();
				await new WaitForSeconds(2);
				await new Fade(message).RunTask();
				if(this != null)
					message.gameObject.SetActive(false);				
			});
		}
	}
}