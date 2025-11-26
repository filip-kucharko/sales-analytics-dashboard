import Header from "./components/Header";
import Dashboard from "./components/Dashboard";

function App() {
  return (
    <div className="min-h-screen bg-slate-950">
      <Header />
      <main className="max-w-7xl mx-auto px-6 py-8">
        <Dashboard />
      </main>
    </div>
  );
}

export default App;
