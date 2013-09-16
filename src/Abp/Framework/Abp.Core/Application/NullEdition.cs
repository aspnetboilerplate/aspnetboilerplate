namespace Abp.Application
{
    public sealed class NullEdition : Edition
    {
        private static readonly NullEdition _instance = new NullEdition();
        public static NullEdition Instance
        {
            get { return _instance; }
        }

        public NullEdition()
            : base("NullEdition")
        {

        }

        public override bool HasFeature(string featureName)
        {
            return true;
        }
    }
}