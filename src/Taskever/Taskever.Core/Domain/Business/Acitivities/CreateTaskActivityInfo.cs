using System;
using Taskever.Domain.Enums;

namespace Taskever.Domain.Business.Acitivities
{
    /// <summary>
    /// Informations for a 'Task creation activity'.
    /// </summary>
    [Activity(ActivityAction.CreateTask)]
    public class CreateTaskActivityInfo : ActivityInfo
    {
        public int CreatorUserId { get; set; }

        public string CreatorUserName { get; set; }

        public int TaskId { get; set; }

        public string TaskTitle { get; set; }

        public int AssignedUserId { get; set; }

        public string AssignedUserName { get; set; }

        public CreateTaskActivityInfo()
        {

        }

        public CreateTaskActivityInfo(
            int creatorUserId,
            string creatorUserName,
            int taskId,
            string taskTitle,
            int assignedUserId,
            string assignedUserName)
        {
            CreatorUserId = creatorUserId;
            CreatorUserName = creatorUserName;
            TaskId = taskId;
            TaskTitle = taskTitle;
            AssignedUserId = assignedUserId;
            AssignedUserName = assignedUserName;
        }

        public override int GetActorUserId()
        {
            return CreatorUserId;
        }

        public override string SerializeData()
        {
            //TODO: Make a more general serialization (like query-string, json or binary?)

            return string.Join("|", new object[] { CreatorUserId, CreatorUserName, TaskId, TaskTitle, AssignedUserId, AssignedUserName });
        }

        public override void DeserializeData(string data)
        {
            //TODO: Make a more general deserialization

            var splitted = data.Split('|');
            CreatorUserId = Convert.ToInt32(splitted[0]);
            CreatorUserName = splitted[1];
            TaskId = Convert.ToInt32(splitted[2]);
            TaskTitle = splitted[3];
            AssignedUserId = Convert.ToInt32(splitted[4]);
            AssignedUserName = splitted[5];
        }
    }
}