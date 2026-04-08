export interface Tenant {
  id: string;
  identifier: string;
  name: string;
  connectionString?: string;
  adminEmail: string;
  isActive: boolean;
  validUpTo?: string;
  issuer?: string;
}

export interface CreateTenantRequest {
  identifier: string;
  name: string;
  connectionString?: string;
  adminEmail: string;
  validUpTo?: string;
}

export interface UpdateTenantRequest {
  name?: string;
  connectionString?: string;
  adminEmail?: string;
  validUpTo?: string;
}
