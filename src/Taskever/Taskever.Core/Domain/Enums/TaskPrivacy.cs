namespace Taskever.Domain.Enums
{
    public enum TaskPrivacy : byte
    {
        /// <summary>
        /// Only the creator user can see the task
        /// </summary>
        Private = 1,

        /// <summary>
        /// Only friends can see the task.
        /// </summary>
        Protected = 2
    }
}