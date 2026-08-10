import { useEffect, useState } from 'react';
import DashboardShell from './components/DashboardShell';
import { ForbiddenWordsPanel } from './components/ForbiddenWordsPanel';
import { LiveModerationPanel } from './components/LiveModerationPanel';
import { SubmissionTable } from './components/SubmissionTable';
import { fetchForbiddenWords, fetchSubmissions } from './services/api';
import type { ContentSubmission, ForbiddenWord, SubmissionStatus } from './types';

function App() {
  const [submissions, setSubmissions] = useState<ContentSubmission[]>([]);
  const [forbiddenWords, setForbiddenWords] = useState<ForbiddenWord[]>([]);
  const [activeFilter, setActiveFilter] = useState<SubmissionStatus | 'All'>('All');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [demoMode, setDemoMode] = useState(false);

  const demoSubmissions: ContentSubmission[] = [
    {
      id: 101,
      textContent: 'This is a test message to demonstrate the moderation dashboard.',
      toxicityScore: 1.2,
      status: 'Approved',
      isFlagged: false,
      submittedAt: new Date().toISOString(),
    },
    {
      id: 102,
      textContent: 'Some unsafe content with a word that would be flagged by the moderation pipeline.',
      toxicityScore: 7.8,
      status: 'Flagged',
      isFlagged: true,
      submittedAt: new Date(Date.now() - 1000 * 60 * 15).toISOString(),
    },
  ];

  const demoForbiddenWords: ForbiddenWord[] = [
    { id: 1, word: 'restricted' },
    { id: 2, word: 'forbidden' },
  ];

  const loadData = async () => {
    setLoading(true);
    setError(null);
    setDemoMode(false);

    try {
      const [submissionsData, forbiddenWordsData] = await Promise.all([
        fetchSubmissions(),
        fetchForbiddenWords(),
      ]);
      setSubmissions(submissionsData.sort((a, b) => b.id - a.id));
      setForbiddenWords(forbiddenWordsData);
    } catch (err) {
      const message =
        err instanceof Error
          ? err.message
          : 'Unable to reach the backend service. Please verify your connection.';
      setError(message);
      setSubmissions(demoSubmissions);
      setForbiddenWords(demoForbiddenWords);
      setDemoMode(true);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleNewSubmission = (result: ContentSubmission) => {
    setSubmissions((current) => [result, ...current]);
  };

  return (
    <DashboardShell>
      <div className="space-y-6">
        <LiveModerationPanel onResult={handleNewSubmission} />
        <SubmissionTable
          submissions={submissions}
          activeFilter={activeFilter}
          onFilterChange={setActiveFilter}
        />
      </div>
      <div className="space-y-6">
        <section className="rounded-[2rem] border border-white/10 bg-slate-950/80 p-6 shadow-glow backdrop-blur-xl">
          <div className="space-y-3">
            <p className="text-sm uppercase tracking-[0.3em] text-slate-500">Overview</p>
            <h2 className="text-2xl font-semibold text-slate-100">Project health</h2>
          </div>
          <div className="mt-6 grid gap-4 sm:grid-cols-2">
            <div className="rounded-3xl border border-slate-800 bg-slate-900/90 p-5">
              <p className="text-sm text-slate-400">Total submissions</p>
              <p className="mt-3 text-3xl font-semibold text-cyan-200">{submissions.length}</p>
            </div>
            <div className="rounded-3xl border border-slate-800 bg-slate-900/90 p-5">
              <p className="text-sm text-slate-400">Forbidden words</p>
              <p className="mt-3 text-3xl font-semibold text-emerald-200">
                {forbiddenWords.length}
              </p>
            </div>
          </div>
          {demoMode ? (
            <div className="mt-6 flex items-center justify-between rounded-3xl bg-amber-500/10 p-5 text-sm text-amber-200">
              <span>Backend unavailable — showing fallback demo data.</span>
              <button
                onClick={loadData}
                className="rounded-xl bg-amber-500/20 px-3 py-1.5 text-xs font-medium hover:bg-amber-500/30 transition-colors"
              >
                Retry
              </button>
            </div>
          ) : null}
          {loading ? (
            <div className="mt-6 rounded-3xl bg-slate-950/80 p-5 text-sm text-slate-400">
              Loading latest data…
            </div>
          ) : error ? (
            <div className="mt-6 rounded-3xl bg-rose-500/10 p-5 text-sm text-rose-200">
              {error}
            </div>
          ) : null}
        </section>
        <ForbiddenWordsPanel
          forbiddenWords={forbiddenWords}
          onWordsUpdated={setForbiddenWords}
        />
      </div>
    </DashboardShell>
  );
}

export default App;
