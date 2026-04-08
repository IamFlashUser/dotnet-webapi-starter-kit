"use client";

import { use } from "react";
import Link from "next/link";
import { ArrowLeft } from "lucide-react";
import { useRolePermissions, useUpdateRolePermissions } from "@/hooks/use-roles";
import { useState, useEffect } from "react";

export default function RoleDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params);
  const { data: roleWithPermissions, isLoading } = useRolePermissions(id);
  const updatePermissions = useUpdateRolePermissions();
  const [selected, setSelected] = useState<Set<string>>(new Set());

  useEffect(() => {
    if (roleWithPermissions?.permissions) {
      setSelected(new Set(roleWithPermissions.permissions.map((p) => p.permission)));
    }
  }, [roleWithPermissions]);

  function handleToggle(permission: string) {
    setSelected((prev) => {
      const next = new Set(prev);
      if (next.has(permission)) {
        next.delete(permission);
      } else {
        next.add(permission);
      }
      return next;
    });
  }

  function handleSave() {
    updatePermissions.mutate({
      roleId: id,
      permissions: Array.from(selected),
    });
  }

  if (isLoading) return <div className="text-muted-foreground">Loading...</div>;
  if (!roleWithPermissions) return <div className="text-muted-foreground">Role not found.</div>;

  const permissionGroups = groupPermissions(roleWithPermissions.permissions);

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/roles" className="inline-flex h-8 w-8 items-center justify-center rounded-md border hover:bg-accent">
          <ArrowLeft className="h-4 w-4" />
        </Link>
        <div>
          <h1 className="text-3xl font-bold tracking-tight">
            {roleWithPermissions.name}
          </h1>
          <p className="text-muted-foreground">
            {roleWithPermissions.description ?? "Manage permissions for this role."}
          </p>
        </div>
      </div>

      <div className="space-y-6 max-w-2xl">
        {Object.entries(permissionGroups).map(([group, permissions]) => (
          <div key={group} className="space-y-2">
            <h3 className="text-sm font-semibold uppercase tracking-wider text-muted-foreground">
              {group}
            </h3>
            <div className="space-y-1">
              {permissions.map((p) => (
                <label
                  key={p.permission}
                  className="flex items-center gap-3 rounded-md border p-3 cursor-pointer hover:bg-muted/50"
                >
                  <input
                    type="checkbox"
                    checked={selected.has(p.permission)}
                    onChange={() => handleToggle(p.permission)}
                    className="h-4 w-4 rounded border-input"
                  />
                  <div>
                    <p className="text-sm font-medium font-mono">{p.permission}</p>
                    {p.description && (
                      <p className="text-xs text-muted-foreground">{p.description}</p>
                    )}
                  </div>
                </label>
              ))}
            </div>
          </div>
        ))}

        <button
          onClick={handleSave}
          disabled={updatePermissions.isPending}
          className="inline-flex h-10 items-center justify-center rounded-md bg-primary px-6 text-sm font-medium text-primary-foreground transition-colors hover:bg-primary/90 disabled:pointer-events-none disabled:opacity-50"
        >
          {updatePermissions.isPending ? "Saving..." : "Save Permissions"}
        </button>
      </div>
    </div>
  );
}

function groupPermissions(permissions: { permission: string; description?: string }[]) {
  const groups: Record<string, typeof permissions> = {};
  for (const p of permissions) {
    const parts = p.permission.split(".");
    const group = parts.length > 1 ? parts[0] : "General";
    if (!groups[group]) groups[group] = [];
    groups[group].push(p);
  }
  return groups;
}
