import { useCallback, useEffect, useState } from "react";
import { useParams, useLocation } from "react-router-dom";
import api from "../services/api";
import { useAuth } from "../context/AuthContext";
import { genreName } from "../services/genres";
import "./MovieDetailPage.css";

const POSTER_BASE = "https://image.tmdb.org/t/p/w500";
const BACKDROP_BASE = "https://image.tmdb.org/t/p/w1280";

function Star({ filled }) {
  return (
    <svg width="20" height="20" viewBox="0 0 24 24"
      fill={filled ? "currentColor" : "none"} stroke="currentColor" strokeWidth="1.5"
      aria-hidden="true">
      <path d="M12 17.27 18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z" />
    </svg>
  );
}

function StarRating({ value }) {
  // value is on a 0–10 scale; render five stars.
  const filledCount = Math.round((value || 0) / 2);
  return (
    <span className="star-rating" aria-label={`${value?.toFixed(1) || 0} out of 10`}>
      {[1, 2, 3, 4, 5].map((i) => (
        <Star key={i} filled={i <= filledCount} />
      ))}
    </span>
  );
}

export default function MovieDetailPage() {
  const { tmdbId } = useParams();
  const location = useLocation();
  const { isAuthenticated } = useAuth();
  const mediaType = location.state?.mediaType === "tv" ? "tv" : "movie";

  const [movie, setMovie] = useState(null);
  const [average, setAverage] = useState(0);
  const [reviews, setReviews] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [toast, setToast] = useState(null);

  const [reviewContent, setReviewContent] = useState("");
  const [score, setScore] = useState(8);
  const [submittingReview, setSubmittingReview] = useState(false);
  const [submittingRating, setSubmittingRating] = useState(false);

  const showToast = (message, type = "success") => {
    setToast({ message, type });
    setTimeout(() => setToast(null), 3000);
  };

  const loadReviews = useCallback(async () => {
    try {
      const { data } = await api.get(`/reviews/movie/${tmdbId}`, {
        params: { page: 1, pageSize: 10 },
      });
      setReviews(data || []);
    } catch {
      setReviews([]);
    }
  }, [tmdbId]);

  const loadAverage = useCallback(async () => {
    try {
      const { data } = await api.get(`/ratings/movie/${tmdbId}/average`);
      setAverage(typeof data === "number" ? data : 0);
    } catch {
      setAverage(0);
    }
  }, [tmdbId]);

  useEffect(() => {
    let active = true;
    (async () => {
      setLoading(true);
      setError("");
      try {
        const { data } = await api.get(`/movies/${mediaType}/${tmdbId}`);
        if (active) setMovie(data);
      } catch {
        if (active) setError("Could not load this title.");
      } finally {
        if (active) setLoading(false);
      }
    })();
    loadAverage();
    loadReviews();
    return () => {
      active = false;
    };
  }, [tmdbId, mediaType, loadAverage, loadReviews]);

  const handleAddFavorite = async () => {
    try {
      await api.post("/favorites", {
        tmdbId: Number(tmdbId),
        title: movie?.title || "Untitled",
        posterPath: movie?.posterPath || "",
        mediaType,
      });
      showToast("Added to favorites");
    } catch (err) {
      showToast(err.response?.data?.message || "Could not add to favorites", "error");
    }
  };

  const handleSubmitReview = async (e) => {
    e.preventDefault();
    if (!reviewContent.trim()) return;
    setSubmittingReview(true);
    try {
      await api.post("/reviews", {
        tmdbId: Number(tmdbId),
        title: movie?.title || "Untitled",
        content: reviewContent.trim(),
      });
      setReviewContent("");
      showToast("Review posted");
      loadReviews();
    } catch (err) {
      showToast(err.response?.data?.message || "Could not post review", "error");
    } finally {
      setSubmittingReview(false);
    }
  };

  const handleSubmitRating = async (e) => {
    e.preventDefault();
    setSubmittingRating(true);
    try {
      await api.post("/ratings", {
        tmdbId: Number(tmdbId),
        title: movie?.title || "Untitled",
        score: Number(score),
      });
      showToast("Rating submitted");
      loadAverage();
    } catch (err) {
      showToast(err.response?.data?.message || "Could not submit rating", "error");
    } finally {
      setSubmittingRating(false);
    }
  };

  const formatDate = (value) =>
    value ? new Date(value).toLocaleDateString(undefined, {
      year: "numeric", month: "short", day: "numeric",
    }) : "";

  if (loading) {
    return (
      <div className="page">
        <div className="container state-block">
          <div className="spinner" />
          <p>Loading…</p>
        </div>
      </div>
    );
  }

  if (error || !movie) {
    return (
      <div className="page">
        <div className="container state-block">
          <p className="error-text">{error || "Title not found."}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="detail">
      {movie.backdropPath && (
        <div
          className="detail-backdrop"
          style={{ backgroundImage: `url(${BACKDROP_BASE}${movie.backdropPath})` }}
        >
          <div className="detail-backdrop-overlay" />
        </div>
      )}

      <div className="container detail-content">
        <div className="detail-main">
          <div className="detail-poster">
            {movie.posterPath ? (
              <img src={`${POSTER_BASE}${movie.posterPath}`} alt={`${movie.title} poster`} />
            ) : (
              <div className="detail-poster-fallback">{movie.title}</div>
            )}
          </div>

          <div className="detail-info">
            <h1 className="detail-title">{movie.title}</h1>

            <div className="detail-meta">
              {movie.releaseDate && <span>{formatDate(movie.releaseDate)}</span>}
              {movie.voteAverage > 0 && (
                <span className="detail-tmdb-score">
                  <Star filled /> {movie.voteAverage.toFixed(1)} TMDB
                </span>
              )}
            </div>

            {movie.genreIds?.length > 0 && (
              <div className="detail-genres">
                {movie.genreIds.map((id) => (
                  <span className="genre-chip" key={id}>
                    {genreName(id)}
                  </span>
                ))}
              </div>
            )}

            <div className="detail-user-rating">
              <span className="detail-rating-label">User Rating</span>
              <StarRating value={average} />
              <span className="detail-rating-value">
                {average > 0 ? `${average.toFixed(1)} / 10` : "Not rated yet"}
              </span>
            </div>

            {movie.overview && <p className="detail-overview">{movie.overview}</p>}

            {isAuthenticated && (
              <button className="btn btn-primary detail-fav-btn" onClick={handleAddFavorite}>
                Add to Favorites
              </button>
            )}
          </div>
        </div>

        {isAuthenticated && (
          <div className="detail-forms">
            <form className="detail-form-card" onSubmit={handleSubmitRating}>
              <h3 className="detail-form-title">Rate this title</h3>
              <div className="detail-rating-row">
                <select value={score} onChange={(e) => setScore(e.target.value)} aria-label="Score">
                  {Array.from({ length: 10 }, (_, i) => i + 1).map((n) => (
                    <option key={n} value={n}>
                      {n}
                    </option>
                  ))}
                </select>
                <button type="submit" className="btn btn-primary" disabled={submittingRating}>
                  {submittingRating ? "Saving…" : "Submit Rating"}
                </button>
              </div>
            </form>

            <form className="detail-form-card" onSubmit={handleSubmitReview}>
              <h3 className="detail-form-title">Write a review</h3>
              <textarea
                rows={4}
                value={reviewContent}
                onChange={(e) => setReviewContent(e.target.value)}
                placeholder="Share your thoughts…"
                aria-label="Review content"
              />
              <button
                type="submit"
                className="btn btn-primary"
                disabled={submittingReview}
                style={{ marginTop: "0.75rem" }}
              >
                {submittingReview ? "Posting…" : "Post Review"}
              </button>
            </form>
          </div>
        )}

        <div className="detail-reviews">
          <h2 className="section-title">Reviews</h2>
          {reviews.length === 0 ? (
            <p className="watchlist-empty-items">No reviews yet. Be the first!</p>
          ) : (
            <div className="review-list">
              {reviews.map((r) => (
                <div className="review-card" key={r.id}>
                  <div className="review-head">
                    <span className="review-author">
                      {r.username || `User ${String(r.userId).slice(0, 8)}`}
                    </span>
                    <span className="review-date">{formatDate(r.createdAt)}</span>
                  </div>
                  <p className="review-content">{r.content}</p>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>

      {toast && <div className={`toast toast-${toast.type}`}>{toast.message}</div>}
    </div>
  );
}
