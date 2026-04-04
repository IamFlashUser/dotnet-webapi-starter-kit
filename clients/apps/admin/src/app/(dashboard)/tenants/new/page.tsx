"use client";

import { useRouter } from "next/navigation";
import { useCreateTenant } from "@/hooks/use-tenants";
import { TenantForm } from "@/components/tenants/tenant-form";
import type { CreateTenantFormValues } from "@/lib/schemas/tenant";
import { ArrowLeft } from "lucide-react";
import Link from "next/link";

export default function NewTenantPage() {
  const router = useRouter();
  const createTenant = useCreateTenant();

  function handleSubmit(data: CreateTenantFormValues) {
    createTenant.mutate(
      {
        identifier: data.identifier,
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
          <h1 className="text-3xl font-bold tracking-tight">Create Tenant</h1>
          <p className="text-muted-foreground">
            Provision a new tenant in the system.
          </p>
        </div>
      </div>

      <TenantForm onSubmit={handleSubmit} isLoading={createTenant.isPending} />
    </div>
  );
}
