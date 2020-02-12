using Abp.Domain.Entities;
using System.Collections.Generic;

namespace Abp.ZeroCore.SampleApp.Core.EntityHistory
{
    public class Advertisement : Entity
    {
        public string Banner { get; set; }

        public ICollection<AdvertisementFeedback> Feedbacks { get; set; }
    }

    public class AdvertisementFeedback
    {
        public int AdvertisementId { get; set; }

        public int CommentId { get; set; }
    }
}
