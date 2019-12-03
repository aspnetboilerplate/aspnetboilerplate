using System;
using System.Collections.Generic;
using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Abp.ZeroCore.SampleApp.Core.EntityHistory
{
    [Audited]
    public class Blog : AggregateRoot, IHasCreationTime
    {
        [DisableAuditing]
        public string Name { get; set; }

        public string Url { get; protected set; }

        public DateTime CreationTime { get; set; }

        public BlogEx More { get; set; }

        public ICollection<Post> Posts { get; set; }

        public Blog()
        {

        }

        public Blog(string name, string url, string bloggerName)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            Name = name;
            Url = url;
            More = new BlogEx { BloggerName = bloggerName };
        }

        public void ChangeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            var oldUrl = Url;
            Url = url;
        }
    }

    public class BlogEx
    {
        public string BloggerName { get; set; }
    }
}