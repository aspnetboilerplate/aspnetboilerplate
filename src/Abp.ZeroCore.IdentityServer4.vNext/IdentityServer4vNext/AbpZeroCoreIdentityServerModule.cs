using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero;
using IdentityServer4.Models;

namespace Abp.IdentityServer4vNext
{
    [DependsOn(typeof(AbpZeroCoreModule), typeof(AbpAutoMapperModule))]
    public class AbpZeroCoreIdentityServervNextModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(config =>
            {
                //PersistedGrant -> PersistedGrantEntity
                config.CreateMap<PersistedGrant, PersistedGrantEntity>()
                    .ForMember(d => d.Id, c => c.MapFrom(s => s.Key));

                //PersistedGrantEntity -> PersistedGrant
                config.CreateMap<PersistedGrantEntity, PersistedGrant>()
                    .ForMember(d => d.Key, c => c.MapFrom(s => s.Id));
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpZeroCoreIdentityServervNextModule).GetAssembly());
        }
    }
}
