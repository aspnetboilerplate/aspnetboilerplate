using System;
using Abp.Localization;

namespace Abp.Notifications
{
    /// <summary>
    /// A notification should be defined first.
    /// This is needed, because then users will see list of notifications and subscribe them.
    /// </summary>
    public class NotificationDefinition
    {
        public string Name { get; private set; }

        public ILocalizableString DisplayName
        {
            get { return _displayName; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _displayName = value;
            }
        }
        private ILocalizableString _displayName;

        public NotificationDefinition(string name, ILocalizableString displayName)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (displayName == null)
            {
                throw new ArgumentNullException("displayName");
            }

            Name = name;
            DisplayName = displayName;
        }

        //TODO: Available chanells, type (entity based or custom...?)
    }
}