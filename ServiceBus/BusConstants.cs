using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus
{
    public class BusConstants
    {
        public const string OrderCreatedEventExchange = "order.api.order.created.event.exchange";

        public const string StockCreatedEventQueue = "stock.api.order.created.event.queue";
        public const string StockCreatedEventQueueMassTransit = "stock.api.order.created.event.masstransit.queue";

    }
}
