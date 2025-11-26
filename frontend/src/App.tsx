import Header from "./components/Header";
import Dashboard from "./components/Dashboard";

function App() {
  return (
    <div className="min-h-screen bg-slate-950">
      <Header />
      <main className="max-w-[1600px] mx-auto px-4 py-4">
        <Dashboard />
      </main>
    </div>
  );
}

export default App;
