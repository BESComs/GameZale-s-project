using System;
using System.Threading.Tasks;

public interface ISequentTask
{
    Func<Task> RunTask { get; }
}
