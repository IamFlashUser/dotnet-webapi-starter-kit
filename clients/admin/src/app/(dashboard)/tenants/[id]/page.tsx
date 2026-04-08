"use client";

import { use } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { ArrowLeft } from "lucide-react";
import { useTenantQuery, useUpdateTenant } from "@/hooks/use-tenants";
import { TenantForm } from "@/components/tenants/tenant-form";
import type { CreateTenantFormValues } from "@/lib/schemas/tenant";

export default function TenantDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params);
  const router = useRouter();
  const { data: tenant, isLoading } = useTenantQuery(id);
  const updateTenant = useUpdateTenant(id);

  function handleSubmit(data: CreateTenantFormValues) {
    updateTenant.mutate(
      {
        name: data.name,
        adminEmail: data.adminEmail,
        connectionString: data.connectionString || undefined,
        validUpTo: data.validUpTo || undefined,
      },
      {
        onSuccess: () => router.push("/tenants"),
      }
    );
  }

  if (isLoading) {
    return <div className="text-muted-foreground">Loading tenant...</div>;
  }

  if (!tenant) {
    return <div className="text-muted-foreground">Tenant not found.</div>;
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link
          href="/tenants"
          className="inline-flex h-8 w-8 items-center justify-center rounded-md border hover:bg-accent"
        >
          <ArrowLeft className="h-4 w-4" />
        </Link>
        <div>
          <h1 className="text-3xl font-bold tracking-tight">
            Edit: {tenant.name}
          </h1>
          <p className="text-muted-foreground font-mono text-sm">
            {tenant.identifier}
          </p>
        </div>
      </div>

      <TenantForm
        isEdit
        defaultValues={{
          identifier: tenant.identifier,
          name: tenant.name,
          adminEmail: tenant.adminEmail,
          connectionString: tenant.connectionString ?? "",
          validUpTo: tenant.validUpTo?.split("T")[0] ?? "",
        }}
        onSubmit={handleSubmit}
        isLoading={updateTenant.isPending}
      />
    </div>
  );
}
