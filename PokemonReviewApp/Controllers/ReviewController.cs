using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
	[Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
	[ApiController]
	public class ReviewController : Controller
	{
		private readonly IReviewRepository _reviewRepository;
		private readonly IMapper _mapper;
		private readonly IReviwerRepository _reviewerRepository;
		private readonly IPokemonRepository _pokemonRepository;

		public ReviewController(IReviewRepository reviewRepository, 
			IMapper mapper, IPokemonRepository pokemonRepository,
			IReviwerRepository reviwerRepository)
        {
			_reviewRepository = reviewRepository;
			_mapper = mapper;
			_reviewerRepository = reviwerRepository;
			_pokemonRepository = pokemonRepository;
		}

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
		public IActionResult GetReviews()
		{
			var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(reviews);
		}

		[HttpGet("{reviewId}")]
		[ProducesResponseType(200, Type = typeof(Review))] //makes api looks cleaner
		[ProducesResponseType(400)]
		public IActionResult GetReview(int reviewId)
		{
			if (!_reviewRepository.ReviewExists(reviewId))
				return NotFound();
			var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			return Ok(review);
		}

		[HttpGet("pokemon/{pokeId}")]
		[ProducesResponseType(200, Type = typeof(Review))] //makes api looks cleaner
		[ProducesResponseType(400)]
		public IActionResult GetReviewsForAPokemon(int pokeId)
		{
			var review = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfAPokemon(pokeId));
			if(!ModelState.IsValid)
				return BadRequest(ModelState);
			return Ok(review);
		}

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult CreateReview([FromQuery] int reviewerId,
			[FromQuery] int pokeId, [FromBody] ReviewDto reviewCreate)
		{
			if (reviewCreate == null)
				return BadRequest(ModelState);
			var reviews = _reviewRepository.GetReviews()
				.Where(c => c.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper())
				.FirstOrDefault();

			if (reviews != null)
			{
				ModelState.AddModelError("", "Review already exists");
				return StatusCode(422, ModelState);
			}

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var reviewMap = _mapper.Map<Review>(reviewCreate);
			reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokeId);
			reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);

			if (!_reviewRepository.CreateReview(reviewMap))
			{
				ModelState.AddModelError("", "Something went wrong while saving");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully created!");

		}

		[HttpPut("{reviewId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult UpadateReview(int reviewId, [FromBody] ReviewDto updatedReview)
		{
			if (updatedReview == null)
				return BadRequest(ModelState);

			if (reviewId != updatedReview.Id)
				return BadRequest(ModelState);

			if (!_reviewRepository.ReviewExists(reviewId))
				return NotFound();

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var reviewMap = _mapper.Map<Review>(updatedReview);

			if (!_reviewRepository.UpdateReview(reviewMap))
			{
				ModelState.AddModelError("", "Something went wrong while updating review");
				return StatusCode(500, ModelState);
			}
			return NoContent();
		}

		[HttpDelete("{reviewId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult DeleteReview(int reviewId)
		{
			//validation
			if (!_reviewRepository.ReviewExists(reviewId))
			{
				return NotFound();
			}
			var reviewToDelete = _reviewRepository.GetReview(reviewId);
			//remember to check tied to this or  not so as this doesnt mess out database
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (!_reviewRepository.DeleteReview(reviewToDelete))
			{
				ModelState.AddModelError("", "Something went wrong deleting review");
			}
			return NoContent();
		}
	}
}
