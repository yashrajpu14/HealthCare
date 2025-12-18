import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Devices from "./pages/Devices";
import Admin from "./pages/Admin";
import { AuthProvider, useAuth } from "./auth/AuthContext";
import { ProtectedRoute } from "./auth/ProtectedRoute";
import { RoleRoute } from "./auth/RoleRoute";
import ProfilePage from "./pages/Profile";

function Home() {
  const a = useAuth();
  return (
    <div className="min-h-screen p-6 bg-slate-50">
      <div className="max-w-2xl mx-auto bg-white rounded-2xl shadow p-6">
        <h1 className="text-2xl font-bold">Home</h1>
        <p className="text-slate-600 mt-2">Logged in: {String(a.isAuthed)} • Role: {a.role}</p>

        <div className="mt-4 flex gap-3">
          <Link className="px-4 py-2 rounded-xl bg-slate-900 text-white" to="/devices">Devices</Link>
          {a.role === "Admin" && (
            <Link className="px-4 py-2 rounded-xl bg-blue-600 text-white" to="/admin">Admin</Link>
          )}
        </div>
      </div>
    </div>
  );
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<ProtectedRoute><Home /></ProtectedRoute>} />
          <Route path="/devices" element={<ProtectedRoute><Devices /></ProtectedRoute>} />
          <Route path="/admin" element={<RoleRoute role="Admin"><Admin /></RoleRoute>} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/profile" element={<ProfilePage />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
