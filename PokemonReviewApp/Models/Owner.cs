namespace PokemonReviewApp.Models
{
	public class Owner
	{
		//Id, name , gym is the column of database table Owner
        public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Gym { get; set; }
		public Country Country { get; set; } //one to many relationship ko one
		//many to many relationship
        public ICollection<PokemonOwner> PokemonOwners { get; set; }

    }
}
