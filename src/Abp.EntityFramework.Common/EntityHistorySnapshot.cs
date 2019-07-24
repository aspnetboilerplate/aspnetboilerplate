using System;
using System.Collections.Generic;
using System.Text;
using Abp.Events.Bus.Entities;

namespace Abp.EntityFramework
{
    public class EntityHistorySnapshot
    {
        /// <summary>
        /// if dictionary contains value it means EntityPropertyChange has that value
        /// </summary>
        public Dictionary<string, string> ChangedPropertiesSnapshots { get; }

        public Dictionary<string, string> PropertyChangesStackTree { get; }

        public EntityHistorySnapshot(Dictionary<string, string> snapshotProperties, Dictionary<string, string> propertyChangesStackTree)
        {
            ChangedPropertiesSnapshots = snapshotProperties;
            PropertyChangesStackTree = propertyChangesStackTree;
        }

        /// <summary>
        /// Shortcut of ChangedProperties
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Changed property. If dictionary is not contain key this will return null</returns>
        public string this[string key] => ChangedPropertiesSnapshots.ContainsKey(key) ? ChangedPropertiesSnapshots[key] : null;

        /// <summary>
        /// returns whether entity property changed in this snapshot.  (ChangedPropertiesSnapshots.ContainsKey(key))
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsPropertyChanged(string key) => ChangedPropertiesSnapshots.ContainsKey(key);
    }
}
