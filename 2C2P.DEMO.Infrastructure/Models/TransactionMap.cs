using _2C2P.DEMO.Domain.Models;
using _2C2P.DEMO.Infrastructure.BsonMapper;
using MongoDB.Bson.Serialization;

namespace _2C2P.DEMO.Infrastructure.Models
{
    public class TransactionMap : IBsonMapper<Transaction>
    {
        public void Map()
        {
            BsonClassMap.RegisterClassMap<Transaction>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapMember(x => x.TransactionId).SetElementName("transId");
                map.MapMember(x => x.Amount).SetElementName("amount");
                map.MapMember(x => x.CurrencyCode).SetElementName("currCode");
                map.MapMember(x => x.TransactionDate).SetElementName("transDate");
                map.MapMember(x => x.Status).SetElementName("status");
            });
        }
    }
}
