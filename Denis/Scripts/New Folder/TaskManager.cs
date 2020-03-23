using Anims;
using UnityEngine;
using Work_Directory.Denis.Scripts.Anims;

public class TaskManager : MonoBehaviour
{
	public static TaskManager Instance;
	public int startTaskId;
	public bool startFromActiveTask;
	[Space]
	public float taskEndDelay = 1f;
	public AnimFade fade;
	private ITask[] _tasks;
	private int _currentTaskId = -1;
	
	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		
		_tasks = gameObject.GetComponentsInChildren<ITask>(true);
	
		for (var i = 0; i < _tasks.Length; i++)
		{
			if (((MonoBehaviour) _tasks[i]).gameObject.activeSelf && startFromActiveTask && _currentTaskId.Equals(-1))
				_currentTaskId = i - 1;
			
			TaskSetActive(_tasks[i], false);
		}
		
		if (_currentTaskId == -1)
			_currentTaskId += startTaskId;
	}

	private void Start()
	{
		StartNextTask();
		fade.Out();
	}
	
	private void StartNextTask()
	{
		if (_currentTaskId != -1)
			TaskSetActive(_tasks[_currentTaskId], false);

		if (_currentTaskId >= _tasks.Length - 1) return;
		_currentTaskId++;
		TaskSetActive(_tasks[_currentTaskId], true);
	}
	
	public bool CheckTask()
	{
		if (!_tasks[_currentTaskId].CheckTaskComplete()) return false;
		Timeout.Set(this, taskEndDelay - fade.duration, () => { fade.In(); });
		Timeout.Set(this, taskEndDelay, () =>
		{
			StartNextTask();
			fade.Out();
		});
		return true;

	}
	
	private static void TaskSetActive(ITask task, bool value)
	{
		(task as MonoBehaviour)?.gameObject.SetActive(value);
	}
}
