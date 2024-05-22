using PokemonReviewApp.Models;
using System.Text.Json.Serialization.Metadata;

namespace PokemonReviewApp.Interfaces
{
	public interface IReviwerRepository
	{
		ICollection<Reviewer> GetReviewers();
		Reviewer GetReviewer(int reviewerId);
		ICollection<Review> GetReviewsByReviewer(int reviewerId);
		bool ReviewerExists(int reviewerId);
		bool CreateReviewer(Reviewer reviewer);
		bool UpdateReviewer(Reviewer reviewer);
		bool DeleteReviewer(Reviewer reviewer);
		bool Save();
	}
}
