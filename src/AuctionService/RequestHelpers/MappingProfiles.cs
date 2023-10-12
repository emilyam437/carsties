using AutoMapper;
using AuctionService.DTOs;
using AuctionService.Entities;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile {

    public MappingProfiles() {
        CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
        CreateMap<Item, AuctionDto>();
        CreateMap<CreateAuctionDto, Auction>().ForMember(dest => dest.Item, opt => opt.MapFrom(source => source));
        CreateMap<CreateAuctionDto, Item>();
    }
}