import { useEffect, useState } from "react";
import { api } from "../api/client";

type Profile = { name: string; email: string; phone?: string | null };

export default function ProfilePage() {
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [err, setErr] = useState<string | null>(null);
  const [ok, setOk] = useState<string | null>(null);

  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");

  const load = async () => {
    setErr(null);
    setOk(null);
    setLoading(true);
    try {
      const res = await api.get<Profile>("/api/users/me");
      setName(res.data.name);
      setEmail(res.data.email);
      setPhone(res.data.phone ?? "");
    } catch (e: any) {
      setErr(e?.response?.data || e.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const save = async () => {
    setErr(null);
    setOk(null);
    setSaving(true);
    try {
      const res = await api.put<Profile>("/api/users/me", {
        name,
        email,
        phone: phone.trim() ? phone.trim() : null,
      });
      setOk("Profile updated successfully.");
      setName(res.data.name);
      setEmail(res.data.email);
      setPhone(res.data.phone ?? "");
    } catch (e: any) {
      setErr(e?.response?.data || e.message);
    } finally {
      setSaving(false);
    }
  };

  const logout = () => {
    localStorage.removeItem("access_token");
    window.location.href = "/login";
  };

  return (
    <div className="min-h-screen bg-slate-100 p-4 flex items-center justify-center">
      <div className="w-full max-w-lg bg-white rounded-2xl shadow p-6">
        <div className="flex items-center justify-between">
          <h1 className="text-2xl font-bold">My Profile</h1>
          <button onClick={logout} className="text-sm font-semibold text-red-600">
            Logout
          </button>
        </div>

        {loading ? (
          <div className="mt-6 text-slate-600">Loading...</div>
        ) : (
          <>
            {err && <div className="mt-4 text-sm text-red-600">{String(err)}</div>}
            {ok && <div className="mt-4 text-sm text-green-700">{ok}</div>}

            <div className="mt-6 space-y-3">
              <div>
                <label className="text-sm font-medium text-slate-700">Name</label>
                <input
                  className="w-full border rounded-xl p-3 mt-1"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  placeholder="Full name"
                />
              </div>

              <div>
                <label className="text-sm font-medium text-slate-700">Email</label>
                <input
                  className="w-full border rounded-xl p-3 mt-1"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="Email"
                />
              </div>

              <div>
                <label className="text-sm font-medium text-slate-700">Contact Number</label>
                <input
                  className="w-full border rounded-xl p-3 mt-1"
                  value={phone}
                  onChange={(e) => setPhone(e.target.value)}
                  placeholder="+91xxxxxxxxxx"
                />
              </div>

              <div className="flex gap-3 pt-2">
                <button
                  onClick={save}
                  disabled={saving}
                  className="flex-1 bg-blue-600 text-white rounded-xl py-3 font-semibold disabled:opacity-60"
                >
                  {saving ? "Saving..." : "Update Profile"}
                </button>
                <button
                  onClick={load}
                  disabled={saving}
                  className="px-4 border rounded-xl font-semibold disabled:opacity-60"
                >
                  Refresh
                </button>
              </div>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
