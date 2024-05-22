using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Models;
using System.Reflection.Metadata;

namespace PokemonReviewApp.Data
{
	public class DataContext:DbContext //inherit the Dbcontext by installing the entity framework
	{
		/*The constructor accepts a parameter of type DbContextOptions<DataContext> and 
		passes it to the base DbContext constructor, allowing the DataContext to be 
		configured with specific database options such as the provider and connection string*/
		public DataContext(DbContextOptions<DataContext> options) :base(options)
		{ 
		}
		//Tells the Context what our tables are by calling all the models
		public DbSet<Category> Categories { get; set; }
		public DbSet<Country> Countries { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Pokemon> Pokemon { get; set; }
        public DbSet<PokemonCategory> PokemonCategories { get; set; }
        public DbSet<PokemonOwner> PokemonOwners { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }

		//for many to many table
		//to manipulate certain table in certain way
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<PokemonCategory>()
				.HasKey(pc => new { pc.PokemonId, pc.CategoryId });
			modelBuilder.Entity<PokemonCategory>()
				.HasOne(p => p.Pokemon)
				.WithMany(pc => pc.PokemonCategories)
				.HasForeignKey(c => c.PokemonId);
			modelBuilder.Entity<PokemonCategory>()
				.HasOne(p => p.Category)
				.WithMany(pc => pc.PokemonCategories)
				.HasForeignKey(c => c.CategoryId);

			modelBuilder.Entity<PokemonOwner>()
				.HasKey(po => new { po.PokemonId, po.OwnerId });
			modelBuilder.Entity<PokemonOwner>()
				.HasOne(p => p.Pokemon)
				.WithMany(pc => pc.PokemonOwners)
				.HasForeignKey(c => c.PokemonId);
			modelBuilder.Entity<PokemonOwner>()
				.HasOne(p => p.Owner)
				.WithMany(pc => pc.PokemonOwners)
				.HasForeignKey(c => c.OwnerId);
		}
	}
}
