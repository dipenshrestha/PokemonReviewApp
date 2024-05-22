namespace PokemonReviewApp.Models
{
	public class Pokemon
	{
		//model is just a Column in the Database table named Pokemon
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime BirthDate { get; set; }
		//one to many relationship ko many tala ko
		public ICollection<Review> Reviews { get; set;}

        //many to many relationship 
        public ICollection<PokemonOwner> PokemonOwners { get; set; }
        public ICollection<PokemonCategory> PokemonCategories { get; set; }
    }
}
