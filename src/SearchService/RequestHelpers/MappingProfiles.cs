using AutoMapper;
using Contracts;

namespace SearchService;

public class MappingProfiles : Profile {

    // create an empty constructor:

    public MappingProfiles() {
        CreateMap<AuctionCreated, Item>();
    }

}