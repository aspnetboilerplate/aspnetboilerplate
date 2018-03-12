using System.Collections.Generic;
using System.Linq;

namespace Abp.Application.Features
{
    /// <summary>
    /// Used to store <see cref="Feature"/>s.
    /// </summary>
    public class FeatureDictionary : Dictionary<string, Feature>
    {
        /// <summary>
        /// Adds all the child features of the current features, recursively.
        /// </summary>
        public void AddAllFeatures()
        {
            foreach (var feature in Values.ToList())
            {
                AddFeatureRecursively(feature);
            }
        }

        private void AddFeatureRecursively(Feature feature)
        {
            //Prevent multiple additions of the same-named feature.
            if (TryGetValue(feature.Name, out var existingFeature))
            {
                if (existingFeature != feature)
                {
                    throw new AbpInitializationException("Duplicate feature name detected for " + feature.Name);
                }
            }
            else
            {
                this[feature.Name] = feature;
            }

            //Add child features (recursive call)
            foreach (var childFeature in feature.Children)
            {
                AddFeatureRecursively(childFeature);
            }
        }
    }
}