using AutoMapper;
using AuctionService.DTOs;
using AuctionService.Entities;
using Contracts;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile {

    public MappingProfiles() {
        CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
        CreateMap<Item, AuctionDto>();

        CreateMap<CreateAuctionDto, Auction>().ForMember(dest => dest.Item, opt => opt.MapFrom(source => source));
        CreateMap<CreateAuctionDto, Item>();
        CreateMap<AuctionDto, AuctionCreated>();

        CreateMap<UpdateAuctionDto, Auction>().ForMember(dest => dest.Item, opt => opt.MapFrom(source => source));
         CreateMap<UpdateAuctionDto, Item>();
         CreateMap<AuctionDto, AuctionUpdated>();
      //  CreateMap<Auction, AuctionUpdated>().IncludeMembers(a => a.Item);
        CreateMap<Item, AuctionUpdated>();

        CreateMap<DeleteAuctionDto, Auction>();
        CreateMap<DeleteAuctionDto, Item>();
        CreateMap<AuctionDto, AuctionDeleted>();
    }
}