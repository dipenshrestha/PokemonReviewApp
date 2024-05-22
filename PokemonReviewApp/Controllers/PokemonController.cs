using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
	[Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
	public class PokemonController : Controller
	{
        private readonly IPokemonRepository _pokemonRepository;
		private readonly IReviewRepository _reviewRepository;
		private readonly IMapper _mapper;
        public PokemonController(IPokemonRepository pokemonRepository,
			IReviewRepository reviewRepository,
			IMapper mapper)
        {
            _pokemonRepository = pokemonRepository;
			_reviewRepository = reviewRepository;
			_mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons() {
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);    
        }

        [HttpGet("{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))] //makes api looks cleaner
		[ProducesResponseType(400)]
		public IActionResult GetPokemon(int pokeId)
		{
            if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();
            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
			return Ok(pokemon);
		}

		[HttpGet("{pokeId}/rating")]
		[ProducesResponseType(200, Type = typeof(decimal))] //makes api looks cleaner
		[ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if(!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();
            var rating = _pokemonRepository.GetPokemonRating(pokeId);
            if(!ModelState.IsValid)
                return BadRequest();
            return Ok(rating);
        }

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		// country leko xa kina ki that is many to one relation ma parxa
		public IActionResult CreatePokemon([FromQuery] int ownerId,
			[FromQuery] int catId, [FromBody] PokemonDto pokemonCreate)
		{
			if (pokemonCreate == null)
				return BadRequest(ModelState);
			var pokemons = _pokemonRepository.GetPokemons()
				.Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
				.FirstOrDefault();

			if (pokemons != null)
			{
				ModelState.AddModelError("", "Pokemon already exists");
				return StatusCode(422, ModelState);
			}

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

			if (!_pokemonRepository.CreatePokemon(ownerId, catId, pokemonMap))
			{
				ModelState.AddModelError("", "Something went wrong while saving");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully created!");

		}

		[HttpPut("{pokeId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult UpadatePokemon(int pokeId, [FromQuery]int ownerId,
			[FromQuery]int catId, [FromBody] PokemonDto updatedPokemon)
		{
			if (updatedPokemon == null)
				return BadRequest(ModelState);

			if (pokeId != updatedPokemon.Id)
				return BadRequest(ModelState);

			if (!_pokemonRepository.PokemonExists(pokeId))
				return NotFound();

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var pokemonMap = _mapper.Map<Pokemon>(updatedPokemon);

			if (!_pokemonRepository.UpdatePokemon(ownerId, catId, pokemonMap))
			{
				ModelState.AddModelError("", "Something went wrong while updating pokemon");
				return StatusCode(500, ModelState);
			}
			return NoContent();
		}

		[HttpDelete("{pokeId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult DeletePokemon(int pokeId)
		{
			//validation
			if (!_pokemonRepository.PokemonExists(pokeId))
			{
				return NotFound();
			}

			//to also delete data that are tied to pokemon 
			var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokeId);

			var pokemonToDelete = _pokemonRepository.GetPokemon(pokeId);
			//remember to check tied to this or  not so as this doesnt mess out database
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
			{
				ModelState.AddModelError("", "Something went wrong deleting Reviews");
			}
			if (!_pokemonRepository.DeletePokemon(pokemonToDelete))
			{
				ModelState.AddModelError("", "Something went wrong deleting pokemon");
			}
			return NoContent();
		}
	}
}
