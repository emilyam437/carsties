using MassTransit;
using Contracts;

namespace AuctionService;

public class AuctionCreatedFaultsConsumer : IConsumer<Fault<AuctionCreated>>{
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context) {
        Console.WriteLine("consuming faulty creation");
        var exception = context.Message.Exceptions.First();
        if (exception.ExceptionType == "System.ArgumentException") {
            context.Message.Message.Model = "FooBar";
            Console.WriteLine("Changing foo to FooBar");
            await context.Publish(context.Message.Message);
        } else {
            Console.WriteLine("Not an argument exception - update error dashboard somewhere");
        }
    }
}