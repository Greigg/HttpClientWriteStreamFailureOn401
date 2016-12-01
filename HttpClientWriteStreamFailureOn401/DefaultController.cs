using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace HttpClientWriteStreamFailureOn401
{
    [RoutePrefix("default")]
    public class DefaultController : ApiController
    {
        [Route("getData")]
        public IHttpActionResult GetData()
        {
            return this.Ok(new ServerResponse
            {
                Message = $"Get data - request recieved at {DateTime.Now.ToString()}"
            });
        }

        [Authorize]
        [Route("getData/authorize/true")]
        public IHttpActionResult GetDataWithAuthorize()
        {
            return this.Ok(new ServerResponse
            {
                Message = $"Get data - with authorize request recieved at {DateTime.Now.ToString()}"
            });
        }

        [UnathorizeWithDefaultResponse]
        [Route("getData/authorize/false")]
        public IHttpActionResult GetDataWithUnauthorizeAndDefaultResponse()
        {
            return this.Ok(new ServerResponse
            {
                Message = $"Get data with authorize and default response - request recieved at {DateTime.Now.ToString()}"
            });
        }

        [UnathorizeWithForbiddenResponse]
        [Route("getData/authorize/false/forbidden")]
        public IHttpActionResult GetDataWithUnauthorizeAndForbiddenResponse()
        {
            return this.Ok(new ServerResponse
            {
                Message = $"Get data with authorize and forbidden response - request recieved at {DateTime.Now.ToString()}"
            });
        }

        [Route("sendData")]
        public IHttpActionResult SendData([FromBody]Data data)
        {
            return this.Ok(new ServerResponse
            {
                Message = $"Send data - request recieved at {DateTime.Now.ToString()} with data: {data.Integer}"
            });
        }

        [Authorize]
        [Route("sendData/authorize/true")]
        public IHttpActionResult SendDataWithAuthorize([FromBody]Data data)
        {
            return this.Ok(new ServerResponse
            {
                Message = $"Send data with authorize - request recieved at {DateTime.Now.ToString()} with data: {data.Integer}"
            });
        }

        [UnathorizeWithDefaultResponse]
        [Route("sendData/authorize/false")]
        public IHttpActionResult SendDataWithUnauthorizeAndDefaultResponse([FromBody]Data data)
        {
            return this.Ok(new ServerResponse
            {
                Message = $"Send data with authorize and default response - request recieved at {DateTime.Now.ToString()} with data: {data.Integer}"
            });
        }

        [UnathorizeWithForbiddenResponse]
        [Route("sendData/authorize/false/forbidden")]
        public IHttpActionResult SendDataWithUnauthorizeAndForbiddenResponse([FromBody]Data data)
        {
            return this.Ok(new ServerResponse
            {
                Message = $"Send data with authorize and forbidden response - request recieved at {DateTime.Now.ToString()} with data: {data.Integer}"
            });
        }
    }

    public class UnathorizeWithDefaultResponseAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return false;
        }
    }

    public class UnathorizeWithForbiddenResponseAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Unauthorised to access this resource");
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return false;
        }
    }
}
