ABP - EntityFramework Core Integration
-----------------------------------------

TODO:

- [x] Repositories
- [x] Unit of work
- [x] Single DbContext Transaction management
- [x] Multiple connstring for same dbcontext
- [x] Multiple DbContext Transaction management

NOT POSSIBLE TO IMPLEMENT:

- Log validation errors - EF Core does not throw DbValidationException
- Data filters (waiting for https://github.com/aspnet/EntityFramework/issues/626 and https://github.com/jcachat/EntityFramework.DynamicFilters/issues/48) - No interceptors to implement it
- DateTime Kind Normalization (while getting entities from database) - No callback to implement it
