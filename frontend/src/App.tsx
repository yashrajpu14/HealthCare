import { useEffect, useState } from "react";
import { api } from "./lib/api";

export default function App() {
  const [status, setStatus] = useState<string>("loading...");

  useEffect(() => {
    api<{ status: string }>("/api/v1/health")
      .then((d) => setStatus(d.status))
      .catch(() => setStatus("backend not reachable"));
  }, []);

  return (
    <div style={{ padding: 24, fontFamily: "system-ui" }}>
      <h1>Hackathon Full-Stack Starter</h1>
      <p>Backend status: <b>{status}</b></p>
    </div>
  );
}
