//using System;
//using Abp.Extensions;
//using Abp.Localization;

//namespace Abp.Notifications
//{
//    /// <summary>
//    /// A notification should be defined first.
//    /// This is needed, because then users can see list of notifications and subscribe them.
//    /// </summary>
//    public class NotificationDefinition
//    {
//        /// <summary>
//        /// Unique notification name.
//        /// </summary>
//        public string Name { get; private set; }

//        /// <summary>
//        /// Display name of the notification.
//        /// </summary>
//        public ILocalizableString DisplayName
//        {
//            get { return _displayName; }
//            set
//            {
//                if (value == null)
//                {
//                    throw new ArgumentNullException("value", "DisplayName can not be null!");
//                }

//                _displayName = value;
//            }
//        }
//        private ILocalizableString _displayName;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="NotificationDefinition"/> class.
//        /// </summary>
//        /// <param name="name">Unique notification name.</param>
//        /// <param name="displayName">Display name of the notification.</param>
//        public NotificationDefinition(string name, ILocalizableString displayName)
//        {
//            if (name.IsNullOrWhiteSpace())
//            {
//                throw new ArgumentException("name can not be null or white scpace!", "name");
//            }

//            Name = name;
//            DisplayName = displayName;
//        }
//    }
//}