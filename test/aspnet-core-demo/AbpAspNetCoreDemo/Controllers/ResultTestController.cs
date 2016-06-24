using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers
{
    public class ResultTestController : MyDemoControllerBase
    {
        public ActionResult JsonTest()
        {
            return Json(new {value = 42});
        }

        public ObjectResult ObjectTest1()
        {
            return new OkObjectResult(43);
        }

        public object ObjectTest2()
        {
            return 44;
        }
    }
}