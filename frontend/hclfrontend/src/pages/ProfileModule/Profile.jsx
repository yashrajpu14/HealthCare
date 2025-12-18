import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "./Profile.css";
import { ArrowLeft, Mail, Phone } from "lucide-react";
import { notify } from "../../utils/toast";
import { userService } from "../../user/user.service";

const Profile = () => {
  const navigate = useNavigate();
  const [isEditing, setIsEditing] = useState(false);
  const [loading, setLoading] = useState(true);

  const [profileData, setProfileData] = useState({
    name: "",
    email: "",
    phone: "",
    role: "",
  });

  const [original, setOriginal] = useState(null);

  useEffect(() => {
    (async () => {
      try {
        setLoading(true);
        const me = await userService.getMe();
        const normalized = {
          name: me?.name ?? "",
          email: me?.email ?? "",
          phone: me?.phone ?? "",
          role: me?.role ?? "",
        };
        setProfileData(normalized);
        setOriginal(normalized);
      } catch (err) {
        const msg = err?.response?.data?.message || err?.response?.data || "Failed to load profile";
        notify(msg, "error");
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setProfileData((p) => ({ ...p, [name]: value }));
  };

  const handleSave = async () => {
    try {
      const updated = await userService.updateMe(profileData);
      const normalized = {
        name: updated?.name ?? profileData.name,
        email: updated?.email ?? profileData.email,
        phone: updated?.phone ?? profileData.phone,
        role: updated?.role ?? profileData.role,
      };
      setProfileData(normalized);
      setOriginal(normalized);
      setIsEditing(false);
      notify("Profile updated successfully", "success");
    } catch (err) {
      if (err?.response?.status === 409) {
        notify("Email already in use", "error");
        return;
      }
      const msg = err?.response?.data?.message || err?.response?.data || "Failed to update profile";
      notify(msg, "error");
    }
  };

  const handleCancel = () => {
    if (original) setProfileData(original);
    setIsEditing(false);
  };

  if (loading) {
    return (
      <div className="profile-container">
        <header className="profile-header">
          <button className="back-btn" onClick={() => navigate("/dashboard")}>
            <ArrowLeft size={24} />
          </button>
          <h1>My Profile</h1>
          <div className="header-spacer"></div>
        </header>
        <main className="profile-main">
          <div className="profile-card">
            <p>Loading...</p>
          </div>
        </main>
      </div>
    );
  }

  return (
    <div className="profile-container">
      <header className="profile-header">
        <button className="back-btn" onClick={() => navigate("/dashboard")}>
          <ArrowLeft size={24} />
        </button>
        <h1>My Profile</h1>
        <div className="header-spacer"></div>
      </header>

      <main className="profile-main">
        <div className="profile-card">
          <div className="avatar-section">
            <img
              src="https://via.placeholder.com/120/667eea/ffffff?text=Profile"
              alt="Profile"
              className="large-avatar"
            />
            <h2>{profileData.name || "User"}</h2>
            <p className="profile-status">{profileData.role || "User"}</p>
          </div>

          <div className="profile-info">
            <div className="info-section">
              <h3>Account Information</h3>
              <div className="info-grid">
                <div className="info-field">
                  <label>Full Name</label>
                  {isEditing ? (
                    <input name="name" value={profileData.name} onChange={handleInputChange} />
                  ) : (
                    <p>{profileData.name}</p>
                  )}
                </div>

                <div className="info-field">
                  <label><Mail size={16} /> Email</label>
                  {isEditing ? (
                    <input name="email" type="email" value={profileData.email} onChange={handleInputChange} />
                  ) : (
                    <p>{profileData.email}</p>
                  )}
                </div>

                <div className="info-field">
                  <label><Phone size={16} /> Phone</label>
                  {isEditing ? (
                    <input name="phone" value={profileData.phone} onChange={handleInputChange} placeholder="Optional" />
                  ) : (
                    <p>{profileData.phone || "-"}</p>
                  )}
                </div>

                <div className="info-field">
                  <label>Role</label>
                  <p>{profileData.role || "User"}</p>
                </div>
              </div>
            </div>
          </div>

          <div className="action-buttons">
            {!isEditing ? (
              <button className="btn btn-primary" onClick={() => setIsEditing(true)}>
                Edit Profile
              </button>
            ) : (
              <>
                <button className="btn btn-primary" onClick={handleSave}>
                  Save Changes
                </button>
                <button className="btn btn-secondary" onClick={handleCancel}>
                  Cancel
                </button>
              </>
            )}
          </div>
        </div>
      </main>
    </div>
  );
};

export default Profile;
