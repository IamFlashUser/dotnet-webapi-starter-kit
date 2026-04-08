"use client";

import { useState } from "react";
import Link from "next/link";
import { Plus } from "lucide-react";
import type { PaginationState, SortingState } from "@tanstack/react-table";
import { useTenantsQuery } from "@/hooks/use-tenants";
import { tenantColumns } from "@/components/tenants/tenant-columns";
import { DataTable } from "@/components/ui/data-table";
import { useDebouncedValue } from "@/hooks/use-debounced-value";

export default function TenantsPage() {
  const [search, setSearch] = useState("");
  const debouncedSearch = useDebouncedValue(search, 300);
  const [pagination, setPagination] = useState<PaginationState>({
    pageIndex: 0,
    pageSize: 10,
  });
  const [sorting, setSorting] = useState<SortingState>([]);

  const { data, isLoading } = useTenantsQuery({
    pageNumber: pagination.pageIndex + 1,
    pageSize: pagination.pageSize,
    search: debouncedSearch || undefined,
    sortBy: sorting[0]?.id,
    sortOrder: sorting[0]?.desc ? "desc" : "asc",
  });

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Tenants</h1>
          <p className="text-muted-foreground">
            Manage all tenants in the system.
          </p>
        </div>
        <Link
          href="/tenants/new"
          className="inline-flex h-9 items-center gap-2 rounded-md bg-primary px-4 text-sm font-medium text-primary-foreground transition-colors hover:bg-primary/90"
        >
          <Plus className="h-4 w-4" /> New Tenant
        </Link>
      </div>

      <DataTable
        columns={tenantColumns}
        data={data?.items ?? []}
        pageCount={data?.totalPages ?? 0}
        pagination={pagination}
        onPaginationChange={setPagination}
        sorting={sorting}
        onSortingChange={setSorting}
        searchValue={search}
        onSearchChange={setSearch}
        searchPlaceholder="Search tenants..."
        isLoading={isLoading}
      />
    </div>
  );
}
