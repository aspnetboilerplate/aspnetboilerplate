using Abp.Utils.Extensions;
using Taskever.Domain.Enums;
using Taskever.Exceptions;

namespace Taskever.Domain.Business.Acitivities
{
    public abstract class ActivityData
    {
        public ActivityAction Action
        {
            get
            {
                //TODO: Make a general-purpose attribute getter!
                var attributes = GetType().GetCustomAttributes(typeof(ActivityDataAttribute), true);
                if (attributes.IsNullOrEmpty())
                {
                    throw new TaskeverException("A class derived from ActivityData must define an ActivityDataAttribute.");
                }

                return ((ActivityDataAttribute) attributes[0]).Action;
            }
        }

        public abstract int GetActorUserId();

        public abstract string SerializeData();

        public abstract void DeserializeData(string data);
    }
}