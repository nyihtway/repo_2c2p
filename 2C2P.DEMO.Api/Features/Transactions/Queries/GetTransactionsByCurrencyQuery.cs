using _2C2P.DEMO.Api.Models;
using _2C2P.DEMO.Infrastructure.Interfaces;
using _2C2P.DEMO.Infrastructure.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using _2C2P.DEMO.Domain.Models;

namespace _2C2P.DEMO.Api.Features.Transactions.Queries
{
    public class GetTransactionsByCurrencyQuery : IRequest<Response<List<TransactionDTO>>>
    {
        public string _currency;
        public GetTransactionsByCurrencyQuery(string currency)
        {
            _currency = currency ?? throw new ArgumentNullException(nameof(currency));
        }

        public class GetTransactionsByCurrencyQueryHandler : IRequestHandler<GetTransactionsByCurrencyQuery, Response<List<TransactionDTO>>>
        {
            private readonly ITransactionRepository _transactionRepository;
            private readonly IMapper _mapper;

            public GetTransactionsByCurrencyQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
            {
                _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<Response<List<TransactionDTO>>> Handle(GetTransactionsByCurrencyQuery request, CancellationToken cancellationToken)
            {
                var currency = request._currency;
                var result = new List<TransactionDTO>();
                IEnumerable<Transaction> transactions= await _transactionRepository.GetTransactionsByCurrency(currency);

                result = _mapper.Map<List<TransactionDTO>>(transactions.ToList());

                return new OkResponse<List<TransactionDTO>>(result);
            }
        }
    }
}
