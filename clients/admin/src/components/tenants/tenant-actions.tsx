"use client";

import Link from "next/link";
import type { Tenant } from "@fsh/api-client";
import { useActivateTenant, useDeactivateTenant } from "@/hooks/use-tenants";
import { MoreHorizontal, Pencil, Power, Users, Shield } from "lucide-react";
import { useState, useRef, useEffect } from "react";

export function TenantActions({ tenant }: { tenant: Tenant }) {
  const [open, setOpen] = useState(false);
  const ref = useRef<HTMLDivElement>(null);
  const activate = useActivateTenant();
  const deactivate = useDeactivateTenant();

  useEffect(() => {
    function handleClick(e: MouseEvent) {
      if (ref.current && !ref.current.contains(e.target as Node)) setOpen(false);
    }
    document.addEventListener("mousedown", handleClick);
    return () => document.removeEventListener("mousedown", handleClick);
  }, []);

  return (
    <div className="relative" ref={ref}>
      <button
        onClick={() => setOpen(!open)}
        className="inline-flex h-8 w-8 items-center justify-center rounded-md text-muted-foreground hover:bg-accent hover:text-accent-foreground"
      >
        <MoreHorizontal className="h-4 w-4" />
      </button>

      {open && (
        <div className="absolute right-0 z-50 mt-1 w-48 rounded-md border bg-popover p-1 shadow-md">
          <Link
            href={`/tenants/${tenant.id}`}
            className="flex w-full items-center gap-2 rounded-sm px-2 py-1.5 text-sm hover:bg-accent"
            onClick={() => setOpen(false)}
          >
            <Pencil className="h-3.5 w-3.5" /> Edit
          </Link>
          <Link
            href={`/tenants/${tenant.id}/users`}
            className="flex w-full items-center gap-2 rounded-sm px-2 py-1.5 text-sm hover:bg-accent"
            onClick={() => setOpen(false)}
          >
            <Users className="h-3.5 w-3.5" /> Users
          </Link>
          <Link
            href={`/tenants/${tenant.id}/roles`}
            className="flex w-full items-center gap-2 rounded-sm px-2 py-1.5 text-sm hover:bg-accent"
            onClick={() => setOpen(false)}
          >
            <Shield className="h-3.5 w-3.5" /> Roles
          </Link>
          <div className="my-1 h-px bg-border" />
          <button
            onClick={() => {
              if (tenant.isActive) {
                deactivate.mutate(tenant.id);
              } else {
                activate.mutate(tenant.id);
              }
              setOpen(false);
            }}
            className="flex w-full items-center gap-2 rounded-sm px-2 py-1.5 text-sm hover:bg-accent"
          >
            <Power className="h-3.5 w-3.5" />
            {tenant.isActive ? "Deactivate" : "Activate"}
          </button>
        </div>
      )}
    </div>
  );
}
