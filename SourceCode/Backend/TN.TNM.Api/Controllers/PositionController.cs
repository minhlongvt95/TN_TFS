using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Position;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Position;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TN.TNM.Api.Controllers
{
    public class PositionController : Controller
    {
        private readonly IPosition iPosition;
        public PositionController(IPosition _iPosition)
        {
            this.iPosition = _iPosition;
        }

        /// <summary>
        /// Get all Position info
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/position/getAllPosition")]
        [Authorize(Policy = "Member")]
        public GetAllPositionResponse GetAllPosition(GetAllPositionRequest request)
        {
            return this.iPosition.GetAllPosition(request);
        }
    }
}
