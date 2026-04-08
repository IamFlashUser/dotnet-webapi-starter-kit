import { z } from "zod";

export const createTenantSchema = z.object({
  identifier: z
    .string()
    .min(2, "Identifier must be at least 2 characters")
    .max(50, "Identifier must be at most 50 characters")
    .regex(/^[a-z0-9-]+$/, "Only lowercase letters, numbers, and hyphens allowed"),
  name: z
    .string()
    .min(2, "Name must be at least 2 characters")
    .max(100, "Name must be at most 100 characters"),
  adminEmail: z.string().email("Invalid email address"),
  connectionString: z.string().optional(),
  validUpTo: z.string().optional(),
});

export type CreateTenantFormValues = z.infer<typeof createTenantSchema>;

export const updateTenantSchema = z.object({
  name: z
    .string()
    .min(2, "Name must be at least 2 characters")
    .max(100, "Name must be at most 100 characters"),
  adminEmail: z.string().email("Invalid email address"),
  connectionString: z.string().optional(),
  validUpTo: z.string().optional(),
});

export type UpdateTenantFormValues = z.infer<typeof updateTenantSchema>;
