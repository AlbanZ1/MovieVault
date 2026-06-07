import { useNavigate } from "react-router-dom";
import "./MovieCard.css";

const IMG_BASE = "https://image.tmdb.org/t/p/w300";

function StarIcon() {
  return (
    <svg width="14" height="14" viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
      <path d="M12 17.27 18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z" />
    </svg>
  );
}

export default function MovieCard({ movie, onAddFavorite, onRemoveFavorite }) {
  const navigate = useNavigate();

  const tmdbId = movie.tmdbId ?? movie.id;
  const title = movie.title || movie.name || "Untitled";
  const mediaType = movie.mediaType || "movie";
  const poster = movie.posterPath ? `${IMG_BASE}${movie.posterPath}` : null;
  const year = movie.releaseDate ? String(movie.releaseDate).slice(0, 4) : null;
  const rating =
    typeof movie.voteAverage === "number" && movie.voteAverage > 0
      ? movie.voteAverage.toFixed(1)
      : null;

  const goToDetail = () =>
    navigate(`/movie/${tmdbId}`, { state: { mediaType } });

  return (
    <article className="movie-card">
      <div className="movie-poster" onClick={goToDetail} role="button" tabIndex={0}
        onKeyDown={(e) => e.key === "Enter" && goToDetail()}>
        {poster ? (
          <img src={poster} alt={`${title} poster`} loading="lazy" />
        ) : (
          <div className="movie-poster-fallback">
            <span>{title}</span>
          </div>
        )}
        {rating && (
          <div className="movie-rating-badge">
            <StarIcon />
            {rating}
          </div>
        )}
      </div>

      <div className="movie-info">
        <h3 className="movie-title" title={title}>
          {title}
        </h3>
        {year && <span className="movie-year">{year}</span>}
      </div>

      <div className="movie-actions">
        <button className="btn btn-primary movie-action-btn" onClick={goToDetail}>
          View Details
        </button>
        {onAddFavorite && (
          <button
            className="btn btn-outline movie-action-btn"
            onClick={() => onAddFavorite(movie)}
          >
            Add to Favorites
          </button>
        )}
        {onRemoveFavorite && (
          <button
            className="btn btn-outline movie-action-btn movie-action-danger"
            onClick={() => onRemoveFavorite(movie)}
          >
            Remove
          </button>
        )}
      </div>
    </article>
  );
}
