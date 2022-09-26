using System.Web.Mvc;
using SmartStore.Web.Framework.Controllers;
using SmartStore.Web.Framework.Seo;

namespace SmartStore.Web.Controllers
{
    public partial class VitaeController : PublicControllerBase
    {
        public VitaeController()
        {
        }

        [RewriteUrl(SslRequirement.No)]
        public ActionResult Index(int id)
        {

            return View();
        }
    }
}
