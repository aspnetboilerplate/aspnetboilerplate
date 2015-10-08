using System;
using System.Web.Mvc;
using Abp.Threading;
using MyProject.News;

namespace MyProject.Web.Controllers
{
    public class NewsController : MyProjectControllerBase
    {
        private readonly INewsAppService _newsAppService;
        public NewsController(INewsAppService newsAppService)
        {
            _newsAppService = newsAppService;
        }


        // GET: News
        public ActionResult Index()
        {
            _newsAppService.Count();
            _newsAppService.Single(p => p.Id == 1);
            return View();
        }
    }
}