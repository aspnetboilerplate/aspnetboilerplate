namespace Abp.Application.Services.Dto
{
    public interface IPagedResultRequest
    {
        int SkipCount { get; set; }
        int MaxResultCount { get; set; }
    }
}