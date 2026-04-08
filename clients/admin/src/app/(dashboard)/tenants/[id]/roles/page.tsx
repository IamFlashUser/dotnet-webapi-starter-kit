"use client";

import { use } from "react";
import Link from "next/link";
import { ArrowLeft } from "lucide-react";

export default function TenantRolesPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params);

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link
          href={`/tenants/${id}`}
          className="inline-flex h-8 w-8 items-center justify-center rounded-md border hover:bg-accent"
        >
          <ArrowLeft className="h-4 w-4" />
        </Link>
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Tenant Roles</h1>
          <p className="text-muted-foreground">
            Manage roles for this tenant.
          </p>
        </div>
      </div>
      <p className="text-muted-foreground">Role management for tenant {id} coming soon...</p>
    </div>
  );
}
