using Aplication.DTOs.Review;
using Aplication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces
{
    public interface IReviewService
    {
        Task AddReview(int reservationId, ReviewCreateDTO dto);
        Task<PropertyRatingVM> GetPropertyRating(int propertyId);
        Task<IEnumerable<ReviewVM>> GetPropertyReviews(int propertyId);
    }
}
