"use client";

import Link from "next/link";
import { useRolesQuery, useDeleteRole } from "@/hooks/use-roles";
import { Shield, Trash2 } from "lucide-react";

export default function RolesPage() {
  const { data: roles, isLoading } = useRolesQuery();
  const deleteRole = useDeleteRole();

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Roles</h1>
        <p className="text-muted-foreground">
          Manage roles and permissions.
        </p>
      </div>

      {isLoading ? (
        <p className="text-muted-foreground">Loading roles...</p>
      ) : !roles?.length ? (
        <p className="text-muted-foreground">No roles found.</p>
      ) : (
        <div className="grid gap-3 max-w-2xl">
          {roles.map((role) => (
            <div
              key={role.id}
              className="flex items-center justify-between rounded-lg border p-4 hover:bg-muted/50 transition-colors"
            >
              <Link href={`/roles/${role.id}`} className="flex items-center gap-3 flex-1">
                <Shield className="h-5 w-5 text-muted-foreground" />
                <div>
                  <p className="font-medium">{role.name}</p>
                  {role.description && (
                    <p className="text-sm text-muted-foreground">{role.description}</p>
                  )}
                </div>
              </Link>
              <button
                onClick={() => {
                  if (confirm(`Delete role "${role.name}"?`)) {
                    deleteRole.mutate(role.id);
                  }
                }}
                className="inline-flex h-8 w-8 items-center justify-center rounded-md text-muted-foreground hover:bg-destructive/10 hover:text-destructive"
              >
                <Trash2 className="h-4 w-4" />
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
