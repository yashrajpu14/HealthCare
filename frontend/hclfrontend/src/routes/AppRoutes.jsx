import React from "react";
import { Routes, Route, Navigate } from "react-router-dom";
import RequireAuth from "../auth/RequireAuth";
import PublicOnly from "../auth/PublicOnly";

import Login from "../pages/LoginModule/Login";
import SignUp from "../pages/LoginModule/SignUp";
import Dashboard from "../pages/DashboardModule/Dashboard";
import Profile from "../pages/ProfileModule/Profile";
import ChangePassword from "../pages/ProfileModule/ChangePassword";
import Home from "../pages/HomeModule/Home";

function AppRoutes() {
  return (
    <Routes>
      {/* Public */}
      <Route element={<PublicOnly />}>
        <Route path="/login" element={<Login />} />
        <Route path="/signup" element={<SignUp />} />
      </Route>

      {/* Protected */}
      <Route element={<RequireAuth />}>
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/profile" element={<Profile />} />
        <Route path="/change-password" element={<ChangePassword />} />
      </Route>

      {/* Home and catch-all */}
      <Route path="/" element={<Home />} />
      <Route path="*" element={<Home />} />
    </Routes>
  );
}

export default AppRoutes;
