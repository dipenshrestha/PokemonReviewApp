using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
	[Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
	[ApiController]
	public class OwnerController : Controller
	{
		private IOwnerRepository _ownerRepository;
		private readonly ICountryRepository _countryRepository;
		private IMapper _mapper;

		public OwnerController(IOwnerRepository ownerRepository, 
			ICountryRepository countryRepository, IMapper mapper)
		{
			_ownerRepository = ownerRepository;
			_countryRepository = countryRepository;
			_mapper = mapper;

		}

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
		public IActionResult GetOwners()
		{
			var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(owners);
		}

		[HttpGet("{ownerId}")]
		[ProducesResponseType(200, Type = typeof(Owner))] //makes api looks cleaner
		[ProducesResponseType(400)]
		public IActionResult GetOwner(int ownerId)
		{
			if (!_ownerRepository.OwnerExists(ownerId))
				return NotFound();
			var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			return Ok(owner);
		}

		[HttpGet("{ownerId}/pokemon")]
		[ProducesResponseType(200, Type = typeof(Owner))] //makes api looks cleaner
		[ProducesResponseType(400)]
		public IActionResult GetPokemonByOwner(int ownerId)
		{
			if(!_ownerRepository.OwnerExists(ownerId))
			{
				return NotFound();
			}
			var owner = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonByOwner(ownerId));
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(owner);
		}

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		// country leko xa kina ki that is many to one relation ma parxa
		public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto ownerCreate)
		{
			if (ownerCreate == null)
				return BadRequest(ModelState);
			var owners = _ownerRepository.GetOwners()
				.Where(c => c.LastName.Trim().ToUpper() == ownerCreate.LastName.TrimEnd().ToUpper())
				.FirstOrDefault();

			if (owners != null)
			{
				ModelState.AddModelError("", "Owner already exists");
				return StatusCode(422, ModelState);
			}

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var ownerMap = _mapper.Map<Owner>(ownerCreate);
			
			//for one relation

			ownerMap.Country = _countryRepository.GetCountry(countryId);

			if (!_ownerRepository.CreateOwner(ownerMap))
			{
				ModelState.AddModelError("", "Something went wrong while saving");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully created!");
		}

		[HttpPut("{ownerId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult UpadateOwner(int ownerId, [FromBody] OwnerDto updatedOwner)
		{
			if (updatedOwner == null)
				return BadRequest(ModelState);

			if (ownerId != updatedOwner.Id)
				return BadRequest(ModelState);

			if (!_ownerRepository.OwnerExists(ownerId))
				return NotFound();

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var ownerMap = _mapper.Map<Owner>(updatedOwner);

			if (!_ownerRepository.UpdateOwner(ownerMap))
			{
				ModelState.AddModelError("", "Something went wrong while updating owner");
				return StatusCode(500, ModelState);
			}
			return NoContent();
		}

		[HttpDelete("{ownerId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult DeleteOwner(int ownerId)
		{
			//validation
			if (!_ownerRepository.OwnerExists(ownerId))
			{
				return NotFound();
			}
			var ownerToDelete = _ownerRepository.GetOwner(ownerId);
			//remember to check tied to this or  not so as this doesnt mess out database
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (!_ownerRepository.DeleteOwner(ownerToDelete))
			{
				ModelState.AddModelError("", "Something went wrong deleting owner");
			}
			return NoContent();
		}
	}
}
