using System;
using Taskever.Domain.Enums;

namespace Taskever.Domain.Business.Acitivities
{
    public static class ActivityInfoFactory
    {
        public static ActivityInfo CreateActivityInfo(this ActivityAction action, string data)
        {
            var activityInfoObj = CreateActivityInfoObject(action);
            activityInfoObj.DeserializeData(data);
            return activityInfoObj;
        }

        private static ActivityInfo CreateActivityInfoObject(ActivityAction action)
        {
            //TODO: Create a action-dataclass mapping dictionary, get type, create instance, deserialize and return data! Don't use switch statement!
            switch (action)
            {
                case ActivityAction.CreateTask:
                    return new CreateTaskActivityInfo();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}