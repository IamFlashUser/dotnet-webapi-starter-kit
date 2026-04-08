"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { createTenantSchema, type CreateTenantFormValues } from "@/lib/schemas/tenant";

interface TenantFormProps {
  defaultValues?: Partial<CreateTenantFormValues>;
  onSubmit: (data: CreateTenantFormValues) => void;
  isLoading?: boolean;
  isEdit?: boolean;
}

export function TenantForm({ defaultValues, onSubmit, isLoading, isEdit }: TenantFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CreateTenantFormValues>({
    resolver: zodResolver(createTenantSchema),
    defaultValues: {
      identifier: "",
      name: "",
      adminEmail: "",
      connectionString: "",
      validUpTo: "",
      ...defaultValues,
    },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6 max-w-lg">
      <FormField label="Identifier" error={errors.identifier?.message}>
        <input
          {...register("identifier")}
          disabled={isEdit}
          placeholder="my-tenant"
          className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
        />
      </FormField>

      <FormField label="Name" error={errors.name?.message}>
        <input
          {...register("name")}
          placeholder="My Tenant"
          className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
        />
      </FormField>

      <FormField label="Admin Email" error={errors.adminEmail?.message}>
        <input
          {...register("adminEmail")}
          type="email"
          placeholder="admin@example.com"
          className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
        />
      </FormField>

      <FormField label="Connection String (optional)" error={errors.connectionString?.message}>
        <input
          {...register("connectionString")}
          placeholder="Host=...;Database=...;..."
          className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm font-mono ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
        />
      </FormField>

      <FormField label="Valid Until (optional)" error={errors.validUpTo?.message}>
        <input
          {...register("validUpTo")}
          type="date"
          className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
        />
      </FormField>

      <button
        type="submit"
        disabled={isLoading}
        className="inline-flex h-10 items-center justify-center rounded-md bg-primary px-6 text-sm font-medium text-primary-foreground transition-colors hover:bg-primary/90 disabled:pointer-events-none disabled:opacity-50"
      >
        {isLoading ? "Saving..." : isEdit ? "Update Tenant" : "Create Tenant"}
      </button>
    </form>
  );
}

function FormField({
  label,
  error,
  children,
}: {
  label: string;
  error?: string;
  children: React.ReactNode;
}) {
  return (
    <div className="space-y-2">
      <label className="text-sm font-medium">{label}</label>
      {children}
      {error && <p className="text-sm text-destructive">{error}</p>}
    </div>
  );
}
