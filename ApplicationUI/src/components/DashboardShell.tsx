import type { ReactNode } from 'react';
import { motion } from 'framer-motion';
import { ShieldCheck, ShieldAlert, Star, Sparkles } from 'lucide-react';

interface DashboardShellProps {
  children: ReactNode;
}

export default function DashboardShell({ children }: DashboardShellProps) {
  return (
    <div className="min-h-screen bg-[radial-gradient(circle_at_top,_rgba(56,189,248,0.14),_transparent_15%),radial-gradient(circle_at_20%_10%,_rgba(34,197,94,0.08),_transparent_10%),#0b0f17] py-8 px-6 text-slate-100 sm:px-10">
      <motion.header initial={{ opacity: 0, y: -24 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.5 }} className="mx-auto flex max-w-[1400px] flex-col gap-6 pb-8">
        <div className="flex flex-col gap-4 rounded-3xl border border-white/10 bg-slate-950/80 p-6 shadow-glow backdrop-blur-xl md:flex-row md:items-center md:justify-between">
          <div className="space-y-3">
            <div className="inline-flex items-center gap-2 rounded-full bg-slate-900/80 px-3 py-2 text-xs uppercase tracking-[0.3em] text-cyan-200/90 ring-1 ring-cyan-400/20">
              <Sparkles className="h-4 w-4" /> Portfolio-ready AI Guard
            </div>
            <div>
              <p className="text-sm uppercase tracking-[0.3em] text-slate-400">AutomatedContentGuard</p>
              <h1 className="text-3xl font-semibold text-slate-100 sm:text-4xl">AI safety moderation dashboard</h1>
            </div>
          </div>
          <div className="grid gap-3 sm:inline-flex sm:grid-flow-col sm:grid-cols-2 sm:items-center">
            <div className="rounded-3xl border border-white/10 bg-slate-900/80 p-4 shadow-glow">
              <p className="text-sm text-slate-400">Live moderation</p>
              <p className="text-xl font-semibold text-cyan-200">Real-time sandbox</p>
            </div>
            <div className="rounded-3xl border border-white/10 bg-slate-900/80 p-4 shadow-glow">
              <p className="text-sm text-slate-400">Data services</p>
              <p className="text-xl font-semibold text-emerald-200">Content history + blacklist</p>
            </div>
          </div>
        </div>
      </motion.header>
      <main className="mx-auto grid max-w-[1400px] gap-6 md:grid-cols-[1.2fr_0.8fr]">
        {children}
      </main>
    </div>
  );
}
