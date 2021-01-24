using _2C2P.DEMO.Api.Features.Transactions.Commands;
using _2C2P.DEMO.Api.Features.Transactions.Queries;
using _2C2P.DEMO.Api.Models;
using _2C2P.DEMO.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _2C2P.DEMO.Api.Features.Transactions
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TransactionsController : ControllerBase
    {
        IMediator _mediator;
        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost()]
        public async Task<IActionResult> InsertTransaction(Transaction transaction)
        {
            var response = await _mediator.Send(new InsertTransactionCommand(transaction));

            if (response.IsError)
            {
                return BadRequest(response.FormatErrors());
            }

            return Ok();
        }

        [HttpGet("currency")]
        [ProducesResponseType(typeof(List<TransactionDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTransactionsByCurrency(string currencyCode)
        {
            var response = await _mediator.Send(new GetTransactionsByCurrencyQuery(currencyCode));

            if (response.IsError)
            {
                return BadRequest(response.FormatErrors());
            }

            return Ok(response.Data);

        }

        [HttpPost("date")]
        [ProducesResponseType(typeof(List<TransactionDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTransactionsByDate(GetTransactionsByDateQuery query)
        {

            var response = await _mediator.Send(query);

            if (response.IsError)
            {
                return BadRequest(response.FormatErrors());
            }

            return Ok(response.Data);
        }

        [HttpGet("status")]
        [ProducesResponseType(typeof(List<TransactionDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTransactionsByStatus(string status)
        {
            var response = await _mediator.Send(new GetTransactionsByStatusQuery(status));

            if (response.IsError)
            {
                return BadRequest(response.FormatErrors());
            }

            return Ok(response.Data);
        }
    }
}
