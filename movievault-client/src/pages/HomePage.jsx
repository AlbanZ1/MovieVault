import { useCallback, useEffect, useState } from "react";
import api from "../services/api";
import MovieCard from "../components/MovieCard";
import "./HomePage.css";

function SearchIcon() {
  return (
    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor"
      strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
      <circle cx="11" cy="11" r="8" />
      <path d="m21 21-4.3-4.3" />
    </svg>
  );
}

export default function HomePage() {
  const [input, setInput] = useState("");
  const [activeQuery, setActiveQuery] = useState("");
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [toast, setToast] = useState(null);

  const showToast = (message, type = "success") => {
    setToast({ message, type });
    setTimeout(() => setToast(null), 3000);
  };

  const loadPopular = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const { data } = await api.get("/movies/popular", {
        params: { mediaType: "movie", page: 1 },
      });
      setResults(data.results || []);
    } catch {
      setError("Could not load popular movies. Is the API running?");
      setResults([]);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadPopular();
  }, [loadPopular]);

  const handleSearch = async (e) => {
    e.preventDefault();
    const query = input.trim();
    if (!query) {
      setActiveQuery("");
      loadPopular();
      return;
    }

    setLoading(true);
    setError("");
    setActiveQuery(query);
    try {
      const { data } = await api.get("/movies/search", {
        params: { query, page: 1, mediaType: "multi" },
      });
      setResults(data.results || []);
    } catch (err) {
      if (err.response?.status === 404) {
        setResults([]);
      } else {
        setError("Search failed. Please try again.");
        setResults([]);
      }
    } finally {
      setLoading(false);
    }
  };

  const handleAddFavorite = async (movie) => {
    try {
      await api.post("/favorites", {
        tmdbId: movie.tmdbId ?? movie.id,
        title: movie.title || movie.name || "Untitled",
        posterPath: movie.posterPath || "",
        mediaType: movie.mediaType || "movie",
      });
      showToast("Added to favorites");
    } catch (err) {
      showToast(
        err.response?.data?.message || "Could not add to favorites",
        "error"
      );
    }
  };

  return (
    <div className="page">
      <div className="container">
        <form className="search-bar" onSubmit={handleSearch}>
          <div className="search-input-wrap">
            <span className="search-icon">
              <SearchIcon />
            </span>
            <input
              type="text"
              value={input}
              onChange={(e) => setInput(e.target.value)}
              placeholder="Search movies and TV shows…"
              aria-label="Search movies and TV shows"
            />
          </div>
          <button type="submit" className="btn btn-primary search-btn">
            Search
          </button>
        </form>

        <h2 className="section-title">
          {activeQuery ? `Search Results for: ${activeQuery}` : "Popular Movies"}
        </h2>

        {loading ? (
          <div className="state-block">
            <div className="spinner" />
            <p>Loading…</p>
          </div>
        ) : error ? (
          <div className="state-block">
            <p className="error-text">{error}</p>
          </div>
        ) : results.length === 0 ? (
          <div className="state-block">
            <p>No results found{activeQuery ? ` for "${activeQuery}"` : ""}.</p>
          </div>
        ) : (
          <div className="movie-grid">
            {results.map((movie) => (
              <MovieCard
                key={movie.id ?? movie.tmdbId}
                movie={movie}
                onAddFavorite={handleAddFavorite}
              />
            ))}
          </div>
        )}
      </div>

      {toast && <div className={`toast toast-${toast.type}`}>{toast.message}</div>}
    </div>
  );
}
