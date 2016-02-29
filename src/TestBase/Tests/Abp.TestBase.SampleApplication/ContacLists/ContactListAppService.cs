using Adorable.Application.Features;

namespace Adorable.TestBase.SampleApplication.ContacLists
{
    public class ContactListAppService : IContactListAppService
    {
        [RequiresFeature(SampleFeatureProvider.Names.Contacts)]
        public void Test()
        {
            //This method is called only if SampleFeatureProvider.Names.Contacts feature is enabled
        }
    }
}