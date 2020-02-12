using System.Collections.Generic;

namespace Abp.EntityHistory
{
    public class EntityHistorySnapshot
    {
        /// <summary>
        /// if dictionary contains value it means EntityPropertyChange has that value. Keys are propertyNames, values are snapshot values
        /// </summary>
        public Dictionary<string, string> ChangedPropertiesSnapshots { get; }

        /// <summary>
        /// Keys are propertyNames, values are stack tree string from present to past
        /// </summary>
        public Dictionary<string, string> PropertyChangesStackTree { get; }

        public EntityHistorySnapshot(Dictionary<string, string> snapshotProperties, Dictionary<string, string> propertyChangesStackTree)
        {
            ChangedPropertiesSnapshots = snapshotProperties;
            PropertyChangesStackTree = propertyChangesStackTree;
        }

        /// <summary>
        /// Shortcut of ChangedProperties
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns>Changed property. If dictionary doesn't contain key, this will return null</returns>
        public string this[string propertyName] => ChangedPropertiesSnapshots.ContainsKey(propertyName) ? ChangedPropertiesSnapshots[propertyName] : null;

        /// <summary>
        /// returns whether entity property changed in this snapshot. (ChangedPropertiesSnapshots.ContainsKey(key))
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool IsPropertyChanged(string propertyName) => ChangedPropertiesSnapshots.ContainsKey(propertyName);
    }
}
