using EzImporter.Models;
using Newtonsoft.Json;
using Sitecore.Services.Infrastructure.Web.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;

namespace EzImporter.Controllers
{
    public class ImportController : ServicesApiController
    {
        [HttpPost]
        public IHttpActionResult Import(ImportModel importModel)
        {
            var rt = new ResultType
            {
                Name = "Test Person"
            };
            return new JsonResult<ResultType>(rt, new JsonSerializerSettings(), Encoding.UTF8, this);
        }
    }

    public class ResultType
    {
        public string Name { get; set; }
    }
}