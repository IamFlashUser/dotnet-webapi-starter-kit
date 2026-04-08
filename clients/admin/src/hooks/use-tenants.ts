import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import type { PaginationParams, Tenant, CreateTenantRequest, UpdateTenantRequest } from "@fsh/api-client";
import { api } from "@/lib/api";

export function useTenantsQuery(params: PaginationParams) {
  return useQuery({
    queryKey: ["tenants", params],
    queryFn: () => api.tenants.list(params).then((r) => r.data),
  });
}

export function useTenantQuery(id: string) {
  return useQuery({
    queryKey: ["tenants", id],
    queryFn: () => api.tenants.get(id).then((r) => r.data),
    enabled: !!id,
  });
}

export function useCreateTenant() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateTenantRequest) => api.tenants.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tenants"] });
    },
  });
}

export function useUpdateTenant(id: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: UpdateTenantRequest) => api.tenants.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tenants"] });
    },
  });
}

export function useActivateTenant() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => api.tenants.activate(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tenants"] });
    },
  });
}

export function useDeactivateTenant() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => api.tenants.deactivate(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tenants"] });
    },
  });
}
