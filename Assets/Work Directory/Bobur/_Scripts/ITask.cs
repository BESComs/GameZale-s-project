using UnityEngine;

namespace DefaultNamespace
{
    public interface ITask
    {
        void TaskCompleted();
        bool CheckTaskComplete();
    }
}