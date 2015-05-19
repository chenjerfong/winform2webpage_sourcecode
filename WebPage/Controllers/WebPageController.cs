using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace WebPage.Controllers
{
    public class WebPageController : ApiController
    {
        [Route("Authorized")]
        public IHttpActionResult Post_Authorized()
        {
            if (ModelState.IsValid == false)
            {
                return InternalServerError();
            }

            try
            {
                var token = new FormsAuthenticationTicket("A27946", true, 1440);
                HttpCookie uck = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(token));
                HttpContext.Current.Response.Cookies.Add(uck);
                return Ok();
            }
            catch
            {
                return InternalServerError();
            }
        }

        [Route("StartRun")]
        public bool Post_Start(Query query)
        {
            if (HttpContext.Current != null && (HttpContext.Current.User == null || Thread.CurrentPrincipal.Identity.IsAuthenticated == false))
            {
                return false;
            }
            try
            {
                Task.Factory.StartNew(() =>
                {
                    CountFoldersTicker.Instance.GetCountFolders(query.connectionId).DoWork(query.path);
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("StopRun")]
        public bool Post_Stop(Query query)
        {
            if (HttpContext.Current != null && (HttpContext.Current.User == null || Thread.CurrentPrincipal.Identity.IsAuthenticated == false))
            {
                return false;
            }
            CountFoldersTicker.Instance.GetCountFolders(query.connectionId).StopWork();
            return true;
        }

    }

    public class Query
    {
        public string connectionId;
        public string path;
    }

}
