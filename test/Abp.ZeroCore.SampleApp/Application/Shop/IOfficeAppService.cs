using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.ZeroCore.SampleApp.Core.Shop;
using Microsoft.EntityFrameworkCore;

namespace Abp.ZeroCore.SampleApp.Application.Shop;

public interface IOfficeAppService : IApplicationService
{
    Task<ListResultDto<OfficeListDto>> GetOffices();
}

public class OfficeAppService : ApplicationService, IOfficeAppService
{
    private readonly IRepository<Office> _officeRepository;
    private readonly IRepository<OfficeTranslation, long> _officeTranslationRepository;

    public OfficeAppService(
        IRepository<Office> officeRepository,
        IRepository<OfficeTranslation, long> officeTranslationRepository
    )
    {
        _officeRepository = officeRepository;
        _officeTranslationRepository = officeTranslationRepository;
    }

    public async Task<ListResultDto<OfficeListDto>> GetOffices()
    {
        var offices = await _officeRepository.GetAllIncluding(p => p.Translations).ToListAsync();
        return new ListResultDto<OfficeListDto>(ObjectMapper.Map<List<OfficeListDto>>(offices));
    }
}