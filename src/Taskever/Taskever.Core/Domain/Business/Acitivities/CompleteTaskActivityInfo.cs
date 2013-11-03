using System;
using Taskever.Domain.Enums;

namespace Taskever.Domain.Business.Acitivities
{
    [Activity(ActivityAction.CompleteTask)]
    public class CompleteTaskActivityInfo : ActivityInfo
    {
        public int TaskId { get; set; }

        public string TaskTitle { get; set; }

        public int AssignedUserId { get; set; }

        public string AssignedUserName { get; set; }

        public CompleteTaskActivityInfo()
        {

        }

        public CompleteTaskActivityInfo(
            int taskId,
            string taskTitle,
            int assignedUserId,
            string assignedUserName)
        {
            TaskId = taskId;
            TaskTitle = taskTitle;
            AssignedUserId = assignedUserId;
            AssignedUserName = assignedUserName;
        }

        public override int GetActorUserId()
        {
            return AssignedUserId;
        }

        public override string SerializeData()
        {
            //TODO: Make a more general serialization (like query-string, json or binary?)

            return string.Join("|", new object[] { TaskId, TaskTitle, AssignedUserId, AssignedUserName });
        }

        public override void DeserializeData(string data)
        {
            //TODO: Make a more general deserialization

            var splitted = data.Split('|');
            TaskId = Convert.ToInt32(splitted[0]);
            TaskTitle = splitted[1];
            AssignedUserId = Convert.ToInt32(splitted[2]);
            AssignedUserName = splitted[3];
        }
    }
}