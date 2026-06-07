import { useEffect, useState } from "react";
import api from "../services/api";
import MovieCard from "../components/MovieCard";

export default function FavoritesPage() {
  const [favorites, setFavorites] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [toast, setToast] = useState(null);

  const showToast = (message, type = "success") => {
    setToast({ message, type });
    setTimeout(() => setToast(null), 3000);
  };

  const loadFavorites = async () => {
    setLoading(true);
    setError("");
    try {
      const { data } = await api.get("/favorites", {
        params: { page: 1, pageSize: 20 },
      });
      setFavorites(data || []);
    } catch {
      setError("Could not load your favorites.");
      setFavorites([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadFavorites();
  }, []);

  const handleRemove = async (favorite) => {
    try {
      await api.delete(`/favorites/${favorite.id}`);
      setFavorites((prev) => prev.filter((f) => f.id !== favorite.id));
      showToast("Removed from favorites");
    } catch {
      showToast("Could not remove favorite", "error");
    }
  };

  return (
    <div className="page">
      <div className="container">
        <h1 className="page-title">My Favorites</h1>

        {loading ? (
          <div className="state-block">
            <div className="spinner" />
            <p>Loading…</p>
          </div>
        ) : error ? (
          <div className="state-block">
            <p className="error-text">{error}</p>
          </div>
        ) : favorites.length === 0 ? (
          <div className="state-block">
            <p>You haven't added any favorites yet.</p>
            <p style={{ color: "var(--text-muted)" }}>
              Browse movies on the Home page and tap “Add to Favorites”.
            </p>
          </div>
        ) : (
          <div className="movie-grid">
            {favorites.map((favorite) => (
              <MovieCard
                key={favorite.id}
                movie={favorite}
                onRemoveFavorite={handleRemove}
              />
            ))}
          </div>
        )}
      </div>

      {toast && <div className={`toast toast-${toast.type}`}>{toast.message}</div>}
    </div>
  );
}
