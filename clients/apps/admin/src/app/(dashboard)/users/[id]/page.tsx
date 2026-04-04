"use client";

import { use } from "react";
import Link from "next/link";
import { ArrowLeft } from "lucide-react";
import { useUserQuery, useUserRoles, useAssignUserRoles } from "@/hooks/use-users";
import { useRolesQuery } from "@/hooks/use-roles";

export default function UserDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params);
  const { data: user, isLoading } = useUserQuery(id);
  const { data: userRoles } = useUserRoles(id);
  const { data: allRoles } = useRolesQuery();
  const assignRoles = useAssignUserRoles(id);

  function handleToggleRole(roleId: string, currentlyEnabled: boolean) {
    if (!userRoles) return;
    const updated = userRoles.map((r) =>
      r.roleId === roleId ? { roleId: r.roleId, enabled: !currentlyEnabled } : { roleId: r.roleId, enabled: r.enabled }
    );
    assignRoles.mutate(updated);
  }

  if (isLoading) return <div className="text-muted-foreground">Loading...</div>;
  if (!user) return <div className="text-muted-foreground">User not found.</div>;

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/users" className="inline-flex h-8 w-8 items-center justify-center rounded-md border hover:bg-accent">
          <ArrowLeft className="h-4 w-4" />
        </Link>
        <div>
          <h1 className="text-3xl font-bold tracking-tight">
            {user.firstName} {user.lastName}
          </h1>
          <p className="text-muted-foreground">{user.email}</p>
        </div>
      </div>

      <div className="grid gap-4 md:grid-cols-2 max-w-2xl">
        <InfoRow label="Username" value={user.userName} />
        <InfoRow label="Email" value={user.email} />
        <InfoRow label="Phone" value={user.phoneNumber ?? "—"} />
        <InfoRow label="Status" value={user.isActive ? "Active" : "Inactive"} />
        <InfoRow label="Email Verified" value={user.emailConfirmed ? "Yes" : "No"} />
      </div>

      {allRoles && userRoles && (
        <div className="space-y-3 max-w-lg">
          <h2 className="text-lg font-semibold">Roles</h2>
          <div className="space-y-2">
            {allRoles.map((role) => {
              const userRole = userRoles.find((r) => r.roleId === role.id);
              const enabled = userRole?.enabled ?? false;
              return (
                <label
                  key={role.id}
                  className="flex items-center gap-3 rounded-md border p-3 cursor-pointer hover:bg-muted/50"
                >
                  <input
                    type="checkbox"
                    checked={enabled}
                    onChange={() => handleToggleRole(role.id, enabled)}
                    className="h-4 w-4 rounded border-input"
                  />
                  <div>
                    <p className="text-sm font-medium">{role.name}</p>
                    {role.description && (
                      <p className="text-xs text-muted-foreground">{role.description}</p>
                    )}
                  </div>
                </label>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
}

function InfoRow({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <p className="text-sm text-muted-foreground">{label}</p>
      <p className="text-sm font-medium">{value}</p>
    </div>
  );
}
