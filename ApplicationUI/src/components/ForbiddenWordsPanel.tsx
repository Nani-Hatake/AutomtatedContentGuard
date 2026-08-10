import { AnimatePresence, motion } from 'framer-motion';
import { Plus, Trash2 } from 'lucide-react';
import { useMemo, useState } from 'react';
import { addForbiddenWord, deleteForbiddenWord, fetchForbiddenWords } from '../services/api';
import type { ForbiddenWord } from '../types';

interface ForbiddenWordsPanelProps {
  forbiddenWords: ForbiddenWord[];
  onWordsUpdated: (words: ForbiddenWord[]) => void;
}

export function ForbiddenWordsPanel({ forbiddenWords, onWordsUpdated }: ForbiddenWordsPanelProps) {
  const [newWord, setNewWord] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const trimmedWord = useMemo(() => newWord.trim(), [newWord]);

  const handleAdd = async () => {
    if (!trimmedWord) {
      setError('Type a word before adding.');
      return;
    }

    setError(null);
    setLoading(true);

    try {
      const created = await addForbiddenWord(trimmedWord, 5);
      onWordsUpdated([...forbiddenWords, created]);
      setNewWord('');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Could not add forbidden word.');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    setLoading(true);
    setError(null);
    try {
      await deleteForbiddenWord(id);
      onWordsUpdated(forbiddenWords.filter((entry) => entry.id !== id));
    } catch (err) {
      setError('Unable to remove word.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <motion.section initial={{ opacity: 0, y: 24 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.5, delay: 0.15 }} className="rounded-[2rem] border border-white/10 bg-slate-950/80 p-6 shadow-glow backdrop-blur-xl">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <p className="text-sm uppercase tracking-[0.3em] text-slate-500">Forbidden words</p>
          <h2 className="mt-2 text-2xl font-semibold text-slate-100">Blacklist management</h2>
          <p className="mt-2 max-w-xl text-sm text-slate-400">Keep the AI moderation strict by controlling blacklisted keywords used by the system.</p>
        </div>
      </div>

      <div className="mt-6 space-y-4 rounded-3xl border border-slate-800 bg-slate-900/90 p-5">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
          <label className="sr-only" htmlFor="forbidden-word">New forbidden word</label>
          <input
            id="forbidden-word"
            className="min-w-0 flex-1 rounded-3xl border border-slate-800 bg-slate-950/90 px-4 py-3 text-sm text-slate-100 outline-none transition focus:border-cyan-400 focus:ring-2 focus:ring-cyan-400/20"
            value={newWord}
            onChange={(event) => setNewWord(event.target.value)}
            placeholder="Type a forbidden word"
          />
          <button
            type="button"
            onClick={handleAdd}
            disabled={loading}
            className="inline-flex items-center justify-center gap-2 rounded-3xl bg-emerald-500 px-5 py-3 text-sm font-semibold text-slate-950 transition hover:bg-emerald-400 disabled:cursor-not-allowed disabled:opacity-60"
          >
            <Plus className="h-4 w-4" /> Add word
          </button>
        </div>
        {error ? <p className="rounded-3xl bg-rose-500/10 px-4 py-3 text-sm text-rose-200">{error}</p> : null}
      </div>

      <div className="mt-6 rounded-3xl border border-slate-800 bg-slate-900/90 p-4">
        <div className="grid gap-3">
          <AnimatePresence>
            {forbiddenWords.map((entry) => (
              <motion.div
                layout
                key={entry.id}
                initial={{ opacity: 0, y: 12 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: -12 }}
                transition={{ duration: 0.2 }}
                className="flex items-center justify-between gap-3 rounded-3xl border border-slate-800 bg-slate-950/90 px-4 py-3 text-slate-200"
              >
                <span className="truncate text-sm font-medium">{entry.word}</span>
                <button
                  type="button"
                  onClick={() => handleDelete(entry.id)}
                  className="inline-flex items-center gap-2 rounded-2xl border border-rose-500/20 bg-rose-500/10 px-3 py-2 text-xs font-semibold text-rose-200 transition hover:bg-rose-500/15"
                >
                  <Trash2 className="h-4 w-4" /> Remove
                </button>
              </motion.div>
            ))}
          </AnimatePresence>

          {forbiddenWords.length === 0 ? (
            <div className="rounded-3xl border border-dashed border-slate-700 bg-slate-950/80 p-8 text-center text-slate-500">
              No forbidden words found yet.
            </div>
          ) : null}
        </div>
      </div>
    </motion.section>
  );
}
