"use client";

import { type ColumnDef } from "@tanstack/react-table";
import type { User } from "@fsh/api-client";
import { cn } from "@fsh/ui";

export const userColumns: ColumnDef<User, unknown>[] = [
  {
    accessorKey: "userName",
    header: "Username",
    enableSorting: true,
    cell: ({ row }) => (
      <span className="font-medium">{row.original.userName}</span>
    ),
  },
  {
    id: "name",
    header: "Name",
    enableSorting: false,
    cell: ({ row }) =>
      `${row.original.firstName} ${row.original.lastName}`,
  },
  {
    accessorKey: "email",
    header: "Email",
    enableSorting: true,
  },
  {
    accessorKey: "isActive",
    header: "Status",
    enableSorting: true,
    cell: ({ row }) => (
      <span
        className={cn(
          "inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium",
          row.original.isActive
            ? "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400"
            : "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400"
        )}
      >
        {row.original.isActive ? "Active" : "Inactive"}
      </span>
    ),
  },
  {
    accessorKey: "emailConfirmed",
    header: "Verified",
    enableSorting: false,
    cell: ({ row }) => (
      <span className="text-sm">
        {row.original.emailConfirmed ? "✓" : "—"}
      </span>
    ),
  },
];
