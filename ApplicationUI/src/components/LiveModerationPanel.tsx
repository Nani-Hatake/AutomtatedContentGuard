import { motion } from 'framer-motion';
import { ArrowRight, Loader2, ShieldAlert, ShieldCheck } from 'lucide-react';
import { useMemo, useState } from 'react';
import { postSubmission } from '../services/api';
import { Badge } from './Badge';
import type { ContentSubmission } from '../types';

const scoreTone = (score: number) => {
  if (score >= 7) return 'danger';
  if (score >= 4) return 'warning';
  return 'success';
};

const scoreColor = (score: number) => {
  if (score >= 7) return 'from-rose-500 via-amber-500 to-amber-300';
  if (score >= 4) return 'from-amber-400 via-amber-300 to-emerald-300';
  return 'from-emerald-400 via-cyan-300 to-sky-300';
};

export function LiveModerationPanel({ onResult }: { onResult: (result: ContentSubmission) => void }) {
  const [text, setText] = useState('');
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<ContentSubmission | null>(null);
  const [error, setError] = useState<string | null>(null);

  const matchedScore = result?.toxicityScore ?? 0;
  const isFlagged = result?.isFlagged ?? false;

  const statusLabel = useMemo(() => {
    if (!result) return 'Idle';
    return isFlagged ? 'Flagged' : 'Approved';
  }, [result, isFlagged]);

  const statusTone = useMemo(() => {
    if (!result) return 'neutral';
    return isFlagged ? 'danger' : 'success';
  }, [result, isFlagged]);

  const handleSubmit = async () => {
    if (!text.trim()) {
      setError('Enter text to analyze.');
      return;
    }

    setError(null);
    setLoading(true);

    try {
      const submission = await postSubmission(text.trim());
      setResult(submission);
      onResult(submission);
    } catch (err) {
      setError('Unable to analyze content. Check your backend and CORS settings.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <motion.section initial={{ opacity: 0, y: 24 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.5 }} className="space-y-6 rounded-[2rem] border border-white/10 bg-slate-950/80 p-6 shadow-glow backdrop-blur-xl">
      <div className="flex items-start justify-between gap-4">
        <div>
          <p className="text-sm uppercase tracking-[0.28em] text-slate-500">Live moderation sandbox</p>
          <h2 className="mt-2 text-2xl font-semibold text-slate-100">Analyze text instantly</h2>
          <p className="mt-2 max-w-xl text-sm text-slate-400">Paste a paragraph and preview the AI moderation score in real time. Every submission is saved to your content history.</p>
        </div>
        <div className="rounded-3xl border border-cyan-500/20 bg-cyan-500/5 px-4 py-3 text-sm text-cyan-200">
          <div className="flex items-center gap-2 font-semibold text-cyan-100">
            <ArrowRight className="h-4 w-4" /> Live review
          </div>
        </div>
      </div>

      <div className="grid gap-6 xl:grid-cols-[1.4fr_0.8fr]">
        <div className="space-y-4 rounded-[1.75rem] border border-white/10 bg-slate-900/90 p-5 shadow-xl shadow-slate-950/20">
          <label className="block text-sm font-semibold text-slate-200">Enter text to analyze</label>
          <textarea
            className="min-h-[220px] w-full rounded-3xl border border-slate-800 bg-slate-950/90 px-4 py-4 text-sm text-slate-100 outline-none transition focus:border-cyan-400 focus:ring-2 focus:ring-cyan-400/20"
            value={text}
            onChange={(event) => setText(event.target.value)}
            placeholder="Type or paste a message here to evaluate its content safety..."
          />
          <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
            <button
              onClick={handleSubmit}
              disabled={loading}
              className="inline-flex items-center justify-center gap-2 rounded-3xl bg-cyan-500 px-5 py-3 text-sm font-semibold text-slate-950 transition hover:bg-cyan-400 disabled:cursor-not-allowed disabled:opacity-60"
            >
              {loading ? <Loader2 className="h-4 w-4 animate-spin" /> : 'Analyze Content'}
            </button>
            <p className="text-xs text-slate-500">Uses backend moderation API at <span className="font-semibold text-slate-200">/api/ContentSubmissions</span>.</p>
          </div>
          {error ? <p className="rounded-3xl bg-rose-500/10 px-4 py-3 text-sm text-rose-200">{error}</p> : null}
        </div>

        <div className="rounded-[1.75rem] border border-white/10 bg-slate-900/90 p-5 shadow-xl shadow-slate-950/20">
          <div className="flex items-center justify-between gap-3">
            <div>
              <p className="text-sm uppercase tracking-[0.3em] text-slate-500">Safety card</p>
              <h3 className="mt-2 text-lg font-semibold text-slate-100">Realtime verdict</h3>
            </div>
            <Badge label={statusLabel} tone={statusTone} icon={isFlagged ? <ShieldAlert className="h-4 w-4" /> : <ShieldCheck className="h-4 w-4" />} />
          </div>

          <div className="mt-6 space-y-6">
            <div className="space-y-4 rounded-3xl border border-slate-800 bg-slate-950/80 p-5">
              <div className="flex items-center justify-between gap-4">
                <div>
                  <p className="text-sm text-slate-400">Toxicity score</p>
                  <p className="mt-2 text-3xl font-semibold text-slate-100">{result ? matchedScore.toFixed(1) : '0.0'} / 10</p>
                </div>
                <div className="rounded-3xl bg-slate-900/70 p-3 text-slate-300">
                  {result ? <span className="text-sm uppercase tracking-[0.2em] text-slate-400">{result.status}</span> : <span className="text-sm uppercase tracking-[0.2em] text-slate-400">Awaiting</span>}
                </div>
              </div>
              <div className="space-y-3">
                <div className="h-3 overflow-hidden rounded-full bg-slate-800">
                  <motion.div
                    initial={{ width: 0 }}
                    animate={{ width: `${Math.min((matchedScore / 10) * 100, 100)}%` }}
                    transition={{ duration: 0.9, ease: 'easeOut' }}
                    className={`h-full rounded-full bg-gradient-to-r ${scoreColor(matchedScore)}`}
                  />
                </div>
                <div className="flex items-center justify-between text-xs text-slate-500">
                  <span>Safe</span>
                  <span>Critical</span>
                </div>
              </div>
            </div>

            <motion.div initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.55 }} className="rounded-3xl border border-slate-800 bg-slate-950/80 p-5">
              <p className="text-sm text-slate-400">Flagged reason</p>
              <p className="mt-3 min-h-[72px] text-base leading-7 text-slate-100">
                {result ? result.status === 'Flagged' ? `AI flagged category: ${result.status.toLowerCase()}` : 'Content passes moderation and can be safely published.' : 'Analyze a piece of text to see the moderation details appear here.'}
              </p>
            </motion.div>
          </div>
        </div>
      </div>
    </motion.section>
  );
}
