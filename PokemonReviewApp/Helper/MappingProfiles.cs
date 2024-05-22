using AutoMapper;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;

//to remove the null values in api or database
//remember to use automapper from nugget package manager
namespace PokemonReviewApp.Helper
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles() 
		{
			//this is done so that the mapping profile works for pokemon, categories etc
			CreateMap<Pokemon, PokemonDto>();
			CreateMap<PokemonDto, Pokemon>();
			CreateMap<Category, CategoryDto>();
			CreateMap<CategoryDto, Category>();
			CreateMap<Country, CountryDto>();
			CreateMap<CountryDto, Country>();
			CreateMap<Owner, OwnerDto>();
			CreateMap<OwnerDto, Owner>();
			CreateMap<Review, ReviewDto>();
			CreateMap<ReviewDto, Review>();
			CreateMap<Reviewer, ReviewerDto>();
			CreateMap<ReviewerDto, Reviewer>();	
		}
	}
}
