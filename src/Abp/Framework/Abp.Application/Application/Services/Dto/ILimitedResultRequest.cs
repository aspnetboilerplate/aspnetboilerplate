namespace Abp.Application.Services.Dto
{
    public interface ILimitedResultRequest
    {
        int MaxResultCount { get; set; }
    }
}