using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Dapper.Repositories;
using Abp.Dapper.Tests.DbContexts;
using Abp.Dapper.Tests.Entities;
using Abp.Data;
using Dapper;

namespace Abp.Dapper.Tests.Repositories
{
    public class CustomRepository : DapperEfRepositoryBase<SampleDapperApplicationDbContext>
    {
        public CustomRepository(IActiveTransactionProvider activeTransactionProvider)
            : base(activeTransactionProvider)
        {
        }

        public void AddProducts()
        {
            var param = new
            {
                LastModificationTime = DateTime.Parse("2019-01-01 08:00:00"),
                CreationTime = DateTime.Parse("2019-01-01 08:00:00")
            };

            Connection.Query("insert into Products values (1, 'watch', false, null, null, @LastModificationTime, null, @CreationTime, null, 1, 0)", param);
            Connection.Query("insert into Products values (2, 'phone', false, null, null, @LastModificationTime, null, @CreationTime, null, 1, 0)", param);
            Connection.Query("insert into Products values (3, 'pad', false, null, null, @LastModificationTime, null, @CreationTime, null, 1, 0)", param);
        }

        public List<Product> GetProducts()
        {
            return Connection.Query<Product>("select * from Products").ToList();
        }
    }
}