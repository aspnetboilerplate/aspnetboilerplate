using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.EntityFrameworkCore.Tests.Domain;

namespace Abp.EntityFrameworkCore.Tests.Ef
{
    public class TicketListItemRepository : SupportRepositoryBase<TicketListItem>
    {
        private IQueryable<TicketListItem> View => Context.TicketListItems;

        public TicketListItemRepository(IDbContextProvider<SupportDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public override IQueryable<TicketListItem> GetAllIncluding(params Expression<Func<TicketListItem, object>>[] propertySelectors) => View;



        public override TicketListItem Load(int id) => throw new NotImplementedException();

        public override TicketListItem Insert(TicketListItem entity) => throw new NotImplementedException();

        public override Task<TicketListItem> InsertAsync(TicketListItem entity) => throw new NotImplementedException();

        public override int InsertAndGetId(TicketListItem entity) => throw new NotImplementedException();

        public override Task<int> InsertAndGetIdAsync(TicketListItem entity) => throw new NotImplementedException();

        public override TicketListItem InsertOrUpdate(TicketListItem entity) => throw new NotImplementedException();

        public override Task<TicketListItem> InsertOrUpdateAsync(TicketListItem entity) => throw new NotImplementedException();

        public override int InsertOrUpdateAndGetId(TicketListItem entity) => throw new NotImplementedException();

        public override Task<int> InsertOrUpdateAndGetIdAsync(TicketListItem entity) => throw new NotImplementedException();

        public override TicketListItem Update(TicketListItem entity) => throw new NotImplementedException();

        public override Task<TicketListItem> UpdateAsync(TicketListItem entity) => throw new NotImplementedException();

        public override TicketListItem Update(int id, Action<TicketListItem> updateAction) => throw new NotImplementedException();

        public override Task<TicketListItem> UpdateAsync(int id, Func<TicketListItem, Task> updateAction) => throw new NotImplementedException();

        public override void Delete(TicketListItem entity) => throw new NotImplementedException();

        public override Task DeleteAsync(TicketListItem entity) => throw new NotImplementedException();

        public override void Delete(int id) => throw new NotImplementedException();

        public override Task DeleteAsync(int id) => throw new NotImplementedException();

        public override void Delete(Expression<Func<TicketListItem, bool>> predicate) => throw new NotImplementedException();

        public override Task DeleteAsync(Expression<Func<TicketListItem, bool>> predicate) => throw new NotImplementedException();
    }
}
