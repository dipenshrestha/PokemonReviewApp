namespace PokemonReviewApp.Models
{
	public class Category
	{
		public int Id { get; set; }
        public string Name { get; set; }
		//many to many relationship
        public ICollection<PokemonCategory> PokemonCategories { get; set; }
    }
}
