using System;
using System.Web.Mvc;
using Abp.Threading;
using MyProject.News;
using System.Collections.Generic;

namespace MyProject.Web.Controllers
{
    public class NewsController : MyProjectControllerBase
    {
        private readonly INewsAppService _newsAppService;
        public NewsController(INewsAppService newsAppService)
        {
            _newsAppService = newsAppService;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            IList<News.News> list = _newsAppService.GetAllList();
            return View(list);
        }

        /// <summary>
        /// 增
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 删
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete(long id)
        {
            _newsAppService.Delete(id);
            return View();
        }

        /// <summary>
        /// 查
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(long id)
        {
            News.News obj = _newsAppService.Get(id);
            return View(obj);
        }

        /// <summary>
        /// 改
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(long id)
        {
            News.News obj = _newsAppService.Get(id);
            return View(obj);
        }

    }
}