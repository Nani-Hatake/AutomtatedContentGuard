import type { ReactNode } from 'react';

interface BadgeProps {
  label: string;
  tone: 'success' | 'danger' | 'warning' | 'neutral';
  icon?: ReactNode;
}

const badgeStyles = {
  success: 'bg-emerald-500/15 text-emerald-300 ring-emerald-500/30',
  danger: 'bg-rose-500/15 text-rose-300 ring-rose-500/30',
  warning: 'bg-amber-500/15 text-amber-300 ring-amber-500/30',
  neutral: 'bg-slate-700/70 text-slate-200 ring-slate-500/20',
};

export function Badge({ label, tone, icon }: BadgeProps) {
  return (
    <span className={`inline-flex items-center gap-2 rounded-full border border-white/10 px-3 py-1 text-xs font-semibold uppercase tracking-[0.24em] ring-1 ${badgeStyles[tone]}`}>
      {icon}
      {label}
    </span>
  );
}
