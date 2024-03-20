using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Sample.Contracts;

namespace Sample.Components.Consumers
{
    public class SubmitOrderWithDiscountConsumer : IConsumer<SubmitOrder>
    {
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            Console.WriteLine($"Submit order with discount command is received {context.Message.CustomerNumber} and price is {context.Message.MoneyInDollars * 0.9} dollars.");


        }
    }
}
