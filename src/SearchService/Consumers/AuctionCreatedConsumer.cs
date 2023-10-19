using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated> {

    private readonly IMapper _mapper;

// constructor:
    public AuctionCreatedConsumer(IMapper mapper) {
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AuctionCreated> context) {

        Console.WriteLine("--> Consuming auction created: "+context.Message.Id);

        var item = _mapper.Map<Item>(context.Message);

        if (item.Model == "foo") throw new ArgumentException("Cannot sell cars of the model foo");

        await item.SaveAsync();
    }
}