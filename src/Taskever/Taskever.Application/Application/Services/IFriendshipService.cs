using Abp.Application.Services;
using Taskever.Application.Services.Dto.Friendships;

namespace Taskever.Application.Services
{
    public interface IFriendshipService : IApplicationService
    {
        GetFriendshipsOutput GetFriendships(GetFriendshipsInput input);

        GetFriendshipsByMostActiveOutput GetFriendshipsByMostActive(GetFriendshipsByMostActiveInput input);

        void ChangeFriendshipProperties(ChangeFriendshipPropertiesInput input);

        SendFriendshipRequestOutput SendFriendshipRequest(SendFriendshipRequestInput input);

        void RemoveFriendship(RemoveFriendshipInput input);

        void AcceptFriendship(AcceptFriendshipInput input);

        void RejectFriendship(RejectFriendshipInput input);

        void CancelFriendshipRequest(CancelFriendshipRequestInput input);

        void UpdateLastVisitTime(UpdateLastVisitTimeInput input);
    }
}