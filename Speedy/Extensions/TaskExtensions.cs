#region References

using System.Threading.Tasks;

#endregion

namespace Speedy.Extensions
{
	/// <summary>
	/// Extensions for Task
	/// </summary>
	public static class TaskExtensions
	{
		#region Methods

		/// <summary>
		/// Determine if a task has started and is completed.
		/// </summary>
		/// <param name="task"> The task to check. </param>
		/// <returns> True if the task is Cancelled, Faulted, or RanToCompletion otherwise false. </returns>
		public static bool IsCompleted(this Task task)
		{
			return task.Status == TaskStatus.Canceled
				|| task.Status == TaskStatus.Faulted
				|| task.Status == TaskStatus.RanToCompletion;
		}

		#endregion
	}
}