"use client";

import { type ColumnDef } from "@tanstack/react-table";
import type { Tenant } from "@fsh/api-client";
import { TenantStatusBadge } from "./tenant-status-badge";
import { TenantActions } from "./tenant-actions";

export const tenantColumns: ColumnDef<Tenant, unknown>[] = [
  {
    accessorKey: "identifier",
    header: "Identifier",
    enableSorting: true,
    cell: ({ row }) => (
      <span className="font-mono text-sm">{row.original.identifier}</span>
    ),
  },
  {
    accessorKey: "name",
    header: "Name",
    enableSorting: true,
    cell: ({ row }) => (
      <span className="font-medium">{row.original.name}</span>
    ),
  },
  {
    accessorKey: "adminEmail",
    header: "Admin Email",
    enableSorting: false,
  },
  {
    accessorKey: "isActive",
    header: "Status",
    enableSorting: true,
    cell: ({ row }) => <TenantStatusBadge isActive={row.original.isActive} />,
  },
  {
    accessorKey: "validUpTo",
    header: "Valid Until",
    enableSorting: true,
    cell: ({ row }) =>
      row.original.validUpTo
        ? new Date(row.original.validUpTo).toLocaleDateString()
        : "—",
  },
  {
    id: "actions",
    header: "",
    cell: ({ row }) => <TenantActions tenant={row.original} />,
  },
];
