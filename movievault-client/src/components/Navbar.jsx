import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import "./Navbar.css";

export default function Navbar() {
  const { user, isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  // The navbar is only part of the authenticated experience.
  if (!isAuthenticated) return null;

  return (
    <nav className="navbar">
      <div className="navbar-inner container">
        <NavLink to="/home" className="navbar-logo">
          MOVIE<span>VAULT</span>
        </NavLink>

        <div className="navbar-links">
          <NavLink to="/home" className="nav-link">
            Home
          </NavLink>
          <NavLink to="/watchlists" className="nav-link">
            Watchlists
          </NavLink>
          <NavLink to="/favorites" className="nav-link">
            Favorites
          </NavLink>
        </div>

        <div className="navbar-user">
          <span className="navbar-username">{user?.username}</span>
          <button className="btn btn-outline navbar-logout" onClick={handleLogout}>
            Logout
          </button>
        </div>
      </div>
    </nav>
  );
}
