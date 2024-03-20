using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Sample.Contracts;
using System.ComponentModel.DataAnnotations;

namespace MassTransitDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private ILogger<OrderController> _logger;
        private IRequestClient<SubmitOrder> _submitOrderRequestClient;
        private ISendEndpointProvider _sendEndpointProvider;

        public OrderController(ILogger<OrderController> logger, 
                               IRequestClient<SubmitOrder> submitOrderRequestClient,
                               ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger;
            _submitOrderRequestClient = submitOrderRequestClient;
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Guid id, string customerNumber)
        {
            var (accepted, rejected) = await _submitOrderRequestClient.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new 
            {
                OrderId = id,
                Timestamp = DateTime.UtcNow,
                CustomerNumber = customerNumber
            });

            if (accepted.IsCompletedSuccessfully)
            {
                var response = await accepted;

                return Ok(response);
            }
            else
            {
                var response = await rejected;

                return BadRequest(response);
            }

        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid id, string customerNumber)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("exchange:order-service"));

            await endpoint.Send<SubmitOrder>(new
            {
                OrderId = id,
                Timestamp = DateTime.UtcNow,
                CustomerNumber = customerNumber
            });

            return Accepted();

        }
    }
}
