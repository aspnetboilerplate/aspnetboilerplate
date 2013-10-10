using System;
using Taskever.Domain.Enums;

namespace Taskever.Domain.Business.Acitivities
{
    public class ActivityAttribute : Attribute
    {
        public ActivityAction Action { get; private set; }

        public ActivityAttribute(ActivityAction action)
        {
            Action = action;
        }
    }
}