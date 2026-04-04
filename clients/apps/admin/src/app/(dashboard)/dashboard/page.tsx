"use client";

import { Building2, Users, Shield, Activity } from "lucide-react";
import { useTenantsQuery } from "@/hooks/use-tenants";
import { useUsersQuery } from "@/hooks/use-users";
import { useRolesQuery } from "@/hooks/use-roles";

export default function DashboardPage() {
  const { data: tenants } = useTenantsQuery({ pageNumber: 1, pageSize: 1 });
  const { data: users } = useUsersQuery({ pageNumber: 1, pageSize: 1 });
  const { data: roles } = useRolesQuery();

  const activeTenants = tenants?.totalCount ?? 0;
  const totalUsers = users?.totalCount ?? 0;
  const totalRoles = roles?.length ?? 0;

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Dashboard</h1>
        <p className="text-muted-foreground">
          Overview of your tenant management system.
        </p>
      </div>

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <StatCard
          title="Total Tenants"
          value={activeTenants}
          description="Provisioned tenants"
          icon={Building2}
        />
        <StatCard
          title="Total Users"
          value={totalUsers}
          description="Across all tenants"
          icon={Users}
        />
        <StatCard
          title="Roles"
          value={totalRoles}
          description="Defined roles"
          icon={Shield}
        />
        <StatCard
          title="System Status"
          value="Healthy"
          description="All services running"
          icon={Activity}
        />
      </div>

      <div className="grid gap-4 md:grid-cols-2">
        <div className="rounded-xl border bg-card p-6">
          <h2 className="text-lg font-semibold mb-4">Recent Tenants</h2>
          {tenants?.items?.length ? (
            <div className="space-y-3">
              {tenants.items.slice(0, 5).map((tenant) => (
                <div key={tenant.id} className="flex items-center justify-between text-sm">
                  <div>
                    <p className="font-medium">{tenant.name}</p>
                    <p className="text-muted-foreground font-mono text-xs">{tenant.identifier}</p>
                  </div>
                  <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${tenant.isActive ? "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400" : "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400"}`}>
                    {tenant.isActive ? "Active" : "Inactive"}
                  </span>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-sm text-muted-foreground">No tenants yet.</p>
          )}
        </div>

        <div className="rounded-xl border bg-card p-6">
          <h2 className="text-lg font-semibold mb-4">Quick Actions</h2>
          <div className="space-y-2">
            <QuickAction href="/tenants/new" label="Create New Tenant" />
            <QuickAction href="/tenants" label="Manage Tenants" />
            <QuickAction href="/users" label="Manage Users" />
            <QuickAction href="/roles" label="Manage Roles" />
          </div>
        </div>
      </div>
    </div>
  );
}

function StatCard({
  title,
  value,
  description,
  icon: Icon,
}: {
  title: string;
  value: string | number;
  description: string;
  icon: React.ComponentType<{ className?: string }>;
}) {
  return (
    <div className="rounded-xl border bg-card p-6 text-card-foreground shadow-sm">
      <div className="flex items-center justify-between">
        <p className="text-sm font-medium text-muted-foreground">{title}</p>
        <Icon className="h-4 w-4 text-muted-foreground" />
      </div>
      <p className="text-3xl font-bold mt-2">{value}</p>
      <p className="text-xs text-muted-foreground mt-1">{description}</p>
    </div>
  );
}

function QuickAction({ href, label }: { href: string; label: string }) {
  return (
    <a
      href={href}
      className="flex items-center rounded-md border px-4 py-2.5 text-sm font-medium transition-colors hover:bg-accent"
    >
      {label}
    </a>
  );
}
