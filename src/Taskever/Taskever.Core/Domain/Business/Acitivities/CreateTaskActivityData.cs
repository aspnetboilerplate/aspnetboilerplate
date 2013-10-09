using System;
using Taskever.Domain.Enums;

namespace Taskever.Domain.Business.Acitivities
{
    public class ActivityDataAttribute : Attribute
    {
        public ActivityAction Action { get; private set; }

        public ActivityDataAttribute(ActivityAction action)
        {
            Action = action;
        }
    }

    /// <summary>
    /// TODO: Add User and task names, denormalize for performance!
    /// </summary>
    [ActivityData(ActivityAction.CreateTask)]
    public class CreateTaskActivityData : ActivityData
    {
        public int CreatorUserId { get; set; }

        public int TaskId { get; set; }

        public int AssignedUserId { get; set; }

        public CreateTaskActivityData()
        {
            
        }

        public CreateTaskActivityData(int creatorUserId, int taskId, int assignedUserId)
        {
            CreatorUserId = creatorUserId;
            TaskId = taskId;
            AssignedUserId = assignedUserId;
        }

        public override int GetActorUserId()
        {
            return CreatorUserId;
        }

        public override string SerializeData()
        {
            //TODO: Make a more general serialization (for example, like query-string)

            return string.Join("|", new object[] {CreatorUserId, TaskId, AssignedUserId});
        }

        public override void DeserializeData(string data)
        {
            var splitted = data.Split('|');
            CreatorUserId = Convert.ToInt32(splitted[0]);
            TaskId = Convert.ToInt32(splitted[1]);
            AssignedUserId = Convert.ToInt32(splitted[2]);
        }
    }

    public static class ActivityDataFactory
    {
        public static ActivityData CreateData(this ActivityAction action, string data)
        {
            //TODO: Create a action-dataclass mapping dictionary, get type, create instance, deserialize and return data!

            var dataObj = new CreateTaskActivityData();
            dataObj.DeserializeData(data);
            return dataObj;
        }
    }
}