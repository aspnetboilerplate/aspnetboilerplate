using System;
using AutoMapper;

namespace Abp.AutoMapper
{
    public static class AbpEmulateAutoMapper
    {
        [Obsolete("Automapper will remove static API, Please use ObjectMapper instead. See https://github.com/aspnetboilerplate/aspnetboilerplate/issues/4667")]
        public static IMapper Mapper { get; set; }
    }
}