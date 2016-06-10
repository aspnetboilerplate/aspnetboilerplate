using Abp.Application.Features;

namespace Abp.TestBase.SampleApplication.ContacLists
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