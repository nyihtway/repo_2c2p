using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _2C2P.DEMO.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2C2P.DEMO.Api.Features.Transactions
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TransactionsController
    {
        IMediator _mediator;
        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("currency")]
        [ProducesResponseType(typeof(List<Transaction>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTransactionsByCurrency(string currencyCode)
        {

            return new OkResult();
        }

        [HttpGet("date")]
        public async Task<IActionResult> GetTransactionsByDate()
        {

            return new OkResult();
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetTransactionsByStatus(string status)
        {

            return new OkResult();
        }
    }
}
