using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using Contracts;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase {

    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    private readonly IPublishEndpoint _publishEndpoint;
    public AuctionsController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint) {
        _context = context;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date) {

        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();
        if (!string.IsNullOrEmpty(date)) {
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime())>0);
        }
    // var auctions = await _context.Auctions.Include(x => x.Item).OrderBy(x => x.Item.Make).ToListAsync();
    //  return _mapper.Map<List<AuctionDto>>(auctions);
  //  return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
  var auctions = await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
   var auctionCount = auctions.Count;

       var result = new
    {
        Auctions = auctions,
        AuctionCount = auctionCount
    };

    return Ok(result);
    }



    [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id) {
        var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null) return NotFound();

     return _mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]

    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto) {
        var auction = _mapper.Map<Auction>(auctionDto);
        // TODO: add current user as seller
        auction.Seller = "test";

        _context.Auctions.Add(auction);

        var newAuction = _mapper.Map<AuctionDto>(auction);

        await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

        var result = await _context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Could not save changes to the auction DB.");

     //   return CreatedAtAction(nameof(GetAuctionById), new {auction.Id}, _mapper.Map<AuctionDto>(auction));

     // replace _mapper.map with newAuction, which is an auctionDto, after creating the rabbitmq and masstransit middleware for messaging.
        return CreatedAtAction(nameof(GetAuctionById), new {auction.Id}, newAuction);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto) {
        var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null) return NotFound();

        // TODO: check seller == current user

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
       // auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
       // auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

    var result = await _context.SaveChangesAsync() >0;
    if (result) return Ok();
    return BadRequest("Problem updating changes");
    }

    [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id) {

            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null) return NotFound();
        

            // TODO: check seller == username
            _context.Auctions.Remove(auction);

            var result = await _context.SaveChangesAsync() >0;
            if (result) return Ok();
    return BadRequest("Problem deleting this auction");

        }

    [HttpDelete]
        public async Task<ActionResult> DeleteAllAuctions() {
        

            // TODO: check seller == username
            _context.Auctions.RemoveRange(_context.Auctions);

            var result = await _context.SaveChangesAsync() > 0;
            if (result) return Ok();
    return BadRequest("Problem deleting auctions");

        }
}
