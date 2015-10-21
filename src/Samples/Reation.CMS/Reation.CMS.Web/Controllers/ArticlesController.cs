using System;
using System.Web.Mvc;
using Abp.Threading;
using Reation.CMS.Articles;
using System.Collections.Generic;
using System.ComponentModel;
using Reation.CMS.Articles.Dtos;

namespace Reation.CMS.Web.Controllers
{
    public class ArticlesController : CMSControllerBase
    {
        private readonly IArticleAppService _articleAppService;
        public ArticlesController(IArticleAppService articleAppService)
        {
            _articleAppService = articleAppService;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            IList<Articles.Article> list = _articleAppService.GetAllList();
            return View(list);
        }

        /// <summary>
        /// 增
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View(new CreateArticleInput());
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
        public ActionResult Create(CreateArticleInput input)
        {
            //OperationResult result = new OperationResult(OperationResultType.Warning, "网络错误，请稍后重试！");

            string msg = string.Empty;

            if (ModelState.IsValid)
            {
                Articles.Article article = _articleAppService.CreateArticle(input);
                if (article != null && article.Id > 0)
                {
                    // result = new OperationResult(OperationResultType.Success, "保存成功");
                    // return Json(result, JsonRequestBehavior.AllowGet);
                    return new RedirectResult("/Articles/List");
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
            _articleAppService.Delete(id);
            return new RedirectResult("/Articles/List");
        }

        /// <summary>
        /// 查
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(long id)
        {
            Articles.Article  obj = _articleAppService.Get(id);
            return View(obj);
        }

        /// <summary>
        /// 改
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(long id)
        {
            Articles.Article obj = _articleAppService.Get(id);
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
        public ActionResult Edit(UpdateArticleInput input)
        {
            Articles.Article model = _articleAppService.Get(input.Id);
            if (ModelState.IsValid)
            {
                //   OperationResult result = new OperationResult(OperationResultType.Warning, "网络错误，请稍后重试！");
               model = _articleAppService.UpdateArticle(input);

                if (model != null)
                {
                    //result = new OperationResult(OperationResultType.Success, "保存成功");
                    //return Json(result, JsonRequestBehavior.AllowGet);
                    return new RedirectResult("/Articles/List");
                }
            }
            return View(model);
        }

        #endregion


    }
}