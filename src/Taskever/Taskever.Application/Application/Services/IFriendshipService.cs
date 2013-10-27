using Abp.Application.Services;
using Taskever.Application.Services.Dto.Friendships;

namespace Taskever.Application.Services
{
    public interface IFriendshipService : IApplicationService
    {
        GetFriendshipsOutput GetFriendships(GetFriendshipsInput input);

        ChangeFriendshipPropertiesOutput ChangeFriendshipProperties(ChangeFriendshipPropertiesInput input);

        SendFriendshipRequestOutput SendFriendshipRequest(SendFriendshipRequestInput input);

        RemoveFriendshipOutput RemoveFriendship(RemoveFriendshipInput input);

        AcceptFriendshipOutput AcceptFriendship(AcceptFriendshipInput input);
    }
}