import { useEffect, useState } from "react";
import api from "../services/api";
import "./WatchlistPage.css";

const THUMB_BASE = "https://image.tmdb.org/t/p/w92";

function CheckIcon({ active }) {
  return active ? (
    <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor"
      strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
      <circle cx="12" cy="12" r="10" fill="rgba(70,211,105,0.15)" />
      <path d="m8 12 3 3 5-6" />
    </svg>
  ) : (
    <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor"
      strokeWidth="2" aria-hidden="true">
      <circle cx="12" cy="12" r="10" />
    </svg>
  );
}

function Chevron({ open }) {
  return (
    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor"
      strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"
      style={{ transform: open ? "rotate(180deg)" : "none", transition: "transform 200ms ease" }}
      aria-hidden="true">
      <path d="m6 9 6 6 6-6" />
    </svg>
  );
}

export default function WatchlistPage() {
  const [watchlists, setWatchlists] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [expandedId, setExpandedId] = useState(null);

  const [showForm, setShowForm] = useState(false);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [creating, setCreating] = useState(false);

  const loadWatchlists = async () => {
    setLoading(true);
    setError("");
    try {
      const { data } = await api.get("/watchlists");
      setWatchlists(data || []);
    } catch {
      setError("Could not load your watchlists.");
      setWatchlists([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadWatchlists();
  }, []);

  const handleCreate = async (e) => {
    e.preventDefault();
    if (!name.trim()) return;
    setCreating(true);
    try {
      const { data } = await api.post("/watchlists", {
        name: name.trim(),
        description: description.trim(),
      });
      setWatchlists((prev) => [...prev, { ...data, items: data.items || [] }]);
      setName("");
      setDescription("");
      setShowForm(false);
    } catch {
      setError("Could not create the watchlist.");
    } finally {
      setCreating(false);
    }
  };

  const handleDeleteWatchlist = async (id) => {
    try {
      await api.delete(`/watchlists/${id}`);
      setWatchlists((prev) => prev.filter((w) => w.id !== id));
    } catch {
      setError("Could not delete the watchlist.");
    }
  };

  const handleToggleItem = async (watchlistId, item) => {
    try {
      await api.patch(`/watchlists/${watchlistId}/items/${item.id}/toggle`);
      setWatchlists((prev) =>
        prev.map((w) =>
          w.id !== watchlistId
            ? w
            : {
                ...w,
                items: w.items.map((i) =>
                  i.id === item.id ? { ...i, isWatched: !i.isWatched } : i
                ),
              }
        )
      );
    } catch {
      setError("Could not update the item.");
    }
  };

  const handleRemoveItem = async (watchlistId, itemId) => {
    try {
      await api.delete(`/watchlists/${watchlistId}/items/${itemId}`);
      setWatchlists((prev) =>
        prev.map((w) =>
          w.id !== watchlistId
            ? w
            : { ...w, items: w.items.filter((i) => i.id !== itemId) }
        )
      );
    } catch {
      setError("Could not remove the item.");
    }
  };

  return (
    <div className="page">
      <div className="container">
        <div className="watchlist-header">
          <h1 className="page-title" style={{ margin: 0 }}>
            My Watchlists
          </h1>
          <button
            className="btn btn-primary"
            onClick={() => setShowForm((s) => !s)}
          >
            {showForm ? "Cancel" : "Create New Watchlist"}
          </button>
        </div>

        {showForm && (
          <form className="watchlist-create" onSubmit={handleCreate}>
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="Watchlist name"
              aria-label="Watchlist name"
              required
            />
            <input
              type="text"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Description (optional)"
              aria-label="Watchlist description"
            />
            <button type="submit" className="btn btn-primary" disabled={creating}>
              {creating ? "Creating…" : "Create"}
            </button>
          </form>
        )}

        {error && <p className="error-text" style={{ marginBottom: "1rem" }}>{error}</p>}

        {loading ? (
          <div className="state-block">
            <div className="spinner" />
            <p>Loading…</p>
          </div>
        ) : watchlists.length === 0 ? (
          <div className="state-block">
            <p>You don't have any watchlists yet.</p>
            <p style={{ color: "var(--text-muted)" }}>
              Create one above to start tracking what you want to watch.
            </p>
          </div>
        ) : (
          <div className="watchlist-list">
            {watchlists.map((w) => {
              const open = expandedId === w.id;
              return (
                <div className="watchlist-card" key={w.id}>
                  <div className="watchlist-card-header">
                    <button
                      className="watchlist-toggle"
                      onClick={() => setExpandedId(open ? null : w.id)}
                      aria-expanded={open}
                    >
                      <Chevron open={open} />
                      <span className="watchlist-name">{w.name}</span>
                      <span className="watchlist-count">
                        {w.items?.length || 0} item
                        {(w.items?.length || 0) === 1 ? "" : "s"}
                      </span>
                    </button>
                    <button
                      className="btn btn-outline watchlist-delete"
                      onClick={() => handleDeleteWatchlist(w.id)}
                    >
                      Delete
                    </button>
                  </div>

                  {w.description && (
                    <p className="watchlist-desc">{w.description}</p>
                  )}

                  {open && (
                    <div className="watchlist-items">
                      {(!w.items || w.items.length === 0) ? (
                        <p className="watchlist-empty-items">
                          No titles in this watchlist yet.
                        </p>
                      ) : (
                        w.items.map((item) => (
                          <div className="watchlist-item" key={item.id}>
                            <div className="watchlist-item-thumb">
                              {item.posterPath ? (
                                <img
                                  src={`${THUMB_BASE}${item.posterPath}`}
                                  alt={`${item.title} poster`}
                                  loading="lazy"
                                />
                              ) : (
                                <div className="watchlist-thumb-fallback" />
                              )}
                            </div>
                            <span className="watchlist-item-title">{item.title}</span>
                            <button
                              className={`watchlist-watched ${item.isWatched ? "is-watched" : ""}`}
                              onClick={() => handleToggleItem(w.id, item)}
                              title={item.isWatched ? "Watched" : "Mark as watched"}
                              aria-label={item.isWatched ? "Watched" : "Mark as watched"}
                            >
                              <CheckIcon active={item.isWatched} />
                            </button>
                            <button
                              className="btn btn-outline watchlist-item-remove"
                              onClick={() => handleRemoveItem(w.id, item.id)}
                            >
                              Remove
                            </button>
                          </div>
                        ))
                      )}
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
}
