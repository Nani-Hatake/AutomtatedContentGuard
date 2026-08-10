import { AnimatePresence, motion } from 'framer-motion';
import { useMemo } from 'react';
import { Badge } from './Badge';
import type { ContentSubmission, SubmissionStatus } from '../types';

interface SubmissionTableProps {
  submissions: ContentSubmission[];
  activeFilter: SubmissionStatus | 'All';
  onFilterChange: (value: SubmissionStatus | 'All') => void;
}

const statusTone = (status: SubmissionStatus) => {
  return status === 'Approved' ? 'success' : 'danger';
};

export function SubmissionTable({ submissions, activeFilter, onFilterChange }: SubmissionTableProps) {
  const filtered = useMemo(() => {
    if (activeFilter === 'All') return submissions;
    return submissions.filter((item) => item.status === activeFilter);
  }, [submissions, activeFilter]);

  return (
    <motion.section initial={{ opacity: 0, y: 24 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.5, delay: 0.1 }} className="rounded-[2rem] border border-white/10 bg-slate-950/80 p-6 shadow-glow backdrop-blur-xl">
      <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
        <div>
          <p className="text-sm uppercase tracking-[0.3em] text-slate-500">Content submissions</p>
          <h2 className="mt-2 text-2xl font-semibold text-slate-100">Submission history</h2>
        </div>
        <div className="flex flex-wrap items-center gap-3">
          {(['All', 'Approved', 'Flagged'] as const).map((status) => (
            <button
              key={status}
              type="button"
              onClick={() => onFilterChange(status)}
              className={`rounded-full border px-4 py-2 text-sm font-semibold transition ${activeFilter === status ? 'border-cyan-400 bg-cyan-500/10 text-cyan-100' : 'border-white/10 bg-slate-900/70 text-slate-300 hover:border-cyan-400 hover:text-cyan-100'}`}
            >
              {status}
            </button>
          ))}
        </div>
      </div>

      <div className="mt-6 overflow-hidden rounded-3xl border border-slate-800 bg-slate-900/90 shadow-inner shadow-slate-950/20">
        <div className="grid gap-4 p-5 text-sm text-slate-400 sm:grid-cols-[1.2fr_1fr] md:grid-cols-[1.5fr_0.9fr_0.8fr] xl:grid-cols-[1.6fr_0.9fr_0.9fr_0.8fr]">
          <span className="font-semibold text-slate-200">ID</span>
          <span className="font-semibold text-slate-200">Content</span>
          <span className="font-semibold text-slate-200">Score</span>
          <span className="font-semibold text-slate-200">Submitted</span>
        </div>
        <div className="space-y-1 border-t border-white/10 px-4 pb-4 pt-3">
          <AnimatePresence>
            {filtered.map((submission) => (
              <motion.div
                layout
                key={submission.id}
                initial={{ opacity: 0, y: 12 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: -12 }}
                transition={{ duration: 0.25 }}
                className="grid gap-4 rounded-3xl border border-slate-800 bg-slate-950/85 p-4 text-sm text-slate-300 shadow-sm sm:grid-cols-[1.2fr_1fr] md:grid-cols-[1.5fr_0.9fr_0.8fr] xl:grid-cols-[1.6fr_0.9fr_0.9fr_0.8fr]"
              >
                <div className="font-semibold text-cyan-100">#{submission.id}</div>
                <div className="truncate text-slate-300">{submission.textContent}</div>
                <div className="flex items-center gap-3">
                  <span className="font-semibold text-slate-100">{submission.toxicityScore.toFixed(1)}</span>
                  <Badge label={submission.status} tone={statusTone(submission.status)} />
                </div>
                <div className="text-slate-500">{new Date(submission.submittedAt).toLocaleString()}</div>
              </motion.div>
            ))}
          </AnimatePresence>
          {filtered.length === 0 ? (
            <div className="rounded-3xl border border-dashed border-slate-700 bg-slate-950/80 p-8 text-center text-slate-500">
              No submissions match this filter yet.
            </div>
          ) : null}
        </div>
      </div>
    </motion.section>
  );
}
