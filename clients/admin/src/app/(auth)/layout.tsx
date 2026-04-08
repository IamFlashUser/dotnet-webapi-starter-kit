import type { ReactNode } from "react";

export default function AuthLayout({ children }: { children: ReactNode }) {
  return (
    <div className="relative flex min-h-svh items-center justify-center overflow-hidden bg-stone-100 p-5 dark:bg-zinc-950">
      {/* Soft background wash */}
      <div className="pointer-events-none absolute inset-0 overflow-hidden">
        <div className="absolute -top-40 -left-40 size-[600px] rounded-full bg-emerald-200/40 blur-[140px] dark:bg-emerald-900/20" />
        <div className="absolute -right-40 -bottom-40 size-[500px] rounded-full bg-teal-200/30 blur-[140px] dark:bg-teal-900/15" />
      </div>

      {/* Two-card container */}
      <div className="relative z-10 flex w-full max-w-[820px] items-stretch">
        {/* ── Left card ── */}
        <div className="relative hidden w-[380px] shrink-0 overflow-hidden rounded-2xl bg-zinc-950 lg:flex lg:flex-col">
          {/* Radial rings */}
          <div className="pointer-events-none absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2">
            <div className="size-[320px] rounded-full border border-white/[0.05]" />
            <div className="absolute inset-8 rounded-full border border-white/[0.04]" />
            <div className="absolute inset-16 rounded-full border border-white/[0.03]" />
          </div>

          {/* Ambient glow */}
          <div className="pointer-events-none absolute -bottom-20 -left-20 size-[300px] rounded-full bg-emerald-500/10 blur-[80px]" />

          {/* Content */}
          <div className="relative z-10 flex flex-1 flex-col justify-between p-9">
            <p className="max-w-[240px] text-[13px] leading-relaxed text-white/40">
              Enterprise-grade multi-tenant platform
              built for teams that ship fast.
            </p>

            <h2 className="text-[2.25rem] leading-[1.15] font-semibold tracking-[-0.02em] text-white">
              Manage
              <br />
              <span className="text-white/60">your platform</span>
            </h2>

            <div className="flex items-center gap-2.5">
              <div className="flex size-7 items-center justify-center rounded-md bg-emerald-500/90">
                <svg className="size-3.5 text-white" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <polygon points="12 2 2 7 12 12 22 7 12 2" />
                  <polyline points="2 17 12 22 22 17" />
                  <polyline points="2 12 12 17 22 12" />
                </svg>
              </div>
              <span className="text-[13px] font-medium tracking-tight text-white/30">
                fullstackhero
              </span>
            </div>
          </div>
        </div>

        {/* ── Right card (overlaps left) ── */}
        <div className="relative z-10 min-h-[580px] w-full rounded-2xl bg-white shadow-xl shadow-black/[0.06] dark:bg-zinc-900 dark:shadow-black/30 lg:-ml-6 lg:max-w-[440px]">
          {children}
        </div>
      </div>
    </div>
  );
}
