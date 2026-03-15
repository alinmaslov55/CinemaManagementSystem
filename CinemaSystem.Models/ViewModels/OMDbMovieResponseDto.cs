namespace CinemaSystem.Models.Dto
{
    public class OMDbMovieResponseDto
    {
        public string? imdbRating { get; set; }
        public List<OMDbRatingDto>? Ratings { get; set; }
    }

    public class OMDbRatingDto
    {
        public string? Source { get; set; }
        public string? Value { get; set; }
    }
}