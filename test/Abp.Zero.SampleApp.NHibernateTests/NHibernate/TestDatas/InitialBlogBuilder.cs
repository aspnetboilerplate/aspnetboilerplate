using Abp.Zero.SampleApp.EntityHistory;
using NHibernate;

namespace Abp.Zero.SampleApp.NHibernate.TestDatas
{
    public class InitialBlogBuilder
    {
        private readonly ISession _session;

        public InitialBlogBuilder(ISession session)
        {
            _session = session;
        }

        public void Build()
        {
            SaveBlogs();
            SaveAdvertisements();
        }


        private void SaveAdvertisements()
        {
            _session.Save(new Advertisement
            {
                Banner = "test-advertisement-1"
            });
        }

        private void SaveBlogs()
        {
            var blog1 = new Blog
            {
                Name = "test-blog-1",
                More = new BlogEx
                {
                    BloggerName = "blogger-1"
                }
            };
            
            _session.Save(blog1);
            // _session.Save(new Post(blog1, "test-post-1-title", "test-post-1-body"));
            
            var blog2 = new Blog
            {
                Name = "test-blog-2",
            };
            blog2.ChangeUrl("http://testblog2.myblogs.com");
            _session.Save(blog2);
        }
    }
}