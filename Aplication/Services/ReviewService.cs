using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aplication.DTOs.Review;
using Aplication.DTOs;
using Aplication.Interfaces;
using Domain.Entity;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Domain.Enums;


namespace Aplication.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReviewService(
            IReviewRepository reviewRepository,
            IReservationRepository reservationRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _reviewRepository = reviewRepository;
            _reservationRepository = reservationRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddReview(int reservationId, ReviewCreateDTO dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedDomainException("Unauthorized");

            var reservation = await _reservationRepository.GetValue(reservationId);

            if (reservation == null)
                throw new ResourceNotFoundException("Reservation not found");

            if (reservation.IdGuest != userId)
                throw new UnauthorizedDomainException("This reservation does not belong to you");

            if (reservation.Status != ReservationStatus.Completed)
                throw new BusinessRuleException("You can only leave a review for a completed reservation.");



            if (await _reviewRepository.Exists(reservation.IdProperty, userId))
                throw new BusinessRuleException("You already reviewed this property.");

            var review = new Review
            {
                IdProperty = reservation.IdProperty,
                IdGuest = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                ReviewDate = DateTime.Now
            };

            await _reviewRepository.Add(review);
        }

        public async Task<PropertyRatingVM> GetPropertyRating(int propertyId)
        {
            var reviews = await _reviewRepository.GetByProperty(propertyId);

            if (!reviews.Any())
            {
                return new PropertyRatingVM
                {
                    PropertyId = propertyId,
                    AverageRating = 0,
                    TotalReviews = 0
                };
            }

            return new PropertyRatingVM
            {
                PropertyId = propertyId,
                AverageRating = Math.Round(reviews.Average(r => r.Rating), 2),
                TotalReviews = reviews.Count
            };
        }
    }
}
