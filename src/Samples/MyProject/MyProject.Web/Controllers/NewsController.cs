using System;
using System.Web.Mvc;
using Abp.Threading;
using MyProject.News;
using System.Collections.Generic;
using System.ComponentModel;
using MyProject.News.Dtos;

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
            return View(new CreateNewsInput());
        }

        #region 新增保存处理

        /// <summary>
        /// 新增保存处理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [Description("保存新增")]
        public ActionResult Create(CreateNewsInput input)
        {
            //OperationResult result = new OperationResult(OperationResultType.Warning, "网络错误，请稍后重试！");

            string msg = string.Empty;

            if (ModelState.IsValid)
            {
                News.News news = _newsAppService.CreateNews(input);
                if (news != null && news.Id > 0)
                {
                    // result = new OperationResult(OperationResultType.Success, "保存成功");
                    // return Json(result, JsonRequestBehavior.AllowGet);
                    return new RedirectResult("/News/List");
                }
            }
            return View(input);
        }

        #endregion


        /// <summary>
        /// 删
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete(long id)
        {
            _newsAppService.Delete(id);
            return new RedirectResult("/News/List");
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

        #region 编辑保存处理

        /// <summary>
        /// 编辑保存处理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [Description("保存编辑")]
        public ActionResult Edit(UpdateNewsInput input)
        {
            News.News model = _newsAppService.Get(input.Id);
            if (ModelState.IsValid)
            {
                //   OperationResult result = new OperationResult(OperationResultType.Warning, "网络错误，请稍后重试！");
               model = _newsAppService.UpdateNews(input);

                if (model != null)
                {
                    //result = new OperationResult(OperationResultType.Success, "保存成功");
                    //return Json(result, JsonRequestBehavior.AllowGet);
                    return new RedirectResult("/News/List");
                }
            }
            return View(model);
        }

        #endregion


    }
}